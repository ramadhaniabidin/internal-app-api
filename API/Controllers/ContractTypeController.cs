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
    public class ContractTypeController : ControllerBase
    {
        private readonly ContractTypeService service;
        public ContractTypeController(ContractTypeService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContractType(ContractTypeModel contractType)
        {
            if (string.IsNullOrEmpty(contractType.Title) || string.IsNullOrEmpty(contractType.Code))
            {
                return BadRequest("Title and Code are required.");
            }
            var existingContractType = await service.GetByCode(contractType.Code);
            if (existingContractType != null)
            {
                return Conflict("A contract type with the same code already exists.");
            }
            await service.CreateContractType(contractType);
            return Ok("Contract type created successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContractTypes(int pageNumber = 1, int pageSize = 10, string? search = "")
        {
            var result = await service.GetContractTypesPaged(pageNumber, pageSize, search);
            if (result == null || !result.Items.Any())
            {
                return NotFound("No contract types found.");
            }
            return Ok(result);
        }
    }
}
