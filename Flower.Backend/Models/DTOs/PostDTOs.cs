using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Slug { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    public class CreatePostDTO
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = string.Empty;

        public string? Summary { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }
    }

    public class UpdatePostDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; } = string.Empty;

        public string? Summary { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }
    }
}
