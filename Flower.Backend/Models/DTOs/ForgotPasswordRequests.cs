using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải dài tối thiểu 6 ký tự.")]
        public string NewPassword { get; set; }
    }
}
