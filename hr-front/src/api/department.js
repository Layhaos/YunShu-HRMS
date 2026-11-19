import request from '@/utils/request';
// ⑬阶段C：部门接口封装——形状照 api/user.js 的模子
// 拦截器已把 ApiResult 拆包：这里 resolve 出来的直接就是 data（平铺列表）

//查全部部门（后端已滤软删除，返回平铺列表）
export const getDepartments = () => {
    return request.get('/Departments');
};

//新增部门（对象走请求体）
export const addDepartment = (data) => {
    return request.post('/Departments', data);
};

//删除部门（软删除：id 走路径段）
export const deleteDepartment = (id) => {
    return request.delete(`/Departments/${id}`);
};
