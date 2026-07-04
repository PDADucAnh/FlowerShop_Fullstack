using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Flower.Data.Entities;

namespace Flower.Backend.Models.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public List<OrderDetailDTO>? OrderDetails { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? PaymentTransactionId { get; set; }
        public DateTime? PaymentPaidAt { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? DeliveryDistrict { get; set; }
        public string? DeliveryAddress { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public bool IsVerified { get; set; }
        public decimal RefundAmount { get; set; }

        public string StatusDisplay => Status switch
        {
            OrderStatus.Pending => "Chờ xử lý",
            OrderStatus.PendingVerification => "Chờ xác minh",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Preparing => "Đang cắm hoa",
            OrderStatus.Shipping => "Đang giao",
            OrderStatus.Completed => "Đã giao",
            OrderStatus.Cancelled => "Đã hủy",
            _ => "Không xác định"
        };

        public bool CanCancel
        {
            get
            {
                if (Status == OrderStatus.Cancelled || Status == OrderStatus.Completed)
                    return false;
                if (Status == OrderStatus.Preparing || Status == OrderStatus.Shipping)
                    return false;
                return true;
            }
        }

        public TimeSpan? TimeUntilDelivery => DeliveryDate.HasValue
            ? DeliveryDate.Value - DateTime.Now
            : null;
    }

    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public string? CustomerName { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateOrderDTO
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? DeliveryDistrict { get; set; }
        public string? DeliveryAddress { get; set; }
    }

    public class OrderItemInput
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderDTO
    {
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public DateTime? DeliveryDate { get; set; }
        [MaxLength(50)]
        public string? DeliveryTimeSlot { get; set; }
        [MaxLength(100)]
        public string? DeliveryDistrict { get; set; }
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
    }

    public class CancelOrderRequest
    {
        [MaxLength(500)]
        public string? Reason { get; set; }
    }

    public class CheckoutRequest
    {
        [Required]
        public int CustomerId { get; set; }

        public string? Notes { get; set; }

        public List<OrderItemDTO>? Items { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string? DeliveryTimeSlot { get; set; }

        [Required]
        [MaxLength(100)]
        public string? DeliveryDistrict { get; set; }

        [Required]
        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }
    }

    public class DeliverySlotDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int CurrentBooked { get; set; }
        public int Available => MaxCapacity - CurrentBooked;
        public bool IsAvailable => Available > 0;
    }
}
