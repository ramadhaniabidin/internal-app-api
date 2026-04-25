using API.Model;
using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NonCommercialController : ControllerBase
    {
        private readonly NonCommercialService service;
        private readonly RoleService roleService;
        private readonly List<int> contractApproverRoleIds;
        private readonly List<int> prApproverRoleIds;
        private readonly List<int> qcfApproverRoleIds;
        private readonly List<int> poReleaseApproverRoleIds;
        private readonly List<int> poContractApproverRoleIds;
        public NonCommercialController(NonCommercialService service, RoleService roleService, IConfiguration configuration)
        {
            this.service = service;
            this.roleService = roleService;
            contractApproverRoleIds = configuration.GetSection("AppSettings:ContractApproverRoleIds").Get<List<int>>() ?? [];
            prApproverRoleIds = configuration.GetSection("AppSettings:PRApproverRoleIds").Get<List<int>>() ?? [];
            qcfApproverRoleIds = configuration.GetSection("AppSettings:QCFApproverRoleIds").Get<List<int>>() ?? [];
            poReleaseApproverRoleIds = configuration.GetSection("AppSettings:POReleaseApproverRoleIds").Get<List<int>>() ?? [];
            poContractApproverRoleIds = configuration.GetSection("AppSettings:POContractApproverRoleIds").Get<List<int>>() ?? [];
        }

        [HttpGet("Index")]
        public async Task<IActionResult> GetNonCommercials()
        {
            var result = await service.GetNonCommercialDataAsync();
            if (result == null)
            {
                return NotFound("No non-commercial items found.");
            }
            return Ok(result);
        }

        [HttpGet("ApproverRole")]
        public async Task<IActionResult> GetApproverRoles(int moduleID)
        {
            Console.WriteLine($"Received request for approver roles with moduleID: {moduleID}");
            if (moduleID < 1)
            {
                return BadRequest("Missing parameter moduleID (int)");
            }
            List<RoleModel> approverRoles = new();
            if (moduleID == 4) approverRoles = await roleService.GetRolesByIdsAsync(contractApproverRoleIds);
            else if (moduleID == 1) approverRoles = await roleService.GetRolesByIdsAsync(prApproverRoleIds);
            else if (moduleID == 2) approverRoles = await roleService.GetRolesByIdsAsync(qcfApproverRoleIds);
            else if (moduleID == 6) approverRoles = await roleService.GetRolesByIdsAsync(poReleaseApproverRoleIds);
            else if (moduleID == 5) approverRoles = await roleService.GetRolesByIdsAsync(poContractApproverRoleIds);
            return Ok(approverRoles);
        }
    }
}
