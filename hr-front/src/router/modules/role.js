// ⑭ 尾巴任务2：角色管理路由——照 permission.js 的模子
// 壳 name = "role"：必须等于后端 T1018 里菜单权限编码（种子 id=8 已灌 + 挂管理员）
import Layout from "@/layout/index.vue";

export default {
    path: "/",
    name: "role",   // ← 权限编码（菜单权限）
    component: Layout,
    children: [
        {
            path: "role",
            component: () => import("@/views/role/index.vue"),
            meta: { title: "角色管理", icon: "Tickets" },
        },
    ],
};
