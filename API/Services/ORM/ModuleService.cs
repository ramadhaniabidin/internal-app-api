using API.Data;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class ModuleService
    {
        private readonly DataContext context;
        public ModuleService(DataContext context) => this.context = context;
        public async Task<List<ModuleModel>> GetAllModulesAsync()
        {
            return await context.Modules.Where(m => m.IsActive).ToListAsync();
        }

        public async Task<List<ModuleModel>> GetModulesByCategoryAsync(int categoryId)
        {
            return await context.Modules
                .Where(m => m.IsActive && m.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
