using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class PhoneBlacklist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
