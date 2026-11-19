using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.IService
{
    //角色业务的专属接口
    public interface IRoleInfoService : IBaseService<RoleInfo>
    {
        //角色列表（授权页的表格数据）
        Task<List<RoleInfo>> GetRoles();

        //该角色当前勾中的权限 id 集合（弹窗打开时回显 setCheckedKeys 用）
        Task<List<int>> GetRolePermissionIds(int roleId);

        //角色授权：前端把勾选的权限 id 用逗号串传上来，删旧插新维护 T_RoleInfos_Permissions
        //返回空串=成功；非空=错误提示（防自锁：「角色管理」菜单权限不可移除）
        Task<string> AssignPermissions(int roleId, string permissionIds);
    }
}
