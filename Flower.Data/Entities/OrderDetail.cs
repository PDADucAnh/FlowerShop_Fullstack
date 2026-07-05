using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal UnitPrice { get; set; }

        [MaxLength(200)]
        public string? ProductName { get; set; }

        [MaxLength(1000)]
        public string? ProductImage { get; set; }

        [MaxLength(50)]
        public string? SizeVariant { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
