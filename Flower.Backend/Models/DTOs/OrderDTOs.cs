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
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public List<OrderDetailDTO>? OrderDetails { get; set; }
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
    }
}
