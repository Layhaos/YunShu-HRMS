using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity.Enum
{
    /// <summary>
    /// 考勤状态：一天一条考勤记录，用这个字段表达当天最终状态
    /// </summary>
    public enum AttendanceStatusEnum
    {
        /// <summary>正常出勤</summary>
        Normal = 0,
        /// <summary>上班打卡晚于 9:00</summary>
        Late = 1,
        /// <summary>下班打卡早于 18:00</summary>
        Early = 2,
        /// <summary>上班/下班缺卡</summary>
        Missed = 3,
        /// <summary>请假（审批通过后自动写入）</summary>
        Leave = 4
    }
}
