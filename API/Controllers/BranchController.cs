using API.Model;
using API.Services.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private const string V = "{code}";
        private readonly BranchService _branchService;
        public BranchController(BranchService branchService) => _branchService = branchService;

        [HttpGet]
        public async Task<IActionResult> GetBranches(int pageNumber = 1, int pageSize = 5, string search = "")
        {
            var branches = await _branchService.GetBranchesAsync(pageNumber, pageSize, search);
            if (branches == null || branches.Items.Count == 0)
            {
                return NotFound("No branches found.");
            }
            return Ok(branches);
        }

        [HttpGet(V)]
        public async Task<IActionResult> GetBranchByCode(string code)
        {
            var branch = await _branchService.GetBranchByCodeAsync(code);
            if (branch == null)
            {
                return NotFound($"No branch found with code: {code}");
            }
            return Ok(branch);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound($"No branch found with ID: {id}");
            }
            return Ok(branch);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranchByCode(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
            {
                return NotFound($"Branch not found");
            }
            await _branchService.DeleteBranchByIdAsync(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(BranchModel branch)
        {
            if (branch == null || string.IsNullOrEmpty(branch.Code) || string.IsNullOrEmpty(branch.Name))
            {
                return BadRequest("Branch code and name are required.");
            }
            var existingBranch = await _branchService.GetBranchByCodeAsync(branch.Code);
            if (existingBranch != null)
            {
                return Conflict($"A branch with code {branch.Code} already exists.");
            }
            await _branchService.CreateBranchAsync(branch);
            return CreatedAtAction(nameof(GetBranchByCode), new { code = branch.Code }, branch);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBranch(BranchModel branch)
        {
            if (branch == null || string.IsNullOrEmpty(branch.Code) || string.IsNullOrEmpty(branch.Name))
            {
                return BadRequest("Branch code and name are required.");
            }
            var existingBranch = await _branchService.GetBranchByCodeAsync(branch.Code);
            if (existingBranch == null)
            {
                return NotFound($"No branch found with code: {branch.Code}");
            }
            branch.Id = existingBranch.Id;
            await _branchService.UpdateBranchAsync(branch);
            return NoContent();

        }

    }
}
