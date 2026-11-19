using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 角色：权限的"批发商"——权限不直接发给用户，而是发给角色，用户再领角色
    /// 用户↔角色 多对多（EF 自动生成中间表），角色↔权限 多对多（同上）
    /// </summary>
    public class RoleInfo : BaseEntity<int>
    {
        /// <summary>
        /// 角色名（如：管理员、普通员工）
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// 角色备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 导航属性：一个角色可以被多个用户拥有（多对多的"多"方之一）
        /// </summary>
        public List<UserInfo> UserInfos { get; set; } = new List<UserInfo>();

        /// <summary>
        /// 导航属性：一个角色拥有多个权限
        /// </summary>
        public List<PermissionInfo> PermissionInfos { get; set; } = new List<PermissionInfo>();
    }
}
