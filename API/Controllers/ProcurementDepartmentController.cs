using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProcurementDepartmentController : ControllerBase
    {
        private readonly ProcurementDepartmentService service;

        public ProcurementDepartmentController(ProcurementDepartmentService service) => this.service = service;

        [HttpGet]
        public async Task<IActionResult> GetProcurementDepartments(int pageNumber = 1, int pageSize = 5, string search = "")
        {
            var result = await service.GetPagedProcDeptAsync(pageNumber, pageSize, search);
            if (result == null || result.Items.Count == 0)
            {
                return NotFound("No procurement departments found.");
            }
            return Ok(result);
        }
    }
}
