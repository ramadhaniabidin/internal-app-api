using API.Data;
using API.Helpers;
using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Services.ORM
{
    public class ContractTypeService
    {
        private readonly DataContext _context;
        private readonly QueryHelper _queryHelper;
        public ContractTypeService(DataContext context, QueryHelper queryHelper)
        {
            _context = context;
            _queryHelper = queryHelper;
        }
        public async Task<List<ContractTypeModel>> GetAllContractTypes()
        {
            return await _context.ContractTypes.ToListAsync();
        }
        public async Task<ContractTypeModel?> GetContractTypeById(int id)
        {
            return await _context.ContractTypes.FirstOrDefaultAsync(ct => ct.Id == id);
        }

        public async Task CreateContractType (ContractTypeModel contractType)
        {
            _context.ContractTypes.Add(contractType);
            await _context.SaveChangesAsync();
        }

        public async Task<ContractTypeModel?> GetByCode(string code)
        {
            return await _context.ContractTypes.FirstOrDefaultAsync(ct => ct.Code == code);
        }

        public async Task UpdateContractType(ContractTypeModel contractType)
        {
            var existing = await _context.ContractTypes.FirstOrDefaultAsync(ct => ct.Id == contractType.Id);
            if (existing != null)
            {
                existing.Title = contractType.Title;
                existing.Code = contractType.Code;
                existing.Updated_Date = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<ContractTypeModel>> GetContractTypesPaged(int pageNumber, int pageSize, string? search)
        {
            var query = _context.ContractTypes.Where(ct => ct.Is_Active);
            return await _queryHelper.GetPagedResultAsync(
                query,
                pageNumber,
                pageSize,
                searchExpression: !string.IsNullOrEmpty(search) ? ct => EF.Functions.ILike(ct.Title, $"%{search}%") : null,
                selector: ct => new ContractTypeModel
                {
                    Id = ct.Id,
                    Title = ct.Title,
                    Code = ct.Code,
                    Is_Active = ct.Is_Active
                },
                orderBy: ct => ct.Title
            );
        }

        public async Task DeleteContractType(int id)
        {
            var contractType = await _context.ContractTypes.FirstOrDefaultAsync(b => b.Id == id);
            if (contractType != null)
            {
                contractType.Is_Active = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
