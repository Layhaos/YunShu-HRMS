using Mass.Cms.Entity;
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
    public class RolesController : ControllerBase
    {
        private readonly IRoleInfoService _roleInfoService;

        public RolesController(IRoleInfoService roleInfoService)
        {
            _roleInfoService = roleInfoService;
        }

        // 角色列表
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleInfoService.GetRoles();
            return Ok(new ApiResult<List<RoleInfo>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = roles
            });
        }

        // 某角色已勾选的权限 id 集合（回显）
        [HttpGet("{roleId}/permissionIds")]
        public async Task<IActionResult> GetRolePermissionIds(int roleId)
        {
            var ids = await _roleInfoService.GetRolePermissionIds(roleId);
            return Ok(new ApiResult<List<int>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = ids
            });
        }

        // 角色授权：删旧插新，[UnitOfWork] 统一提交
        [HttpPost("assign")]
        [UnitOfWork]
        public async Task<IActionResult> AssignPermissions([FromBody] RoleAssignDto dto)
        {
            var error = await _roleInfoService.AssignPermissions(dto.RoleId, dto.PermissionIds ?? "0");
            if (!string.IsNullOrEmpty(error))
            {
                return Ok(new ApiResult<string> { IsSuccess = false, Message = error });
            }
            return Ok(new ApiResult<string> { IsSuccess = true, Message = "授权成功" });
        }
    }
}
