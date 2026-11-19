using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity.Enum
{
    /// <summary>
    /// 申请类型：请假 / 加班（一张申请表两种业务，靠类型字段分流）
    /// </summary>
    public enum LeaveTypeEnum
    {
        /// <summary>请假</summary>
        Leave = 0,
        /// <summary>加班</summary>
        Overtime = 1
    }
}
