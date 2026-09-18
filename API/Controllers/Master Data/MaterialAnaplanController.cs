using API.Model;
using API.Model.Master_Data;
using API.Services.ORM;
using API.Services.ORM.Master_Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Master_Data
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialAnaplanController : ControllerBase
    {
        private readonly MaterialAnaplanService service;
        private readonly GeneralLedgerService glService;
        private readonly ProcurementDepartmentService procDeptService;
        public MaterialAnaplanController(
            MaterialAnaplanService service, 
            GeneralLedgerService glService,
            ProcurementDepartmentService procDeptService)
        {
            this.service = service;
            this.glService = glService;
            this.procDeptService = procDeptService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var result = await service.GetAll(pageNumber, pageSize, searchBy, keyword);
            if (result == null || result.Items.Count == 0)
            {
                return NotFound("No material anaplan found.");
            }
            return Ok(result);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var gl = await service.GetById(id);
            if (gl == null)
            {
                return NotFound($"No material anaplan found with ID: {id}");
            }
            return Ok(gl);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaterialAnaplan model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                return BadRequest("Material Code is required");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                return BadRequest("Material Description is required");
            }
            if (model.GeneralLedgerId == 0)
            {
                return BadRequest("General Ledger is required");
            }
            if (model.ProcurementDepartmentId == 0)
            {
                return BadRequest("Procurement Department is required");
            }

            var generalLedger = await glService.GetById(model.GeneralLedgerId);
            if(generalLedger == null)
            {
                return BadRequest($"There is no General Ledger with Id: {model.GeneralLedgerId}");
            }

            var procDept = await procDeptService.GetById(model.ProcurementDepartmentId);
            if (procDept == null)
            {
                return BadRequest($"There is no General Ledger with Id: {model.ProcurementDepartmentId}");
            }

            var existing = await service.GetByCode(model.Code);
            if (existing != null && !string.IsNullOrEmpty(existing.Code))
            {
                return Conflict($"There is already an existing item for Code \"{model.Code}\"");
            }

            await service.Create(model);
            return Ok("Material Anaplan created successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> Update(MaterialAnaplan model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                return BadRequest("Code is required");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                return BadRequest("Description is required");
            }

            var existing = await service.GetByCode(model.Code);
            if (existing == null)
            {
                return NotFound($"There is no Material Anaplan found for Code \"{model.Code}\"");
            }
            model.Id = existing.Id;
            await service.Update(model);
            return NoContent();
        }

        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.Delete(id);
            return NoContent();
        }
    }
}
