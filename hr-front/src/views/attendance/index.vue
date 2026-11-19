<template>
    <el-card>
        <el-tabs v-model="activeTab">
            <!-- ====== Tab1 我的考勤 ====== -->
            <el-tab-pane label="我的考勤" name="record">
                <div style="margin-bottom: 12px;">
                    <el-button type="primary" @click="onClockIn">打上班卡</el-button>
                    <el-button type="success" @click="onClockOut">打下班卡</el-button>
                </div>
                <el-table :data="myList" border stripe v-loading="loading">
                    <el-table-column label="日期" :formatter="fmtDate" prop="workDate" width="140" />
                    <el-table-column label="上班打卡" :formatter="fmtTime" prop="clockIn" width="160" />
                    <el-table-column label="下班打卡" :formatter="fmtTime" prop="clockOut" width="160" />
                    <el-table-column label="状态" width="100">
                        <template #default="{ row }">
                            <el-tag :type="attStatus[row.status]?.type">{{ attStatus[row.status]?.text }}</el-tag>
                        </template>
                    </el-table-column>
                </el-table>
            </el-tab-pane>

            <!-- ====== Tab2 我的申请 ====== -->
            <el-tab-pane label="我的申请" name="mine">
                <el-button type="primary" style="margin-bottom: 12px;" @click="openApplyDialog">提交申请</el-button>
                <el-table :data="myApplies" border stripe v-loading="loading">
                    <el-table-column label="类型" width="90">
                        <template #default="{ row }">{{ applyTypeMap[row.applyType] }}</template>
                    </el-table-column>
                    <el-table-column label="开始时间" :formatter="fmtDateTime" prop="startTime" width="170" />
                    <el-table-column label="结束时间" :formatter="fmtDateTime" prop="endTime" width="170" />
                    <el-table-column label="事由" prop="reason" />
                    <el-table-column label="状态" width="100">
                        <template #default="{ row }">
                            <el-tag :type="applyStatusMap[row.status]?.type">{{ applyStatusMap[row.status]?.text }}</el-tag>
                        </template>
                    </el-table-column>
                    <el-table-column label="审批意见" prop="approveRemark" width="160" />
                </el-table>
            </el-tab-pane>

            <!-- ====== Tab3 待我审批（功能权限：attendance:approve） ====== -->
            <el-tab-pane v-if="checkPointPermission('attendance:approve')" label="待我审批" name="approve">
                <el-table :data="pendingList" border stripe v-loading="loading">
                    <el-table-column label="类型" width="90">
                        <template #default="{ row }">{{ applyTypeMap[row.applyType] }}</template>
                    </el-table-column>
                    <el-table-column label="申请人" prop="userId" width="90" />
                    <el-table-column label="开始时间" :formatter="fmtDateTime" prop="startTime" width="170" />
                    <el-table-column label="结束时间" :formatter="fmtDateTime" prop="endTime" width="170" />
                    <el-table-column label="事由" prop="reason" />
                    <el-table-column label="操作" width="170">
                        <template #default="{ row }">
                            <el-button type="success" size="small" @click="onApprove(row, true)">通过</el-button>
                            <el-button type="danger" size="small" @click="onApprove(row, false)">驳回</el-button>
                        </template>
                    </el-table-column>
                </el-table>
            </el-tab-pane>
        </el-tabs>

        <!-- 申请弹窗 -->
        <el-dialog v-model="applyShow" title="提交申请" width="460px">
            <el-form :model="applyForm" label-width="80px">
                <el-form-item label="类型">
                    <el-select v-model="applyForm.applyType" style="width: 100%;">
                        <el-option label="请假" :value="0" />
                        <el-option label="加班" :value="1" />
                    </el-select>
                </el-form-item>
                <el-form-item label="时间段">
                    <el-date-picker v-model="applyForm.range" type="datetimerange"
                        value-format="YYYY-MM-DD HH:mm:ss" start-placeholder="开始" end-placeholder="结束"
                        style="width: 100%;" />
                </el-form-item>
                <el-form-item label="事由">
                    <el-input v-model="applyForm.reason" type="textarea" :rows="3" maxlength="200"
                        show-word-limit placeholder="请填写事由（必填）" />
                </el-form-item>
            </el-form>
            <template #footer>
                <el-button @click="applyShow = false">取消</el-button>
                <el-button type="primary" @click="onSubmitApply">提交</el-button>
            </template>
        </el-dialog>

        <!-- 驳回意见弹窗 -->
        <el-dialog v-model="rejectShow" title="驳回（填审批意见）" width="420px">
            <el-input v-model="rejectRemark" type="textarea" :rows="3" maxlength="200"
                show-word-limit placeholder="驳回原因（可选）" />
            <template #footer>
                <el-button @click="rejectShow = false">取消</el-button>
                <el-button type="danger" @click="doReject">确认驳回</el-button>
            </template>
        </el-dialog>
    </el-card>
</template>

<script setup>
// ★ M03 考勤页：三个 tab（我的考勤 / 我的申请 / 待我审批）
// 审批 tab 用 checkPointPermission('attendance:approve') 控制可见——功能权限=按钮/区块级控制（⑭ 那套）
// 数据流：api/attendance.js（拦截器已拆 ApiResult 包）→ ref 裸定义 → 表格渲染
import { ref, onMounted } from "vue";
import { ElMessage } from "element-plus";
import {
    clockIn, clockOut, getMyAttendance,
    applyLeave, getMyApplications, getPendingList, approveLeave,
} from "@/api/attendance";
import { checkPointPermission } from "@/utils/checkPermission";

const activeTab = ref("record");
const loading = ref(false);

// —— 状态映射（后端存 int，前端负责翻译成人话）——
const attStatus = {
    0: { text: "正常", type: "success" },
    1: { text: "迟到", type: "warning" },
    2: { text: "早退", type: "warning" },
    3: { text: "缺卡", type: "info" },
    4: { text: "请假", type: "primary" },
};
const applyStatusMap = {
    0: { text: "待审批", type: "warning" },
    1: { text: "已通过", type: "success" },
    2: { text: "已驳回", type: "danger" },
};
const applyTypeMap = { 0: "请假", 1: "加班" };

// —— 数据源 ref 裸定义 ——
const myList = ref([]);
const myApplies = ref([]);
const pendingList = ref([]);

// —— 日期格式化（后端回来的是 ISO 串，表格里只展示需要的精度）——
const pad = (n) => String(n).padStart(2, "0");
const fmtDate = (row, col, val) => val ? `${val.slice(0, 10)}` : "—";
const fmtTime = (row, col, val) => val ? `${val.slice(11, 16)}` : "—";
const fmtDateTime = (row, col, val) => val ? `${val.slice(0, 10)} ${val.slice(11, 16)}` : "—";

// —— 打卡 ——
async function onClockIn() {
    try {
        await clockIn();
        ElMessage.success("上班打卡成功");
        await loadAll();
    } catch (msg) {
        ElMessage.error(msg);   //拦截器 reject 出来的就是提示语（如"今天已经打过上班卡了"）
    }
}
async function onClockOut() {
    try {
        await clockOut();
        ElMessage.success("下班打卡成功");
        await loadAll();
    } catch (msg) {
        ElMessage.error(msg);
    }
}

// —— 我的申请：弹窗提交 ——
const applyShow = ref(false);
const applyForm = ref({ applyType: 0, range: [], reason: "" });
function openApplyDialog() {
    applyForm.value = { applyType: 0, range: [], reason: "" };   //每次打开重置表单
    applyShow.value = true;
}
async function onSubmitApply() {
    const f = applyForm.value;
    if (!f.range || f.range.length !== 2) { ElMessage.error("请选择时间段"); return; }
    if (!f.reason || !f.reason.trim()) { ElMessage.error("事由必填"); return; }
    try {
        await applyLeave({ applyType: f.applyType, startTime: f.range[0], endTime: f.range[1], reason: f.reason.trim() });
        ElMessage.success("申请已提交，等待审批");
        applyShow.value = false;
        await loadAll();
    } catch (msg) {
        ElMessage.error(msg);   //如"结束时间不能早于开始时间"
    }
}

// —— 审批：通过直接走；驳回先弹意见 ——
const rejectShow = ref(false);
const rejectRemark = ref("");
const rejectId = ref(0);
async function onApprove(row, isPass) {
    if (isPass) {
        await doApprove(row.id, true, "同意");
    } else {
        rejectId.value = row.id;
        rejectRemark.value = "";
        rejectShow.value = true;
    }
}
async function doReject() {
    await doApprove(rejectId.value, false, rejectRemark.value);
    rejectShow.value = false;
}
async function doApprove(id, isPass, remark) {
    try {
        await approveLeave({ applicationId: id, isPass, remark });
        ElMessage.success(isPass ? "审批通过" : "已驳回");
        await loadAll();
    } catch (msg) {
        ElMessage.error(msg);   //如"该申请已被审批"
    }
}

// —— 首屏：一次拉三份数据（审批页没权限的人拉了也白拉？不——后端接口是登录即可调，
// 前端 tab 只是视觉隐藏；真正防越权靠后端权限点，这里保持简单）——
async function loadAll() {
    loading.value = true;
    try {
        const tasks = [getMyAttendance(), getMyApplications()];
        if (checkPointPermission("attendance:approve")) tasks.push(getPendingList());
        const [a, b, c] = await Promise.all(tasks);
        myList.value = a || [];
        myApplies.value = b || [];
        pendingList.value = c || [];
    } finally {
        loading.value = false;
    }
}

onMounted(loadAll);
</script>
