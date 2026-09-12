using API.Model;
using API.Model.Master_Data;
using API.Services.ORM.Master_Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Master_Data
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralLedgerController : ControllerBase
    {
        private readonly GeneralLedgerService service;
        public GeneralLedgerController(GeneralLedgerService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var result = await service.GetAll(pageNumber, pageSize, searchBy, keyword);
            if (result == null || !result.Items.Any())
            {
                return NotFound("No general ledgers found.");
            }
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var gl = await service.GetById(id);
            if (gl == null)
            {
                return NotFound($"No general ledger found with ID: {id}");
            }
            return Ok(gl);
        }

        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GeneralLedgers model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                return BadRequest("Code is required");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                return BadRequest("Description is required");
            }

            var existing = await service.GetExisting(model);
            if (existing != null )
            {
                return Conflict($"There is already an existing GL for Code \"{existing.Code}\"");
            }

            await service.Create(model);
            return Ok("General Ledger created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update(GeneralLedgers model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                return BadRequest("Code is required");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                return BadRequest("Description is required");
            }

            var existing = await service.GetExisting(model);
            if (existing == null)
            {
                return NotFound($"There is no GL found for Code \"{model.Code}\"");
            }
            model.Id = existing.Id;
            await service.Update(model);
            return NoContent();
        }
    }
}
