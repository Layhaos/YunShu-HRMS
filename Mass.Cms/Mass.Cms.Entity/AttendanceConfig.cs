using Mass.Cms.Entity.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class AttendanceConfig : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("T1019_Attendances");
            builder.Property(x => x.UserId).HasDefaultValue(0);
            //状态存 int，读出来强转回枚举（EF 对枚举默认就存底层 int）
            builder.Property(x => x.Status).HasDefaultValue(AttendanceStatusEnum.Normal);
            //同一用户同一天只允许一条：加索引挡重复（并发下最后的防线在 Redis 锁，这里是库层兜底）
            builder.HasIndex(x => new { x.UserId, x.WorkDate }).IsUnique();
        }
    }
}
