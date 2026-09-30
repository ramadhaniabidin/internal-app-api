using API.Data;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class BranchService
    {
        private readonly DataContext _context;
        public BranchService(DataContext context) => _context = context;

        public async Task<List<BranchModel>> GetAllBranchesAsync()
        {
            return await _context.Branches
                .Where(b => b.IsActive)
                .Select(b => new BranchModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    IsActive = b.IsActive
                })
                .ToListAsync();
        }

        public async Task<PagedResult<BranchModel>> GetBranchesAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.Branches.Where(b => b.IsActive);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    EF.Functions.ILike(b.Name, $"%{search}%"));
            }
            var totalCount = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .OrderBy(b => b.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BranchModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    IsActive = b.IsActive
                })
                .ToListAsync();
            return new PagedResult<BranchModel>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPage,
                SearchTerm = search
            };
        }

        public async Task<BranchModel?> GetBranchByIdAsync(int id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null || !branch.IsActive) return null;
            return new BranchModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Code = branch.Code,
                IsActive = branch.IsActive
            };
        }

        public async Task<BranchModel?> GetBranchByCodeAsync(string code)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Code == code && b.IsActive);
            if (branch == null) return null;
            return new BranchModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Code = branch.Code,
                IsActive = branch.IsActive
            };
        }

        public async Task DeleteBranchByCodeAsync(string code)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Code == code);
            if (branch != null)
            {
                branch.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBranchByIdAsync(int id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);
            if (branch != null)
            {
                branch.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnDeleteBranchByCodeAsync(string code)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Code == code);
            if (branch != null)
            {
                branch.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateBranchAsync(BranchModel branch)
        {
            var newBranch = new BranchModel
            {
                Name = branch.Name,
                Code = branch.Code,
                IsActive = branch.IsActive
            };
            _context.Branches.Add(newBranch);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBranchAsync(BranchModel branch)
        {
            var existingBranch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branch.Id);
            if (existingBranch != null)
            {
                existingBranch.Name = branch.Name;
                existingBranch.Code = branch.Code;
                existingBranch.IsActive = branch.IsActive;
                await _context.SaveChangesAsync();
            }
        }
    }
}
