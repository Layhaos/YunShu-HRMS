using Mass.Cms.Entity;
using Mass.Cms.Entity.Dtos;
using Mass.Cms.Entity.Enum;
using Mass.Cms.IRepository;
using Mass.Cms.IService;
using Microsoft.EntityFrameworkCore;

namespace Mass.Cms.Service
{
    public class PermissionInfoService : BaseService<PermissionInfo>, IPermissionInfoService
    {
        private readonly IPermissionInfoRepository permissionInfoRepository;
        //三张子表的仓储：switch 里按类型往对应子表插，专门注进来
        private readonly IPermissionMenuRepository permissionMenuRepository;
        private readonly IPermissionPointRepository permissionPointRepository;
        private readonly IPermissionApiRepository permissionApiRepository;

        public PermissionInfoService(IPermissionInfoRepository permissionInfoRepository,
            IPermissionMenuRepository permissionMenuRepository,
            IPermissionPointRepository permissionPointRepository,
            IPermissionApiRepository permissionApiRepository)
        {
            //把专属仓储交给父类的公共字段，父类五件套方法都走它
            base.repository = permissionInfoRepository;
            //再存一份给自己，写权限专属方法时用
            this.permissionInfoRepository = permissionInfoRepository;
            this.permissionMenuRepository = permissionMenuRepository;
            this.permissionPointRepository = permissionPointRepository;
            this.permissionApiRepository = permissionApiRepository;
        }

        //★ 权限列表（练习版：全量平铺，代工）
        public Task<List<PermissionInfo>> GetPermissions()
        {
            return permissionInfoRepository.LoadEntities(p => p.IsDeleted == false).ToListAsync();
        }

        //★★★ ⑭ 尾巴【你写区】：AddPermission 四步（约 30 行）★★★
        //输入：PermissionDto（混合 DTO：主表字段 + 三类子表的个性字段全在里面）
        //输出：bool（真正的入库由 [UnitOfWork] 过滤器在 Action 跑完后统一 SaveChanges——
        //      一次提交一个事务，主表和子表一起成一起败，这里只负责"记待办"）
        //四步思路（照老师版）：
        //  1. 造 PermissionInfo 主表实体：从 dto 抄公共字段（Name/Code/Type/Description/ParentId），
        //     **顺手补三行账**：CreateDate=Now、EditDate=Now、IsDeleted=false（不补入库 0001-01-01，⑬ 的坑）
        //  2. permissionInfoRepository.AddEntity(permissionInfo)——主表"记待办"
        //  3. switch (permissionDto.PermissionType) 按 类型 分拣，三个 case：
        //     (int)PermissionTypeEnum.PermissionMenu → 造 PermissionMenu 子实体：
        //         MenuIcon/MenunOrder 从 dto 抄 → 补三行账 →
        //         ★关键一行★ permissionMenu.PermissionInfo = permissionInfo;
        //         （一对一挂接：EF 保存时自动把主表的 Id 填进子表外键 PermissionId）
        //         → permissionMenuRepository.AddEntity(permissionMenu)
        //     PermissionPoint → 同款：PointClass/PointIcon/PointStatus + 挂接 + AddEntity
        //     PermissionApi → 同款：ApiUrl/ApiMethod + 挂接 + AddEntity
        //  4. return true;
        //  ★为什么子表不自己填 PermissionId：导航属性一挂，EF 帮你填——这叫"一对一挂接"（⑫ 弹药）
        public Task<bool> AddPermission(PermissionDto permissionDto)
        {
            var permissionInfo = new PermissionInfo
            {
                PermissionName = permissionDto.PermissionName,        // 从 dto 抄
                PermissionCode = permissionDto.PermissionCode,        // 从 dto 抄
                PermissionType = permissionDto.PermissionType,        // 从 dto 抄
                PermissionDescription = permissionDto.PermissionDescription, // 从 dto 抄
                ParentId = permissionDto.ParentId,              // 从 dto 抄
                CreateDate = DateTime.Now,
                EditDate = DateTime.Now,
                IsDeleted = false
            };

            permissionInfoRepository.AddEntity(permissionInfo);

            switch (permissionDto.PermissionType)
            {
                case (int)PermissionTypeEnum.PermissionMenu:
                    var permissionMenu = new PermissionMenu
                    {
                        MenuIcon = permissionDto.MenuIcon,       // 从 dto 抄
                        MenunOrder = permissionDto.MenunOrder,     // 从 dto 抄
                        CreateDate = DateTime.Now,
                        EditDate = DateTime.Now,
                        IsDeleted = false,
                        PermissionInfo = permissionInfo   // ← 挂接，EF 自动填外键
                    };
                    permissionMenuRepository.AddEntity(permissionMenu);
                    break;

                case (int)PermissionTypeEnum.PermissionPoint:
                    var permissionPoint = new PermissionPoint
                    {
                        PointClass = permissionDto.PointClass,     // 从 dto 抄
                        PointIcon = permissionDto.PointIcon,      // 从 dto 抄
                        PointStatus = permissionDto.PointStatus,    // 从 dto 抄
                        CreateDate = DateTime.Now,
                        EditDate = DateTime.Now,
                        IsDeleted = false,
                        PermissionInfo = permissionInfo
                    };
                    permissionPointRepository.AddEntity(permissionPoint);
                    break;

                case (int)PermissionTypeEnum.PermissionApi:
                    var permissionApi = new PermissionApi
                    {
                        ApiUrl = permissionDto.ApiUrl,         // 从 dto 抄
                        ApiMethod = permissionDto.ApiMethod,      // 从 dto 抄
                        CreateDate = DateTime.Now,
                        EditDate = DateTime.Now,
                        IsDeleted = false,
                        PermissionInfo = permissionInfo
                    };
                    permissionApiRepository.AddEntity(permissionApi);
                    break;
            }




            //4. 全同步方法（增删改只是记待办，没碰库）→ 摘 async，Task.FromResult 手工包装
            return Task.FromResult(true);
        }
    }
}
