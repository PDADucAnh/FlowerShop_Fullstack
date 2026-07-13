using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class AdminNotification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // Order, Payment, Promotion, Review, System

        [MaxLength(100)]
        public string? ReferenceId { get; set; }

        public int? UserId { get; set; } // Nullable, if null it's for all admins/staff

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }
    }
}
