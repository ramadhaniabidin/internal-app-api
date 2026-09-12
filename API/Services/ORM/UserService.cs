using API.Data;
using API.Helpers;
using API.Model;
using API.Model.Display;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class UserService
    {
        private readonly DataContext _context;
        private readonly QueryHelper _queryHelper;
        public UserService(DataContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }

        public async Task<UserModel?> GetByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<PagedResult<UserDisplay>> GetPagedUserAsync(int pageNumber = 1, int pageSize = 5, string? search = "")
        {
            var query = _context.Users.Where(u => u.IsActive && !string.IsNullOrEmpty(u.FullName));
            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression: !string.IsNullOrEmpty(search) ? u => EF.Functions.ILike(u.FullName, $"%{search}%") : null,
                selector: u => new UserDisplay
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Department = u.Department ?? "",
                    Division = u.Division ?? "",
                    Role = u.Role ?? "",
                    IsActive = u.IsActive,
                    Username = u.Username ?? "",
                    Email = u.Email ?? "",
                },
                orderBy: u => u.Id
            );
        }
    }
}
