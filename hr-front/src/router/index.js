import { createRouter, createWebHistory } from "vue-router";
import home from "./modules/home.js"
import department from "./modules/department.js"
import permission from "./modules/permission.js"
import role from "./modules/role.js"
import user from "./modules/user.js"

// ★ ⑭ 阶段C 改造：路由表分两个池子
//staticRoutes：人人可见——登录页 + 首页壳（首页不做权限控制，练习版约定）
//asyncRoutes：要凭"权限编码"过滤后才允许挂——壳对象的 name === 权限编码，filterRoutes 靠它匹配
const staticRoutes = [
    {
        path: "/login",
        name: "LoginIndex",
        component: () => import("@/views/login/index.vue"),
    },
    home,
];

// ⚠️ 404 从静态数组里撤了！它必须"最后动态添加"：
// 注册靠前的话，/:pathMatch(.*)* 会拦住后挂的动态路由（刷新就 404）
// ——挂载动作在守卫 checkAuthentication.js 的【你写区】里

const asyncRoutes = [department, permission, role, user];

// createRouter 只装静态的——动态的等守卫拿到权限编码、过滤完再逐个 addRoute
const routes = [...staticRoutes]

//⑭ 尾巴任务4：登出拆动态路由（防换号越权）
//拆法：遍历当前路由表，名字不在静态池里的（department/permission/role/user 壳 + 404）全部 removeRoute
export function resetRouter() {
    const staticNames = new Set(staticRoutes.map((r) => r.name));
    router.getRoutes().forEach((r) => {
        if (r.name && !staticNames.has(r.name)) {
            router.removeRoute(r.name);
        }
    });
}

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router

//导出给 Vuex permission 模块用：
//  staticRoutes → 菜单基线（state.routes 初始值）
//  asyncRoutes  → filterRoutes 的过滤原料
export { staticRoutes, asyncRoutes };
