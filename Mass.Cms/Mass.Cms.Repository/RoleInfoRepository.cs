using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.Repository
{
    public class RoleInfoRepository : BaseRepository<RoleInfo>, IRoleInfoRepository
    {
        //同构四件套之一：只要把 DbContext 从构造函数递给父类，五件套方法全部白嫖
        public RoleInfoRepository(MyDbContext myDbContext) : base(myDbContext)
        {
        }
    }
}
