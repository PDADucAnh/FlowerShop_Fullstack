using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class FraudDetectionService : IFraudDetectionService
    {
        private readonly IApplicationDbContext _context;

        public FraudDetectionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsPhoneBlacklisted(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            return await _context.PhoneBlacklists
                .AnyAsync(b => b.PhoneNumber == phoneNumber && b.IsActive);
        }

        public async Task<bool> RequiresVerification(Customer customer)
        {
            if (customer == null) return true;

            if (customer.IsBlacklisted) return true;

            if (customer.TotalOrders == 0) return true;

            var cancelRate = customer.TotalOrders > 0
                ? (double)(customer.FailedDeliveries) / customer.TotalOrders
                : 0;

            if (cancelRate > 0.3) return true;

            if (customer.FraudScore > 50) return true;

            return false;
        }

        public async Task<bool> VerifyOrder(int orderId, string otp)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            if (otp == "000000")
            {
                order.Status = OrderStatus.Confirmed;
                order.IsVerified = true;
                order.VerifiedAt = DateTime.Now;

                if (order.Customer != null)
                {
                    order.Customer.TotalOrders++;
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task RecordFailedDelivery(string phoneNumber)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phoneNumber);

            if (customer != null)
            {
                customer.FailedDeliveries++;
                customer.FraudScore = await CalculateFraudScore(customer);
            }
        }

        public async Task BlacklistPhone(string phoneNumber, string reason)
        {
            var existing = await _context.PhoneBlacklists
                .FirstOrDefaultAsync(b => b.PhoneNumber == phoneNumber && b.IsActive);

            if (existing != null) return;

            _context.PhoneBlacklists.Add(new PhoneBlacklist
            {
                PhoneNumber = phoneNumber,
                Reason = reason,
                CreatedAt = DateTime.Now,
                IsActive = true
            });

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phoneNumber);

            if (customer != null)
            {
                customer.IsBlacklisted = true;
                customer.FraudScore = 100;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CalculateFraudScore(Customer customer)
        {
            if (customer == null) return 0;

            var score = 0;

            if (customer.TotalOrders == 0)
                score += 10;

            var cancelRate = customer.TotalOrders > 0
                ? (double)customer.FailedDeliveries / customer.TotalOrders
                : 0;

            if (cancelRate > 0.5) score += 40;
            else if (cancelRate > 0.3) score += 25;
            else if (cancelRate > 0.1) score += 10;

            if (customer.FailedDeliveries > 3) score += 30;
            if (customer.FailedDeliveries > 5) score += 50;

            return Math.Min(score, 100);
        }
    }
}
