// ⑭ 尾巴：权限管理路由——照 department.js 的模子
// 壳 name = "permission"：必须等于后端 T1018 里菜单权限编码（种子 id=4 已灌）
// filterRoutes 按它匹配；children 带 meta，侧边菜单自动长出这一项
import Layout from "@/layout/index.vue";

export default {
    path: "/",
    name: "permission",   // ← 权限编码（菜单权限）
    component: Layout,
    children: [
        {
            path: "permission",
            component: () => import("@/views/permission/index.vue"),
            meta: { title: "权限管理", icon: "Key" },
        },
    ],
};
