namespace LogiTechAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Varsayılan olarak herkes 'User' (Müşteri)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
