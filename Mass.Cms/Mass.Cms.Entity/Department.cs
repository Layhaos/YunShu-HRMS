using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 部门：树形结构，靠 ParentId 自引用（0 = 顶级部门）
    /// 存库只存"平铺的一条条"，树形由前端拿平铺列表递归转换
    /// </summary>
    public class Department : BaseEntity<int>
    {
        /// <summary>
        /// 部门名称（同级之间不能重名）
        /// </summary>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// 部门编码（全表唯一）
        /// </summary>
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// 部门描述
        /// </summary>
        public string? DepartmentDescr { get; set; }

        /// <summary>
        /// 父部门 Id：0 表示顶级部门。
        /// 注意：故意不给 Parent/Children 导航属性、不建真外键——
        /// 顶级用 0 当"不存在的爹"，真外键会当场拦下顶级插入。
        /// 树的层级关系由前端递归转换承担，数据库只管平铺。
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 负责人（指向 UserInfo.Id，暂不挂外键，前端下拉选择）
        /// </summary>
        public int? ManagerId { get; set; }
    }
}
