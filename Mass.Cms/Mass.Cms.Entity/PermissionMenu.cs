using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 菜单权限子表：菜单名称在 PermissionInfo 主表里，这里只放菜单的"个性字段"
    /// </summary>
    public class PermissionMenu : BaseEntity<int>
    {
        /// <summary>
        /// 菜单图标（前端 element-plus 图标名）
        /// </summary>
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单排序
        /// </summary>
        public string? MenunOrder { get; set; }

        /// <summary>
        /// 一对一导航：回指主表
        /// </summary>
        public PermissionInfo? PermissionInfo { get; set; }

        /// <summary>
        /// 显式外键：指向权限主表
        /// </summary>
        public int PermissionId { get; set; }
    }
}
