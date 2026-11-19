using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.Repository
{
    //实现 IBaseRepository<T> 接口：上层（Service）只依赖接口，不依赖实现类
    public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
    {
        //接收子类传来的MyDbContext对象
        private readonly MyDbContext dbContext;
        public BaseRepository(MyDbContext myDbContext)
        {
            this.dbContext = myDbContext;
        }

        public bool AddEntity(T entity)
        {//要实现这些方法就要拿到MyDbContext对象
            /*dbContext.Users.Add(entity);
            dbContext.SaveChangesAsync();*/
            //1.这里不能写死，这是公共类里的属性
            //2.SaveChanges如果在这里面搞，那每次有一条更改，
            //都会执行一次数据库操作、而且不在同一个事务里
            //所以应该要用工作单元，
            //在中间件的返回代码里面执行一次，
            //因为这期间HttpResponse对象和MyDbContext对象都是同一个

            dbContext.Set<T>().Add(entity);
            //Set是根据传过来的T类型，找到dbContext里面，
            //我们创建的对应属性的DbSet

            return true;//只做联系
            
        }

        public bool DeleteEntity(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            return true;
        }

        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
        {
            return dbContext.Set<T>().Where(whereLambda);
        }

        public IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int totalCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderbyLambda, bool isAsc)
        {
            var temp = dbContext.Set<T>().Where(whereLambda);
            totalCount = temp.Count();
            if (isAsc)
            {
                temp = temp.OrderBy<T, S>(orderbyLambda)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                temp = temp.OrderByDescending<T, S>(orderbyLambda)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);
            }

            return temp;
        }

        public bool UpdateEntity(T entity)
        {
            dbContext.Set<T>().Update(entity);
            return true;
        }

        //⑮ 新增：显式保存出口（见 IBaseRepository 注释）——Service 里需要"立刻落库"的少数方法用
        public Task<int> SaveChangesAsync()
        {
            return dbContext.SaveChangesAsync();
        }
    }
}
