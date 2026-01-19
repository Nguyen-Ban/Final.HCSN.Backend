using Final.HCSN.Core.Interface.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Final.HCSN.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepo _departmentRepo;
        public DepartmentsController(IDepartmentRepo departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentRepo.GetAllAsync();
            return Ok(departments);
        }
    }
}
