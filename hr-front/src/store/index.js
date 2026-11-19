import { createStore } from "vuex";
import user from './modules/user';
import permission from './modules/permission';   // ⑭ 阶段C：权限路由模块
export default createStore({
    modules:{
        user,
        permission
    }
});