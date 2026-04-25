using API.Model;

namespace API.Services.ORM
{
    public class NonCommercialService
    {
        private readonly BranchService branchService;
        private readonly StatusService statusService;
        private readonly ProcurementDepartmentService procurementDepartmentService;
        private readonly ModuleService moduleService;
        private readonly int moduleCategoryId = 4;

        public NonCommercialService(BranchService branchService, StatusService statusService, ProcurementDepartmentService procurementDepartmentService, ModuleService moduleService)
        {
            this.branchService = branchService;
            this.statusService = statusService;
            this.procurementDepartmentService = procurementDepartmentService;
            this.moduleService = moduleService;
        }

        public async Task<NonCommercialListModel> GetNonCommercialDataAsync()
        {
            var branches = await branchService.GetAllBranchesAsync();
            var statuses = await statusService.GetAllStatusesAsync();
            var procurementDepartments = await procurementDepartmentService.GetAllProcurementDepartmentsAsync();
            var modules = await moduleService.GetModulesByCategoryAsync(moduleCategoryId);
            return new NonCommercialListModel
            {
                Branches = branches,
                Statuses = statuses,
                ProcurementDepartments = procurementDepartments,
                Modules = modules
            };
        }

    }
}
