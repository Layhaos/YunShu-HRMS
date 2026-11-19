using System;
using System.Collections.Generic;
using System.Text;

namespace Mass.Cms.WebApi.Models
{
    /// <summary>
    /// 员工分配角色的接收袋：只在 Controller 门口用，住 WebApi\Models
    /// roleIds 是前端多选 join(',') 的逗号串（串值契约，空选发 "0" 保底）
    /// </summary>
    public class UserAssignDto
    {
        public int UserId { get; set; }

        public string? RoleIds { get; set; }
    }
}
