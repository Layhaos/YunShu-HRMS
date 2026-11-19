namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 文章的"查询DTO"：只放前端要展示的字段。
    /// 注意没有 UserInfo 导航属性 —— DTO 是"平的"，序列化再也不会有循环
    /// </summary>
    public class ArticleDto
    {
        public long Id { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EditDate { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }
    }
}
