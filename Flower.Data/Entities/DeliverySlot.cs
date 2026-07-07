using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class DeliverySlot
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public DateTime DeliveryDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string TimeSlot { get; set; }

        public int MaxCapacity { get; set; }

        public int CurrentBooked { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
