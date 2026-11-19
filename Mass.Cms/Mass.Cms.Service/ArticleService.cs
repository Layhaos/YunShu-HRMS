using Mass.Cms.Entity;
using Mass.Cms.Entity.Search;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Service
{
    public class ArticleService : BaseService<Article>, IArticleService
    {
        private readonly IArticleRepository articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            base.repository = articleRepository;
            this.articleRepository = articleRepository;
        }

        public IQueryable<Article> LoadSearchArticles(ArticleSearch articleSearch, bool isDeleted)
        {
            var temp = articleRepository.LoadEntities(a => a.IsDeleted == isDeleted);

            if (!string.IsNullOrEmpty(articleSearch.Title))
            {
                temp = temp.Where(a => a.Title.Contains(articleSearch.Title));
            }

            //时间区间
            if (!string.IsNullOrEmpty(articleSearch.FromCreateDate))
            {
                temp = temp.Where(a => a.CreateDate >= Convert.ToDateTime(articleSearch.FromCreateDate));
            }
            if (!string.IsNullOrEmpty(articleSearch.ToCreateDate))
            {
                temp = temp.Where(a => a.CreateDate <= Convert.ToDateTime(articleSearch.ToCreateDate));
            }
            articleSearch.TotalCount = temp.Count();

            //排序分页
            if (articleSearch.Order)
            {
                temp = temp.OrderBy(a => a.Id)
                    .Skip((articleSearch.PageIndex - 1) * articleSearch.PageSize)
                    .Take(articleSearch.PageSize);
            }
            else
            {
                temp = temp.OrderByDescending(a => a.Id)
                    .Skip((articleSearch.PageIndex - 1) * articleSearch.PageSize)
                    .Take(articleSearch.PageSize);
            }

            return temp;
        }
    }
}
