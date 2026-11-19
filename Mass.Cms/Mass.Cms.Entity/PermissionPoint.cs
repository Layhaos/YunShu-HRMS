using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 功能权限子表：控制页面里的按钮（显示/隐藏、可点/禁用）
    /// </summary>
    public class PermissionPoint : BaseEntity<int>
    {
        /// <summary>
        /// 按钮的样式（class 名）
        /// </summary>
        public string? PointClass { get; set; }

        /// <summary>
        /// 按钮的图标
        /// </summary>
        public string? PointIcon { get; set; }

        /// <summary>
        /// 按钮的状态
        /// </summary>
        public int PointStatus { get; set; }

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
