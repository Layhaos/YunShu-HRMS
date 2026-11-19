using Autofac;
using System.Reflection;

namespace Mass.Cms.WebApi.AutofaceDI
{
    /// <summary>
    /// Autofac 批量注册模块：Override Load，容器构建时在这里注册一切
    /// 约定：Service 项目里类名都以 Service 结尾，Repository 项目里以 Repository 结尾
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //通过Autofac进行批量注册，不用再自己一个个手动AddScoped
            //1.获取程序集
            var assemblies = new[]
            {
                Assembly.Load("Mass.Cms.Service"),
                //Assembly.Load("Mass.Cms.IService"),
                Assembly.Load("Mass.Cms.Repository"),
                //Assembly.Load("Mass.Cms.IRepository")
            };

            //2.注册方法
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t=>t.Name.EndsWith("Service")||t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }
}
