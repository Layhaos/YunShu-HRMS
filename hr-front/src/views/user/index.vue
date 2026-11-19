<template>
    <div class="page-container">
        <div class="page-header">
            <h2 class="page-title">员工管理</h2>
            <p class="page-desc">给员工分配角色（RBAC 第三环：用户 ← 角色 ← 权限）</p>
        </div>
        <div class="page-card">
            <el-table :data="userList" border stripe>
                <el-table-column prop="id" label="编号" width="70" />
                <el-table-column prop="userName" label="姓名" min-width="100" />
                <el-table-column prop="userPhone" label="手机号" min-width="130" />
                <el-table-column prop="userEmail" label="邮箱" min-width="150" show-overflow-tooltip />
                <el-table-column label="性别" width="80">
                    <template #default="{ row }">
                        <span>{{ row.gender === 1 ? "男" : "女" }}</span>
                    </template>
                </el-table-column>
                <el-table-column prop="createDate" label="入职时间" width="130" />
                <el-table-column label="操作" width="120">
                    <template #default="{ row }">
                        <el-button type="primary" link @click="openAssign(row)">分配角色</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>

        <!-- 分配角色弹窗：多选下拉（角色数量少，select 比 tree 合适） -->
        <el-dialog v-model="assignShow" :title="`分配角色 - ${currentUser.userName || ''}`" width="420px">
            <!-- multiple 多选；v-model 收的是选中项 value（角色 id）数组 -->
            <el-select v-model="selectedRoleIds" multiple placeholder="选择角色" style="width: 100%">
                <el-option v-for="r in roleList" :key="r.id" :label="r.roleName" :value="r.id" />
            </el-select>
            <template #footer>
                <el-button @click="assignShow = false">取消</el-button>
                <el-button type="primary" @click="onSubmitAssign">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { ElMessage } from "element-plus";
import { getUserList, getUserRoleIds, assignRoles } from "@/api/user";
import { getRoles } from "@/api/role";

//数据源：ref 裸定义
const userList = ref([]);
const roleList = ref([]);
const assignShow = ref(false);
const currentUser = ref({});
const selectedRoleIds = ref([]);   //多选框收的 id 数组

//拉员工列表
async function fetchUsers() {
    const list = await getUserList();
    userList.value = list;
}

//★ 打开弹窗：两发并行——角色选项 + 该用户已有 roleIds（回显）
async function openAssign(row) {
    currentUser.value = row;
    const [roles, ids] = await Promise.all([getRoles(), getUserRoleIds(row.id)]);
    roleList.value = roles;
    selectedRoleIds.value = ids;   //回显：多选框直接喂 id 数组
    assignShow.value = true;
}

//★ 提交：id 数组 join(',') 逗号串；空选发 "0" 保底（串值契约，1112 弹药）
async function onSubmitAssign() {
    const ids = selectedRoleIds.value.length ? selectedRoleIds.value.join(",") : "0";
    await assignRoles(currentUser.value.id, ids);
    ElMessage.success("分配成功");
    assignShow.value = false;
}

onMounted(() => {
    fetchUsers();
});
</script>
