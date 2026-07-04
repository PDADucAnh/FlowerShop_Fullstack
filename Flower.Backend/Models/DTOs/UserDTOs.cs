using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; }
    }

    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(200)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vai trò không được để trống")]
        public string Role { get; set; }
    }

    public class UpdateUserDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        public string? Password { get; set; }
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
