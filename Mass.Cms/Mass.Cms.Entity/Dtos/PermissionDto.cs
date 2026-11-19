using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity.Dtos
{
    /// <summary>
    /// 权限混合 DTO：新增权限一次要写两张表（主表 + 按类型的子表），
    /// PermissionInfo 和三个子表实体都接不全这些字段 → 造"两份都要"的混合 DTO。
    /// 跨层旅行住公共层（Entity 项目）——依赖方向单向朝下，下层永远看不见上层。
    /// </summary>
    public class PermissionDto
    {
        //==== 主表公共字段（三种类型都填）====
        /// <summary>权限名称：菜单名 / 按钮名 / API 名</summary>
        public string? PermissionName { get; set; }

        /// <summary>权限编码：菜单给前端动态路由匹配 / 按钮给 v-if / API 给后端 Filter 比对</summary>
        public string? PermissionCode { get; set; }

        /// <summary>权限类型：1菜单 2按钮 3API（PermissionTypeEnum）</summary>
        public int PermissionType { get; set; }

        /// <summary>权限描述</summary>
        public string? PermissionDescription { get; set; }

        /// <summary>父权限：菜单=0 顶级；按钮/API 挂在某个菜单权限下面</summary>
        public int ParentId { get; set; }

        //==== 菜单权限个性字段（type=1 才用）====
        public string? MenuIcon { get; set; }

        /// <summary>菜单排序</summary>
        public string? MenunOrder { get; set; }

        //==== 按钮权限个性字段（type=2 才用）====
        public string? PointClass { get; set; }

        public string? PointIcon { get; set; }

        public int PointStatus { get; set; }

        //==== API 权限个性字段（type=3 才用）====
        /// <summary>API 地址，如 /api/Departments</summary>
        public string? ApiUrl { get; set; }

        /// <summary>请求方式 GET/POST/DELETE...</summary>
        public string? ApiMethod { get; set; }
    }
}
