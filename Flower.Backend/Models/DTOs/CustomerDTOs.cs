using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int TotalOrders { get; set; }
        public int SuccessfulDeliveries { get; set; }
        public int FailedDeliveries { get; set; }
        public bool IsBlacklisted { get; set; }
        public int FraudScore { get; set; }
    }

    public class CreateCustomerDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; }
    }

    public class UpdateCustomerDTO
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PasswordHash { get; set; }
    }
}
