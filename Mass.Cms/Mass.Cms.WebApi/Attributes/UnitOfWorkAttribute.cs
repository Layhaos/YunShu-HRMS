namespace Mass.Cms.WebApi.Attributes
{
    //标记在"要统一提交数据库"的 Action 方法上，Filter 看到它才执行 SaveChanges
    [AttributeUsage(AttributeTargets.Method)]
    public class UnitOfWorkAttribute : Attribute
    {
    }
}
