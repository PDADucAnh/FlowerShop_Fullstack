using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CategoryProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateCategoryProductDTO
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
    }

    public class UpdateCategoryProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
    }
}
