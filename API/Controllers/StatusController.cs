using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusController : ControllerBase
    {
        private readonly StatusService service;
        public StatusController(StatusService service) => this.service = service;

        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await service.GetAllStatusesAsync();
            if (statuses == null || statuses.Count == 0)
            {
                return NotFound("No statuses found.");
            }
            return Ok(statuses);
        }
    }
}
