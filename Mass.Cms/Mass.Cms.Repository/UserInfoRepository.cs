using Mass.Cms.Entity;
using Mass.Cms.EntityFrameworkCore;
using Mass.Cms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Mass.Cms.Repository
{
    public class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
    {
        //因为任意一个类实现增删改查的代码基本一致
        //所以这里也封装出去


        public UserInfoRepository(MyDbContext myDbContext) : base(myDbContext)
        {

        }
    }
}
