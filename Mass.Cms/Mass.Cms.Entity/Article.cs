using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 文章实体：一对多里的"多"方（用户→文章）。
    /// 注意继承 BaseEntity&lt;long&gt;，体会基类泛型 Id 的意义：文章主键用 long（bigint），向大容量预留
    /// </summary>
    public class Article : BaseEntity<long>
    {
        public string? Title { get; set; }

        public string? Content { get; set; }

        /// <summary>
        /// 导航属性：这篇文章属于哪个用户（一对多的"多"方）
        /// </summary>
        public UserInfo? UserInfo { get; set; }
    }
}
