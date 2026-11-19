using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class PermissionInfoConfig : IEntityTypeConfiguration<PermissionInfo>
    {
        public void Configure(EntityTypeBuilder<PermissionInfo> builder)
        {
            builder.ToTable("T1018_PermissionInfos");
            builder.Property(x => x.PermissionName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PermissionCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PermissionDescription).HasMaxLength(500);

            // ★ 多对多：角色↔权限（中间表 T_RoleInfos_Permissions，只含两列外键）
            builder.HasMany<RoleInfo>(s => s.RoleInfos)
                   .WithMany(s => s.PermissionInfos)
                   .UsingEntity(a => a.ToTable("T_RoleInfos_Permissions"));

            // ★ 三张 1对1 子表：各自挂显式外键 PermissionId
            // WithOne 反向导航 + HasForeignKey 指明外键住在子表上
            builder.HasOne<PermissionApi>(p => p.PermissionApi)
                   .WithOne(p => p.PermissionInfo)
                   .HasForeignKey<PermissionApi>(d => d.PermissionId);

            builder.HasOne<PermissionMenu>(p => p.PermissionMenu)
                   .WithOne(p => p.PermissionInfo)
                   .HasForeignKey<PermissionMenu>(d => d.PermissionId);

            builder.HasOne<PermissionPoint>(p => p.PermissionPoint)
                   .WithOne(p => p.PermissionInfo)
                   .HasForeignKey<PermissionPoint>(d => d.PermissionId);
        }
    }
}
