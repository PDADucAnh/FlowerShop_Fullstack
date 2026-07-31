using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string Content { get; set; }

        [MaxLength(500)]
        public string? Summary { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int PostCategoryId { get; set; }

        [ForeignKey("PostCategoryId")]
        public virtual PostCategory PostCategory { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
