using API.Model.Master_Data;

namespace API.Model.Display
{
    public class MaterialAnaplanDiplay: MaterialAnaplan
    {
        public string Display { get; set; } = string.Empty;
        public string GeneralLedgerCode { get; set; } = string.Empty;
        public string GeneralLedgerDescription { get; set; } = string.Empty;
        public string Concatenate {  get; set; } = string.Empty;
        public string ProcDeptName {  get; set; } = string.Empty;
        public string ProcDeptCode { get; set; } = string.Empty;
    }
}
