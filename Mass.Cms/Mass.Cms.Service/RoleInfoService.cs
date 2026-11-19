using Mass.Cms.Entity;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Mass.Cms.Service
{
    public class RoleInfoService : BaseService<RoleInfo>, IRoleInfoService
    {
        private readonly IRoleInfoRepository roleInfoRepository;
        //授权时按 id 捞权限实体用
        private readonly IPermissionInfoRepository permissionInfoRepository;
        //⑮ Redis：角色权限变了，要删"该角色下所有用户"的权限缓存
        private readonly IConnectionMultiplexer redis;

        public RoleInfoService(IRoleInfoRepository roleInfoRepository,
            IPermissionInfoRepository permissionInfoRepository,
            IConnectionMultiplexer redis)
        {
            //把专属仓储交给父类的公共字段，父类五件套方法都走它
            base.repository = roleInfoRepository;
            //再存一份给自己，写角色专属方法时用
            this.roleInfoRepository = roleInfoRepository;
            this.permissionInfoRepository = permissionInfoRepository;
            this.redis = redis;
        }

        //★ 角色列表（授权页的表格数据）
        public Task<List<RoleInfo>> GetRoles()
        {
            return roleInfoRepository.LoadEntities(r => r.IsDeleted == false).ToListAsync();
        }

        //★ 该角色当前勾中的权限 id 集合（回显用）
        public async Task<List<int>> GetRolePermissionIds(int roleId)
        {
            var role = await roleInfoRepository.LoadEntities(r => r.Id == roleId)
                .Include(r => r.PermissionInfos)
                .FirstOrDefaultAsync();
            if (role == null)
            {
                return new List<int>();
            }
            //只要没被软删的权限 id
            return role.PermissionInfos.Where(p => p.IsDeleted == false).Select(p => p.Id).ToList();
        }

        //★ 角色授权：删旧插新（EF 多对多导航自动 Delete/Insert 中间表），集合差相同直接 return
        //返回空串=成功；非空=给前端的错误提示（防自锁校验的提示语从这里带出去）
        public async Task<string> AssignPermissions(int roleId, string permissionIds)
        {
            //串值契约：空选发 "0" 保底（1112 弹药）——"0" 不匹配任何权限 id，等价清空
            var ids = permissionIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var i) ? i : 0)
                .Where(i => i > 0)
                .Distinct()
                .ToList();

            //防自锁校验：「角色管理」菜单权限不允许从任何角色上移除——
            //否则该角色下的用户一刷新，路由不挂、菜单消失，自己把自己锁在门外（1117 实测翻车现场）
            var roleMenu = await permissionInfoRepository
                .LoadEntities(p => p.PermissionCode == "role" && p.IsDeleted == false)
                .FirstOrDefaultAsync();
            if (roleMenu != null && !ids.Contains(roleMenu.Id))
            {
                return "不能移除「角色管理」菜单权限：移除后该角色的用户将无法再进入授权页";
            }

            var role = await roleInfoRepository.LoadEntities(r => r.Id == roleId)
                .Include(r => r.PermissionInfos)
                .Include(r => r.UserInfos)   //⑮ 顺路带上用户：改了角色权限，这些用户的缓存都要删
                .FirstOrDefaultAsync();
            if (role == null)
            {
                return "角色不存在";
            }

            var target = await permissionInfoRepository
                .LoadEntities(p => ids.Contains(p.Id) && p.IsDeleted == false)
                .ToListAsync();

            //集合差为 0 直接 return——少一次无意义的删+插（返回空串=成功）
            var current = role.PermissionInfos.Where(p => p.IsDeleted == false).Select(p => p.Id).OrderBy(x => x).ToList();
            var wanted = target.Select(p => p.Id).OrderBy(x => x).ToList();
            if (current.SequenceEqual(wanted))
            {
                return "";
            }

            //删旧插新：导航集合整体换成 target——EF 自动对中间表做 Delete/Insert，不用手写 SQL
            role.PermissionInfos.Clear();
            foreach (var p in target)
            {
                role.PermissionInfos.Add(p);
            }

            //⑮ 先更库后删缓存：角色权限变了，"领这个角色的每个用户"的权限缓存全部作废
            await roleInfoRepository.SaveChangesAsync();
            var database = redis.GetDatabase(3);
            foreach (var u in role.UserInfos.Where(u => u.IsDeleted == false))
            {
                await database.KeyDeleteAsync($"perm:user:{u.Id}");
            }
            return "";   //空串 = 成功
        }
    }
}
