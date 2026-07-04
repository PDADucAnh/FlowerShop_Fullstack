using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IMemoryCache _memoryCache;

        public PaymentController(IPaymentService paymentService, IOrderService orderService, IMemoryCache memoryCache)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _memoryCache = memoryCache;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PaymentWebhookRequest request)
        {
            var result = await _paymentService.ProcessWebhook(request);
            if (!result)
                return BadRequest(new { message = "Xử lý webhook thất bại" });

            return Ok(new { message = "Cập nhật thanh toán thành công" });
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
    }
}
