import { createApp } from 'vue'
import App from './App.vue'
import "@/styles/index.scss"   //全局设计变量 + 页面骨架类（样式封装，全站复用）
import store from './store/index.js'
import router from './router'
import "@/utils/checkAuthentication"
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import globalComponents from './components/index.js'

const app = createApp(App)
// Element 图标全局注册：模板里 <component :is="图标名"/> 才找得到
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}
app.use(store)
  .use(router)
  .use(globalComponents)
  .mount('#app')
