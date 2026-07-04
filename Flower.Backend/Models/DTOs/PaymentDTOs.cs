using System;
using System.ComponentModel.DataAnnotations;
using Flower.Data.Entities;

namespace Flower.Backend.Models.DTOs
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        public string? Notes { get; set; }

        public string MethodDisplay => Method switch
        {
            PaymentMethod.OnlinePayment => "Chuyển khoản",
            PaymentMethod.COD => "Tiền mặt (COD)",
            _ => "Không xác định"
        };

        public string StatusDisplay => Status switch
        {
            PaymentStatus.Pending => "Chờ thanh toán",
            PaymentStatus.Completed => "Đã thanh toán",
            PaymentStatus.Failed => "Thất bại",
            PaymentStatus.Refunded => "Đã hoàn tiền",
            PaymentStatus.PartialRefund => "Hoàn tiền một phần",
            _ => "Không xác định"
        };
    }

    public class PaymentWebhookRequest
    {
        public string TransactionId { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Signature { get; set; }
    }

    public class VerificationRequest
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string Otp { get; set; } = string.Empty;
    }
}
