using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 角色授权的接收袋：只在 Controller 门口用，住 WebApi\Models
    /// permissionIds 是前端 getCheckedKeys join(',') 出来的逗号串（串值契约，空选发 "0" 保底）
    /// </summary>
    public class RoleAssignDto
    {
        public int RoleId { get; set; }

        public string? PermissionIds { get; set; }
    }
}
