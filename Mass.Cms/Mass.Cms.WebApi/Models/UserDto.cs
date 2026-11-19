namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 用户的"查询DTO"。Type 已对齐前端需求：CreateDate 变成 "2026年11月1日" 字符串
    /// TODO【步6现场】Articles 集合属性先不放——等你在 GET /api/users/articles
    /// 里亲眼看见序列化死循环爆炸后，再回来加它，你就懂它为什么必须存在
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 注意类型是 string 和实体的 DateTime 不同 —— ForMember 手动映射格式化
        /// </summary>
        public string CreateDate { get; set; } = string.Empty;

        public DateTime EditDate { get; set; }

        public string? UserName { get; set; }

        public string? UserEmail { get; set; }

        public string? UserPhone { get; set; }

        public int Gender { get; set; }

        public string? PhotoUrl { get; set; }

        /// <summary>
        /// 该用户发布的所有文章（第三幕救场属性）。
        /// 循环被打破的关键：ArticleDto 是平的、没有往回指的 UserInfo ——
        /// UserInfo→Articles→(ArticleDto，到此为止)，链有尽头，序列化不再绕圈
        /// </summary>
        public List<ArticleDto> Articles { get; set; } = new();
    }
}
