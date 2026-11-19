using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity.Enum
{
    /// <summary>
    /// 申请单状态：提交后走审批流，状态单向流转 待审批→已通过/已驳回
    /// </summary>
    public enum ApplyStatusEnum
    {
        /// <summary>待审批（提交后的初始状态）</summary>
        Pending = 0,
        /// <summary>已通过</summary>
        Approved = 1,
        /// <summary>已驳回</summary>
        Rejected = 2
    }
}
