using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class FlashSale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<FlashSaleProduct>? FlashSaleProducts { get; set; }
    }
}
