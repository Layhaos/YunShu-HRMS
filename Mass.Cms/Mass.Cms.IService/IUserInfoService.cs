using Mass.Cms.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    //用户业务的专属接口：目前公共能力够用，为空
    //等哪天用户业务有专属方法（如"按手机号找回用户"），写在这里
    public interface IUserInfoService : IBaseService<UserInfo>
    {
        /// <summary>
        /// 获取某用户的权限编码字典：{ "menus": [菜单编码...], "points": [按钮编码...] }
        /// 登录后前端守卫调 by-phone 时后端顺路算好返回
        /// </summary>
        Task<Dictionary<string, List<string>>> GetUserPermissions(UserInfo user);

        //⑭ 尾巴任务3：员工分配角色
        Task<List<int>> GetUserRoleIds(int userId);
        Task<bool> AssignRoles(int userId, string roleIds);
    }
}
