using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendorController : ControllerBase
    {
        private readonly VendorService service;
        public VendorController(VendorService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVendors(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var result = await service.GetVendorsPages(pageNumber, pageSize, searchBy, keyword);
            return Ok(result);
        }
    }
}
