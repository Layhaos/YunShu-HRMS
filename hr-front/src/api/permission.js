import request from '@/utils/request';
// ⑭ 尾巴：权限接口封装——形状照 api/department.js 的模子
// 拦截器已把 ApiResult 拆包：这里 resolve 出来的直接就是 data（平铺列表）

//查全部权限（后端已滤软删除，返回平铺列表）
export const getPermissions = () => {
    return request.get('/Permissions');
};

//新增权限（混合 DTO 走请求体：主表字段+按类型的子表个性字段）
export const addPermission = (data) => {
    return request.post('/Permissions', data);
};
