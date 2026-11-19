using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.Repository
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(MyDbContext myDbContext) : base(myDbContext)
        {

        }
    }
}
