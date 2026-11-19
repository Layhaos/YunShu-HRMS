using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 权限基础表：菜单/按钮/API 三种权限共用一张主表，公共字段（名称/编码/类型）存这里，
    /// 各自的"个性字段"（图标/按钮样式/API地址）拆到三张 1对1 子表——
    /// 好处：主表不堆一堆互相不相干的空列，子表各管各的
    /// </summary>
    public class PermissionInfo : BaseEntity<int>
    {
        /// <summary>
        /// 权限名称：菜单权限=菜单名，按钮权限=按钮名，API权限=API名
        /// </summary>
        public string? PermissionName { get; set; }

        /// <summary>
        /// 权限类型：1菜单 / 2按钮 / 3API（PermissionTypeEnum）
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// 权限编码：有这个编码=有这个权限。
        /// 菜单编码 → 前端动态路由的判断依据（路由 name 必须等于编码）
        /// 按钮编码 → 前端 v-if 控按钮
        /// API编码  → 后端 ActionFilter 校验接口调用资格
        /// </summary>
        public string? PermissionCode { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string? PermissionDescription { get; set; }

        /// <summary>
        /// 权限的父子关系：菜单是爹，按钮和 API 是挂在菜单下的子权限
        /// （点菜单→看到页面→页面有按钮→按钮调API，一条链）
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 一对一导航：API 权限的"个性字段"（三选一，按 PermissionType 只填一个）
        /// </summary>
        public PermissionApi? PermissionApi { get; set; }

        /// <summary>
        /// 一对一导航：菜单权限的"个性字段"
        /// </summary>
        public PermissionMenu? PermissionMenu { get; set; }

        /// <summary>
        /// 一对一导航：按钮权限的"个性字段"
        /// </summary>
        public PermissionPoint? PermissionPoint { get; set; }

        /// <summary>
        /// 导航属性：一个权限被多个角色拥有
        /// </summary>
        public List<RoleInfo> RoleInfos { get; set; } = new List<RoleInfo>();
    }
}
