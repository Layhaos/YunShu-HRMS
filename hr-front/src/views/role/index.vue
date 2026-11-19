<template>
    <div class="page-container">
        <!-- 页头 -->
        <div class="page-header">
            <h2 class="page-title">角色管理</h2>
            <p class="page-desc">权限的"批发商"：权限发给角色，用户再领角色（RBAC 中间层）</p>
        </div>
        <div class="page-card">
            <el-table :data="roleList" border stripe>
                <el-table-column prop="id" label="编号" width="70" />
                <el-table-column prop="roleName" label="角色名" min-width="120" />
                <el-table-column prop="remark" label="备注" min-width="160" show-overflow-tooltip />
                <el-table-column label="操作" width="120">
                    <template #default="{ row }">
                        <el-button type="primary" link @click="openAssign(row)">授权</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </div>

        <!-- 授权弹窗：el-tree 勾权限 -->
        <el-dialog v-model="assignShow" :title="`角色授权 - ${currentRole.roleName}`" width="420px">
            <!-- :props 说明书：label 用权限名、children 是子级；node-key 是 diff 和勾选的身份 -->
            <el-tree
                ref="treeEl"
                :data="permissionTree"
                :props="{ label: 'permissionName', children: 'children' }"
                node-key="id"
                show-checkbox
                default-expand-all
            />
            <template #footer>
                <el-button @click="assignShow = false">取消</el-button>
                <el-button type="primary" @click="onSubmitAssign">确定</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from "vue";
import { ElMessage } from "element-plus";
import { getRoles, getRolePermissionIds, assignPermissions } from "@/api/role";
import { getPermissions } from "@/api/permission";

//数据源：script setup 规矩，ref 裸定义
const roleList = ref([]);
const permissionTree = ref([]);   //权限树（喂 el-tree）
const treeEl = ref(null);        //模板 ref：拿 el-tree 实例调 setCheckedKeys/getCheckedKeys
const assignShow = ref(false);
const currentRole = ref({});

//★ toTree 递归转树（⑬ 的看家函数，照 department 页复刻）
//filter 筛出该层 → map 展开 + 递归喊孩子 → 筛空返回 [] 自然收敛
function toTree(list, parentId) {
    return list
        .filter((item) => item.parentId === parentId)
        .map((item) => ({
            ...item,
            children: toTree(list, item.id),
        }));
}

//拉角色列表
async function getRoleList() {
    const list = await getRoles();
    roleList.value = list;
}

//★ 打开授权弹窗：两发并行——权限树 + 该角色已勾的 id，回来 setCheckedKeys 回显
async function openAssign(row) {
    currentRole.value = row;
    //权限平铺列表 → 转树（按钮/API 挂在所属菜单下）
    const flat = await getPermissions();
    permissionTree.value = toTree(flat, 0);
    assignShow.value = true;
    //回显：查该角色现有权限 id → setCheckedKeys
    //（注意要在弹窗内容渲染后再设——用 nextTick 等 DOM 挂上）
    const ids = await getRolePermissionIds(row.id);
    await nextTick();
    treeEl.value?.setCheckedKeys(ids);
}

//★ 提交：getCheckedKeys(false) → join(',') 逗号串 → 后端删旧插新
//false = 不带半选的父节点（比如只勾了某个按钮，它的菜单父节点是半选，不算勾上）
async function onSubmitAssign() {
    const checked = treeEl.value.getCheckedKeys(false);
    //串值契约：空选发 "0" 保底（空串让后端按逗号切分解析时炸不了——"0" 不匹配任何权限 id，等价清空）
    const ids = checked.length ? checked.join(",") : "0";
    try {
        await assignPermissions(currentRole.value.id, ids);
        ElMessage.success("授权成功");
        assignShow.value = false;
    } catch (msg) {
        //拦截器 reject 出来的就是提示语（如防自锁校验的拦截信息）
        ElMessage.error(msg);
    }
}

onMounted(() => {
    getRoleList();
});
</script>
