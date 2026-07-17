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
        Preparing = 6,
        PendingPayment = 7,
        Paid = 8,
        ReadyForDelivery = 9,
        Refunded = 10,
        CancelledByCustomer = 11,
        CancelledByShop = 12,
        RefundPending = 13
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
        PartialRefund = 4,
        Expired = 5,
        Cancelled = 6,
        RefundPending = 7,
        PartialRefundPending = 8,
        PartialRefunded = 9
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

        public int? DeliverySlotId { get; set; }

        [MaxLength(100)]
        public string? DeliveryDistrict { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(200)]
        public string? RecipientName { get; set; }

        [MaxLength(20)]
        public string? RecipientPhone { get; set; }

        [MaxLength(200)]
        public string? DeliveryReceiverName { get; set; }

        [MaxLength(20)]
        public string? DeliveryReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? DeliveryProvince { get; set; }

        [MaxLength(100)]
        public string? DeliveryWard { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddressLine { get; set; }

        [MaxLength(20)]
        public string? DeliveryPostalCode { get; set; }

        public DateTime? CancelledAt { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public bool IsVerified { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public decimal RefundAmount { get; set; }

        [MaxLength(50)]
        public string? CancelledBy { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CancellationFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        public DateTime? RefundRequestedAt { get; set; }

        public DateTime? RefundCompletedAt { get; set; }

        public DateTime? TargetFinishedTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? PromotionId { get; set; }

        public int? CouponId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        [ForeignKey("PromotionId")]
        public virtual PromotionCampaign? Promotion { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon? Coupon { get; set; }
    }
}
