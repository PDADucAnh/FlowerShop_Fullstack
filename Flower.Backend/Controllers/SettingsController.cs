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
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public SettingsController(
            ISystemSettingService settingService,
            ILogger<SettingsController> logger,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _settingService = settingService;
            _logger = logger;
            _emailService = emailService;
            _notificationService = notificationService;
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

            await _notificationService.NotifyEntityChanged("SystemSettings");

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

            await _notificationService.NotifyEntityChanged("SystemSettings");

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

            await _notificationService.NotifyEntityChanged("SystemSettings");

            TempData["Success"] = "Cập nhật cấu hình VNPay thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/SaveCloudinary
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCloudinary(CloudinarySettings model)
        {
            var oldSetting = await _settingService.GetSetting<CloudinarySettings>("Cloudinary");

            if (model.ApiSecret == "••••••••••••")
            {
                model.ApiSecret = oldSetting.ApiSecret;
                ModelState.Remove("ApiSecret");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu cấu hình Cloudinary không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var username = User.Identity?.Name ?? "Admin";
            await _settingService.SaveSetting("Cloudinary", model, username);

            var auditOld = new CloudinarySettings { CloudName = oldSetting.CloudName, ApiKey = oldSetting.ApiKey, ApiSecret = "Redacted", Folder = oldSetting.Folder };
            var auditNew = new CloudinarySettings { CloudName = model.CloudName, ApiKey = model.ApiKey, ApiSecret = "Redacted", Folder = model.Folder };

            _logger.LogInformation("AUDIT LOG: User {User} updated Cloudinary Settings. Old Value: {Old}, New Value: {New}",
                username, JsonSerializer.Serialize(auditOld), JsonSerializer.Serialize(auditNew));

            TempData["Success"] = "Cập nhật cấu hình Cloudinary thành công.";
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

            await _notificationService.NotifyEntityChanged("SystemSettings");

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

            await _notificationService.NotifyEntityChanged("SystemSettings");

            TempData["Success"] = "Cập nhật cấu hình đơn hàng thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Settings/TestEmail
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestEmail(string toEmail)
        {
            var smtpSettings = await _settingService.GetSetting<SmtpSettings>("Smtp");
            var host = smtpSettings?.Host ?? "N/A";
            var port = smtpSettings?.Port.ToString() ?? "N/A";

            try
            {
                if (_emailService == null)
                {
                    throw new InvalidOperationException("EmailService chưa được cấu hình.");
                }

                await _emailService.SendTestEmailAsync(toEmail);
                _logger.LogInformation("SMTP Test Email Success: Sent to={Email}, Host={Host}, Port={Port}, SentAt={Time}",
                    toEmail, host, port, DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));

                return Json(new { success = true, message = "Đã gửi email thử nghiệm thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP Test Email Failed: Sent to={Email}, Host={Host}, Port={Port}, SentAt={Time}",
                    toEmail, host, port, DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));

                return Json(new { success = false, message = "Không thể gửi email. Chi tiết xem log hệ thống." });
            }
        }
    }
}
