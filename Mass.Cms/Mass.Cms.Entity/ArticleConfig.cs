using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.Entity
{
    public class ArticleConfig : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("T_Articles");

            builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
            //正文不给长度上限：文章长度不可控，nvarchar(max) 是有意为之
            builder.Property(x => x.Content).IsRequired();

            //一对多：一篇文章只属于一个用户；一个用户可以有很多文章；外键 UserInfoId 非空
            builder.HasOne(a => a.UserInfo).WithMany(b => b.Articles).IsRequired();
        }
    }
}
