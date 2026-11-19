<template>
  <el-container class="app-wrapper">
    <!-- 左：侧边栏（菜单从路由表生成） -->
    <el-aside width="200px">
      <el-menu
        router
        class="app-menu"
        :default-active="$route.path"
        background-color="#304156"
        text-color="#bfcbd9"
        active-text-color="#409EFF"
      >
        <el-menu-item v-for="item in menuRoutes" :key="item.path" :index="item.path">
          <el-icon><component :is="item.icon" /></el-icon>
          <span>{{ item.title }}</span>
        </el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <!-- 上：顶栏（标题 + 用户名 + 头像 + 退出占位） -->
      <el-header class="app-header">
        <span class="app-title">人力资源管理平台</span>
        <div class="app-user">
          <span class="app-userName">{{ userName }}</span>
          <el-avatar :size="32" icon="UserFilled" />
          <!-- ⑨ 退出登录：下一节接线，先占位 -->
          <el-button link type="danger" @click="handleLogout">退出</el-button>
        </div>
      </el-header>

      <!-- 中：内容区——children 子页面全画在这里；留白由子页 .page-container 负责 -->
      <el-main class="app-main">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
// 组件统一 <script setup>；仓库不适用此糖（永远 export default）——两个世界别混
import { computed } from "vue";
import { useRouter } from "vue-router";
import store from "@/store";
import { resetRouter } from "@/router";
import { ElMessageBox, ElMessage } from 'element-plus';

const router = useRouter();

// 菜单生成器：⑭ 阶段C 起读 Vuex permission 模块的 routes，不再读 router.options.routes——
// 原因：守卫动态 router.addRoute() 挂进去的路由不具响应式，router 的表变了菜单不会刷新；
// 而 Vuex 的 state.routes 由 setRoutes 维护（静态+动态合并），菜单跟着它长
// 注意：每个路由模块自带一个 path:"/" 的壳，所以要 filter 捞出所有壳再合并 children
// （find 只返回第一个匹配——那样永远只显示第一个模块的菜单）
const menuRoutes = computed(() => {
  const shells = store.state.permission.routes.filter((r) => r.path === "/");
  return shells
    .flatMap((shell) => shell.children || [])          //所有壳的子页摊平合并
    .filter((c) => !c.hidden && c.meta && c.meta.title)
    .map((c) => ({ path: "/" + c.path, title: c.meta.title, icon: c.meta.icon }));
});

// 用户名：回填的"人证"——守卫拉回资料后这里自动长出名字
const userName = computed(() => store.state.user.userInfo.userName || "");

// 退出：⑨ 实现
const handleLogout = () => {
  // 1. ElMessageBox.confirm(提示语, 标题, 配置) → 返回 Promise
  ElMessageBox.confirm(
    '你确定要退出登录吗？', 
    '温馨提示', 
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning',
    }
  ).then(() => {
    // 2. 确定(resolve) → commit removeToken → router.push("/login")
    
    // 清除 Vuex 里的 token 和 userInfo（同时也会清除 localStorage 里的双保险）
    store.commit("user/removeToken");
    //⑭ 尾巴任务4：登出防越权——清两份"菜单/路由拷贝"，缺一不可
    //① resetRouter：拆掉守卫 addRoute 挂上的真路由（地址栏输 /department 跳不进）
    resetRouter();
    //② 清 Vuex 菜单基线：退回只剩首页（跨模块 commit 必带 { root: true }，1113 弹药）
    store.commit("permission/setRoutes", [], { root: true });
    
    // 提示一下
    ElMessage.success("已安全退出");
    
    // 跳回登录页
    router.push("/login");

  }).catch(() => {
    // 用户点了取消，什么也不干
  });
};
</script>

<style scoped>
.app-wrapper {
  height: 100vh;
}
.el-aside {
  background: var(--sidebar-bg);
  box-shadow: 2px 0 8px rgba(0, 21, 41, 0.15);
  z-index: 1;   /* 压住内容区的阴影，不让它盖到侧边栏上 */
}
.app-menu {
  height: 100%;
  border-right: none;
}
/* :deep() 穿透：el-menu-item 是子组件内部元素，scoped 默认够不着 */
.app-menu :deep(.el-menu-item) {
  margin: 4px 10px;              /* 菜单项四周留缝 → 高亮块变圆角小条 */
  border-radius: 6px;
  transition: background-color 0.2s;
}
.app-menu :deep(.el-menu-item:hover) {
  background: rgba(255, 255, 255, 0.08);   /* 悬停微亮 */
}
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #fff;
  box-shadow: var(--card-shadow);   /* 边框换浅阴影，和卡片质感统一 */
  z-index: 1;
}
.app-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-main);
}
.app-user {
  display: flex;
  align-items: center;
  gap: 12px;
}
.app-userName {
  color: var(--text-regular);
}
/* 内容区：浅灰底 + 去内边距（留白交给页面的 .page-container 管） */
.app-main {
  background: var(--page-bg);
  padding: 0;
}
</style>
