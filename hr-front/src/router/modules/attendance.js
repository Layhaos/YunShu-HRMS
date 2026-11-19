import Layout from "@/layout/index.vue"; // 系统壳：静态 import（父路由固定，不懒加载——每页都要用它）

// M03 考勤管理路由——照 department.js 的模子
// name 必须等于权限编码（后端 T1018_PermissionInfos 种子已灌 code=attendance，id=9）
// 进了 asyncRoutes + meta 有 title，侧边菜单就自动长出"考勤管理"
export default {
    path: "/",
    name: "attendance",   // ← filterRoutes 按这个名字匹配菜单权限编码
    component: Layout,
    children: [
        {
            path: "attendance",
            component: () => import("@/views/attendance/index.vue"),
            meta: { title: "考勤管理", icon: "Clock" },   // icon 是全局注册过的 Element 图标名
        },
    ],
}
