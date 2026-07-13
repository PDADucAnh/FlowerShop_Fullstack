using Flower.Backend.Models.DTOs;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface ISystemSettingService
    {
        Task<T> GetSetting<T>(string key) where T : new();
        Task SaveSetting<T>(string key, T settingValue, string? updatedBy = null);
        Task<AllSystemSettingsViewModel> GetAllSettings();
        Task SaveAllSettings(AllSystemSettingsViewModel settings, string? updatedBy = null);
    }
}
