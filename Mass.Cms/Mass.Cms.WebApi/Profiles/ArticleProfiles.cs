using AutoMapper;
using Mass.Cms.Entity;
using Mass.Cms.WebApi.Models;

namespace Mass.Cms.WebApi.Profiles
{
    /// <summary>
    /// 文章映射配置：Article → ArticleDto 同名字段自动映射，一个 CreateMap 搞定
    /// </summary>
    public class ArticleProfiles : Profile
    {
        public ArticleProfiles()
        {
            CreateMap<Article, ArticleDto>();
        }
    }
}
