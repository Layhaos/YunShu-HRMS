import Layout from "@/layout/index.vue"; // 系统壳：静态 import（父路由固定，不懒加载——每页都要用它）


    
export default {
        path: "/",
        component: Layout,      // 父路由：壳；children 里的页面画进壳的 <router-view>
        redirect: "/home",      // 访问 / 时进默认子页
        children: [
            {
                path: "home",
                component: () => import("@/views/home/index.vue"),
                meta: { title: "首页", icon: "HomeFilled" },   // meta 给"菜单生成器"用的原料
            },
        ],
    }