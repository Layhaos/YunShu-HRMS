using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    public interface IBaseService<T> where T : class, new()
    {
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize,out int totalCount, 
            Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, S>> orderbyLambda, bool isAsc);
        bool AddEntity(T entity);
        bool DeleteEntity(T entity);
        bool UpdateEntity(T entity);
    }

    //TODO【你来填】：这里要定义业务层的公共泛型接口，命名 IBaseService<T>，约束和 IBaseRepository<T> 一样
    //约束：where T : class, new()
    //
    //提示：这个接口是 IBaseRepository<T> 的"镜像"——业务层要具备和仓储一样的能力，
    //方法名、参数、返回值一比一模过来。对照 Mass.Cms.IRepository\IBaseRepository.cs 抄结构：
    //
    //  IQueryable<T> LoadEntities(...)...
    //  IQueryable<T> LoadPageEntities<S>(...)...
    //  bool AddEntity(T entity);
    //  bool DeleteEntity(T entity);
    //  bool UpdateEntity(T entity);
    //
    //不明白为什么要长一模一样？可以先写，写完当前进 ArticelService 时再体会（代码文件 IArticelService.cs 里有说明）

}
