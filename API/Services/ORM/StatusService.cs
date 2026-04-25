using API.Data;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class StatusService
    {
        private readonly DataContext context;
        public StatusService(DataContext context) => this.context = context;
        public async Task<List<StatusModel>> GetAllStatusesAsync()
        {
            return await context.Statuses.Where(s => s.IsActive).ToListAsync();
        }
    }
}
