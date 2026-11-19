import Layout from "@/layout/index.vue"; // 系统壳：静态 import（父路由固定，不懒加载——每页都要用它）

// ⑬阶段C：部门管理路由——照 home.js 的模子
// ⑭阶段C 升级：这个壳要凭权限编码"department"才能挂——
// name 必须等于权限编码（后端 T1018_PermissionInfos 里种子数据已灌 code=department）
// 进了 children 数组 + meta 有 title，侧边菜单就自动长出这一项（菜单是从路由表生成的）
export default {
    path: "/",
    name: "department",   // ← filterRoutes 按这个名字匹配菜单权限编码
    component: Layout,
    children: [
        {
            path: "department",
            component: () => import("@/views/department/index.vue"),
            meta: { title: "部门管理", icon: "Grid" },   // icon 是全局注册过的 Element 图标名
        },
    ],
}
