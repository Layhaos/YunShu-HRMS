using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 请假/加班申请提交参数（收货箱 DTO：只装前端该送的字段）
    /// </summary>
    public class LeaveApplyDto
    {
        /// <summary>
        /// 申请类型：0=请假 1=加班
        /// </summary>
        [Required]
        public int ApplyType { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 事由
        /// </summary>
        [Required]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 审批提交参数
    /// </summary>
    public class LeaveApproveDto
    {
        /// <summary>
        /// 申请单 Id
        /// </summary>
        [Required]
        public int ApplicationId { get; set; }

        /// <summary>
        /// true=通过 false=驳回
        /// </summary>
        [Required]
        public bool IsPass { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Remark { get; set; }
    }
}
