using System;
using System.Collections.Generic;
using Mass.Cms.Entity.Enum;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 请假/加班申请单：提交→待审批→通过/驳回（状态机单向流转）
    /// 审批人 / 审批意见 / 审批时间在审批动作发生时才填
    /// </summary>
    public class LeaveApplication : BaseEntity<int>
    {
        /// <summary>
        /// 申请人（指向 UserInfo.Id）
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 申请类型：请假 / 加班
        /// </summary>
        public LeaveTypeEnum ApplyType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// 单据状态：待审批/已通过/已驳回
        /// </summary>
        public ApplyStatusEnum Status { get; set; }

        /// <summary>
        /// 审批人（指向 UserInfo.Id，审批时写入）
        /// </summary>
        public int? ApproverId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string? ApproveRemark { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApproveDate { get; set; }
    }
}
