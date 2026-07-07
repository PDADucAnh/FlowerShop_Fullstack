using System;
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class AdvertisementDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAdvertisementDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        public string? ImageUrl { get; set; }

        public string? LinkUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateAdvertisementDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        public string? Subtitle { get; set; }

        public string? ImageUrl { get; set; }

        public string? LinkUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
