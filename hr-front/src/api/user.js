import request from '@/utils/request';
export const login = (data)=>{
    return request.post('/Login',data);
};
export const getUserInfo=(phone)=>{
    return request.get(`/Users/by-phone/${phone}`);
};

//⑭ 尾巴任务3：员工页接口（ApiResult 包壳的新接口，不走老的裸数组遗留）
//查员工列表
export const getUserList = () => {
    return request.get('/Users/list');
};

//查某用户已分配的角色 id 集合（弹窗回显）
export const getUserRoleIds = (userId) => {
    return request.get(`/Users/${userId}/roleIds`);
};

//员工分配角色：多选 join(',') 逗号串（串值契约，空选发 "0" 保底）
export const assignRoles = (userId, roleIds) => {
    return request.post('/Users/assign-roles', { userId, roleIds });
};