namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 用户的"创建DTO"：只收新增接口该收的字段。
    /// 为什么不用 UserDto 收：UserDto 是展示用的（有只算出来不改的格式化日期）；
    /// 为什么不用 UserInfo 实体收：会暴露 Id/CreateDate/IsDeleted 这些后端自己管的东西
    /// </summary>
    public class UserCreateDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 密码：新增用户时前端必须提供
        /// </summary>
        public string? UserPassword { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? UserPhone { get; set; }

        /// <summary>
        /// 性别：0男 1女。前端不传时视为 0
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 头像路径：前端不传时走默认图片
        /// </summary>
        public string? PhotoUrl { get; set; }
    }
}
