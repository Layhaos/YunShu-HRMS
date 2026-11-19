using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class RoleInfoConfig : IEntityTypeConfiguration<RoleInfo>
    {
        public void Configure(EntityTypeBuilder<RoleInfo> builder)
        {
            builder.ToTable("T1017_RoleInfos");
            builder.Property(x => x.RoleName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Remark).HasMaxLength(200);

            // ★ 多对多：用户↔角色
            // WithMany 两边各挂导航属性，EF 自动造中间表，UsingEntity 给它起名
            // 中间表只有两列外键：UserInfoId / RoleInfoId
            builder.HasMany<UserInfo>(s => s.UserInfos)
                   .WithMany(s => s.RoleInfos)
                   .UsingEntity(a => a.ToTable("T_UserInfos_RoleInfos"));
        }
    }
}
