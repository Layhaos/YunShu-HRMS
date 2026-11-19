using Mass.Cms.Entity;
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
    /// 请假/加班申请与审批接口
    /// 提交/查自己的：所有登录用户；审批：管理员（前端用 attendance:approve 功能权限控制入口）
    /// </summary>
    [Route("api/[controller]")]   //→ /api/leaveApplications
    [ApiController]
    [Authorize]
    public class LeaveApplicationsController : ControllerBase
    {
        private readonly ILeaveApplicationService leaveApplicationService;

        public LeaveApplicationsController(ILeaveApplicationService leaveApplicationService)
        {
            this.leaveApplicationService = leaveApplicationService;
        }

        /// <summary>
        /// 提交申请  POST /api/leaveApplications （请假/加班共用，靠 ApplyType 分流）
        /// </summary>
        [HttpPost]
        [UnitOfWork]   //新增只记待办，Filter 统一提交
        public async Task<IActionResult> Apply([FromBody] LeaveApplyDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var error = await leaveApplicationService.ApplyAsync(userId, dto.ApplyType, dto.StartTime, dto.EndTime, dto.Reason);
            if (!string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = error });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "申请已提交，等待审批" });
        }

        /// <summary>
        /// 我的申请单  GET /api/leaveApplications/mine
        /// </summary>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await leaveApplicationService.GetMineAsync(userId);
            return Ok(new ApiResult<List<LeaveApplication>> { IsSuccess = true, Message = "查询成功", Data = list });
        }

        /// <summary>
        /// 待审批列表  GET /api/leaveApplications/pending
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingList()
        {
            var list = await leaveApplicationService.GetPendingListAsync();
            return Ok(new ApiResult<List<LeaveApplication>> { IsSuccess = true, Message = "查询成功", Data = list });
        }

        /// <summary>
        /// 审批  POST /api/leaveApplications/approve
        /// isPass=true 通过（请假类型会自动写考勤），false 驳回
        /// 审批 + 写考勤两张表的改动由 [UnitOfWork] 统一提交 = 同一个事务
        /// </summary>
        [HttpPost("approve")]
        [UnitOfWork]
        public async Task<IActionResult> Approve([FromBody] LeaveApproveDto dto)
        {
            var approverId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var error = await leaveApplicationService.ApproveAsync(approverId, dto.ApplicationId, dto.IsPass, dto.Remark ?? "");
            if (!string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = error });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = dto.IsPass ? "审批通过" : "已驳回" });
        }
    }
}
