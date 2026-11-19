using Mass.Cms.Entity.Enum;
using Mass.Cms.IRepository;
using Mass.Cms.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mass.Cms.WebApi.Filters
{
    // ★ ⑭ 阶段D：API 权限校验过滤器【作业②·主战场】
    // 立场：API 权验是"真防线"——前端按钮 v-if 只是体验优化，
    //       拿 Postman 绕过前端直接调接口的人，拦在这。
    // 用法：贴在要保护的 Action 上 —— [TypeFilter(typeof(ApiPermissionFilter))]
    //       TypeFilter = 由 DI 容器现场造这个过滤器，构造器里的仓储参数自动注入
    public class ApiPermissionFilter : IAsyncActionFilter
    {
        private readonly IUserInfoRepository userInfoRepository;
        private readonly IRoleInfoRepository roleInfoRepository;

        public ApiPermissionFilter(IUserInfoRepository userInfoRepository, IRoleInfoRepository roleInfoRepository)
        {
            this.userInfoRepository = userInfoRepository;
            this.roleInfoRepository = roleInfoRepository;
        }

        // 选 IAsyncActionFilter（和 UnitOfWorkFilter 同款）而不是同步版 OnActionExecuting：
        // 中间要 await 查库（EF 异步方法），同步版等不了异步查询。
        // 流程：先验权 → 不过直接打 403 回去（next() 都不调，Action 永远不执行）→ 过了才 next() 放行
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ===== 第一步：解 Token 取用户编号 =====
            // 请求头 Authorization 的形状固定是："Bearer eyJhbGci..."（方案名+空格+凭证）
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {//检查请求头 Authorization 是否存在、是否以 "Bearer " 开头
                context.Result = new ObjectResult(new ApiResult<object>
                {//返回响应体
                    IsSuccess = false,
                    Message = "无效的授权头"
                })//设置状态码
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            // 去前缀和首尾空格，拿到纯 token 字符串
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // 拆出 JwtSecurityToken（只解码不验签——签名 [Authorize] 已经验过了，这里只是拆开读内容）
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // 从 Claims 捞 ClaimTypes.NameIdentifier —— 登录时塞的就是用户 Id
            // （对照 LoginController 第 38 行：Claim(ClaimTypes.NameIdentifier, user.Id.ToString())）
            // inline out 声明：不用提前定义 userId，TryParse 里现声明现赋值带出来
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value) || !long.TryParse(userIdClaim.Value, out long userId))
            {
                context.Result = new ObjectResult(new ApiResult<object>
                {
                    IsSuccess = false,
                    Message = "无效的用户身份"
                })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            // ===== 第二步：查角色（照阶段B规范：接收结果再遍历）=====
            var dbUser = await userInfoRepository
                .LoadEntities(u => u.Id == userId)
                .Include(u => u.RoleInfos)
                .FirstOrDefaultAsync();
            if (dbUser == null)
            {
                context.Result = new ObjectResult(new ApiResult<object>
                {
                    IsSuccess = false,
                    Message = "用户不存在"
                })
                { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }

            // ===== 第三步：查"这个用户能调的接口清单" =====
            // foreach 每个角色，二次 await 查 roleInfoRepository：
            // Include(PermissionInfos) 再 ThenInclude(PermissionApi)——顺着权限拐进 API 子表，一次带齐三层
            // 筛 type==API 的权限，把子表里的 (ApiUrl, ApiMethod) 收进清单
            var allowedApis = new List<(string ApiUrl, string ApiMethod)>();
            foreach (var role in dbUser.RoleInfos)
            {
                var dbRole = await roleInfoRepository
                    .LoadEntities(r => r.Id == role.Id)
                    .Include(r => r.PermissionInfos)
                    .ThenInclude(p => p.PermissionApi)
                    .FirstOrDefaultAsync();
                if (dbRole != null && dbRole.PermissionInfos != null)
                {
                    foreach (var permission in dbRole.PermissionInfos)
                    {
                        if (permission.PermissionType == (int)PermissionTypeEnum.PermissionApi && permission.PermissionApi != null)
                        {
                            allowedApis.Add((permission.PermissionApi.ApiUrl, permission.PermissionApi.ApiMethod));
                        }
                    }
                }
            }

            // ===== 第四步：取当前请求的"地址+方式" =====
            var requestPath = context.HttpContext.Request.Path.Value;    //如 /api/Departments
            var requestMethod = context.HttpContext.Request.Method;      //GET / POST / DELETE

            // ===== 第五步：比对 —— 请求的 (path, method) 是否命中清单里任意一条 =====
            bool hasPermission = allowedApis.Any(api =>
                string.Equals(requestPath, api.ApiUrl, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(requestMethod, api.ApiMethod, StringComparison.OrdinalIgnoreCase)
            );

            if (hasPermission)
            {
                await next(); // 有权限，放行
            }
            else
            {
                // 没权限，拦截打回 403：Result 一赋值，管线就跳过 next()，Action 不执行——这就是"拦截"
                context.Result = new ObjectResult(new ApiResult<object>
                {
                    IsSuccess = false,
                    Message = "无 API 权限"
                })
                { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
