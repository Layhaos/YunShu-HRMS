using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Mass.Cms.WebApi.Filters
{
    //全局过滤器：Action 跑完后检查有没有 [UnitOfWork]，有 + 无异常才统一 SaveChangesAsync
    //原理回顾（你第一课写的）：仓储 Add 只"记待办"，真正写库只有这一个出口
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            //出异常就放弃保存：数据库保持原样 = 事务失效保护
            if (result.Exception != null)
            {
                return;
            }

            var controllerActionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDesc == null)
            {
                return;
            }

            var unitOfAttr = controllerActionDesc.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if (unitOfAttr == null)
            {
                return;
            }

            var myDbContext = context.HttpContext.RequestServices.GetService<MyDbContext>();
            if (myDbContext != null)
            {
                await myDbContext.SaveChangesAsync();
            }
        }
    }
}
