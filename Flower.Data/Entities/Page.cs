using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Slug { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
