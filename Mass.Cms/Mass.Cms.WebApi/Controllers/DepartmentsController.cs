using Mass.Cms.Entity;
using Mass.Cms.IService;
using Mass.Cms.WebApi.Attributes;
using Mass.Cms.WebApi.Filters;
using Mass.Cms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mass.Cms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController:ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize]//查全部
        [TypeFilter(typeof(ApiPermissionFilter))]//⑭ 阶段D：API 权验上岗——先只护 GET（种子里目前只灌了 /api/Departments+GET 这一条）
        public async Task<IActionResult> GetAllDepartments()
        {
            //过滤掉软删除，查出平铺列表
            var departments = await _departmentService.LoadEntities(d => d.IsDeleted == false).ToListAsync();
            //包装成ApiResult对象返回给前端，方便前端统一处理
            return Ok(new ApiResult<List<Department>>
            {
                IsSuccess = true,
                Message = "查询成功",
                Data = departments
            });
        }

        [HttpPost]
        [UnitOfWork]
        [Authorize]//新增
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {

            department.CreateDate = DateTime.Now;
            department.EditDate = DateTime.Now;
            department.IsDeleted = false;//先创建再追加

            //await加在了bool上，报错
            //await
            _departmentService.AddEntity(department);
            //这里是同步的直接删改


            return Ok(new ApiResult<Department>
            {
                IsSuccess = true,
                Message = "新增成功",
                Data = department
            });
        }

        [HttpDelete("{id}")]
        [UnitOfWork]
        [Authorize]
        public async Task<IActionResult> DeleteDepartment([FromRoute] int id)
        {
            // 1. 先查询该部门是否存在且未被删除
            var department = await _departmentService.LoadEntities(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();

            if (department == null)
            {
                return Ok(new ApiResult<string>
                {
                    IsSuccess = false,
                    Message = "部门不存在"
                });
            }

            // 2. 软删除：只改标记，不删物理数据
            department.IsDeleted = true;
            department.EditDate = DateTime.Now; // 顺手更新一下修改时间

            // 3. 更新到数据库
            _departmentService.UpdateEntity(department);

            return Ok(new ApiResult<string>
            {
                IsSuccess = true,
                Message = "删除成功"
            });
        }
    }
}
