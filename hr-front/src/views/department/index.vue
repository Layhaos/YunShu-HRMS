<template>
    <div class="page-container">
        <!-- 页头：标题 + 一句话说明 -->
        <div class="page-header">
            <h2 class="page-title">部门管理</h2>
            <p class="page-desc">维护公司组织架构树，支持新增子部门与删除部门（软删除）</p>
        </div>
        <div class="page-card">
            <!-- el-tree：树形控件
                 :data      喂树形数据（不是平铺的！是转换完的）
                 :props     告诉 el-tree 去哪个属性拿"显示文字"和"下一层"
                 default-expand-all  默认全展开（数据少时看得清楚） -->
            <el-tree
                :data="treeData"
                :props="defaultProps"
                default-expand-all
            >
                <!-- 作用域插槽：el-tree 把每个节点的数据塞进 data 交给我们
                     每个节点渲染一个 TreeTools（组件复用，各自领自己的数据）
                     is-root：顶级（parentId===0）不给删 -->
                <template #default="{ data }">
                    <TreeTools
                        :tree-node="data"
                        :is-root="data.parentId === 0"
                        @add-depart-event="onAddDepart"
                        @delete-depart-event="getDepartmentList"
                    />
                </template>
            </el-tree>
        </div>

        <!-- 新增弹窗：显隐用 addShow 布尔控；departNode 记"新部门挂谁下面" -->
        <AddDepart
            :show-dialog="addShow"
            :depart-node="departNode"
            @handle-close-event="addShow = false"
            @load-departs-event="getDepartmentList"
        />
    </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { getDepartments } from "@/api/department";
import TreeTools from "./components/TreeTools.vue";
import AddDepart from "./components/AddDepart.vue";

//script setup 规矩：模板里直接用顶层变量——所以数据源用 ref 裸定义，别包进 state 对象
const treeData = ref([]);     //el-tree 只认树形，吃这个
const defaultProps = {        //el-tree 的"说明书"：名称去哪拿、孩子去哪拿
    label: "departmentName",  //节点上显示的文字 = 这条记录的部门名
    children: "children",     //往下展开 = 找这条记录的 children 数组
};

//弹窗接线：记住点的是哪个节点 + 打开弹窗
const addShow = ref(false);
const departNode = ref({});
function onAddDepart(node) {
    departNode.value = node;
    addShow.value = true;
}

//★★ 核心手写点：平铺列表 → 树形结构（递归）★★
//输入：后端平铺数组，每条 { id, departmentName, parentId }（parentId=0 是顶级）
//输出：树形数组，顶级部门在里面，每个部门身后跟着 children
//思路：要"parentId 为 X 的所有孩子"——先筛出这一层，再让每个孩子去喊自己的孩子
function toTree(list, parentId = 0) {
    //第一件事：从平铺列表里筛出"爹是 parentId"的记录 → 这一层级的所有节点
    return list
        .filter((item) => item.parentId === parentId)
        .map((item) => ({
            ...item,   //自己这条记录原样保留（id/名称/编码都带过去）
            //递归点：我的孩子是谁？→ 拿"我的 id"当爹再筛一遍
            //递归自然收敛：越往深处筛出来的越少，筛空了返回 []，就到头了
            children: toTree(list, item.id),
        }));
}

//拉数据：接口 → 平铺 → 转树 → 喂 el-tree
//从 onMounted 抽成独立函数——弹窗新增/删除后也要调它刷新树，别复制两份
async function getDepartmentList() {
    //拦截器已拆包：resolve 出来的直接是 data 平铺列表
    const list = await getDepartments();
    treeData.value = toTree(list);   //平铺进，树形出（ref 要 .value 赋值）
}

onMounted(() => {
    getDepartmentList();
});
</script>

<style scoped>
/* 树节点行：拉高到 40px、圆角、悬停浅灰——el-tree 默认 26px 太挤 */
.page-card :deep(.el-tree-node__content) {
    height: 40px;
    border-radius: 6px;
    transition: background-color 0.2s;
}
.page-card :deep(.el-tree-node__content:hover) {
    background: #f5f7fa;
}
</style>
