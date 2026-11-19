using Mass.Cms.Entity;
using Mass.Cms.Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mass.Cms.IService
{
    //权限业务的专属接口：⑭ 尾巴权限 CRUD 上岗
    public interface IPermissionInfoService : IBaseService<PermissionInfo>
    {
        /// <summary>
        /// 新增权限：主表 + 按类型的子表，一次落两张
        /// </summary>
        Task<bool> AddPermission(PermissionDto permissionDto);

        /// <summary>
        /// 权限列表（练习版：全量平铺，不带分页搜索）
        /// </summary>
        Task<List<PermissionInfo>> GetPermissions();
    }
}
