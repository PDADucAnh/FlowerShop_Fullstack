using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class PostCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public System.Collections.Generic.List<PostDTO>? Posts { get; set; }
    }

    public class CreatePostCategoryDTO
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }
    }

    public class UpdatePostCategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }
    }
}
