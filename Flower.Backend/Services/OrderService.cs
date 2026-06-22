using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Flower.Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDTO>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => o.ToDTO());
        }

        public async Task<OrderDTO?> GetDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order?.ToDTO();
        }

        public async Task<(bool Success, string Message, int OrderId)> CreateOrder(
            int customerId, string? notes, List<OrderItemInput> items)
        {
            try
            {
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
                if (!customerExists)
                    return (false, "Khách hàng không tồn tại", 0);

                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = customerId,
                    Status = OrderStatus.Pending,
                    Notes = notes
                };

                if (items != null && items.Count > 0)
                {
                    var productIds = items.Select(i => i.ProductId).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();
                    var productDict = products.ToDictionary(p => p.Id);

                    newOrder.OrderDetails = items.Select(item =>
                    {
                        if (!productDict.TryGetValue(item.ProductId, out var product))
                        {
                            throw new KeyNotFoundException($"Sản phẩm không tồn tại");
                        }

                        return new OrderDetail
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price
                        };
                    }).ToList();
                }

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return (true, "Đặt hàng thành công!", newOrder.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating order for customer {CustomerId}", customerId);
                return (false, "Lỗi cơ sở dữ liệu khi tạo đơn hàng", 0);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Product not found for customer {CustomerId}", customerId);
                return (false, ex.Message, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating order for customer {CustomerId}", customerId);
                return (false, "Lỗi không xác định khi tạo đơn hàng", 0);
            }
        }

        public async Task<bool> Update(int id, UpdateOrderDTO dto)
        {
            if (id != dto.Id)
                return false;

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            dto.UpdateEntity(order);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Orders.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
