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

        [MaxLength(50)]
        public string? ReferenceType { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        [MaxLength(20)]
        public string? Priority { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(500)]
        public string? NavigationUrl { get; set; }

        public string? Metadata { get; set; }
    }
}
