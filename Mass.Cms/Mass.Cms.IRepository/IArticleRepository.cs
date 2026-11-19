using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IRepository
{
    //目前没有专属方法，公共仓储的五个方法都是接口继承来的
    public interface IArticleRepository : IBaseRepository<Article>
    {
    }
}
