using System.Net.NetworkInformation;
using System.Reflection;

namespace API.Model
{
    public class NonCommercialListModel
    {
        public List<BranchModel> Branches { get; set; } = new();
        public List<ProcurementDepartmentModel> ProcurementDepartments { get; set; } = new();
        public List<ModuleModel> Modules { get; set; } = new();
        public List<StatusModel> Statuses { get; set; } = new();
    }
}
