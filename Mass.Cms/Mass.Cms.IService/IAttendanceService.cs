using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    public interface IAttendanceService
    {
        /// <summary>
        /// 上班打卡：一天一条，重复打报错；9:00 后打记迟到
        /// 返回空串=成功，非空=给前端的提示
        /// </summary>
        Task<string> ClockInAsync(int userId);

        /// <summary>
        /// 下班打卡：必须先有上班卡；18:00 前打记早退
        /// </summary>
        Task<string> ClockOutAsync(int userId);

        /// <summary>
        /// 我的考勤记录（按日期倒序）
        /// </summary>
        Task<List<Attendance>> GetMyListAsync(int userId);
    }
}
