import request from '@/utils/request';
// ⑭ 尾巴任务2：角色接口封装——形状照 api/permission.js 的模子
// 拦截器已把 ApiResult 拆包：这里 resolve 出来的直接就是 data

//查角色列表（授权页表格）
export const getRoles = () => {
    return request.get('/Roles');
};

//查某角色已勾选的权限 id 集合（弹窗回显 setCheckedKeys 用）
export const getRolePermissionIds = (roleId) => {
    return request.get(`/Roles/${roleId}/permissionIds`);
};

//角色授权：勾选的权限 id 用逗号串传（串值契约，空选发 "0" 保底）
export const assignPermissions = (roleId, permissionIds) => {
    return request.post('/Roles/assign', { roleId, permissionIds });
};
