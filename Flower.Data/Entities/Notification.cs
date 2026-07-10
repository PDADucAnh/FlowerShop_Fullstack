using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int? OrderId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
