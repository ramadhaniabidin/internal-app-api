using API.Data;
using API.Helpers;
using API.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace API.Services.ORM
{
    public class RoleService
    {
        private readonly DataContext _context;
        private readonly QueryHelper _queryHelper;

        public RoleService(DataContext appDbContext, QueryHelper queryHelper)
        {
            _context = appDbContext;
            _queryHelper = queryHelper;
        }

        public async Task<List<RoleModel>> GetAllRoles()
        {
            var roles = await _context.Roles.Where(b => b.IsActive).ToListAsync();
            return roles;
        }

        public async Task<List<RoleModel>> GetRolesByIdsAsync(List<int> roleIds)
        {
            var roles = await _context.Roles.Where(b => roleIds.Contains(b.Id) && b.IsActive).ToListAsync();
            return roles;
        }

        public async Task<PagedResult<RoleModel>> GetPagedRoleAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.Roles.Where(r => r.IsActive);
            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression: !string.IsNullOrEmpty(search) ? r => EF.Functions.ILike(r.Name, $"%{search}%") : null,
                selector: r => new RoleModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Alias = r.Alias,
                    IsActive = r.IsActive
                },
                orderBy: r => r.Name
            );
        }

        public async Task<RoleModel?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task UpdateRoleAsync(RoleModel role)
        {
            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole != null)
            {
                existingRole.Name = role.Name;
                existingRole.Alias = role.Alias;
                existingRole.LastUpdatedAt = DateTime.UtcNow;
                _context.Roles.Update(existingRole);
                await _context.SaveChangesAsync();
            }
        }
    }
}
