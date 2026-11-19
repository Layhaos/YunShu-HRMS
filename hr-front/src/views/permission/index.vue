<template>
    <div class="page-container">
        <!-- 页头：标题 + 一句话说明 -->
        <div class="page-header">
            <h2 class="page-title">权限管理</h2>
            <p class="page-desc">菜单 / 按钮 / API 三类权限统一管理，新增时按类型动态切换字段</p>
        </div>
        <div class="page-card">
            <!-- 工具行：新增按钮 -->
            <div class="toolbar">
                <el-button type="primary" @click="openAdd">新增权限</el-button>
            </div>

            <!-- 列表：平铺展示（权限表没有树形需求，ParentId 只用来溯源） -->
            <el-table :data="permissionList" border stripe>
                <el-table-column prop="id" label="编号" width="70" />
                <el-table-column prop="permissionName" label="权限名称" min-width="120" />
                <el-table-column prop="permissionCode" label="权限编码" min-width="150" />
                <!-- 类型列：数字 → 中文 + 颜色 tag -->
                <el-table-column label="类型" width="90">
                    <template #default="{ row }">
                        <el-tag :type="typeMap[row.permissionType].tag" effect="plain">
                            {{ typeMap[row.permissionType].text }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="permissionDescription" label="描述" min-width="140" show-overflow-tooltip />
                <el-table-column prop="parentId" label="父权限" width="80" />
            </el-table>
        </div>

        <!-- 新增弹窗：v-model 绑 addShow 布尔；按类型动态切字段 -->
        <el-dialog v-model="addShow" title="新增权限" width="480px" @close="handleClose">
            <el-form ref="formEl" :model="permissionForm" :rules="rules" label-width="90px">
                <el-form-item label="权限类型" prop="permissionType">
                    <el-radio-group v-model="permissionForm.permissionType">
                        <el-radio :value="1">菜单</el-radio>
                        <el-radio :value="2">按钮</el-radio>
                        <el-radio :value="3">API</el-radio>
                    </el-radio-group>
                </el-form-item>
                <el-form-item label="权限名称" prop="permissionName">
                    <el-input v-model="permissionForm.permissionName" placeholder="如：部门管理 / 删除部门 / 查询部门接口" />
                </el-form-item>
                <el-form-item label="权限编码" prop="permissionCode">
                    <el-input v-model="permissionForm.permissionCode" placeholder="如：department / department:del / api:departments:get" />
                </el-form-item>
                <el-form-item label="父权限" prop="parentId">
                    <el-input v-model.number="permissionForm.parentId" placeholder="菜单填 0，按钮/API 填所属菜单权限的编号" />
                </el-form-item>
                <el-form-item label="描述" prop="permissionDescription">
                    <el-input v-model="permissionForm.permissionDescription" type="textarea" :rows="2" />
                </el-form-item>

                <!-- 动态区①：菜单权限的个性字段（type=1 才显示） -->
                <template v-if="permissionForm.permissionType === 1">
                    <el-divider content-position="left">菜单属性</el-divider>
                    <el-form-item label="菜单图标" prop="menuIcon">
                        <el-input v-model="permissionForm.menuIcon" placeholder="element-plus 图标名，如 Key / Grid" />
                    </el-form-item>
                    <el-form-item label="菜单排序" prop="menunOrder">
                        <el-input v-model="permissionForm.menunOrder" placeholder="如 3" />
                    </el-form-item>
                </template>

                <!-- 动态区②：按钮权限的个性字段（type=2 才显示） -->
                <template v-if="permissionForm.permissionType === 2">
                    <el-divider content-position="left">按钮属性</el-divider>
                    <el-form-item label="按钮样式" prop="pointClass">
                        <el-input v-model="permissionForm.pointClass" placeholder="class 名" />
                    </el-form-item>
                    <el-form-item label="按钮图标" prop="pointIcon">
                        <el-input v-model="permissionForm.pointIcon" placeholder="图标名" />
                    </el-form-item>
                    <el-form-item label="按钮状态" prop="pointStatus">
                        <el-input v-model.number="permissionForm.pointStatus" placeholder="1 可用" />
                    </el-form-item>
                </template>

                <!-- 动态区③：API 权限的个性字段（type=3 才显示） -->
                <template v-if="permissionForm.permissionType === 3">
                    <el-divider content-position="left">API 属性</el-divider>
                    <el-form-item label="API 地址" prop="apiUrl">
                        <el-input v-model="permissionForm.apiUrl" placeholder="如 /api/Departments" />
                    </el-form-item>
                    <el-form-item label="请求方式" prop="apiMethod">
                        <el-select v-model="permissionForm.apiMethod" placeholder="选请求方式" style="width: 100%">
                            <el-option label="GET" value="GET" />
                            <el-option label="POST" value="POST" />
                            <el-option label="DELETE" value="DELETE" />
                            <el-option label="PUT" value="PUT" />
                        </el-select>
                    </el-form-item>
                </template>
            </el-form>
            <template #footer>
                <el-button @click="handleClose">取消</el-button>
                <el-button type="primary" @click="onSubmitAdd">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { ElMessage } from "element-plus";
import { getPermissions, addPermission } from "@/api/permission";

//script setup 规矩：模板里直接用顶层变量——数据源用 ref 裸定义
const permissionList = ref([]);

//类型映射：列表 tag 用
const typeMap = {
    1: { text: "菜单", tag: "primary" },
    2: { text: "按钮", tag: "success" },
    3: { text: "API", tag: "warning" },
};

//弹窗显隐：一个布尔管到底（弹窗是本页内联的，不需要子组件 props 那套）
const addShow = ref(false);
const formEl = ref(null);

//表单数据源：主表字段 + 三类子表个性字段全在一个对象里（和后端 PermissionDto 一一对应）
const permissionForm = ref({});
const initForm = () => ({
    permissionType: 1,
    permissionName: "",
    permissionCode: "",
    permissionDescription: "",
    parentId: 0,
    //子表个性字段（按类型用得上哪几个）
    menuIcon: "",
    menunOrder: "",
    pointClass: "",
    pointIcon: "",
    pointStatus: 1,
    apiUrl: "",
    apiMethod: "GET",
});

//校验：必填三件套（编码是权限系统的身份证，必填不商量）
const rules = {
    permissionName: [{ required: true, message: "请输入权限名称", trigger: "blur" }],
    permissionCode: [{ required: true, message: "请输入权限编码", trigger: "blur" }],
    permissionType: [{ required: true, message: "请选择权限类型", trigger: "change" }],
};

//拉数据：接口 → 平铺列表直接喂表格（权限是平铺的，不用转树）
//抽成独立函数——新增成功后也要调它刷新列表
async function getPermissionList() {
    //拦截器已拆包：resolve 出来的直接是 data 平铺列表
    const list = await getPermissions();
    permissionList.value = list;
}

//打开弹窗（工具行按钮）：重置一份干净表单再开
function openAdd() {
    permissionForm.value = initForm();
    addShow.value = true;
}

//关闭弹窗：resetFields 清输入+清红字
function handleClose() {
    formEl.value?.resetFields();
    addShow.value = false;
}

//★★ 提交逻辑（本轮代工，用户过目）：validate 二道门 → 发请求 → 成功三连 ★★
//照 ⑬ 的规矩：validate 只走 Promise 一个渠道（不传 callback），.then 里干正事，
//.catch 接"校验没过"——红字 element 自己弹在表单上，我们不用管
//⚠️ 函数名不能叫 addPermission——和 api 导入的 addPermission 撞名，
//   函数声明会遮蔽 import，.then 里调用就变成无限递归调自己（webpack 已报重复声明）
function onSubmitAdd() {
    formEl.value
        .validate()
        .then(async () => {
            //把整个表单对象发给后端——后端 AddPermission 按类型分拣落两张表
            await addPermission(permissionForm.value);
            ElMessage.success("新增成功");
            handleClose();
            getPermissionList();
        })
        .catch(() => {
            //校验没过：什么都不做（红字已经在表单上了）
        });
}

onMounted(() => {
    getPermissionList();
});
</script>

<style scoped>
/* 工具行：按钮和表格之间留呼吸感 */
.toolbar {
    margin-bottom: 12px;
    display: flex;
    justify-content: flex-end;
}
</style>
