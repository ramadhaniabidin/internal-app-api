namespace API.Model
{
    public class ContractTypeModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime Created_Date { get; set; } = DateTime.UtcNow;
        public DateTime Updated_Date { get; set; }
        public bool Is_Active { get; set; } = true;
    }
}
