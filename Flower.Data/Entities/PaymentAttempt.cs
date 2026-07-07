using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class PaymentAttempt
    {
        [Key]
        public int Id { get; set; }

        public int PaymentId { get; set; }

        public int AttemptNumber { get; set; }

        public string? GatewayRequest { get; set; }

        public string? GatewayResponse { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
    }
}
