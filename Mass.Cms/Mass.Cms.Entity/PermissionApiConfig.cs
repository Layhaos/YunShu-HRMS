using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class PermissionApiConfig : IEntityTypeConfiguration<PermissionApi>
    {
        public void Configure(EntityTypeBuilder<PermissionApi> builder)
        {
            builder.ToTable("T1021_PermissionApis");
            builder.Property(x => x.ApiUrl).HasMaxLength(200);
            builder.Property(x => x.ApiMethod).HasMaxLength(10);
        }
    }
}
