using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class CustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressLine { get; set; }
        public string? PostalCode { get; set; }
        public string? Note { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerAddressDTO
    {
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string? ReceiverName { get; set; }

        [MaxLength(20)]
        public string? ReceiverPhone { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        [MaxLength(500)]
        public string? AddressLine { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }
}
