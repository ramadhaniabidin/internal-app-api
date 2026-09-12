using API.Data;
using API.Model;
using API.Model.Display;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.Services.ORM
{
    public class UserProcDeptService
    {
        private readonly DataContext _context;

        public UserProcDeptService(DataContext context)
        {
            _context = context;
        }

        private IQueryable<UserProcDeptDisplay> GenerateQueryForGetAll(
            int pageNumber = 1,
            int pageSize = 5,
            string? searchBy = "",
            string? keyword = "")
        {
            var query = from u in _context.UserProcurementDepartments.Where(u => u.Active)
                        join user in _context.Users on u.UserId equals user.Id
                        join proc in _context.ProcurementDepartments on u.ProcurementDepartmentId equals proc.Id
                        join b in _context.Branches on u.BranchId equals b.Id
                        select new UserProcDeptDisplay
                        {
                            Id = u.Id,
                            Active = u.Active,
                            UserAccount = user.Username,
                            BranchCode = b.Code,
                            BranchId = b.Id,
                            BranchName = b.Name,
                            CreatedDate = u.CreatedDate,
                            ProcDeptCode = proc.Code,
                            ProcDeptName = proc.Title,
                            ProcurementDepartmentId = proc.Id,
                            UpdatedDate = u.UpdatedDate,
                            UserEmail = user.Email,
                            UserFullName = user.FullName,
                            UserId = user.Id
                        };

            if (!string.IsNullOrWhiteSpace(searchBy) && !string.IsNullOrWhiteSpace(keyword))
            {
                var columns = searchBy.Split(';',StringSplitOptions.TrimEntries);
                var values = keyword.Split(';', StringSplitOptions.TrimEntries);
                int filterCount = Math.Min(columns.Length, values.Length);

                for (int i = 0; i < filterCount; i++)
                {
                    string col = columns[i].ToLower();
                    string val = values[i];
                    query = col switch
                    {
                        "procurement department" => query.Where(v => EF.Functions.ILike(v.ProcDeptName ?? "", $"%{val}%")),
                        "branch" => query.Where(v => EF.Functions.ILike(v.BranchName ?? "", $"%{val}%")),
                        "email" => query.Where(v => EF.Functions.ILike(v.UserEmail ?? "", $"%{val}%")),
                        _ => query
                    };
                }
            }

            return query;
        }

        public async Task<PagedResult<UserProcDeptDisplay>> GetAll(int pageNumber = 1, int pageSize = 5, string? searchBy = "", string? keyword = "")
        {
            var query = GenerateQueryForGetAll(pageNumber, pageSize, searchBy, keyword);
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<UserProcDeptDisplay>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                SearchTerm = keyword
            };
        }

        public async Task Create(UserProcurementDepartmentsModel model)
        {
            await _context.UserProcurementDepartments.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProcDeptDisplay?> GetExisting(UserProcurementDepartmentsModel model)
        {
            var query = GenerateQueryForGetAll();
            var existing = await query.Where(q => q.BranchId == model.UserId && q.BranchId == model.BranchId && q.ProcurementDepartmentId == model.ProcurementDepartmentId)
                .FirstOrDefaultAsync();
            return existing;
        }
    }
}
