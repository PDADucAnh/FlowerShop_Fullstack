using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Flower.Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDeliverySlotService _deliverySlotService;
        private readonly IPaymentService _paymentService;
        private readonly IFraudDetectionService _fraudDetectionService;
        private readonly StockLockService _stockLockService;
        private readonly IEmailService _emailService;
        private readonly TimeSettings _timeSettings;
        private readonly IMemoryCache _memoryCache;
        private readonly IOrderCancellationService _orderCancellationService;

        public OrderService(
            IApplicationDbContext context,
            ILogger<OrderService> logger,
            IHttpContextAccessor httpContextAccessor,
            IDeliverySlotService deliverySlotService,
            IPaymentService paymentService,
            IFraudDetectionService fraudDetectionService,
            StockLockService stockLockService,
            IEmailService emailService,
            TimeSettings timeSettings,
            IMemoryCache memoryCache,
            IOrderCancellationService orderCancellationService)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _deliverySlotService = deliverySlotService;
            _paymentService = paymentService;
            _fraudDetectionService = fraudDetectionService;
            _stockLockService = stockLockService;
            _emailService = emailService;
            _timeSettings = timeSettings;
            _memoryCache = memoryCache;
            _orderCancellationService = orderCancellationService;
        }

        private async Task<int?> GetCurrentCustomerId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var authType = httpContext.User.FindFirst("AuthType")?.Value;
            if (authType != "Customer") return null;

            var email = httpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            return customer?.Id;
        }

        private IQueryable<Order> ApplyOwnershipFilter(IQueryable<Order> query)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return query;

            var authType = httpContext.User.FindFirst("AuthType")?.Value;
            if (authType == "Customer")
            {
                var email = httpContext.User.Identity?.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(o => o.Customer != null && o.Customer.Email == email);
                }
            }

            return query;
        }

        public async Task<IEnumerable<OrderDTO>> GetAll()
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate);

            query = ApplyOwnershipFilter(query);

            var orders = await query.ToListAsync();
            return orders.Select(o => o.ToDTO());
        }

        public async Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate);

            query = ApplyOwnershipFilter(query);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<OrderDTO>
            {
                Items = items.Select(o => o.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<OrderDTO?> GetDetail(int id)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.Id == id);

            query = ApplyOwnershipFilter(query);

            var order = await query.FirstOrDefaultAsync();
            return order?.ToDTO();
        }

        public async Task<(bool Success, string Message, int OrderId)> CreateOrder(
            int customerId, string? notes, List<OrderItemInput> items, DateTime? orderDate = null,
            OrderStatus? status = null, PaymentMethod? paymentMethod = null,
            DateTime? deliveryDate = null, string? deliveryTimeSlot = null,
            string? deliveryDistrict = null, string? deliveryAddress = null)
        {
            try
            {
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
                if (!customerExists)
                    return (false, "Khách hàng không tồn tại", 0);

                var customer = await _context.Customers.FindAsync(customerId);

                using var transaction = await _context.Database.BeginTransactionAsync();

                var method = paymentMethod ?? PaymentMethod.COD;
                var initialStatus = status ?? (method == PaymentMethod.COD ? OrderStatus.PendingVerification : OrderStatus.Pending);

                if (method == PaymentMethod.COD)
                {
                    bool isBlacklisted = false;
                    if (customer != null && !string.IsNullOrEmpty(customer.Phone))
                    {
                        isBlacklisted = await _fraudDetectionService.IsPhoneBlacklisted(customer.Phone);
                    }

                    if (!isBlacklisted && !string.IsNullOrEmpty(notes))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(notes, @"SĐT:\s*([0-9]+)");
                        if (match.Success)
                        {
                            isBlacklisted = await _fraudDetectionService.IsPhoneBlacklisted(match.Groups[1].Value);
                        }
                    }

                    if (isBlacklisted)
                        return (false, "Số điện thoại này đã bị chặn thanh toán COD do lịch sử bùng đơn hàng. Vui lòng thanh toán online.", 0);
                }

                var vnNow = DateTimeUtils.GetVietnamTime();
                if (deliveryDate.HasValue)
                {
                    if (deliveryDate.Value.Date < vnNow.Date)
                    {
                        return (false, "Ngày giao hàng không hợp lệ", 0);
                    }

                    if (deliveryDate.Value.Date == vnNow.Date)
                    {
                        if (!string.IsNullOrEmpty(deliveryTimeSlot))
                        {
                            var parts = deliveryTimeSlot.Split('-');
                            if (parts.Length > 0 && TimeSpan.TryParse(parts[0], out var slotStartTime))
                            {
                                var minAllowedTime = vnNow.TimeOfDay.Add(TimeSpan.FromHours(_timeSettings.LeadTimeHours));
                                if (slotStartTime < minAllowedTime)
                                {
                                    return (false, "Khung giờ chọn không khả dụng, vui lòng chọn khung giờ khác.", 0);
                                }
                            }
                        }
                    }
                }

                if (deliveryDate.HasValue && !string.IsNullOrEmpty(deliveryTimeSlot))
                {
                    foreach (var item in items ?? new List<OrderItemInput>())
                    {
                        var locked = await _deliverySlotService.TryLockSlot(item.ProductId, deliveryDate.Value, deliveryTimeSlot);
                        if (!locked)
                            return (false, "Khung giờ này đã bận, vui lòng chọn khung giờ khác.", 0);
                    }
                }

                DateTime? targetFinishedTime = null;
                if (deliveryDate.HasValue && !string.IsNullOrEmpty(deliveryTimeSlot))
                {
                    var parts = deliveryTimeSlot.Split('-');
                    if (parts.Length > 0 && TimeSpan.TryParse(parts[0], out var slotStartTime))
                    {
                        targetFinishedTime = deliveryDate.Value.Date.Add(slotStartTime).AddMinutes(-_timeSettings.PreShippingMinutes);
                    }
                }

                var newOrder = new Order
                {
                    OrderDate = orderDate ?? DateTime.Now,
                    CustomerId = customerId,
                    Status = initialStatus,
                    Notes = notes,
                    PaymentMethod = method,
                    PaymentStatus = PaymentStatus.Pending,
                    DeliveryDate = deliveryDate,
                    DeliveryTimeSlot = deliveryTimeSlot,
                    DeliveryDistrict = deliveryDistrict,
                    DeliveryAddress = deliveryAddress,
                    TargetFinishedTime = targetFinishedTime
                };

                Dictionary<int, Product> productDict = new Dictionary<int, Product>();
                if (items != null && items.Count > 0)
                {
                    var productIds = items.Select(i => i.ProductId).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();
                    productDict = products.ToDictionary(p => p.Id);

                    foreach (var item in items)
                    {
                        if (!productDict.TryGetValue(item.ProductId, out var product))
                            throw new KeyNotFoundException($"Sản phẩm không tồn tại");

                        int reserved = _stockLockService.GetReservedStock(item.ProductId);
                        int availableStock = product.StockQuantity - reserved;
                        if (availableStock < item.Quantity)
                            return (false, $"Sản phẩm '{product.Name}' không đủ hàng (còn: {availableStock}, yêu cầu: {item.Quantity})", 0);
                    }

                    newOrder.OrderDetails = items.Select(item =>
                {
                    return new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                }).ToList();

                    if (method == PaymentMethod.OnlinePayment)
                    {
                        foreach (var item in items)
                        {
                            _stockLockService.ReserveStock(item.ProductId, item.Quantity, TimeSpan.FromMinutes(15));
                        }
                    }
                }

            if (method == PaymentMethod.COD && items != null)
            {
                foreach (var item in items)
                {
                    var affected = await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Products SET StockQuantity = StockQuantity - {0} WHERE Id = {1} AND StockQuantity >= {0}",
                        item.Quantity, item.ProductId);
                    if (affected == 0)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Sản phẩm '{productDict[item.ProductId].Name}' vừa hết hàng, vui lòng thử lại.", 0);
                    }
                }
            }

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            if (method == PaymentMethod.COD && customer != null)
            {
                var requiresVerification = await _fraudDetectionService.RequiresVerification(customer);
                if (!requiresVerification)
                {
                    newOrder.Status = OrderStatus.Confirmed;
                    newOrder.IsVerified = true;
                    newOrder.VerifiedAt = DateTime.Now;
                    customer.TotalOrders++;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var otp = new Random().Next(100000, 999999).ToString();
                    _memoryCache.Set("otp_" + newOrder.Id, otp, TimeSpan.FromMinutes(10));

                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var emailService = _emailService;
                                await emailService.SendOtpEmailAsync(customer.Email, customer.FullName, otp);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to send OTP email for order {OrderId}", newOrder.Id);
                            }
                        });
                    }
                }
            }

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
            if (id != dto.Id) return false;

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            var oldStatus = order.Status;
            dto.UpdateEntity(order);

            try
            {
                await _context.SaveChangesAsync();

                var statusChangedToConfirmed = oldStatus != OrderStatus.Confirmed && order.Status == OrderStatus.Confirmed;
                var statusChangedToCompleted = oldStatus != OrderStatus.Completed && order.Status == OrderStatus.Completed;
                var statusChangedToShipping = oldStatus != OrderStatus.Shipping && order.Status == OrderStatus.Shipping;

                if (statusChangedToConfirmed || statusChangedToCompleted || statusChangedToShipping)
                {
                    await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
                    await _context.Entry(order).Collection(o => o.OrderDetails).LoadAsync();
                    if (order.OrderDetails != null)
                    {
                        foreach (var detail in order.OrderDetails)
                        {
                            await _context.Entry(detail).Reference(d => d.Product).LoadAsync();
                        }
                    }

                    if (order.Customer != null && !string.IsNullOrEmpty(order.Customer.Email))
                    {
                        if (statusChangedToConfirmed)
                        {
                            await _emailService.SendOrderConfirmedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                        }
                        else if (statusChangedToShipping)
                        {
                            await _emailService.SendOrderShippingEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                        }
                        else if (statusChangedToCompleted)
                        {
                            await _emailService.SendOrderCompletedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                        }
                    }
                }

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
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Cancel(int id)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Id == id);

            query = ApplyOwnershipFilter(query);

            var order = await query.FirstOrDefaultAsync();
            if (order == null) return false;

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.PendingVerification && order.Status != OrderStatus.Confirmed)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.Now;

            if (order.OrderDetails != null)
            {
                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                    if (product != null)
                        product.StockQuantity += detail.Quantity;

                    if (!string.IsNullOrEmpty(order.DeliveryTimeSlot) && order.DeliveryDate.HasValue)
                        await _deliverySlotService.ReleaseSlot(detail.ProductId, order.DeliveryDate.Value, order.DeliveryTimeSlot);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelWithReason(int id, string? reason)
        {
            return await _orderCancellationService.CancelWithReason(id, reason);
        }

        public async Task<(bool Success, string Message)> CancelWithPolicy(int id, string? reason = null)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .Where(o => o.Id == id);

            query = ApplyOwnershipFilter(query);

            var order = await query.FirstOrDefaultAsync();
            if (order == null)
                return (false, "Đơn hàng không tồn tại");

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Completed)
                return (false, "Đơn hàng đã được xử lý trước đó");

            if (order.Status == OrderStatus.Preparing || order.Status == OrderStatus.Shipping)
                return (false, "Đơn hàng đang trong quá trình sản xuất/giao hàng, không thể hủy");

            var delta = order.DeliveryDate.HasValue
                ? (order.DeliveryDate.Value - DateTime.Now).TotalHours
                : 999;

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.PendingVerification && delta <= 4)
                return (false, "Đơn hàng cách thời gian giao dưới 4 giờ, không thể hủy. Vui lòng liên hệ hotline để được hỗ trợ.");

            decimal refundPercent;
            string message;

            if (delta > 24)
            {
                refundPercent = 1.0m;
                message = "Hủy đơn thành công. Tiền sẽ được hoàn lại 100%.";
            }
            else
            {
                refundPercent = 0.5m;
                message = "Hủy đơn thành công. Phí hủy 50% giá trị đơn hàng sẽ được khấu trừ do nguyên liệu hoa tươi đã được chuẩn bị.";
            }

            var totalAmount = order.OrderDetails?.Sum(od => od.Quantity * od.UnitPrice) ?? 0;
            var refundAmount = totalAmount * refundPercent;

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.Now;
            order.CancellationReason = reason ?? "Hủy theo yêu cầu";
            order.RefundAmount = refundAmount;

            bool wasDeducted = order.PaymentMethod == PaymentMethod.COD 
                || (order.PaymentMethod == PaymentMethod.OnlinePayment && order.PaymentStatus == PaymentStatus.Completed);

            if (order.OrderDetails != null)
            {
                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    if (wasDeducted)
                    {
                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                        if (product != null)
                            product.StockQuantity += detail.Quantity;
                    }

                    if (!string.IsNullOrEmpty(order.DeliveryTimeSlot) && order.DeliveryDate.HasValue)
                        await _deliverySlotService.ReleaseSlot(detail.ProductId, order.DeliveryDate.Value, order.DeliveryTimeSlot);

                    _stockLockService.ReleaseReservedStock(detail.ProductId, detail.Quantity);
                }
            }

            if (order.PaymentStatus == PaymentStatus.Completed && refundAmount > 0)
            {
                await _paymentService.RefundPayment(id, refundAmount);
                message += $" Số tiền hoàn: {refundAmount:N0} VND.";
            }

            await _context.SaveChangesAsync();
            return (true, message);
        }

        public async Task<(bool Success, string Message)> ProcessCODOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "Đơn hàng không tồn tại");

            if (order.PaymentMethod != PaymentMethod.COD)
                return (false, "Đơn hàng không phải COD");

            if (order.Status != OrderStatus.PendingVerification)
                return (false, "Đơn hàng không ở trạng thái chờ xác minh");

            if (order.Customer != null)
            {
                var requiresVerification = await _fraudDetectionService.RequiresVerification(order.Customer);

                order.Status = OrderStatus.Confirmed;
                order.IsVerified = true;
                order.VerifiedAt = DateTime.Now;
                order.Customer.TotalOrders++;
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(order.Customer.Email))
                {
                    await _emailService.SendOrderConfirmedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                }

                if (!requiresVerification)
                {
                    return (true, "Đơn hàng đã được xác nhận tự động");
                }

                return (true, "Đơn hàng đã được xác nhận qua xác minh thủ công");
            }

            return (false, "Không tìm thấy thông tin khách hàng");
        }

        public async Task<bool> AutoCancelUnverifiedOrders(int timeoutMinutes = 30)
        {
            var cutoff = DateTime.Now.AddMinutes(-timeoutMinutes);

            var expiredOrders = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Status == OrderStatus.PendingVerification
                    && o.OrderDate <= cutoff)
                .ToListAsync();

            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Cancelled;
                order.CancelledAt = DateTime.Now;
                order.CancellationReason = "Tự động hủy do quá thời gian xác minh";

                if (order.OrderDetails != null)
                {
                    var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();

                    foreach (var detail in order.OrderDetails)
                    {
                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                        if (product != null)
                            product.StockQuantity += detail.Quantity;

                        if (!string.IsNullOrEmpty(order.DeliveryTimeSlot) && order.DeliveryDate.HasValue)
                            await _deliverySlotService.ReleaseSlot(detail.ProductId, order.DeliveryDate.Value, order.DeliveryTimeSlot);
                    }
                }
            }

            if (expiredOrders.Any())
                await _context.SaveChangesAsync();

            return expiredOrders.Any();
        }

        public async Task<bool> IsPhoneBlacklisted(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            return await _fraudDetectionService.IsPhoneBlacklisted(phone);
        }
    }
}
