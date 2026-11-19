using AutoMapper;
using Mass.Cms.Entity;
using Mass.Cms.IService;
using Mass.Cms.WebApi.Attributes;
using Mass.Cms.WebApi.Models;
using Mass.Cms.WebApi.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mass.Cms.WebApi.Controllers
{
    /// <summary>
    /// 用户接口。RESTful：URL 全是名词复数，动作靠"请求方式"表达
    /// teachers注意：同一个控制器里"请求方式+路由"的组合必须唯一
    /// </summary>
    [Route("api/[controller]")]   //API 前缀 + 控制器名 → /api/users
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserInfoService userInfoService;
        private readonly IArticleService articleService;
        private readonly IMapper mapper;

        public UsersController(IUserInfoService userInfoService, IArticleService articleService, IMapper mapper)
        {
            this.userInfoService = userInfoService;
            this.articleService = articleService;
            this.mapper = mapper;
        }

        /// <summary>
        /// 查询所有用户  GET /api/users （查询成功 → 200）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            //枚举发生点：ToListAsync 在这里真正发 SQL ← 你今天学透的那一套
            var users = await userInfoService.LoadEntities(u => u.IsDeleted == false).ToListAsync();

            if (users == null || users.Count <= 0)
            {
                return NotFound("没有找到数据");  //404：语义上"没有资源"而不是 200+空数组
            }

            var userDtos = mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);   //← Ok 内部把对象序列化成 JSON！记住这点，爆炸的锅一半在它
        }

        /// <summary>
        /// 查找个用户  GET /api/users/5 ——编号写在 URL 里叫"路径参数"，FromRoute 收
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await userInfoService.LoadEntities(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("没有找到用户");
            }

            return Ok(mapper.Map<UserDto>(user));
        }

        /// <summary>
        /// 查找一个用户发布的所有文章  GET /api/users/5/articles
        /// 路由里 {userId}/articles 比上面多一段articles → 和"查用户"组合不冲突
        /// </summary>
        [HttpGet("{userId}/articles")]
        public async Task<IActionResult> GetUserArticles([FromRoute] int userId)
        {
            var user = await userInfoService.LoadEntities(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("没有找到用户");
            }

            //a.UserInfo == user：EF 把导航属性比较翻译成 a.UserInfoId == user.Id 的表连接
            var articles = await articleService.LoadEntities(a => a.UserInfo == user).ToListAsync();
            if (articles == null || articles.Count <= 0)
            {
                return NotFound("没有找到该用户发布的文章");
            }

            var articleDtos = mapper.Map<List<ArticleDto>>(articles);
            return Ok(articleDtos);
        }

        /// <summary>
        /// ⚠️ 埋雷接口：查询所有用户及各自文章  GET /api/users/articles
        /// 现在一！个！字！都！不！要！改！先按下 Swagger 执行一次，看它爆！
        /// </summary>
        [HttpGet("articles")]
        public async Task<IActionResult> GetUsersAndArticles()
        {
            //Include：把每个用户的 Articles 一并查回来（关联查询、一次SQL带出）
            var users = await userInfoService.LoadEntities(u => u.IsDeleted == false)
                .Include(u => u.Articles)
                .ToListAsync();

            //========== 阶段A（已炸存档）==========
            //return Ok(users);

            //========== 阶段B：映射成 DTO 再返回 ==========
            var userDtos = mapper.Map<List<UserDto>>(users);
            return Ok(userDtos);
            //→ 这版不死循环了，但你会发现返回里文章全部消失！为什么？想一想再翻答案：
            //→ 因为 UserDto 还没有 Articles 属性，映射时实体里的文章数据没地方放，直接丢了
            //
            //========== 阶段C（给 UserDto 加 Articles 属性后）==========
            //再执行，文章全回来了 → 循环炸/数据丢/DTO救场 三幕剧演完
        }

        /// <summary>
        /// 按手机号查用户详情  GET /api/users/by-phone/{phone}
        /// 给前端路由守卫用的：登录后存手机号，页面刷新时守卫拿它来还原"当前登录人是谁"
        /// ⑭ 起返回升级为 { user, permissions }：user=基本资料，permissions=权限编码字典
        /// （前端守卫顺路把权限也拿走——一次请求干两件事）
        /// 返回统一 ApiResult 结构（前端 request.js 的响应拦截器按这个解构）
        /// </summary>
        [HttpGet("by-phone/{phone}")]
        public async Task<IActionResult> GetUserByPhone([FromRoute] string phone)
        {
            var user = await userInfoService.LoadEntities(u => u.UserPhone == phone).FirstOrDefaultAsync();
            if (user == null)
            {
                return Ok(new ApiResult<object?> { IsSuccess = false, Message = "用户不存在", Data = null });
            }

            var dict = await userInfoService.GetUserPermissions(user);   //权限编码：{ menus:[...], points:[...] }

            return Ok(new ApiResult<object>
            {
                IsSuccess = true,
                Message = "查询成功",
                //匿名对象精确挑字段：密码、IsDeleted 这些不该给前端的根本不放进返回
                Data = new
                {
                    user = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        userPhone = user.UserPhone,
                        photoUrl = user.PhotoUrl,
                    },
                    permissions = dict,
                }
            });
        }

        /// <summary>
        /// 新增用户  POST /api/users （新增成功 → 201）
        /// </summary>
        [HttpPost]
        [UnitOfWork]   //跑完没异常 → Filter 统一 SaveChangesAsync 一次入库
        public IActionResult CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            //收货箱DTO → 仓库实体（你Profiles里那条反向映射）
            var userInfo = mapper.Map<UserInfo>(userCreateDto);

            //前端没送、也不该送的字段，后端自己派默认值
            userInfo.CreateDate = DateTime.Now;
            userInfo.EditDate = DateTime.Now;
            userInfo.IsDeleted = false;

            userInfoService.AddEntity(userInfo);   //只记待办，不立刻写库（三种-trigger SaveChanges在你学的UoW里）

            //201 的规范返回：状态码 + 一条"去哪查最新数据"的 URL 响应头
            //第一个参数是"方法名"不是路由：框架取向这个名字所在的 Action 生成地址
            //第二个参数返回创建成果 → 也即 mapper 从实体倒回 UserDto 展示柜：密码你不会收回去哦
            var userDto = mapper.Map<UserDto>(userInfo);
            return CreatedAtAction("GetAllUsers", userDto);
        }

        /// <summary>
        /// ⑭ 尾巴任务3：员工页列表（练习版：全量，ApiResult 包壳——前端拦截器按 isSuccess 拆包，
        /// 老的 GetAllUsers 是裸数组不包壳的教学遗留，前端页面一律走这个新接口）
        /// </summary>
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList()
        {
            var users = await userInfoService.LoadEntities(u => u.IsDeleted == false).ToListAsync();
            var userDtos = mapper.Map<List<UserDto>>(users);
            return Ok(new ApiResult<List<UserDto>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = userDtos
            });
        }

        /// <summary>
        /// 某用户已分配的角色 id 集合（弹窗回显）
        /// </summary>
        [HttpGet("{userId}/roleIds")]
        public async Task<IActionResult> GetUserRoleIds([FromRoute] int userId)
        {
            var ids = await userInfoService.GetUserRoleIds(userId);
            return Ok(new ApiResult<List<int>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = ids
            });
        }

        /// <summary>
        /// 员工分配角色：删旧插新，[UnitOfWork] 统一提交
        /// </summary>
        [HttpPost("assign-roles")]
        [UnitOfWork]
        public async Task<IActionResult> AssignRoles([FromBody] UserAssignDto dto)
        {
            var result = await userInfoService.AssignRoles(dto.UserId, dto.RoleIds ?? "0");
            if (!result)
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = "分配失败：用户不存在" });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "分配成功" });
        }
    }
}
