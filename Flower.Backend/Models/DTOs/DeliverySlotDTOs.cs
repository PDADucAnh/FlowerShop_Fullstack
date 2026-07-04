using System;

namespace Flower.Backend.Models.DTOs
{
    public class AvailableSlotDTO
    {
        public int ProductId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public int Available { get; set; }
        public bool IsAvailable => Available > 0;
    }

    public class DistrictDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
