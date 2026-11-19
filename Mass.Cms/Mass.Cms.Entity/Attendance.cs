using System;
using System.Collections.Generic;
using Mass.Cms.Entity.Enum;
using System.Text;

namespace Mass.Cms.Entity
{
    /// <summary>
    /// 考勤记录：一个用户一天一条（同一天重复打卡 = 更新已有记录，不插新行）
    /// 上班打卡补 ClockIn，下班打卡补 ClockOut，Status 在打卡瞬间判定
    /// </summary>
    public class Attendance : BaseEntity<int>
    {
        /// <summary>
        /// 考勤人（指向 UserInfo.Id，不挂真外键——与 Department.ManagerId 同一处理）
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 考勤日期（只算到"天"，一天一条的关键字段）
        /// </summary>
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// 上班打卡时间（当天第一次打卡写入；没打就是 null）
        /// </summary>
        public DateTime? ClockIn { get; set; }

        /// <summary>
        /// 下班打卡时间（下班卡写入；没打就是 null）
        /// </summary>
        public DateTime? ClockOut { get; set; }

        /// <summary>
        /// 当天状态：正常/迟到/早退/缺卡/请假
        /// </summary>
        public AttendanceStatusEnum Status { get; set; }
    }
}
