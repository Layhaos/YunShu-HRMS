using Mass.Cms.Entity;
using Mass.Cms.Entity.Enum;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Service
{
    /// <summary>
    /// 请假/加班申请与审批。
    /// 事务亮点：审批通过 + 写考勤是两张表的改动，全在一个 Action 里完成，
    /// 由 [UnitOfWork] 过滤器在请求末尾统一 SaveChanges —— EF Core 一个 SaveChanges 本身就是事务，
    /// 要么"审批通过 + 考勤落账"一起成，要么一起回滚
    /// </summary>
    public class LeaveApplicationService : ILeaveApplicationService
    {
        private readonly ILeaveApplicationRepository leaveApplicationRepository;
        private readonly IAttendanceRepository attendanceRepository;

        public LeaveApplicationService(ILeaveApplicationRepository leaveApplicationRepository, IAttendanceRepository attendanceRepository)
        {
            this.leaveApplicationRepository = leaveApplicationRepository;
            this.attendanceRepository = attendanceRepository;
        }

        public async Task<string> ApplyAsync(int userId, int applyType, DateTime startTime, DateTime endTime, string reason)
        {
            if (endTime < startTime)
            {
                return "结束时间不能早于开始时间";
            }

            var apply = new LeaveApplication
            {
                UserId = userId,
                ApplyType = (LeaveTypeEnum)applyType,
                StartTime = startTime,
                EndTime = endTime,
                Reason = reason,
                Status = ApplyStatusEnum.Pending,
                CreateDate = DateTime.Now,
                EditDate = DateTime.Now,
                IsDeleted = false
            };
            leaveApplicationRepository.AddEntity(apply);   //只记待办，[UnitOfWork] 统一入库
            return "";
        }

        public async Task<List<LeaveApplication>> GetMineAsync(int userId)
        {
            return await leaveApplicationRepository
                .LoadEntities(l => l.UserId == userId && l.IsDeleted == false)
                .OrderByDescending(l => l.CreateDate)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetPendingListAsync()
        {
            return await leaveApplicationRepository
                .LoadEntities(l => l.Status == ApplyStatusEnum.Pending && l.IsDeleted == false)
                .OrderBy(l => l.CreateDate)
                .ToListAsync();
        }

        public async Task<string> ApproveAsync(int approverId, int applicationId, bool isPass, string remark)
        {
            var apply = await leaveApplicationRepository
                .LoadEntities(l => l.Id == applicationId && l.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (apply == null)
            {
                return "申请单不存在";
            }
            if (apply.Status != ApplyStatusEnum.Pending)
            {
                return "该申请已被审批";
            }

            //通过 + 请假 → 起止日期逐天写考勤（状态=请假）
            if (isPass && apply.ApplyType == LeaveTypeEnum.Leave)
            {
                for (var day = apply.StartTime.Date; day <= apply.EndTime.Date; day = day.AddDays(1))
                {
                    var att = await attendanceRepository
                        .LoadEntities(a => a.UserId == apply.UserId && a.WorkDate == day)
                        .FirstOrDefaultAsync();
                    if (att == null)
                    {
                        attendanceRepository.AddEntity(new Attendance
                        {
                            UserId = apply.UserId,
                            WorkDate = day,
                            ClockIn = null,
                            ClockOut = null,
                            Status = AttendanceStatusEnum.Leave,
                            CreateDate = DateTime.Now,
                            EditDate = DateTime.Now,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        //已有记录（可能打过卡）也覆盖为请假，ClockIn/ClockOut 保留原值不动
                        att.Status = AttendanceStatusEnum.Leave;
                        att.EditDate = DateTime.Now;
                    }
                }
            }

            apply.Status = isPass ? ApplyStatusEnum.Approved : ApplyStatusEnum.Rejected;
            apply.ApproverId = approverId;
            apply.ApproveRemark = remark;
            apply.ApproveDate = DateTime.Now;
            apply.EditDate = DateTime.Now;
            //不在这里 SaveChanges：连同考勤的改动一起交给 [UnitOfWork] 统一提交（一个事务）
            return "";
        }
    }
}
