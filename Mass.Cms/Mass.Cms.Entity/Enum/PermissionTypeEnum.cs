using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity.Enum
{
    /// <summary>
    /// 权限类型：一张基础表存三种权限，靠这个字段分流到三张子表
    /// </summary>
    public enum PermissionTypeEnum
    {
        /// <summary>
        /// 菜单权限（控制左侧导航显示哪个菜单）
        /// </summary>
        PermissionMenu = 1,
        /// <summary>
        /// 功能权限（控制页面里哪个按钮可见/可点）
        /// </summary>
        PermissionPoint = 2,
        /// <summary>
        /// Api权限（控制哪个接口允许被调用）
        /// </summary>
        PermissionApi = 3
    }
}
