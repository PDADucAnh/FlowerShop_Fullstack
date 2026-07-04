using Flower.Data.Entities;
using System;
using System.Collections.Generic;

namespace Flower.Backend.Models.DTOs
{
    public class OrderInputDTO
    {
        public int CustomerId { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDTO>? Items { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryTimeSlot { get; set; }
        public string? DeliveryDistrict { get; set; }
        public string? DeliveryAddress { get; set; }
    }
}
