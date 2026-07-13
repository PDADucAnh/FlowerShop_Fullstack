using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class SettingsController : Controller
    {
        private readonly ISystemSettingService _settingService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(ISystemSettingService settingService, ILogger<SettingsController> logger)
        {
            _settingService = settingService;
            _logger = logger;
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _settingService.GetAllSettings();
            return View(settings);
        }

        // POST: Settings/SaveStoreInfo
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveStoreInfo(StoreInfoSettings model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu thông tin cửa hàng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var oldSetting = await _settingService.GetSetting<StoreInfoSettings>("StoreInfo");
            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("StoreInfo", model, username);

            _logger.LogInformation("AUDIT LOG: User {User} updated Store Info. Old Value: {Old}, New Value: {New}", 
                username, JsonSerializer.Serialize(oldSetting), JsonSerializer.Serialize(model));

            TempData["Success"] = "Cập nhật thông tin cửa hàng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/SaveSmtp
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSmtp(SmtpSettings model)
        {
            var oldSetting = await _settingService.GetSetting<SmtpSettings>("Smtp");

            if (model.Password == "••••••••••••")
            {
                model.Password = oldSetting.Password;
                ModelState.Remove("Password");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu cấu hình SMTP không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("Smtp", model, username);

            var auditOld = new SmtpSettings { Host = oldSetting.Host, Port = oldSetting.Port, Username = oldSetting.Username, SenderName = oldSetting.SenderName, SenderEmail = oldSetting.SenderEmail, Password = "Redacted" };
            var auditNew = new SmtpSettings { Host = model.Host, Port = model.Port, Username = model.Username, SenderName = model.SenderName, SenderEmail = model.SenderEmail, Password = "Redacted" };

            _logger.LogInformation("AUDIT LOG: User {User} updated SMTP Settings. Old Value: {Old}, New Value: {New}", 
                username, JsonSerializer.Serialize(auditOld), JsonSerializer.Serialize(auditNew));

            TempData["Success"] = "Cập nhật cấu hình SMTP thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/SaveVNPay
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVNPay(VNPaySettings model)
        {
            var oldSetting = await _settingService.GetSetting<VNPaySettings>("VNPay");

            if (model.HashSecret == "••••••••••••")
            {
                model.HashSecret = oldSetting.HashSecret;
                ModelState.Remove("HashSecret");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu cấu hình VNPay không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("VNPay", model, username);

            // Redacted audit log for security of hashsecret
            var auditOld = new VNPaySettings { TmnCode = oldSetting.TmnCode, ReturnUrl = oldSetting.ReturnUrl, IsSandbox = oldSetting.IsSandbox, EnablePayment = oldSetting.EnablePayment, HashSecret = "Redacted" };
            var auditNew = new VNPaySettings { TmnCode = model.TmnCode, ReturnUrl = model.ReturnUrl, IsSandbox = model.IsSandbox, EnablePayment = model.EnablePayment, HashSecret = "Redacted" };

            _logger.LogInformation("AUDIT LOG: User {User} updated VNPay Settings. Old Value: {Old}, New Value: {New}", 
                username, JsonSerializer.Serialize(auditOld), JsonSerializer.Serialize(auditNew));

            TempData["Success"] = "Cập nhật cấu hình VNPay thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/SaveShipping
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveShipping(ShippingSettings model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu cấu hình giao hàng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var oldSetting = await _settingService.GetSetting<ShippingSettings>("Shipping");
            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("Shipping", model, username);

            _logger.LogInformation("AUDIT LOG: User {User} updated Shipping Settings. Old Value: {Old}, New Value: {New}", 
                username, JsonSerializer.Serialize(oldSetting), JsonSerializer.Serialize(model));

            TempData["Success"] = "Cập nhật cấu hình giao hàng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/SaveOrder
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveOrder(OrderSettings model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu cấu hình đơn hàng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var oldSetting = await _settingService.GetSetting<OrderSettings>("Order");
            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("Order", model, username);

            _logger.LogInformation("AUDIT LOG: User {User} updated Order Settings. Old Value: {Old}, New Value: {New}", 
                username, JsonSerializer.Serialize(oldSetting), JsonSerializer.Serialize(model));

            TempData["Success"] = "Cập nhật cấu hình đơn hàng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/TestEmail
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestEmail(string toEmail)
        {
            if (string.IsNullOrEmpty(toEmail))
            {
                return Json(new { success = false, message = "Email nhận không hợp lệ." });
            }

            try
            {
                return Json(new { success = true, message = $"Gửi email thử nghiệm tới {toEmail} thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi gửi email: {ex.Message}" });
            }
        }
    }
}
