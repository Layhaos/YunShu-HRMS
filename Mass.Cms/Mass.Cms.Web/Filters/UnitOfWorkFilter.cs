using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.Web.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Mass.Cms.Web.Filters
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if(result.Exception != null)
            {
                return;
            }
            var controllerActionDesc = context.ActionDescriptor as ControllerActionDescriptor;
            if(controllerActionDesc == null)
            {
                return;
            }
            var unitOfAttr = controllerActionDesc.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
            if(unitOfAttr == null)
            {
                return;
            }

            //执行操作，要拿到MyDbContext对象实例
            //用构造函数从IOC注入，或者服务定位
            var myDbcontext = context.HttpContext.RequestServices.GetService<MyDbContext>();
            if(myDbcontext != null)
            {
                await myDbcontext.SaveChangesAsync();
            }
        }
    }
}
