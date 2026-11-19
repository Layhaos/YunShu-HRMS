using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class UserInfoConfig : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("T1015_Users");
            builder.Property(x => x.UserName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.UserPassword).HasMaxLength(20).IsRequired();
            builder.Property(x => x.UserEmail).HasMaxLength(100).IsRequired();
            builder.Property(x => x.UserPhone).HasMaxLength(16).IsRequired();
            builder.Property(x => x.Gender).HasDefaultValue(0);
            builder.Property(x => x.PhotoUrl).HasMaxLength(100).HasDefaultValue("images/default.jpg");
        }

    }
}
