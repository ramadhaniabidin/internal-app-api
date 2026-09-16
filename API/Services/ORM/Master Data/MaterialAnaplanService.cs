using API.Data;
using API.Model;
using API.Model.Display;
using API.Model.Master_Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Services.ORM.Master_Data
{
    public class MaterialAnaplanService
    {
        private readonly DataContext _context;
        public MaterialAnaplanService(DataContext context)
        {
            _context = context;
        }

        private IQueryable<MaterialAnaplanDiplay> GenerateQueryForAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = from m in _context.MaterialAnaplans.Where(m => m.Active)
                        join p in _context.ProcurementDepartments on m.ProcurementDepartmentId equals p.Id
                        join gl in _context.GeneralLedgers on m.GeneralLedgerId equals gl.Id
                        select new MaterialAnaplanDiplay
                        {
                            Id = m.Id,
                            Code = m.Code,
                            Description = m.Description,
                            GeneralLedgerCode = gl.Code,
                            GeneralLedgerDescription = gl.Description,
                            GeneralLedgerId = gl.Id,
                            Concatenate = $"{m.Code} {m.Description}",
                            ValuationClass = m.ValuationClass,
                            ProcDeptCode = p.Code,
                            ProcDeptName = p.Title,
                            ProcurementDepartmentId = p.Id,
                            CreatedDate = m.CreatedDate,
                            UpdatedDate = m.UpdatedDate,
                            Display = m.Code + " - " + m.Description
                        };

            Expression<Func<MaterialAnaplanDiplay, bool>>? searchExpression = null;

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

        public async Task<PagedResult<MaterialAnaplanDiplay>> GetAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = GenerateQueryForAll(pageNumber, pageSize, searchBy, keyword);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(i => i.Description)
                .ToListAsync();

            return new PagedResult<MaterialAnaplanDiplay>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SearchTerm = keyword
            };
        }

        public async Task Create(MaterialAnaplan model)
        {
            model.CreatedDate = DateTime.UtcNow;
            await _context.MaterialAnaplans.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<MaterialAnaplan?> GetByCode(string code)
        {
            return await _context.MaterialAnaplans.FirstOrDefaultAsync(m => m.Code == code && m.Active);
        }

        public async Task<MaterialAnaplan?> GetById(int id)
        {
            return await _context.MaterialAnaplans.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
