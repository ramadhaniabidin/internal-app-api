using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleService service;
        public ModuleController(ModuleService service)
        {
            this.service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            var modules = await service.GetAllModulesAsync();
            if (modules == null || modules.Count == 0)
            {
                return NotFound("No modules found.");
            }
            return Ok(modules);

        }

        [HttpGet("Category/{categoryId}")]
        public async Task<IActionResult> GetModulesByCategory(int categoryId)
        {
            var modules = await service.GetModulesByCategoryAsync(categoryId);
            if (modules == null || modules.Count == 0)
            {
                return NotFound($"No modules found for category ID: {categoryId}");
            }
            return Ok(modules);

        }

    }
}
