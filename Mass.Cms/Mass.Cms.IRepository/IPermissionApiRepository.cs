using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.IRepository
{
    //API权限子表专属仓储：目前公共能力够用，为空
    public interface IPermissionApiRepository : IBaseRepository<PermissionApi>
    {
    }
}
