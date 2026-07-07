using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Required]
        [Column("Password")]
        public string PasswordHash { get; set; }

        public int? DefaultAddressId { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

        public int TotalOrders { get; set; }

        public int SuccessfulDeliveries { get; set; }

        public int FailedDeliveries { get; set; }

        public bool IsBlacklisted { get; set; }

        public int FraudScore { get; set; }

        [MaxLength(100)]
        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public bool EmailVerified { get; set; }

        public bool PhoneVerified { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
