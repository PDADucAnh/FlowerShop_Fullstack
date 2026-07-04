namespace Flower.Backend.Models.DTOs
{
    public class LoginResult
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = string.Empty;
        public string AuthType { get; set; } = string.Empty;
    }
}
