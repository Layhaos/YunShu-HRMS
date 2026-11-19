using Mass.Cms.Entity;
using Mass.Cms.IRepository;
using Mass.Cms.IService;

namespace Mass.Cms.Service
{
    public class DepartmentService : BaseService<Department>, IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            //把专属仓储交给父类的公共字段，父类五件套方法都走它
            base.repository = departmentRepository;
            //再存一份给自己，写部门专属方法时用
            this.departmentRepository = departmentRepository;
        }
    }
}
