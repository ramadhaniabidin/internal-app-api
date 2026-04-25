using API.Data;
using API.Helpers;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class ProcurementDepartmentService
    {
        private readonly DataContext _context;
        private readonly QueryHelper _queryHelper;
        public ProcurementDepartmentService(DataContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }

        public async Task<List<ProcurementDepartmentModel>> GetAllProcurementDepartmentsAsync()
        {
            return await _context.ProcurementDepartments
                .Where(pd => pd.Is_Active)
                .Select(pd => new ProcurementDepartmentModel
                {
                    Id = pd.Id,
                    Title = pd.Title,
                    Is_Active = pd.Is_Active
                })
                .ToListAsync();
        }

        public async Task<PagedResult<ProcurementDepartmentModel>> GetPagedProcDeptAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.ProcurementDepartments.Where(pd => pd.Is_Active);
            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression: !string.IsNullOrEmpty(search) ? pd => EF.Functions.ILike(pd.Title, $"%{search}%") : null,
                selector: pd => new ProcurementDepartmentModel
                {
                    Id = pd.Id,
                    Title = pd.Title,
                    Is_Active = pd.Is_Active
                },
                orderBy: pd => pd.Title
            );
        }
    }
}
