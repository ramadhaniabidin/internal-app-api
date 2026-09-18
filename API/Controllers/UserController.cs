using API.Model;
using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserService userService;
        
        public UserController(UserService userService) => this.userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync(int pageNumber = 1, int pageSize = 5, string? search = "")
        {
            var result = await userService.GetPagedUserAsync(pageNumber, pageSize, search);
            if (result == null || result.Items.Count == 0)
            {
                return BadRequest("No users found.");
            }
            return Ok(result);
        }

    }
}
