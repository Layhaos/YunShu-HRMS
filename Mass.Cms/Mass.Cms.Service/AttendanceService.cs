using Mass.Cms.Entity;
using Mass.Cms.Entity.Enum;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Service
{
    /// <summary>
    /// 考勤服务。打卡的两道防线：
    /// 1.Redis 分布式锁（key=用户编号+日期，SETNX 语义 LockTake，10 秒兜底）挡住并发连点；
    /// 2.数据库 (UserId, WorkDate) 唯一索引做最后兜底。
    /// 锁的 key 设计沿用课堂口径：限制"同一个用户"的并发 → 用用户编号做 key
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository attendanceRepository;
        private readonly IConnectionMultiplexer redis;

        //工作时段口径：9:00 上班、18:00 下班（写死常量，以后要配置化再抽 appsettings）
        private static readonly TimeSpan WorkStart = new TimeSpan(9, 0, 0);
        private static readonly TimeSpan WorkEnd = new TimeSpan(18, 0, 0);

        public AttendanceService(IAttendanceRepository attendanceRepository, IConnectionMultiplexer redis)
        {
            this.attendanceRepository = attendanceRepository;
            this.redis = redis;
        }

        public async Task<string> ClockInAsync(int userId)
        {
            var today = DateTime.Today;
            var db = redis.GetDatabase(11);   //锁住 11 号库，沿用课堂口径
            var lockKey = $"attendance:clock:{userId}:{today:yyyyMMdd}";
            var token = Guid.NewGuid().ToString();
            if (!db.LockTake(lockKey, token, TimeSpan.FromSeconds(10)))
            {
                return "打卡处理中，请勿连续点击";
            }
            try
            {
                var now = DateTime.Now;
                var existing = await attendanceRepository
                    .LoadEntities(a => a.UserId == userId && a.WorkDate == today)
                    .FirstOrDefaultAsync();
                if (existing != null && existing.ClockIn != null)
                {
                    return "今天已经打过上班卡了";
                }

                var record = new Attendance
                {
                    UserId = userId,
                    WorkDate = today,
                    ClockIn = now,
                    //打卡瞬间判定：晚于 9:00 = 迟到，否则正常
                    Status = now.TimeOfDay > WorkStart ? AttendanceStatusEnum.Late : AttendanceStatusEnum.Normal,
                    CreateDate = now,
                    EditDate = now,
                    IsDeleted = false
                };
                attendanceRepository.AddEntity(record);
                //在持锁期间显式落库（⑮ 的显式保存出口）：先落库再释放锁，
                //避免"锁已放、数据没落"的空窗被第二个请求钻进来
                await attendanceRepository.SaveChangesAsync();
                return "";
            }
            finally
            {
                db.LockRelease(lockKey, token);
            }
        }

        public async Task<string> ClockOutAsync(int userId)
        {
            var today = DateTime.Today;
            var now = DateTime.Now;
            var existing = await attendanceRepository
                .LoadEntities(a => a.UserId == userId && a.WorkDate == today)
                .FirstOrDefaultAsync();

            if (existing == null || existing.ClockIn == null)
            {
                return "今天还没打上班卡";
            }
            if (existing.ClockOut != null)
            {
                return "今天已经打过下班卡了";
            }

            existing.ClockOut = now;
            //简化口径：18:00 前打下班卡 = 早退（就算早上迟到过，最终状态也按早退记）
            if (now.TimeOfDay < WorkEnd)
            {
                existing.Status = AttendanceStatusEnum.Early;
            }
            existing.EditDate = now;
            await attendanceRepository.SaveChangesAsync();
            return "";
        }

        public async Task<List<Attendance>> GetMyListAsync(int userId)
        {
            return await attendanceRepository
                .LoadEntities(a => a.UserId == userId && a.IsDeleted == false)
                .OrderByDescending(a => a.WorkDate)
                .Take(60)
                .ToListAsync();
        }
    }
}
