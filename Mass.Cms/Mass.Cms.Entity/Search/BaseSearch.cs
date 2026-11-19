using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Entity.Search
{
    /// <summary>
    /// 分页搜索的公共条件：所有搜索条件类的基类
    /// 放在 Entity 项目的原因（老师课上的设计）：
    /// Search 类要被 Service、WebApi 等多个层使用，放在实体层，
    /// 引用了 Entity 的项目就能直接用，避免项目间互相引用形成循环依赖
    /// </summary>
    public class BaseSearch
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        /// <summary>
        /// 符合条件的总条数：由后端查询时算出来[回写]给调用方，
        /// 不是前端传进来的！这是它之后要和 Params 类分离的原因
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// true=升序 false=降序
        /// </summary>
        public bool Order { get; set; }
    }
}
