import request from '@/utils/request';
// M03 考勤模块接口封装——形状照 api/department.js 的模子
// 拦截器已把 ApiResult 拆包：成功 resolve 出来的直接是 data，失败 reject 出来的是提示语

//上班打卡（token 里已带当前登录人，后端自己认人）
export const clockIn = () => {
    return request.post('/Attendance/clock-in');
};

//下班打卡
export const clockOut = () => {
    return request.post('/Attendance/clock-out');
};

//我的考勤记录
export const getMyAttendance = () => {
    return request.get('/Attendance/my');
};

//提交请假/加班申请（type: 0=请假 1=加班）
export const applyLeave = (data) => {
    return request.post('/LeaveApplications', data);
};

//我的申请单
export const getMyApplications = () => {
    return request.get('/LeaveApplications/mine');
};

//待审批列表（管理员）
export const getPendingList = () => {
    return request.get('/LeaveApplications/pending');
};

//审批（isPass: true=通过 false=驳回）
export const approveLeave = (data) => {
    return request.post('/LeaveApplications/approve', data);
};
