import { getStorage, setStorage, removeStorage } from "@/utils/storage";
import { login, getUserInfo } from "@/api/user";

const TokenKey = 'hr_token';
const PhoneKey = 'hr_phone';


export default {
    namespaced: true,

    state: () => ({
        token: getStorage(TokenKey) || '',
        userInfo: {

        },
        // 功能权限编码数组（按钮编码，如 ["department:del"]）——控按钮用（⑭ 阶段C）
        permissions: [],
    }),
    mutations: {
        updateToken(state, token) {
            state.token = token;
            setStorage(TokenKey, token);
        },
        updateUserInfo(state, info) {
            state.userInfo = info;
        },
        // 按钮权限编码入库（菜单编码不在这存——它去 permission 模块干路由过滤）
        updatePermissions(state, points) {
            state.permissions = points || [];
        },
        removeToken(state) {
            state.token = '';
            state.userInfo = {};
            state.permissions = [];
            removeStorage(TokenKey);
            removeStorage(PhoneKey);
        },
    },
    actions: {
        async login({ commit }, loginForm) {
            const token = await login(loginForm);
            commit('updateToken', token);
            setStorage(PhoneKey, loginForm.userPhone);
            return token;
        },
        async getUserInfo({ commit }) {
            const phone = getStorage(PhoneKey);
            // 手机号没了就抛错，交给守卫的 catch 兜底（双清 + 踢回登录），绝不能静默 return
            if (!phone) throw new Error("手机号不存在，请重新登录");
            const result = await getUserInfo(phone);
            commit('updateUserInfo', result.user);
            // 按钮编码存用户模块（控按钮）；菜单编码不在这存——由守卫转手给 permission 模块过滤路由
            commit('updatePermissions', result.permissions.points);
            // 整个权限字典 return 给守卫：menus 要去 permission/filterRoutes 干活
            return result.permissions;
        },
    },
};