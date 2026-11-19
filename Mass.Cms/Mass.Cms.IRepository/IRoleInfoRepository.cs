using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.IRepository
{
    //角色专属仓储：目前公共能力够用，为空；角色专属查询写在里头
    public interface IRoleInfoRepository : IBaseRepository<RoleInfo>
    {
    }
}
