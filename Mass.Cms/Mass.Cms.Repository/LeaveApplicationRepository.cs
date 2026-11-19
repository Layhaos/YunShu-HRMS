using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.Repository
{
    public class LeaveApplicationRepository : BaseRepository<LeaveApplication>, ILeaveApplicationRepository
    {
        public LeaveApplicationRepository(MyDbContext myDbContext) : base(myDbContext)
        {

        }
    }
}
