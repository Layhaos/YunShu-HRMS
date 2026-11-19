using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class UserInfo : BaseEntity<int>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 密码
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
        /// 性别：0表示男，1表示女
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 用户头像，存储头像路径
        /// </summary>
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// 导航属性：一个用户可以发布多篇文章（一对多的"一"方）
        /// </summary>
        public List<Article> Articles { get; set; } = new List<Article>();

        /// <summary>
        /// 导航属性：一个用户拥有多个角色（多对多，中间表 T_UserInfos_RoleInfos，
        /// 关系在 RoleInfoConfig 里配置——导航属性两边都挂，配置只需写一处）
        /// </summary>
        public List<RoleInfo> RoleInfos { get; set; } = new List<RoleInfo>();
    }
}
