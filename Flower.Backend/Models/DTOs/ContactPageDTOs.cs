using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class PageDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePageDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class UpdatePageDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = string.Empty;
        public string? Slug { get; set; }
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class ContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateContactDTO
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Subject { get; set; } = string.Empty;
        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Message { get; set; } = string.Empty;
    }

    public class MarkReadContactDTO
    {
        public bool IsRead { get; set; }
    }
}
