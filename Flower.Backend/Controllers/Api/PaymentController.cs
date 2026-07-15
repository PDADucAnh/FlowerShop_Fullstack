using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IVnPayService _vnPayService;
        private readonly IOrderService _orderService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PaymentController> _logger;
        private readonly IAdminNotificationService _adminNotificationService;

        public PaymentController(
            IPaymentService paymentService,
            IVnPayService vnPayService,
            IOrderService orderService,
            IMemoryCache memoryCache,
            ILogger<PaymentController> logger,
            IAdminNotificationService adminNotificationService)
        {
            _paymentService = paymentService;
            _vnPayService = vnPayService;
            _orderService = orderService;
            _memoryCache = memoryCache;
            _logger = logger;
            _adminNotificationService = adminNotificationService;
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PaymentWebhookRequest request)
        {
            var result = await _paymentService.ProcessWebhook(request);
            if (!result)
                return BadRequest(new { message = "Xử lý webhook thất bại" });

            return Ok(new { message = "Cập nhật thanh toán thành công" });
        }

        [HttpPost("create-vnpay-url")]
        public async Task<IActionResult> CreateVnpayUrl([FromBody] VnPaymentRequestModel model)
        {
            var order = await _orderService.GetDetail(model.OrderId);
            if (order == null)
                return BadRequest(new { message = "Đơn hàng không tồn tại" });

            if (order.Status != OrderStatus.PendingPayment)
                return BadRequest(new { message = "Đơn hàng không ở trạng thái chờ thanh toán" });

            if (order.PaymentMethod != PaymentMethod.OnlinePayment)
                return BadRequest(new { message = "Đơn hàng không phải thanh toán online" });

            if (order.PaymentStatus == PaymentStatus.Completed)
                return BadRequest(new { message = "Đơn hàng đã được thanh toán" });

            var totalAmount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount;
            if (totalAmount <= 0)
                return BadRequest(new { message = "Số tiền thanh toán không hợp lệ" });

            var paymentId = await _paymentService.CreatePendingPayment(model.OrderId, totalAmount, PaymentMethod.OnlinePayment);
            _logger.LogInformation("Create Payment: PaymentId={PaymentId}, OrderId={OrderId}, Amount={Amount}",
                paymentId, model.OrderId, totalAmount);

            var vnpayModel = new VnPaymentRequestModel
            {
                OrderId = model.OrderId,
                PaymentId = paymentId,
                Amount = (double)totalAmount,
                OrderDescription = $"Thanh toán đơn hàng {model.OrderId}",
                Name = model.Name
            };
            var url = _vnPayService.CreatePaymentUrl(vnpayModel, HttpContext);

            _logger.LogInformation("Redirect VNPAY: OrderId={OrderId}, PaymentId={PaymentId}, Amount={Amount}", model.OrderId, paymentId, totalAmount);

            return Ok(new { url, paymentId });
        }

        [AllowAnonymous]
        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnpayCallback()
        {
            _logger.LogInformation("VNPAY Callback received");

            var response = _vnPayService.PaymentExecute(Request.Query);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();

            _logger.LogInformation("VNPAY Callback: Success={Success}, ResponseCode={Code}, OrderId={OrderId}, TransactionId={TxnId}, Amount={Amount}",
                response.Success, response.VnPayResponseCode, response.OrderId, response.TransactionId, response.Amount);

            if (!int.TryParse(response.OrderId, out var orderId))
            {
                _logger.LogWarning("VNPAY Callback: invalid OrderId={OrderId}", response.OrderId);
                return Redirect($"{GetClientUrl()}/cart?payment=error");
            }

            if (!response.Success)
            {
                _logger.LogInformation("Payment Failed: OrderId={OrderId}, ResponseCode={Code}",
                    response.OrderId, response.VnPayResponseCode);

                await _paymentService.MarkPaymentFailed(orderId, response.VnPayResponseCode, ipAddress, userAgent);

                try
                {
                    await _adminNotificationService.CreateNotification(
                        "Thanh toán thất bại",
                        $"Thanh toán cho đơn hàng #{orderId} thất bại qua VNPay (Mã phản hồi: {response.VnPayResponseCode}).",
                        "Payment",
                        orderId.ToString()
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create payment failure notification for order {OrderId}", orderId);
                }

                return Redirect($"{GetClientUrl()}/order-confirmation?orderId={orderId}&payment={response.VnPayResponseCode}");
            }

            var order = await _orderService.GetDetail(orderId);
            if (order == null)
            {
                _logger.LogWarning("VNPAY Callback: order not found, OrderId={OrderId}", orderId);
                return Redirect($"{GetClientUrl()}/cart?payment=error");
            }

            if (order.PaymentStatus == PaymentStatus.Completed)
            {
                _logger.LogInformation("VNPAY Callback: order already paid, OrderId={OrderId}", orderId);
                return Redirect($"{GetClientUrl()}/order-confirmation?orderId={orderId}&payment=success");
            }

            _logger.LogInformation("Verify Amount: Expected={Expected}, Got={Got}, OrderId={OrderId}",
                order.TotalAmount, response.Amount, orderId);

            var (success, message) = await _paymentService.ConfirmOnlinePayment(
                orderId, response.TransactionId, response.Amount, ipAddress, userAgent, response.VnPayResponseCode);

            if (!success)
            {
                _logger.LogWarning("Payment confirm failed: OrderId={OrderId}, Reason={Reason}", orderId, message);
                return Redirect($"{GetClientUrl()}/order-confirmation?orderId={orderId}&payment={response.VnPayResponseCode}&error={Uri.EscapeDataString(message)}");
            }

            _logger.LogInformation("Payment Success: OrderId={OrderId}, TransactionId={TransactionId}",
                orderId, response.TransactionId);

            try
            {
                await _adminNotificationService.CreateNotification(
                    "Thanh toán VNPay thành công",
                    $"Đơn hàng #{orderId} đã được thanh toán thành công qua VNPay số tiền {response.Amount:#,##0} VNĐ (Mã giao dịch: {response.TransactionId}).",
                    "Payment",
                    orderId.ToString()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment success notification for order {OrderId}", orderId);
            }

            return Redirect($"{GetClientUrl()}/order-confirmation?orderId={orderId}&payment=success");
        }

        [HttpPost("retry/{orderId}")]
        public async Task<IActionResult> RetryPayment(int orderId)
        {
            var order = await _orderService.GetDetail(orderId);
            if (order == null)
                return BadRequest(new { message = "Đơn hàng không tồn tại" });

            if (order.PaymentMethod != PaymentMethod.OnlinePayment)
                return BadRequest(new { message = "Phương thức thanh toán không hợp lệ" });

            if (order.Status != OrderStatus.PendingPayment)
                return BadRequest(new { message = "Đơn hàng không thể thanh toán lại" });

            var (retrySuccess, retryMessage, paymentId, _) = await _paymentService.CreateRetryPayment(orderId, PaymentMethod.OnlinePayment);
            if (!retrySuccess)
                return BadRequest(new { message = retryMessage });

            var totalAmount = order.FinalAmount > 0 ? order.FinalAmount : order.TotalAmount;
            var vnpayModel = new VnPaymentRequestModel
            {
                OrderId = orderId,
                PaymentId = paymentId,
                Amount = (double)totalAmount,
                OrderDescription = $"Thanh toán lại đơn hàng {orderId}",
                Name = order.CustomerName
            };
            var url = _vnPayService.CreatePaymentUrl(vnpayModel, HttpContext);

            _logger.LogInformation("Retry Payment: OrderId={OrderId}, PaymentId={PaymentId}, Amount={Amount}",
                orderId, paymentId, totalAmount);

            return Ok(new { url, paymentId });
        }

        [HttpPost("{orderId}/verify")]
        public async Task<IActionResult> VerifyCOD(int orderId, [FromBody] VerificationRequest request)
        {
            var cacheKey = "otp_" + orderId;
            if (!_memoryCache.TryGetValue(cacheKey, out string cachedOtp) || cachedOtp != request.Otp)
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn" });

            _memoryCache.Remove(cacheKey);

            var (success, message) = await _orderService.ProcessCODOrder(orderId);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        private string GetClientUrl()
        {
            var envUrl = Environment.GetEnvironmentVariable("CLIENT_URL");
            if (!string.IsNullOrWhiteSpace(envUrl))
                return envUrl;

            var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            return config?["ClientUrl"] ?? "http://localhost:3000";
        }
    }
}
