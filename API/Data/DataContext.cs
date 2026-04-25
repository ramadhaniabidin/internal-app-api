using API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<BranchModel> Branches => Set<BranchModel>();
        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<ProcurementDepartmentModel> ProcurementDepartments => Set<ProcurementDepartmentModel>();
        public DbSet<ModuleModel> Modules => Set<ModuleModel>();
        public DbSet<StatusModel> Statuses => Set<StatusModel>();
        public DbSet<RoleModel> Roles => Set<RoleModel>();
        public DbSet<ContractTypeModel> ContractTypes => Set<ContractTypeModel>();
    }
}
