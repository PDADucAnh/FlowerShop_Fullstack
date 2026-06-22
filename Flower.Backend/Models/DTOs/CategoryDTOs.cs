using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.Collections.Generic.List<PostDTO>? Posts { get; set; }
    }

    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
    }

    public class UpdateCategoryDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
    }
}
