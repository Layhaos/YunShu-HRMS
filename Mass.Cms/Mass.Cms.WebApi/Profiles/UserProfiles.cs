using AutoMapper;
using Mass.Cms.Entity;
using Mass.Cms.WebApi.Models;

namespace Mass.Cms.WebApi.Profiles
{
    /// <summary>
    /// 用户映射配置。两条：
    /// ① UserInfo → UserDto（查询时用）
    /// ② UserCreateDto → UserInfo（新增时用）
    /// </summary>
    public class UserProfiles : Profile
    {
        public UserProfiles()
        {
            //同名属性自动映射：Id/UserName/UserEmail/UserPhone/Gender/PhotoUrl/EditDate
            //CreateDate 两边类型不同（实体 DateTime → DTO string），ForMember 单独指定规则
            CreateMap<UserInfo, UserDto>()
                .ForMember(dest => dest.CreateDate,
                    opt => opt.MapFrom(src => src.CreateDate.Year + "年"
                                                + src.CreateDate.Month + "月"
                                                + src.CreateDate.Day + "日"));

            //创建DTO → 实体：新增接口用的方向，同名自动映射
            //（前端没送的 Id/CreateDate/EditDate/IsDeleted 由控制器手动补默认值）
            CreateMap<UserCreateDto, UserInfo>();
        }
    }
}
