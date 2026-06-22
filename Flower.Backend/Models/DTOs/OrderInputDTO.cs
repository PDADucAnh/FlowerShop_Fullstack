using System.Collections.Generic;

namespace Flower.Backend.Models.DTOs
{
    public class OrderInputDTO
    {
        public int CustomerId { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDTO>? Items { get; set; }
    }
}
