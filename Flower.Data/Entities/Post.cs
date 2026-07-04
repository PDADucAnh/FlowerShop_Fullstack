using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu d? không du?c d? tr?ng")]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required(ErrorMessage = "N?i dung không du?c d? tr?ng")]
        public string Content { get; set; }

        [MaxLength(500)]
        public string? Summary { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
