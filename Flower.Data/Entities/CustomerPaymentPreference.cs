using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class CustomerPaymentPreference
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int PaymentMethodId { get; set; }

        public bool IsDefault { get; set; }

        public DateTime? LastUsedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethodDefinition? PaymentMethod { get; set; }
    }
}
