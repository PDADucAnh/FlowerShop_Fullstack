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
        private readonly IPromotionService _promotionService;
        private readonly ICouponService _couponService;
        private readonly IAdminNotificationService _adminNotificationService;

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
            IOrderCancellationService orderCancellationService,
            IPromotionService promotionService,
            ICouponService couponService,
            IAdminNotificationService adminNotificationService)
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
            _promotionService = promotionService;
            _couponService = couponService;
            _adminNotificationService = adminNotificationService;
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
                .Include(o => o.Promotion)
                .Include(o => o.Coupon)
                .OrderByDescending(o => o.OrderDate);

            query = ApplyOwnershipFilter(query);

            var orders = await query.ToListAsync();
            return orders.Select(o => o.ToDTO()).ToList();
        }

        public async Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize)
        {
            IQueryable<Order> query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Promotion)
                .Include(o => o.Coupon)
                .OrderByDescending(o => o.OrderDate);

            query = ApplyOwnershipFilter(query);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(o => o.ToDTO()).ToList();
            return new PagedResult<OrderDTO>
            {
                Items = dtos,
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
                .Include(o => o.Promotion)
                .Include(o => o.Coupon)
                .Where(o => o.Id == id);

            query = ApplyOwnershipFilter(query);

            var order = await query.FirstOrDefaultAsync();
            if (order == null) return null;
            return order.ToDTO();
        }

        public async Task<(bool Success, string Message, int OrderId)> CreateOrder(
            int customerId, string? notes, List<OrderItemInput> items, DateTime? orderDate = null,
            OrderStatus? status = null, PaymentMethod? paymentMethod = null,
            DateTime? deliveryDate = null, string? deliveryTimeSlot = null,
            string? deliveryDistrict = null, string? deliveryAddress = null,
            string? recipientName = null, string? recipientPhone = null,
            string? couponCode = null)
        {
            if (items == null || items.Count == 0)
                return (false, "Giỏ hàng trống, vui lòng chọn sản phẩm", 0);

            try
            {
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
                if (!customerExists)
                    return (false, "Khách hàng không tồn tại", 0);

                var customer = await _context.Customers.FindAsync(customerId);

                using var transaction = await _context.Database.BeginTransactionAsync();

                var method = paymentMethod ?? PaymentMethod.COD;
                var initialStatus = status ?? (method == PaymentMethod.COD ? OrderStatus.PendingVerification : OrderStatus.PendingPayment);

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

                var utcNow = DateTime.UtcNow;
                if (deliveryDate.HasValue)
                {
                    if (deliveryDate.Value.Date < utcNow.Date)
                    {
                        return (false, "Ngày giao hàng không hợp lệ", 0);
                    }

                    if (deliveryDate.Value.Date == utcNow.Date)
                    {
                        if (!string.IsNullOrEmpty(deliveryTimeSlot))
                        {
                            var parts = deliveryTimeSlot.Split('-');
                            if (parts.Length > 0 && TimeSpan.TryParse(parts[0], out var slotStartTime))
                            {
                                var minAllowedTime = utcNow.TimeOfDay.Add(TimeSpan.FromHours(_timeSettings.LeadTimeHours));
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

                // Load products from DB for validation and mapping
                var productIds = items?.Select(i => i.ProductId).Distinct().ToList() ?? new List<int>();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                // Fetch active promotions (respecting Vietnam Time)
                var activePromotions = await _promotionService.GetActivePromotions();

                decimal originalAmount = 0;
                decimal promotionDiscount = 0;
                int? appliedPromotionId = null;
                var itemDiscounts = new Dictionary<int, decimal>();

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if (!products.TryGetValue(item.ProductId, out var product))
                        {
                            return (false, "Một hoặc nhiều sản phẩm trong giỏ hàng không tồn tại", 0);
                        }

                        // Determine size adjustment based on SizeVariant
                        decimal sizeAdjustment = 0;
                        if (item.SizeVariant == "Deluxe") sizeAdjustment = 300000;
                        else if (item.SizeVariant == "Grand") sizeAdjustment = 600000;

                        var latestOriginalPrice = product.Price + sizeAdjustment;
                        originalAmount += latestOriginalPrice * item.Quantity;

                        // Find the best active promotion for this product
                        var bestPromo = activePromotions
                            .Where(p => p.ProductIds.Contains(item.ProductId))
                            .OrderByDescending(p => p.Priority)
                            .FirstOrDefault();

                        decimal itemDiscount = 0;
                        if (bestPromo != null)
                        {
                            itemDiscount = bestPromo.DiscountType == DiscountType.Percent
                                ? product.Price * bestPromo.DiscountValue / 100m
                                : bestPromo.DiscountValue;

                            if (appliedPromotionId == null)
                                appliedPromotionId = bestPromo.PromotionId;
                        }

                        itemDiscounts[item.ProductId] = itemDiscount;
                        promotionDiscount += itemDiscount * item.Quantity;

                        // Validation: If client-sent unit price differs from backend recalculated price, reject order!
                        var latestCurrentPrice = latestOriginalPrice - itemDiscount;
                        if (item.UnitPrice != latestCurrentPrice)
                        {
                            return (false, "Một hoặc nhiều sản phẩm đã thay đổi giá. Vui lòng kiểm tra lại giỏ hàng.", 0);
                        }
                    }
                }

                // Handle coupon
                decimal couponDiscount = 0;
                int? appliedCouponId = null;

                if (!string.IsNullOrEmpty(couponCode))
                {
                    var applyRequest = new ApplyCouponRequest
                    {
                        Code = couponCode,
                        CustomerId = customerId,
                        OrderTotal = originalAmount - promotionDiscount
                    };
                    var couponValidation = await _couponService.ValidateAndApply(applyRequest);
                    if (couponValidation.IsValid && couponValidation.Coupon != null)
                    {
                        couponDiscount = couponValidation.DiscountAmount;
                        appliedCouponId = couponValidation.Coupon.Id;
                    }
                }

                var totalDiscount = promotionDiscount + couponDiscount;
                var finalAmount = originalAmount - totalDiscount;
                if (finalAmount < 0) finalAmount = 0;

                var newOrder = new Order
                {
                    OrderDate = orderDate ?? DateTime.UtcNow,
                    CustomerId = customerId,
                    Status = initialStatus,
                    Notes = notes,
                    PaymentMethod = method,
                    PaymentStatus = PaymentStatus.Pending,
                    DeliveryDate = deliveryDate,
                    DeliveryTimeSlot = deliveryTimeSlot,
                    DeliveryDistrict = deliveryDistrict,
                    DeliveryAddress = deliveryAddress,
                    RecipientName = recipientName,
                    RecipientPhone = recipientPhone,
                    TargetFinishedTime = targetFinishedTime,
                    PromotionId = appliedPromotionId,
                    CouponId = appliedCouponId,
                    OriginalAmount = originalAmount,
                    DiscountAmount = totalDiscount,
                    FinalAmount = finalAmount
                };

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var product = products[item.ProductId];
                        int reserved = _stockLockService.GetReservedStock(item.ProductId);
                        int availableStock = product.StockQuantity - reserved;
                        if (availableStock < item.Quantity)
                            return (false, $"Sản phẩm '{product.Name}' không đủ hàng (còn: {availableStock}, yêu cầu: {item.Quantity})", 0);
                    }

                    newOrder.OrderDetails = items.Select(item =>
                    {
                        var product = products[item.ProductId];
                        var itemDiscount = itemDiscounts[item.ProductId];
                        return new OrderDetail
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            ProductName = product.Name,
                            ProductImage = product.ImageUrl,
                            SizeVariant = item.SizeVariant,
                            Discount = itemDiscount,
                            Subtotal = item.UnitPrice * item.Quantity
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
                        return (false, $"Sản phẩm '{products[item.ProductId].Name}' vừa hết hàng, vui lòng thử lại.", 0);
                    }
                }
            }

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // If coupon was applied, create usage record
            if (appliedCouponId.HasValue && !string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == couponCode);
                if (coupon != null)
                {
                    coupon.UsedCount++;
                    _context.CouponUsages.Add(new CouponUsage
                    {
                        CouponId = coupon.Id,
                        OrderId = newOrder.Id,
                        CustomerId = customerId,
                        DiscountAmount = couponDiscount,
                        UsedAt = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();

            if (customer != null)
            {
                customer.TotalOrders++;
            }

            if (method == PaymentMethod.COD && customer != null)
            {
                var requiresVerification = await _fraudDetectionService.RequiresVerification(customer);
                if (!requiresVerification)
                {
                    newOrder.Status = OrderStatus.Confirmed;
                    newOrder.IsVerified = true;
                    newOrder.VerifiedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var otp = new Random().Next(100000, 999999).ToString();
                    _memoryCache.Set("otp_" + newOrder.Id, otp, TimeSpan.FromMinutes(10));

                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        try
                        {
                            await _emailService.SendOtpEmailAsync(customer.Email, customer.FullName, otp);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to send OTP email for order {OrderId}", newOrder.Id);
                        }
                    }
                }
            }

            try
            {
                await _adminNotificationService.CreateNotification(
                    "Đơn hàng mới",
                    $"Đơn hàng #{newOrder.Id} vừa được tạo bởi khách hàng {customer?.FullName ?? "Unknown"}.",
                    "Order",
                    newOrder.Id.ToString()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create admin notification for order {OrderId}", newOrder.Id);
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

        public async Task<bool> CancelWithReason(int id, string? reason)
        {
            return await _orderCancellationService.CancelWithReason(id, reason);
        }

        public async Task<(bool Success, string Message)> CancelByCustomer(int id, string? reason = null)
        {
            return await _orderCancellationService.CancelByCustomer(id, reason);
        }

        public async Task<(bool Success, string Message)> CancelByShop(int id, string? reason = null)
        {
            return await _orderCancellationService.CancelByShop(id, reason);
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
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var requiresVerification = await _fraudDetectionService.RequiresVerification(order.Customer);

                    order.Status = OrderStatus.Confirmed;
                    order.IsVerified = true;
                    order.VerifiedAt = DateTime.UtcNow;
                    order.Customer.TotalOrders++;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

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
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return (false, "Không tìm thấy thông tin khách hàng");
        }

        public async Task<bool> IsPhoneBlacklisted(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            return await _fraudDetectionService.IsPhoneBlacklisted(phone);
        }
    }
}
