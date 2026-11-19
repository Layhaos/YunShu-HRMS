using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class PermissionMenuConfig : IEntityTypeConfiguration<PermissionMenu>
    {
        public void Configure(EntityTypeBuilder<PermissionMenu> builder)
        {
            builder.ToTable("T1019_PermissionMenus");
            builder.Property(x => x.MenuIcon).HasMaxLength(200);
            builder.Property(x => x.MenunOrder).HasMaxLength(20);
        }
    }
}
