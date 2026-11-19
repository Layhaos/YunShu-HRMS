using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// API权限子表：记录"哪个接口地址+哪种请求方式"——
    /// 后端 ActionFilter 拿当前请求的地址+方式来这里反查，查到编码才放行
    /// </summary>
    public class PermissionApi : BaseEntity<int>
    {
        /// <summary>
        /// API 的地址（如 /api/Departments）
        /// </summary>
        public string? ApiUrl { get; set; }

        /// <summary>
        /// 请求方式（GET/POST/DELETE...）
        /// </summary>
        public string? ApiMethod { get; set; }

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
