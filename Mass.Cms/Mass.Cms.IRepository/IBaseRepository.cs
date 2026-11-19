using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.IRepository
{
    public interface IBaseRepository<T> where T:class,new()//传过来的泛型必须是引用类，可以new实例化的
    {
        //拿出来的是公共的，就不能再只写UserInfo了，改用泛型
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int totalCount,
            Expression<Func<T, bool>> whereLambda,
            Expression<Func<T, S>> orderbyLambda, bool isAsc);
        //这个s是方法的泛型，在调方法时指定方法的类型
        //写死了int，排序就只能用int类型的了
        bool AddEntity(T entity);
        bool DeleteEntity(T entity);
        bool UpdateEntity(T entity);

        //⑮ 新增：显式保存出口。平时增删改只记待办、由 [UnitOfWork] 过滤器统一保存；
        //但"改权限 + 删缓存"必须先等数据库真落库再删缓存（先更库后删缓存），
        //所以这类方法在 Service 里自己先调它，过滤器之后那一次 SaveChanges 没有待办，等于空跑
        Task<int> SaveChangesAsync();
    }
}
