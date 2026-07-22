using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDataProtector _protector;

        public SystemSettingService(
            ApplicationDbContext context,
            IMemoryCache cache,
            IConfiguration configuration,
            IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _cache = cache;
            _configuration = configuration;
            _protector = dataProtectionProvider.CreateProtector("Flower.Settings.Secrets");
        }

        private string SafeDecrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return string.Empty;
            try
            {
                return _protector.Unprotect(encryptedText);
            }
            catch
            {
                return encryptedText; // Fallback to plain text if not encrypted or key changed
            }
        }

        private string SafeEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            try
            {
                return _protector.Protect(plainText);
            }
            catch
            {
                return plainText;
            }
        }

        public async Task<T> GetSetting<T>(string key) where T : new()
        {
            string cacheKey = $"setting_{key}";
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                return cachedValue;
            }

            var setting = await _context.SystemSettings.FindAsync(key);
            T result;
            if (setting == null)
            {
                result = GetFallbackSetting<T>(key);
            }
            else
            {
                try
                {
                    result = JsonSerializer.Deserialize<T>(setting.Value) ?? new T();
                }
                catch
                {
                    result = new T();
                }

                // Decrypt sensitive fields
                if (result is SmtpSettings smtp)
                {
                    smtp.Password = SafeDecrypt(smtp.Password);
                }
                else if (result is VNPaySettings vnpay)
                {
                    vnpay.HashSecret = SafeDecrypt(vnpay.HashSecret);
                }
            }

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;
        }

        public async Task SaveSetting<T>(string key, T settingValue, string? updatedBy = null)
        {
            // Protect sensitive fields on copy before serialization
            T protectedValue;
            var jsonTemp = JsonSerializer.Serialize(settingValue);
            protectedValue = JsonSerializer.Deserialize<T>(jsonTemp) ?? settingValue;

            if (protectedValue is SmtpSettings smtp)
            {
                smtp.Password = SafeEncrypt(smtp.Password);
            }
            else if (protectedValue is VNPaySettings vnpay)
            {
                vnpay.HashSecret = SafeEncrypt(vnpay.HashSecret);
            }

            var json = JsonSerializer.Serialize(protectedValue);
            var setting = await _context.SystemSettings.FindAsync(key);

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = key,
                    Value = json,
                    Description = $"Cấu hình cho {key}",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = updatedBy
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = json;
                setting.UpdatedAt = DateTime.UtcNow;
                setting.UpdatedBy = updatedBy;
            }

            await _context.SaveChangesAsync();

            // Invalidate Cache
            string cacheKey = $"setting_{key}";
            _cache.Remove(cacheKey);
        }

        public async Task<AllSystemSettingsViewModel> GetAllSettings()
        {
            return new AllSystemSettingsViewModel
            {
                Store = await GetSetting<StoreInfoSettings>("StoreInfo"),
                Smtp = await GetSetting<SmtpSettings>("Smtp"),
                VNPay = await GetSetting<VNPaySettings>("VNPay"),
                Cloudinary = await GetSetting<CloudinarySettings>("Cloudinary"),
                Shipping = await GetSetting<ShippingSettings>("Shipping"),
                Order = await GetSetting<OrderSettings>("Order")
            };
        }

        public async Task SaveAllSettings(AllSystemSettingsViewModel settings, string? updatedBy = null)
        {
            await SaveSetting("StoreInfo", settings.Store, updatedBy);
            await SaveSetting("Smtp", settings.Smtp, updatedBy);
            await SaveSetting("VNPay", settings.VNPay, updatedBy);
            await SaveSetting("Shipping", settings.Shipping, updatedBy);
            await SaveSetting("Order", settings.Order, updatedBy);
        }

        private T GetFallbackSetting<T>(string key) where T : new()
        {
            if (key == "Smtp" && typeof(T) == typeof(SmtpSettings))
            {
                var configSection = _configuration.GetSection("EmailSettings");
                var smtp = new SmtpSettings
                {
                    Host = configSection["SmtpHost"] ?? "smtp.gmail.com",
                    Port = int.TryParse(configSection["SmtpPort"], out var port) ? port : 587,
                    Username = configSection["Username"] ?? "",
                    Password = configSection["Password"] ?? "",
                    SenderName = configSection["SenderName"] ?? "FlowerShop",
                    SenderEmail = configSection["SenderEmail"] ?? "noreply@flowershop.com"
                };
                return (T)(object)smtp;
            }
            if (key == "VNPay" && typeof(T) == typeof(VNPaySettings))
            {
                var configSection = _configuration.GetSection("Vnpay");
                var vnpay = new VNPaySettings
                {
                    TmnCode = configSection["TmnCode"] ?? "",
                    HashSecret = configSection["HashSecret"] ?? "",
                    ReturnUrl = configSection["PaymentBackReturnUrl"] ?? "",
                    IsSandbox = true,
                    EnablePayment = true
                };
                return (T)(object)vnpay;
            }
            if (key == "Cloudinary" && typeof(T) == typeof(CloudinarySettings))
            {
                var configSection = _configuration.GetSection("CloudinarySettings");
                var cloudinary = new CloudinarySettings
                {
                    CloudName = Environment.GetEnvironmentVariable("CLOUDINARY__CLOUDNAME")
                        ?? configSection["CloudName"] ?? "",
                    ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY__APIKEY")
                        ?? configSection["ApiKey"] ?? "",
                    ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY__APISECRET")
                        ?? configSection["ApiSecret"] ?? "",
                    Folder = configSection["Folder"] ?? "flowershop_products"
                };
                return (T)(object)cloudinary;
            }

            if (key == "Order" && typeof(T) == typeof(OrderSettings))
            {
                var timeSection = _configuration.GetSection("TimeSettings");
                var order = new OrderSettings
                {
                    AutoCancelMinutes = int.TryParse(timeSection["PreShippingMinutes"], out var mins) ? mins : 30,
                    EnableCOD = true,
                    EnableOnlinePayment = true
                };
                return (T)(object)order;
            }
            return new T();
        }
    }
}
