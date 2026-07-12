using Flower.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class OrderInputDTO
    {
        [Required(ErrorMessage = "CustomerId là bắt buộc")]
        public int CustomerId { get; set; }
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Danh sách sản phẩm không được để trống")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 sản phẩm")]
        public List<OrderItemDTO>? Items { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? DeliveryDistrict { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        [MaxLength(50)]
        public string? CouponCode { get; set; }
    }
}
