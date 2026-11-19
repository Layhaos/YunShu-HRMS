using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    //部门业务的专属接口：目前公共能力够用，为空
    //等哪天部门业务有专属方法（如"同级查重"），写在这里
    public interface IDepartmentService : IBaseService<Department>
    {
    }
}
