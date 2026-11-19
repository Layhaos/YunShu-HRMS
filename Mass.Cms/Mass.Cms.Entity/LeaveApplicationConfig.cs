using Mass.Cms.Entity.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class LeaveApplicationConfig : IEntityTypeConfiguration<LeaveApplication>
    {
        public void Configure(EntityTypeBuilder<LeaveApplication> builder)
        {
            builder.ToTable("T1020_LeaveApplications");
            builder.Property(x => x.UserId).HasDefaultValue(0);
            builder.Property(x => x.Reason).HasMaxLength(200);
            builder.Property(x => x.ApproveRemark).HasMaxLength(200);
            builder.Property(x => x.Status).HasDefaultValue(ApplyStatusEnum.Pending);
        }
    }
}
