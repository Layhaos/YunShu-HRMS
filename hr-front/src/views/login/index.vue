<template>
  <div class="login-container">
    <el-form class="login-card" :model="loginForm" :rules="rules" ref="loginFormRef">
      <h2>人力资源管理平台</h2>
      <!-- prop 要和 rules 键名、字段名完全一致，大小写也算 -->
      <el-form-item prop="userPhone">
        <el-input v-model="loginForm.userPhone" placeholder="请输入手机号" @keyup.enter="handleLogin" />
      </el-form-item>
      <el-form-item prop="userPassword">
        <el-input type="password" v-model="loginForm.userPassword" placeholder="请输入密码" show-password @keyup.enter="handleLogin" />
      </el-form-item>
      <el-button type="primary" style="width:100%" @click="handleLogin">登 录</el-button>
    </el-form>
  </div>
</template>

<script setup>
// 组件统一 <script setup>：顶层 const 自动暴露给模板（语法糖 = setup(){ return {...} }）
// 仓库（store/modules/user.js）不适用这个糖，它永远 export default 大对象——两个世界别混
import { reactive, ref } from "vue"
import { useRouter } from "vue-router"
import { ElMessage } from "element-plus"
import store from "@/store"
import { checkPhone } from "@/utils/check"

const router = useRouter()   // 组件里拿路由实例的标准姿势；只有守卫那种"普通 js"才被迫直接 import 实例

const loginForm = reactive({ userPhone: "", userPassword: "" })
const loginFormRef = ref(null)   // 变量名与模板 ref="loginFormRef" 对上，El 自动接线
const rules = {
  userPhone: [
    { required: true, message: "手机号不能为空", trigger: "blur" },
    { validator: checkPhone, trigger: "blur" },
  ],
  userPassword: [
    { required: true, message: "密码不能为空", trigger: "blur" },
  ],
}

// 一道门 validate：不过直接罢工；过了走二道门（后端真校验）
const handleLogin = () => {
  loginFormRef.value.validate(async (valid) => {
    if (!valid) return
    try {
      await store.dispatch("user/login", loginForm)   // 成功后 token/手机号已在 action 里双写入库
      ElMessage.success("登录成功！")
      router.push("/")   // 指向当前路由里挂 home 的那条规则
    } catch (e) {
      ElMessage.error(e)   // 失败提示来自响应拦截器 reject 出来的 message
    }
  })
}
</script>

<style scoped>
.login-container {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
  background: #f0f2f5;
}
.login-card {
  width: 360px;
  padding: 32px 40px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.12);
}
.login-card h2 {
  text-align: center;
  margin-bottom: 24px;
  font-size: 20px;
  color: #303133;
}
</style>
