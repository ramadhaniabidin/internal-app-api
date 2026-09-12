using API.Data;
using API.Model;
using API.Model.Display;
using API.Model.Master_Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Services.ORM.Master_Data
{
    public class GeneralLedgerService
    {
        private readonly DataContext context;
        public GeneralLedgerService(DataContext context)
        {
            this.context = context;
        }

        private IQueryable<GeneralLedgers> GenerateQueryForAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = context.GeneralLedgers.Where(gl => gl.IsActive);
            Expression<Func<GeneralLedgers, bool>>? searchExpression = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchColumn = searchBy ?? "";
                searchExpression = searchColumn.ToLower() switch
                {
                    "code" => gl => EF.Functions.ILike(gl.Code ?? "", $"%{keyword}%"),
                    "name" => gl => EF.Functions.ILike(gl.Description ?? "", $"%{keyword}%"),
                    _ => gl => EF.Functions.ILike(gl.Description ?? "", $"%{keyword}%")
                };
            }

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }
            return query;
        }

        public async Task<PagedResult<GeneralLedgers>> GetAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = GenerateQueryForAll(pageNumber, pageSize, searchBy, keyword);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(i => i.Description)
                .ToListAsync();

            return new PagedResult<GeneralLedgers>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SearchTerm = keyword
            };
        }

        public async Task<GeneralLedgers?> GetById(int id)
        {
            return await context.GeneralLedgers.FirstOrDefaultAsync(gl => gl.Id == id);
        }

        public async Task Delete(int id)
        {
            var gl = await GetById(id);
            if(gl != null)
            {
                gl.IsActive = false;
                gl.UpdatedDate = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task Create(GeneralLedgers model)
        {
            model.CreatedDate = DateTime.UtcNow;
            model.IsActive = true;
            await context.GeneralLedgers.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task<GeneralLedgers?> GetExisting(GeneralLedgers model)
        {
            var query = context.GeneralLedgers.Where(gl => gl.IsActive && gl.Code == model.Code);
            var existing = await query
                .FirstOrDefaultAsync();
            return existing;
        }

        public async Task Update(GeneralLedgers model)
        {
            var existing = await context.GeneralLedgers.FirstOrDefaultAsync(gl => gl.Id ==  model.Id);
            if(existing != null)
            {
                existing.UpdatedDate = DateTime.UtcNow;
                existing.Code = model.Code;
                existing.Description = model.Description;
                await context.SaveChangesAsync();
            }
        }
    }
}
