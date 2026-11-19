// ⑭ 尾巴任务3：员工管理路由——壳 name = "user"（种子 id=5 菜单权限编码已灌）
import Layout from "@/layout/index.vue";

export default {
    path: "/",
    name: "user",   // ← 权限编码（菜单权限）
    component: Layout,
    children: [
        {
            path: "user",
            component: () => import("@/views/user/index.vue"),
            meta: { title: "员工管理", icon: "User" },
        },
    ],
};
