using Mass.Cms.Entity;
using Mass.Cms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    public interface IArticleService : IBaseService<Article>
    {
        /// <summary>
        /// 多条件分页搜索文章
        /// </summary>
        /// <param name="articleSearch">搜索条件（含分页/排序/回写 TotalCount 的载体）</param>
        /// <param name="isDeleted">查"未删除"(false)还是"已删除"(true)</param>
        /// <returns>当前页的数据（IQueryable，还没执行 SQL）</returns>
        IQueryable<Article> LoadSearchArticles(ArticleSearch articleSearch, bool isDeleted);
    }
}
