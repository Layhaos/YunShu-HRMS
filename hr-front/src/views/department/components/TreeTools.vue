<template>
    <!-- flex:1 让本行占满节点剩余宽度（父级 .el-tree-node__content 是 flex），
         悬停高亮才有"整行"效果；布局用普通 div+flex，不再套 el-row/el-col -->
    <div class="tree-row">
        <div class="tree-row__left">
            <span class="tree-row__name">{{ treeNode.departmentName }}</span>
            <el-tag
                v-if="treeNode.departmentCode"
                size="small"
                type="info"
                effect="plain"
            >{{ treeNode.departmentCode }}</el-tag>
        </div>
        <el-dropdown @command="handleCommand">
            <span class="el-dropdown-link">
                操作
                <el-icon class="el-icon--right"><ArrowDown /></el-icon>
            </span>
            <template #dropdown>
                <el-dropdown-menu>
                    <el-dropdown-item command="add">添加子部门</el-dropdown-item>
                    <!-- ★ ⑭ 阶段D 顺手小活【你写一行】：给本项加权限控制——
                         v-if 变双条件：!isRoot && checkPointPermission('department:del')
                         （没权限的按钮直接不渲染，而不是渲染了再禁用） -->
                    <el-dropdown-item v-if="!isRoot && checkPointPermission('department:del')" command="del">删除部门</el-dropdown-item>
                </el-dropdown-menu>
            </template>
        </el-dropdown>
    </div>
</template>

<script setup>
import { ElMessageBox, ElMessage } from "element-plus";
import { deleteDepartment } from "@/api/department";
// ★ ⑭ 阶段C 顺手小活：功能权限校验工具（骨架已备好，检查你在 utils/checkPermission.js 的实现）
// script setup 的规矩：顶层 import 的函数，模板里直接就能用（v-if 里随便调）
import { checkPointPermission } from "@/utils/checkPermission";

//props：treeNode = el-tree 当前节点的数据对象。
//el-tree 循环渲染时，每个节点各实例化一个 TreeTools，各自通过 props 领自己的数据——这就是抽组件的理由。
//isRoot = 顶级部门不给删（研发部没有"删除部门"这一项）。
const props = defineProps({
    treeNode: { type: Object, required: true },
    isRoot: { type: Boolean, default: false },
});

//对外广播：add 带着当前节点出去（父组件拿它的 id 当新部门的 parentId）；del 完成后喊一声"刷新树"
const emit = defineEmits(["addDepartEvent", "deleteDepartEvent"]);

//下拉菜单统一入口：command 值分发（el-dropdown 的规矩：菜单项配 command，事件里拿值分流）
const handleCommand = async (type) => {
    if (type === "add") {
        emit("addDepartEvent", props.treeNode);
    } else {
        //删除走二次确认：confirm() 的 then 才是真删，catch 是点了取消
        ElMessageBox.confirm(
            `确定要删除 ${props.treeNode.departmentName} 部门吗？`,
            "删除部门",
            { confirmButtonText: "确定", cancelButtonText: "取消", type: "warning" }
        )
            .then(async () => {
                await deleteDepartment(props.treeNode.id);   //软删除，后端补 IsDeleted=1
                ElMessage.success("删除成功");
                emit("deleteDepartEvent");                   //让父组件重新拉树
            })
            .catch(() => {
                ElMessage.info("取消删除");
            });
    }
};
</script>

<style scoped>
.tree-row {
    flex: 1;                                   /* 占满节点剩余宽度 */
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding-right: 8px;
}
.tree-row__left {
    display: flex;
    align-items: center;
    gap: 8px;
    min-width: 0;                              /* 名称超长时允许省略号 */
}
.tree-row__name {
    font-size: 14px;
    color: var(--text-main);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}
.el-dropdown-link {
    display: inline-flex;                      /* 图标和"操作"垂直居中对齐 */
    align-items: center;
    font-size: 13px;
    color: var(--primary);
    cursor: pointer;
    transition: opacity 0.2s;
}
.el-dropdown-link:hover {
    opacity: 0.75;
}
</style>
