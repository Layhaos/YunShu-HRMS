namespace Mass.Cms.WebApi.Params
{
    /// <summary>
    /// 文章搜索接口的前端入参：分页（继承）+ 文章专属条件
    /// </summary>
    public class ArticleParams : BaseParams
    {
        /// <summary> 标题关键字，可为空 </summary>
        public string? Title { get; set; }

        /// <summary> 创建时间起（含） </summary>
        public string? FromCreateDate { get; set; }

        /// <summary> 创建时间止（含） </summary>
        public string? ToCreateDate { get; set; }

        /// <summary>
        /// 塑形参数：前端指定"只返回哪些字段"（如 fields=id,title）。
        /// 空或不传 = 不塑形，返回全量字段
        /// </summary>
        public string? Fields { get; set; }
    }
}
