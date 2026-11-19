using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Entity.Search
{
    /// <summary>
    /// 文章的多条件搜索：公共分页条件（继承 BaseSearch）+ 文章专属条件
    /// 时间字段用 string 接收（前端传来的是字符串），用到时再转 DateTime
    /// </summary>
    public class ArticleSearch : BaseSearch
    {
        /// <summary>
        /// 标题关键字：不为空时按"包含"搜索
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 创建时间范围-起（含），如 "2026-10-01"
        /// </summary>
        public string? FromCreateDate { get; set; }

        /// <summary>
        /// 创建时间范围-止（含）
        /// </summary>
        public string? ToCreateDate { get; set; }
    }
}
