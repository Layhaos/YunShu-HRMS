<template>
    <el-dialog title="新增部门" width="480px" v-model="dialogVisible" :close-on-click-modal="false" @close="handleClose">
        <el-form label-width="100px" :model="formData" :rules="formRules" ref="formRef">
            <el-form-item label="部门名称" prop="departmentName">
                <el-input style="width: 80%" placeholder="1-50个字符" v-model="formData.departmentName" />
            </el-form-item>
            <el-form-item label="部门编码" prop="departmentCode">
                <el-input style="width: 80%" placeholder="1-50个字符" v-model="formData.departmentCode" />
            </el-form-item>
            <el-form-item label="部门介绍" prop="departmentDescr">
                <el-input style="width: 80%" type="textarea" :rows="3" v-model="formData.departmentDescr" />
            </el-form-item>
        </el-form>
        <template #footer>
            <el-button type="primary" @click="createDepart">确定</el-button>
            <el-button @click="handleClose">取消</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { ref, watch, reactive } from "vue";
import { getDepartments, addDepartment } from "@/api/department";

const props = defineProps({
    showDialog: { type: Boolean, default: false },      //父组件用布尔控我显隐
    departNode: { type: Object, default: () => ({}) },  //点的是哪个节点（新部门挂它下面）
});
const emit = defineEmits(["handleCloseEvent", "loadDepartsEvent"]);

//watch 控显隐：父组件只管改 showDialog 布尔，我这边跟着搬进 el-dialog 的 v-model
const dialogVisible = ref(false);
watch(() => props.showDialog, (val) => { dialogVisible.value = val; });

const formRef = ref(null);   //el-form 实例：validate/resetFields 都靠它
const formData = reactive({
    departmentName: "",
    departmentCode: "",
    departmentDescr: "",
    parentId: 0,   //提交时由 departNode.id 覆盖
});

//★★★ 你写区 ①：同级部门名查重（参考老师 checkNameRpeater，他拼错了 Rpeater，你写 Repeater）★★★
//思路三步：
//  1. getDepartments() 拿全量（拦截器已拆包，resolve 直接是平铺列表）
//  2. filter( item.parentId === props.departNode.id ) —— 只跟"和你要挂的位置同级"的比
//  3. some( item.departmentName === value ) —— 命中 = 重名
//  命中：callback(new Error(`已存在${value}部门了`))；没命中：callback() 放行
//  注意：这里不能用 async！async 函数永远返回 Promise，async-validator 会按 Promise
//  分支接管流程，和你手动调的 callback 打架（红字不出、提交不发）。用 .then 拿数据。
const checkNameRepeater = (rule, value, callback) => {
    getDepartments().then((result)=>{
        result
        .filter((item) => item.parentId === props.departNode.id)
        .some((item) => item.departmentName === value)
        ?callback(new Error(`已存在${value}部门！`))
        :callback();
    });
    

};

//★★★ 你写区 ②：部门编码全局查重 ★★★
//思路两步：
//  1. getDepartments() 拿全量
//  2. some( item.departmentCode === value ) —— 编码全公司唯一，不筛同级、不看爹
const checkCodeRepeater = (rule, value, callback) => {
    getDepartments().then((result)=>{
        result.some(item=>item.departmentCode === value)
        ?callback(new Error(`已存在${value}部门编码！`))
        :callback(); 
    });
   
};

//校验规则：同一字段挂多条，从上往下过——必填/长度过了才轮到查重
const formRules = reactive({
    departmentName: [
        { required: true, message: "部门名称不能为空", trigger: "blur" },
        { min: 1, max: 50, message: "部门名称要求1-50个字符", trigger: "blur" },
        { validator: checkNameRepeater, trigger: "blur" },
    ],
    departmentCode: [
        { required: true, message: "部门编码不能为空", trigger: "blur" },
        { min: 1, max: 50, message: "部门编码要求1-50个字符", trigger: "blur" },
        { validator: checkCodeRepeater, trigger: "blur" },
    ],
    departmentDescr: [
        { required: true, message: "部门介绍不能为空", trigger: "blur" },
    ],
});

//★★★ 你写区 ③：提交（约10行）★★★
//思路：formRef.value.validate(async (valid) => { if (valid) { ... } })
//  里面三件事：
//  1. await addDepartment({ ...formData, parentId: props.departNode.id })
//     —— 注意展开顺序！formData 里也有 parentId(默认0)，必须让它排前面、
//        显式的 departNode.id 排后面"压轴"——JS 规矩：同名键后面的说了算。
//        写反了就全进成顶级部门（实测踩过的坑）
//  2. handleClose() —— 关弹窗 + 重置表单
//  3. emit("loadDepartsEvent") —— 让父组件重新拉树，新部门立刻长出来
const createDepart = () => {
    formRef.value.validate(async(valid)=>{
        if(!valid) return;
                await addDepartment({ ...formData, parentId: props.departNode.id });
                handleClose();
                emit("loadDepartsEvent");
    });
};

//关弹窗：resetFields 重置表单（清输入+清校验红字），再通知父组件把 showDialog 拨回 false
const handleClose = () => {
    formRef.value?.resetFields();
    emit("handleCloseEvent");
};
</script>
