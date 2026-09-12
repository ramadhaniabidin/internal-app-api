using API.Model;
using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProcDeptController : ControllerBase
    {
        private readonly UserProcDeptService service;
        public UserProcDeptController(UserProcDeptService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var result = await service.GetAll(pageNumber, pageSize, searchBy, keyword);
            if (result == null || !result.Items.Any())
            {
                return NotFound("No user procurement department found.");
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserProcurementDepartmentsModel model)
        {
            if(model.UserId == 0)
            {
                return BadRequest("User is required");
            }
            if (model.BranchId == 0)
            {
                return BadRequest("Branch is required");
            }
            if (model.ProcurementDepartmentId == 0)
            {
                return BadRequest("Procurement Department is required");
            }

            var existing = await service.GetExisting(model);
            if(existing != null && !string.IsNullOrEmpty(existing.BranchName) && !string.IsNullOrEmpty(existing.ProcDeptName) && !string.IsNullOrEmpty(existing.UserFullName))
            {
                return Conflict($"There is already an existing item for User \"{existing.UserFullName}\" and Branch \"{existing.BranchName}\" and Proc Dept \"{existing.ProcDeptName}\"");
            }

            await service.Create(model);
            return Ok("User Procurement department created successfully.");
        }
    }
}
