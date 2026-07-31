using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Sku { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateProductVariantDTO
    {
        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }

    public class UpdateProductVariantDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên size không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        public bool IsDefault { get; set; }
    }
}
