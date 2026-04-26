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

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetContractTypeById(int id)
        {
            var contractType = await service.GetContractTypeById(id);
            if (contractType == null)
            {
                return NotFound($"No branch found with ID: {id}");
            }
            return Ok(contractType);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetContractTypeByCode(string code)
        {
            var branch = await service.GetByCode(code);
            if (branch == null)
            {
                return NotFound($"No branch found with code: {code}");
            }
            return Ok(branch);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateContractType([FromBody]ContractTypeModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Title))
            {
                return BadRequest("Contract Type code and name are required.");
            }
            var existingBranch = await service.GetByCode(model.Code);
            if (existingBranch == null)
            {
                return NotFound($"No branch found with code: {model.Code}");
            }
            model.Id = existingBranch.Id;
            await service.UpdateContractType(model);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractType(int id)
        {
            var contractType = await service.GetContractTypeById(id);
            if (contractType == null)
            {
                return NotFound($"Contract type not found");
            }
            await service.DeleteContractType(id);
            return NoContent();
        }
    }
}
