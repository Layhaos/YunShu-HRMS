using Mass.Cms.Entity;
using Mass.Cms.Entity.Dtos;
using Mass.Cms.IService;
using Mass.Cms.WebApi.Attributes;
using Mass.Cms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mass.Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionInfoService _permissionInfoService;

        public PermissionsController(IPermissionInfoService permissionInfoService)
        {
            _permissionInfoService = permissionInfoService;
        }

        // 权限列表（练习版：全量平铺，不带分页）
        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _permissionInfoService.GetPermissions();
            return Ok(new ApiResult<List<PermissionInfo>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = permissions
            });
        }

        // 新增权限：一次写两张表（主表+子表），所以贴 [UnitOfWork]——
        // Action 跑完过滤器统一 SaveChanges，主表子表同一事务一起成/一起败
        [HttpPost]
        [UnitOfWork]
        public async Task<IActionResult> CreatePermissions([FromBody] PermissionDto permissionDto)
        {
            var result = await _permissionInfoService.AddPermission(permissionDto);
            if (!result)
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = "新增失败" });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "新增成功" });
        }
    }
}
