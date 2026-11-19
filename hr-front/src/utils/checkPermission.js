// ★ ⑭ 阶段C：功能权限校验工具（控按钮用）
// 用法（组件里）：v-if="checkPointPermission('department:del')" —— 有编码才渲染按钮
// 数据源：登录后守卫已把按钮编码数组存进 store.state.user.permissions
import store from "@/store";

// 老师版函数名是 checkPointPermssion（ Permssion 少个 i，拼写瑕疵）——练习版统一规范拼写
export function checkPointPermission(point) {
    const permissions = store.state.user.permissions || [];
    // 空数组显式挡一道（some 在空数组上永远 false，但先挡更清晰、防 undefined）
    if (permissions.length === 0) return false;
    return permissions.some(p => p === point);
}
