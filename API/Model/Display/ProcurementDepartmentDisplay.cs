namespace API.Model.Display
{
    public class ProcurementDepartmentDisplay
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Approver_Id { get; set; } = 0;
        public UserDisplay? Approver { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Created_Date { get; set; } = DateTime.UtcNow;
        public bool Is_Active { get; set; } = true;
    }
}
