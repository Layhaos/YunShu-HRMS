import router from "@/router";
import store from "@/store";

// 白名单：不登录也该进的页面
const whiteList = ["/login"]

router.beforeEach(async (to, from, next) => {
    const token = store.state.user.token;

    // 第一关：白名单页面（/login）——但带票的人来登录页，弹回主页
    if (whiteList.includes(to.path)) {
        if (token) {
            next("/home");
        } else {
            next();
        }
        return;
    }

    // 第二关：非白名单页面，没票 → 踢回登录页
    if (!token) {
        next("/login");
        return;
    }

    // 第三关：有票有资料（点菜单场景）→ 直接过，防重复请求
    const isUserInfoEmpty = Object.keys(store.state.user.userInfo).length === 0;
    if (!isUserInfoEmpty) {
        next();
        return;
    }

    // 第四关：有票没资料（首次进场 / F5 刷新）→ 拉一次用户信息
    try {
        // dispatch 返回值 = user/getUserInfo return 的权限字典 { menus, points }
        const permissions = await store.dispatch("user/getUserInfo");
        // ① 过滤：拿菜单编码去 permission 模块筛出"有资格"的路由壳
        const routes = await store.dispatch("permission/filterRoutes", permissions.menus);
        // ② 挂真路由：Vuex 只管菜单显示，能不能真跳转还得 router 表里有
        routes.forEach(route => {
            router.addRoute(route);
        });
        // ③ 404 最后挂（注册靠前会拦住刚挂的动态路由，刷新就 404）
        // 带上名字：登出 resetRouter 要按名字把它拆掉（换号登录时守卫会重新挂新的）
        router.addRoute({
            path: "/:pathMatch(.*)*",
            name: "NotFound",
            component: () => import("@/views/notfound/index.vue"),
        });
        // 路由表刚变了 → next(to.path) 重新解析目标地址（写 next() 会扑空）
        next(to.path);
    } catch (error) {
        // 兜底：资料拉不到（手机号没了 / 401 / 后端炸）→ 双清 + 踢回登录页
        store.commit("user/removeToken");
        next("/login");
    }
})