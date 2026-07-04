using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderDetailService(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private IQueryable<OrderDetail> ApplyOwnershipFilter(IQueryable<OrderDetail> query)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return query;

            var authType = httpContext.User.FindFirst("AuthType")?.Value;
            if (authType == "Customer")
            {
                var email = httpContext.User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(od => od.Order != null && od.Order.Customer != null && od.Order.Customer.Email == email);
                }
            }

            return query;
        }

        public async Task<IEnumerable<OrderDetailDTO>> GetAll()
        {
            IQueryable<OrderDetail> query = _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                    .ThenInclude(p => p.CategoryProduct);

            query = ApplyOwnershipFilter(query);

            var list = await query.ToListAsync();
            return list.Select(od => od.ToDTO());
        }

        public async Task<PagedResult<OrderDetailDTO>> GetPaged(int page, int pageSize)
        {
            IQueryable<OrderDetail> query = _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                    .ThenInclude(p => p.CategoryProduct)
                .OrderByDescending(od => od.Id);

            query = ApplyOwnershipFilter(query);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<OrderDetailDTO>
            {
                Items = items.Select(od => od.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDetailDTO?> GetById(int id)
        {
            IQueryable<OrderDetail> query = _context.OrderDetails
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                    .ThenInclude(p => p.CategoryProduct)
                .Where(od => od.Id == id);

            query = ApplyOwnershipFilter(query);

            var detail = await query.FirstOrDefaultAsync();
            return detail?.ToDTO();
        }

        public async Task<IEnumerable<OrderDetailDTO>> GetByOrderId(int orderId)
        {
            IQueryable<OrderDetail> query = _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)
                .Include(od => od.Product)
                    .ThenInclude(p => p.CategoryProduct);

            query = ApplyOwnershipFilter(query);

            var list = await query.ToListAsync();
            return list.Select(od => od.ToDTO());
        }

        public async Task<OrderDetailDTO> Create(OrderDetailDTO dto)
        {
            var orderExists = await _context.Orders.AnyAsync(o => o.Id == dto.OrderId);
            if (!orderExists)
                throw new KeyNotFoundException($"OrderId {dto.OrderId} không tồn tại");

            var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                throw new KeyNotFoundException($"ProductId {dto.ProductId} không tồn tại");

            var orderDetail = new OrderDetail
            {
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail.ToDTO();
        }

        public async Task<bool> Update(int id, OrderDetailDTO dto)
        {
            if (id != dto.Id)
                return false;

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
                return false;

            orderDetail.ProductId = dto.ProductId;
            orderDetail.Quantity = dto.Quantity;
            orderDetail.UnitPrice = dto.UnitPrice;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.OrderDetails.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
                return false;

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
