using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "T�n s?n ph?m kh�ng du?c d? tr?ng")]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryProductId { get; set; }

        [ForeignKey("CategoryProductId")]
        public virtual CategoryProduct? CategoryProduct { get; set; }

        public int ViewCount { get; set; }

        public int AddToCartCount { get; set; }
    }
}
