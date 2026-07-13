using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> GetSetting<T>(string key) where T : new()
        {
            var setting = await _context.SystemSettings.FindAsync(key);
            if (setting == null)
            {
                return new T();
            }

            try
            {
                return JsonSerializer.Deserialize<T>(setting.Value) ?? new T();
            }
            catch
            {
                return new T();
            }
        }

        public async Task SaveSetting<T>(string key, T settingValue, string? updatedBy = null)
        {
            var json = JsonSerializer.Serialize(settingValue);
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
        }

        public async Task<AllSystemSettingsViewModel> GetAllSettings()
        {
            return new AllSystemSettingsViewModel
            {
                Store = await GetSetting<StoreInfoSettings>("StoreInfo"),
                Smtp = await GetSetting<SmtpSettings>("Smtp"),
                VNPay = await GetSetting<VNPaySettings>("VNPay"),
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
    }
}
