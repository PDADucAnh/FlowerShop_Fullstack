using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Refund
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int? PaymentId { get; set; }

        [MaxLength(100)]
        public string? RequestedBy { get; set; }

        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [MaxLength(50)]
        public string? RefundType { get; set; }

        public int RefundPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }

        public int RefundStatus { get; set; }

        [MaxLength(200)]
        public string? GatewayRefundId { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
    }
}
