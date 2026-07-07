using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        [MaxLength(2000)]
        public string? ImageUrl { get; set; }

        [MaxLength(1000)]
        public string? LinkUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
