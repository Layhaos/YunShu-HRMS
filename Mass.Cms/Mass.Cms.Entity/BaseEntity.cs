using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{//存储公共属性
    //Id的泛型化：每张表的主键类型自己决定（int/long…），公共字段仍然只有基类一处
    public class BaseEntity<T>
    {
        public T Id { get; set; }

        /// <summary>
        /// 添加记录时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新记录时间
        /// </summary>
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 软删除标记：1=已删除。查询时按 IsDeleted==false 过滤，而不是物理 Delete
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
