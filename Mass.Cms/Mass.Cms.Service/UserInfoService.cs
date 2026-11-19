using Mass.Cms.Entity;
using Mass.Cms.Entity.Enum;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Mass.Cms.Service
{
    public class UserInfoService : BaseService<UserInfo>, IUserInfoService
    {
        private readonly IUserInfoRepository userInfoRepository;
        private readonly IRoleInfoRepository roleInfoRepository;
        //⑮ Redis 连接工厂（单例，线程安全）：GetDatabase 是廉价操作，随时调都行
        private readonly IConnectionMultiplexer redis;

        public UserInfoService(IUserInfoRepository userInfoRepository, IRoleInfoRepository roleInfoRepository,
            IConnectionMultiplexer redis)
        {
            //把专属仓储交给父类的公共字段，父类五件套方法都走它
            base.repository = userInfoRepository;
            //再存一份给自己，写用户专属方法时用
            this.userInfoRepository = userInfoRepository;
            //角色仓储：查权限时要顺着 角色→权限 拐个弯，专门注进来
            this.roleInfoRepository = roleInfoRepository;
            this.redis = redis;
        }

        //★★★ ⑭ 阶段B【你写区】：GetUserPermissions（约 30 行）★★★
        //输入：登录成功的用户实体（Id 可用）；输出：权限编码字典 { menus:[...], points:[...] }
        //五步思路（照 notes/04 速记，练习版"规范写法"）：
        //  1. await 查用户 Include(u => u.RoleInfos) —— 接收结果再遍历（别学老师版
        //     直接遍历 user.RoleInfos 靠 EF 身份解析，能跑但顺序靠运气，属于隐式依赖）
        //  2. foreach 每个角色 → 二次 await 查 roleInfoRepository 的角色 Include(r => r.PermissionInfos)
        //  3. foreach 角色的权限 → switch (permission.PermissionType) 分拣：
        //     (int)PermissionTypeEnum.PermissionMenu(1) → menus 收 PermissionCode
        //     (int)PermissionTypeEnum.PermissionPoint(2) → points 收 PermissionCode
        //  4. Distinct() 去重（多角色共享同一权限会重复收）
        //  5. 组字典返回 { "menus": menus, "points": points }
        public async Task<Dictionary<string, List<string>>> GetUserPermissions(UserInfo user)
        {
            //⑮ 缓存键约定：perm:user:{用户编号}——"谁"的"权限"一眼看懂，冒号分隔 ARDM 里自动分组
            var cacheKey = $"perm:user:{user.Id}";
            //GetDatabase(3)：我们约定权限编码住 db3（0~15 是物理隔离的库，和别的用途分开）
            var database = redis.GetDatabase(3);

            //① 先查缓存：命中就直接用，一次 Redis 读 < 1ms，省掉下面四五发 SQL
            var cached = await database.StringGetAsync(cacheKey);
            if (cached.HasValue)
            {
                //存的时候是 "menu1,menu2|point1,point2"，取出来按 | 切开再按 , 切开
                var parts = cached.ToString().Split('|');
                return new Dictionary<string, List<string>>
                {
                    { "menus", parts[0].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() },
                    { "points", parts.Length > 1 ? parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>() },
                };
            }

            //② 缓存没有（miss）→ 走原来的查库逻辑
            var menus = new List<string>();
            var points = new List<string>();

            var dbUser = await userInfoRepository.LoadEntities(u => u.Id == user.Id)
                .Include(u => u.RoleInfos)
                .FirstOrDefaultAsync();

            if(dbUser != null && dbUser.RoleInfos != null)
            {
                foreach(var role in dbUser.RoleInfos)
                {
                    var dbRole = await roleInfoRepository.LoadEntities(r => r.Id == role.Id)
                        .Include(r => r.PermissionInfos)
                        .FirstOrDefaultAsync();
                    if(dbRole != null && dbRole.PermissionInfos != null)
                    {
                        foreach(var permission in dbRole.PermissionInfos)
                        {
                            if(permission.PermissionType == (int)PermissionTypeEnum.PermissionMenu)
                            {
                                menus.Add(permission.PermissionCode);
                            }
                            else if (permission.PermissionType == (int)PermissionTypeEnum.PermissionPoint)
                            {
                                points.Add(permission.PermissionCode);
                            }
                        }
                    }
                }
            }
            var distinctMenus = menus.Distinct().ToList();
            var distinctPoints = points.Distinct().ToList();

            //③ 查完回填缓存（cache-aside 回写）：值 = menus逗号串 | points逗号串
            //TTL 30 分钟兜底：就算哪次"删缓存"漏了，最多脏 30 分钟也会自己过期
            var cacheValue = $"{string.Join(",", distinctMenus)}|{string.Join(",", distinctPoints)}";
            await database.StringSetAsync(cacheKey, cacheValue, TimeSpan.FromMinutes(30));

            return new Dictionary<string, List<string>>
            {
                { "menus", distinctMenus },
                { "points", distinctPoints },
            };
        }

        //★★★ ⑭ 尾巴任务3：员工分配角色 ★★★
        //★ 查某用户已分配的角色 id 集合（弹窗回显用）
        public async Task<List<int>> GetUserRoleIds(int userId)
        {
            var user = await userInfoRepository.LoadEntities(u => u.Id == userId)
                .Include(u => u.RoleInfos)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return new List<int>();
            }
            return user.RoleInfos.Where(r => r.IsDeleted == false).Select(r => r.Id).ToList();
        }

        //★ 员工分配角色：串值契约同角色授权（空选发 "0" 保底），删旧插新走导航集合
        public async Task<bool> AssignRoles(int userId, string roleIds)
        {
            var ids = roleIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var i) ? i : 0)
                .Where(i => i > 0)
                .Distinct()
                .ToList();

            var user = await userInfoRepository.LoadEntities(u => u.Id == userId)
                .Include(u => u.RoleInfos)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }

            var target = await roleInfoRepository
                .LoadEntities(r => ids.Contains(r.Id) && r.IsDeleted == false)
                .ToListAsync();

            //集合差为 0 直接 return（防多余删插）
            var current = user.RoleInfos.Where(r => r.IsDeleted == false).Select(r => r.Id).OrderBy(x => x).ToList();
            var wanted = target.Select(r => r.Id).OrderBy(x => x).ToList();
            if (current.SequenceEqual(wanted))
            {
                return true;
            }

            //删旧插新：EF 自动维护 T_UserInfos_RoleInfos 中间表
            user.RoleInfos.Clear();
            foreach (var r in target)
            {
                user.RoleInfos.Add(r);
            }

            //⑮ 先更库：缓存一致性要求"先更库后删缓存"——必须等 SaveChanges 真落库了再删缓存。
            //如果反过来（先删缓存），删完到落库之间任何请求查权限都会 miss → 拿旧数据回填缓存 → 脏到下次删除
            //这里显式保存后，[UnitOfWork] 过滤器之后那次 SaveChanges 没有待办，等于空跑，无副作用
            await userInfoRepository.SaveChangesAsync();

            //后删缓存：该用户的权限变了，他的缓存必须作废，下次查询重新从库算
            await redis.GetDatabase(3).KeyDeleteAsync($"perm:user:{userId}");
            return true;
        }
    }
}
