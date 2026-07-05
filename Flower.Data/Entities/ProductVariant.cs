using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,0)")]
        public decimal PriceAdjustment { get; set; }

        public bool IsDefault { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
