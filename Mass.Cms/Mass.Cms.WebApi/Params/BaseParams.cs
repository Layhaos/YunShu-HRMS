namespace Mass.Cms.WebApi.Params
{
    /// <summary>
    /// 前端"允许传"的分页参数。和 Entity/Search/BaseSearch 的区别就看一个字段：
    /// 这里【没有】TotalCount —— 总条数是后端算出来回填的，不该由前端传。
    /// 不单独做 Params 类的话，Swagger 会给 TotalCount 生成一个输入框，前端传了也白传
    /// </summary>
    public class BaseParams
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        /// <summary> true=升序 false=降序 </summary>
        public bool Order { get; set; }
    }
}
