using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Entity
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("T1016_Departments");
            builder.Property(x => x.DepartmentName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.DepartmentCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.DepartmentDescr).HasMaxLength(200);
            builder.Property(x => x.ParentId).HasDefaultValue(0);  //0 = 顶级部门
        }
    }
}
