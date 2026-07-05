using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public enum OrderStatus
    {
        Pending = 0,
        Shipping = 1,
        Completed = 2,
        Cancelled = 3,
        PendingVerification = 4,
        Confirmed = 5,
        Preparing = 6
    }

    public enum PaymentMethod
    {
        OnlinePayment = 0,
        COD = 1
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3,
        PartialRefund = 4
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public int CustomerId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Notes { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [MaxLength(200)]
        public string? PaymentTransactionId { get; set; }

        public DateTime? PaymentPaidAt { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(50)]
        public string? DeliveryTimeSlot { get; set; }

        [MaxLength(100)]
        public string? DeliveryDistrict { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(200)]
        public string? RecipientName { get; set; }

        [MaxLength(20)]
        public string? RecipientPhone { get; set; }

        public DateTime? CancelledAt { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public bool IsVerified { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public decimal RefundAmount { get; set; }

        public DateTime? TargetFinishedTime { get; set; }
    }
}
