using AutoMapper;
using Mass.Cms.Entity.Search;
using Mass.Cms.IService;
using Mass.Cms.WebApi.Extensions;
using Mass.Cms.WebApi.Helper;
using Mass.Cms.WebApi.Models;
using Mass.Cms.WebApi.Params;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mass.Cms.WebApi.Controllers
{
    /// <summary>
    /// 文章搜索接口（多条件 + 分页 + 排序）
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService articleService;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public ArticlesController(IArticleService articleService, IMapper mapper, IMemoryCache memoryCache)
        {
            this.articleService = articleService;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// 多条件搜索文章  GET /api/articles?title=xx&fromCreateDate=...
        /// [FromQuery]：URL 问号传参 → 自动装进 ArticleParams（结构型数据绑定）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] ArticleParams articleParams)
        {
            //塑形前置校验：fields 里只要有一个字段在 ArticleDto 里不存在，就整体拦下
            //第二个参数是缓存键："articleDto:Properties" —— UserDto 等其他 DTO 换别的键，防互踩
            if (!CheckPropertyInfoHelper.IsCheckPropertyInfo<ArticleDto>(articleParams.Fields, memoryCache, "articleDto"))
            {
                return BadRequest("传递的参数fields有错误");
            }
            //Params（给前端用的）→ Search（给后端用的）：手动逐字段搬进检索对象
            //为什么不用 AutoMapper：俩类字段几乎全一样但语义层不同，手写一眼看懂不玩魔法
            ArticleSearch articleSearch = new ArticleSearch()
            {
                PageIndex = articleParams.PageIndex,
                PageSize = articleParams.PageSize,
                Order = articleParams.Order,
                Title = articleParams.Title,
                FromCreateDate = articleParams.FromCreateDate,
                ToCreateDate = articleParams.ToCreateDate,
            };

            var articles = await articleService.LoadSearchArticles(articleSearch, false).ToListAsync();

            if (articles == null || articles.Count <= 0)
            {
                return NotFound("没有找到对应的文章");
            }

            var articleDtos = mapper.Map<List<ArticleDto>>(articles);

            //前端 UI 要的格式：total + data 数组；data 走塑形——fields 为空时全量返回
            var totalCount = articleSearch.TotalCount;
            return Ok(new { total = totalCount, data = articleDtos.ShapeDatas<ArticleDto>(articleParams.Fields) });
        }
    }
}
