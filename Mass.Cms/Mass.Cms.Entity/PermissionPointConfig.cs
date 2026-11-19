using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class PermissionPointConfig : IEntityTypeConfiguration<PermissionPoint>
    {
        public void Configure(EntityTypeBuilder<PermissionPoint> builder)
        {
            builder.ToTable("T1020_PermissionPoints");
            builder.Property(x => x.PointClass).HasMaxLength(50);
            builder.Property(x => x.PointIcon).HasMaxLength(50);
        }
    }
}
