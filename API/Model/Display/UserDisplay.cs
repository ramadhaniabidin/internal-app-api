namespace API.Model.Display
{
    public class UserDisplay
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = "User";
        public string? Division { get; set; }
        public string? Department { get; set; }
        public bool IsActive { get; set; }
    }
}
