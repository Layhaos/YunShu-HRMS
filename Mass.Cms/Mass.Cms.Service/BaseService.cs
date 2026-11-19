using Mass.Cms.IRepository;
using Mass.Cms.IService;
using System.Linq.Expressions;

namespace Mass.Cms.Service
{
    /// <summary>
    /// 业务层公共基类：把仓储的能力原样"转发"出去
    /// 你会发现每个方法就一行 repository.Xxx(...)——这是故意的：
    /// 1. BaseService 自己**不新增**逻辑，公共能力 = 仓储能力
    /// 2. 专属业务逻辑写在具体 XxxService 里（比如 ArticelService 的搜索条件拼接）
    /// 3. 这样上层（控制器）永远只认识 IService 接口，业务逻辑换了实现它不知道也不关心
    /// </summary>
    public class BaseService<T> : IBaseService<T> where T : class, new()
    {
        //由子类（UserInfoService/ArticelService）构造时注入自己的专属仓储
        protected IBaseRepository<T> repository { get; set; } = null!;

        public bool AddEntity(T entity)
        {
            return repository.AddEntity(entity);
        }

        public bool DeleteEntity(T entity)
        {
            return repository.DeleteEntity(entity);
        }

        public bool UpdateEntity(T entity)
        {
            return repository.UpdateEntity(entity);
        }

        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
        {
            return repository.LoadEntities(whereLambda);
        }

        public IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int totalCount,
            Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, S>> orderbyLambda, bool isAsc)
        {
            return repository.LoadPageEntities(pageIndex, pageSize, out totalCount, whereLambda, orderbyLambda, isAsc);
        }
    }
}
