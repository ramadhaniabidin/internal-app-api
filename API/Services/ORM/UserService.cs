using API.Data;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class UserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context) => _context = context;

        public async Task<UserModel?> GetByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
