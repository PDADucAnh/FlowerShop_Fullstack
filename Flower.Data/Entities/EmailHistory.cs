using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class EmailHistory
    {
        [Key]
        public int Id { get; set; }

        public int? CustomerId { get; set; }

        public int? OrderId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EmailType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Recipient { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Subject { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateTime? SentAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }
    }
}
