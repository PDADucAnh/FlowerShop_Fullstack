using Flower.Backend.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IDeliverySlotService
    {
        Task<List<AvailableSlotDTO>> GetAvailableSlots(int productId, int daysAhead = 7);
        Task<bool> TryLockSlot(int productId, DateTime deliveryDate, string timeSlot);
        Task ReleaseSlot(int productId, DateTime deliveryDate, string timeSlot);
        Task<List<AvailableSlotDTO>> GetAvailableSlotsByDistrict(string district, DateTime date);
    }
}
