using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public int? PaymentMethodId { get; set; }

        [MaxLength(50)]
        public string? Gateway { get; set; }

        [MaxLength(200)]
        public string? TransactionId { get; set; }

        [MaxLength(50)]
        public string? GatewayResponseCode { get; set; }

        [MaxLength(50)]
        public string? BankCode { get; set; }

        [MaxLength(1000)]
        public string? PaymentUrl { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? RefundedAt { get; set; }

        [MaxLength(200)]
        public string? RefundTransactionId { get; set; }

        [MaxLength(50)]
        public string? RefundResponseCode { get; set; }

        [MaxLength(100)]
        public string? RefundedBy { get; set; }

        [MaxLength(500)]
        public string? RefundNote { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethodDefinition? PaymentMethodRef { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
