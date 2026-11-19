using Mass.Cms.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mass.Cms.EntityFrameworkCore
{
    public class MyDbContext:DbContext
    {
        public DbSet<UserInfo> Users { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<RoleInfo> RoleInfos { get; set; }

        public DbSet<PermissionInfo> PermissionInfos { get; set; }

        public DbSet<PermissionApi> PermissionApis { get; set; }

        public DbSet<PermissionMenu> PermissionMenus { get; set; }

        public DbSet<PermissionPoint> PermissionPoints { get; set; }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //获取输出目录，连着不在同一个项目，但是引用了，就到输出目录找
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mass.Cms.Entity.dll");
            var assembly = Assembly.LoadFrom(assemblyPath);//通过路径反射拿到程序集
            //注意：Config 类（IEntityTypeConfiguration<>）都定义在 Entity 项目里，
            //所以必须扫描 Entity 程序集；扫 this.GetType().Assembly（EF 层自己）会一无所获，
            //表现为：表名/长度/默认值等配置全部不生效（之前生成的 InitUser 就是这么漏掉的）
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

    }
}
