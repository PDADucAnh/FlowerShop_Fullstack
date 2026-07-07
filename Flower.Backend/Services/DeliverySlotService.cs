using Flower.Data;
using Flower.Backend.Models;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class DeliverySlotService : IDeliverySlotService
    {
        private readonly IApplicationDbContext _context;
        private readonly TimeSettings _timeSettings;

        private static readonly string[] DefaultTimeSlots =
        {
            "08:00-10:00",
            "10:00-12:00",
            "13:00-15:00",
            "15:00-17:00",
            "17:00-19:00",
            "19:00-21:00"
        };

        public DeliverySlotService(IApplicationDbContext context, TimeSettings timeSettings)
        {
            _context = context;
            _timeSettings = timeSettings;
        }

        public async Task<List<AvailableSlotDTO>> GetAvailableSlots(int productId, int daysAhead = 7)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(daysAhead);
            var slots = await _context.DeliverySlots
                .Where(s => s.ProductId == productId
                    && s.DeliveryDate >= today
                    && s.DeliveryDate <= endDate
                    && s.IsActive)
                .ToListAsync();

            var result = new List<AvailableSlotDTO>();

            for (var date = today; date <= endDate; date = date.AddDays(1))
            {
                foreach (var timeSlot in DefaultTimeSlots)
                {
                    var existingSlot = slots.FirstOrDefault(s =>
                        s.DeliveryDate.Date == date.Date && s.TimeSlot == timeSlot);

                    if (existingSlot != null)
                    {
                        result.Add(new AvailableSlotDTO
                        {
                            ProductId = productId,
                            DeliveryDate = date,
                            TimeSlot = timeSlot,
                            Available = existingSlot.MaxCapacity - existingSlot.CurrentBooked
                        });
                    }
                    else
                    {
                        result.Add(new AvailableSlotDTO
                        {
                            ProductId = productId,
                            DeliveryDate = date,
                            TimeSlot = timeSlot,
                            Available = _timeSettings.MaxOrdersPerSlot
                        });
                    }
                }
            }

            return result;
        }

        public async Task<bool> TryLockSlot(int productId, DateTime deliveryDate, string timeSlot)
        {
            var slot = await _context.DeliverySlots
                .FirstOrDefaultAsync(s =>
                    s.ProductId == productId
                    && s.DeliveryDate.Date == deliveryDate.Date
                    && s.TimeSlot == timeSlot);

            if (slot != null)
            {
                if (slot.CurrentBooked >= slot.MaxCapacity)
                    return false;

                slot.CurrentBooked++;
                return true;
            }

            var newSlot = new Data.Entities.DeliverySlot
            {
                ProductId = productId,
                DeliveryDate = deliveryDate.Date,
                TimeSlot = timeSlot,
                MaxCapacity = _timeSettings.MaxOrdersPerSlot,
                CurrentBooked = 1,
                IsActive = true
            };
            _context.DeliverySlots.Add(newSlot);
            return true;
        }

        public async Task ReleaseSlot(int productId, DateTime deliveryDate, string timeSlot)
        {
            var slot = await _context.DeliverySlots
                .FirstOrDefaultAsync(s =>
                    s.ProductId == productId
                    && s.DeliveryDate.Date == deliveryDate.Date
                    && s.TimeSlot == timeSlot);

            if (slot != null && slot.CurrentBooked > 0)
            {
                slot.CurrentBooked--;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AvailableSlotDTO>> GetAvailableSlotsByDistrict(string district, DateTime date)
        {
            var products = await _context.Products.ToListAsync();
            var result = new List<AvailableSlotDTO>();

            foreach (var product in products)
            {
                var slots = await GetAvailableSlots(product.Id, 7);
                result.AddRange(slots.Where(s => s.DeliveryDate.Date == date.Date));
            }

            return result;
        }
    }
}
