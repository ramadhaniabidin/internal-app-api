using API.Model;
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
        public async Task<IActionResult> GetProcurementDepartments(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var result = await service.GetPagedData(pageNumber, pageSize, searchBy, keyword);
            if (result == null || result.Items.Count == 0)
            {
                return NotFound("No procurement departments found.");
            }
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contractType = await service.GetById(id);
            if (contractType == null)
            {
                return NotFound($"No procurement department found with ID: {id}");
            }
            return Ok(contractType);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var procDept = await service.GetByCode(code);
            if (procDept == null)
            {
                return NotFound($"No procurement department found with code: {code}");
            }
            return Ok(procDept);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProcurementDepartmentModel procDept)
        {
            if (string.IsNullOrEmpty(procDept.Title) || string.IsNullOrEmpty(procDept.Code))
            {
                return BadRequest("Title and Code are required.");
            }
            var existingProcDept = await service.GetByCode(procDept.Code);
            if (existingProcDept != null)
            {
                return Conflict("A procurement department with the same code already exists.");
            }
            await service.Create(procDept);
            return Ok("Procurement department type created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProcurementDepartmentModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Title))
            {
                return BadRequest("Procurement Department code and name are required.");
            }
            var procDept = await service.GetByCode(model.Code);
            if (procDept == null)
            {
                return NotFound($"No contract type found with code: {model.Code}");
            }
            model.Id = procDept.Id;
            await service.Update(model);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contractType = await service.GetById(id);
            if (contractType == null)
            {
                return NotFound($"Procurement Department not found");
            }
            await service.Delete(id);
            return NoContent();
        }
    }
}
