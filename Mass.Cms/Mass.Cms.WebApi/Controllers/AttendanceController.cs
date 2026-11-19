using Mass.Cms.Entity;
using Mass.Cms.Entity.Enum;
using Mass.Cms.IService;
using Mass.Cms.WebApi.Attributes;
using Mass.Cms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mass.Cms.WebApi.Controllers
{
    /// <summary>
    /// 考勤接口：打卡（上班/下班）+ 我的考勤 + 审批走 LeaveApplications 控制器
    /// 当前登录人从 JWT 的 ClaimTypes.NameIdentifier 取（登录时塞的就是用户 Id）
    /// </summary>
    [Route("api/[controller]")]   //→ /api/attendance
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            this.attendanceService = attendanceService;
        }

        /// <summary>
        /// 上班打卡  POST /api/attendance/clock-in
        /// </summary>
        [HttpPost("clock-in")]
        public async Task<IActionResult> ClockIn()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var error = await attendanceService.ClockInAsync(userId);
            if (!string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = error });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "上班打卡成功" });
        }

        /// <summary>
        /// 下班打卡  POST /api/attendance/clock-out
        /// </summary>
        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOut()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var error = await attendanceService.ClockOutAsync(userId);
            if (!string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = error });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "下班打卡成功" });
        }

        /// <summary>
        /// 我的考勤记录  GET /api/attendance/my
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyList()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await attendanceService.GetMyListAsync(userId);
            return Ok(new ApiResult<List<Attendance>>
            {
                IsSuccess = true,
                Message = "查询成功",
                //实体没有导航属性，不存在循环引用，直接返回（映射成 DTO 的必要性在"实体带导航/敏感字段"时才出现）
                Data = list
            });
        }
    }
}
