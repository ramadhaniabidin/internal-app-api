using API.Data;
using API.Helpers;
using API.Model;
using API.Model.Display;
using API.Model.Master_Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        private IQueryable<ProcurementDepartmentDisplay> GenerateQueryForAll(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = from proc in _context.ProcurementDepartments.Where(proc => proc.Is_Active)
                        join u in _context.Users
                        on proc.Approver_Id equals u.Id
                        orderby proc.Title
                        select new ProcurementDepartmentDisplay
                        {
                            Id = proc.Id,
                            Category = proc.Category,
                            Is_Active = proc.Is_Active,
                            Code = proc.Code,
                            Title = proc.Title,
                            Created_Date = proc.Created_Date,
                            Approver_Id = u.Id,
                            Approver = new UserDisplay
                            {
                                IsActive = u.IsActive,
                                Department = u.Department ?? "",
                                Division = u.Division ?? "",
                                Email = u.Email ?? "",
                                FullName = u.FullName ?? "",
                                Id = u.Id,
                                Role = u.Role ?? "",
                                Username = u.Username ?? ""
                            }
                        };
            Expression<Func<ProcurementDepartmentDisplay, bool>>? searchExpression = null;

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchColumn = searchBy ?? "";
                searchExpression = searchColumn.ToLower() switch
                {
                    "code" => gl => EF.Functions.ILike(gl.Code ?? "", $"%{keyword}%"),
                    "name" => gl => EF.Functions.ILike(gl.Title ?? "", $"%{keyword}%"),
                    _ => gl => EF.Functions.ILike(gl.Title ?? "", $"%{keyword}%")
                };
            }

            if (searchExpression != null)
            {
                query = query.Where(searchExpression);
            }
            return query;
        }

        public async Task<List<ProcurementDepartmentModel>> GetAll()
        {
            return await _context.ProcurementDepartments
                .Where(pd => pd.Is_Active)
                .Select(pd => new ProcurementDepartmentModel
                {
                    Id = pd.Id,
                    Title = pd.Title,
                    Is_Active = pd.Is_Active,
                    Approver_Id = pd.Approver_Id,
                })
                .ToListAsync();
        }

        public async Task<PagedResult<ProcurementDepartmentDisplay>> GetPagedData(int pageNumber = 1, int pageSize = 10, string? searchBy = "", string? keyword = "")
        {
            var query = GenerateQueryForAll(pageNumber, pageSize, searchBy, keyword);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProcurementDepartmentDisplay>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages <= 0 ? 1 : totalPages,
                SearchTerm = ""
            };
        }

        public async Task<ProcurementDepartmentModel?> GetById(int id)
        {
            return await _context.ProcurementDepartments.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProcurementDepartmentModel?> GetByCode(string code)
        {
            return await _context.ProcurementDepartments.FirstOrDefaultAsync(ct => ct.Code == code);
        }

        public async Task Create(ProcurementDepartmentModel procDept)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(procDept.Approver_Name));
            procDept.Approver_Name = user == null ? "" : user.FullName;
            procDept.Approver_Id = user == null ? 0 : user.Id;
            await _context.ProcurementDepartments.AddAsync(procDept);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProcurementDepartmentModel procDept)
        {
            var existing = await GetById(procDept.Id);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == Convert.ToInt32(procDept.Approver_Name));
            if (existing != null)
            {
                existing.Title = procDept.Title;
                existing.Code = procDept.Code;
                existing.Approver_Name = user == null ? "" : user.FullName;
                existing.Approver_Email = user == null ? "" : user.Email;
                existing.Approver_Id = user == null ? 0 : user.Id;
                existing.Approver_Account = user == null ? "" : user.Username;
                existing.Category = procDept.Category;
                existing.Updated_Date = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var contractType = await _context.ProcurementDepartments.FirstOrDefaultAsync(b => b.Id == id);
            if (contractType != null)
            {
                contractType.Is_Active = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
