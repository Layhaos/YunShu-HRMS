using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    public interface ILeaveApplicationService
    {
        /// <summary>
        /// 提交申请（请假/加班）：起止时间合法即入待审批
        /// 返回空串=成功，非空=提示
        /// </summary>
        Task<string> ApplyAsync(int userId, int applyType, DateTime startTime, DateTime endTime, string reason);

        /// <summary>
        /// 我的申请单
        /// </summary>
        Task<List<LeaveApplication>> GetMineAsync(int userId);

        /// <summary>
        /// 待审批列表（管理员视角）
        /// </summary>
        Task<List<LeaveApplication>> GetPendingListAsync();

        /// <summary>
        /// 审批：通过+请假 → 自动写考勤（状态=请假，起止日期逐天落账）
        /// 多表操作由 [UnitOfWork] 统一提交，天然在一个事务里
        /// </summary>
        Task<string> ApproveAsync(int approverId, int applicationId, bool isPass, string remark);
    }
}
