using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult?> Login(string identifier, string password);
        Task<(bool Success, string Message)> Register(string password, string fullName, string? email, string? phone, string? address);
        Task<LoginResult?> GetProfile(string identifier, string authType);
        Task<string> CreateRefreshTokenAsync(int userId, string? deviceInfo = null);
        Task<int?> ValidateRefreshTokenAsync(string rawToken);
        Task RevokeUserTokensAsync(int userId);
        Task RevokeTokenAsync(string rawToken);
        Task<(bool Success, string Message)> ForgotPassword(string email, string clientUrl);
        Task<(bool Success, string Message)> ResetPassword(string token, string newPassword);
        Task<(bool Success, string Message, LoginResult? Result)> UpdateProfile(string identifier, string authType, string fullName, string? phone, string? address);
        Task<(bool Success, string Message)> ChangePassword(string identifier, string authType, string currentPassword, string newPassword);
    }
}
