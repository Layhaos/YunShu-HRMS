using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.IRepository
{
    //权限专属仓储：目前公共能力够用，为空
    public interface IPermissionInfoRepository : IBaseRepository<PermissionInfo>
    {
    }
}
