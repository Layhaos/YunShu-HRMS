// ★ ⑭ 阶段C：权限路由模块（namespaced）
// 职责：拿着后端吐的菜单权限编码，从 asyncRoutes 里筛出"你有资格进"的路由壳
// 双份落点：commit 进 Vuex → Layout 菜单读（addRoute 进 router 的路由不具响应式，
// 菜单必须读 Vuex 才会刷新）；return 给守卫 → 逐个 router.addRoute 挂真路由
import { staticRoutes, asyncRoutes } from "@/router";

export default {
    namespaced: true,

    state() {
        return {
            // 菜单数据源：初始=静态路由（只有首页）；过滤完成后=静态+动态
            // Layout 的菜单生成器读的是它，不是 router.options.routes
            routes: staticRoutes,
        };
    },

    mutations: {
        // 过滤出的动态路由 + 静态基线 = 菜单新基线（展开合并，别覆盖丢了首页）
        setRoutes(state, dynamicRoutes) {
            state.routes = [...staticRoutes, ...dynamicRoutes];
        },
    },

    actions: {
        // ★★★ ⑭ 阶段C【你写区】：filterRoutes（约 6 行）★★★
        //入参 menus：后端权限字典里的菜单编码数组，如 ["department"]
        //思路：
        //  1. 造空数组 routes
        //  2. forEach 每个 menu 编码 → asyncRoutes.filter(item => item.name === menu)
        //     命中的"壳路由"收编进 routes（老师版一行流：routes.push(...asyncRoutes.filter(...))）
        //  3. context.commit("setRoutes", routes) —— 菜单基线更新，Layout 自动长出新菜单
        //  4. return routes —— 守卫拿去逐个 router.addRoute（Vuex 只管菜单，真路由得挂 router）
        filterRoutes(context, menuList) {
            const routes = [];
            menuList.forEach(menu => {
                const temp = asyncRoutes.filter(item => item.name === menu);
                routes.push(...temp);
            });
            context.commit("setRoutes", routes);
            return routes;
        },
    },
};
