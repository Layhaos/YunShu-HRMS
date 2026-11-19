using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Repository
{
    public class PermissionMenuRepository : BaseRepository<PermissionMenu>, IPermissionMenuRepository
    {
        //同构四件套之一：只要把 DbContext 从构造函数递给父类，五件套方法全部白嫖
        public PermissionMenuRepository(MyDbContext myDbContext) : base(myDbContext)
        {
        }
    }
}
