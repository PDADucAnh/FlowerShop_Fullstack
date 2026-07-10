using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class CancellationPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderStatus { get; set; } = string.Empty;

        public int RefundPercent { get; set; }

        public int CancellationFeePercent { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
