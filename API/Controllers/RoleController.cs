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
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles(int pageNumber = 1, int pageSize = 10, string? search = "")
        {
            var roles = await _roleService.GetPagedRoleAsync(pageNumber, pageSize, search);
            if (roles == null || roles.Items.Count == 0)
            {
                return NotFound("No Roles found.");
            }
            return Ok(roles);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] RoleModel role)
        {
            try
            {
                await _roleService.UpdateRoleAsync(role);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
