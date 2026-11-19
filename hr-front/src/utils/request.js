import axios from "axios"
import store from "@/store"
import router from "@/router";
import { ElMessage } from "element-plus";

const service = axios.create({
    baseURL: process.env.VUE_APP_BASE_API,
    timeout: 5000,
});

//请求拦截器
service.interceptors.request.use(
    (config) => {
        //去VUEX里拿token
        const token = store.state.user.token;
        //const token = "";
        //const token = localStorage.getItem('hr_token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
            //有token加到请求头
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

//响应拦截器
service.interceptors.response.use(
    (response) => {
        //拿到后端返回的ApiResult结构
        const { isSuccess, message, data } = response.data;
        if (isSuccess) {
            return data;
            //data返回页面，这里使用了解构赋值，就不用再到页面里面去拆了
        } else {
            return Promise.reject(message);
        }
    },
    (error) => {//token失效，清掉登录信息、调回登录页
        if (error.response//无响应
            && error.response.status === 401//未授权
            && error.response.headers["act"] === "expired") //过期
        {
            //localStorage.removeItem("hr_token");
            //localStorage.removeItem("hr_phone");

            store.commit("user/removeToken");//公共仓库执行RemoveToken
            ElMessage.error("登录失效，请重新登陆");

            //window.location.href = "/login";//整页刷新
            router.push("/login");//页面内切登录页，不用重载

            return Promise.reject(error);//外抛异常，让catch捕获
        }
        else
        {
            return Promise.reject(error.message);
        }
    }
);

export default service;