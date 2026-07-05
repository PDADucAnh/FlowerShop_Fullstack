using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;

namespace Flower.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly PasswordHasher<User> _userPasswordHasher;
        private readonly PasswordHasher<Customer> _customerPasswordHasher;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IApplicationDbContext context, IEmailService emailService, ILogger<AuthService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _userPasswordHasher = new PasswordHasher<User>();
            _customerPasswordHasher = new PasswordHasher<Customer>();
        }

        private static LoginResult MapUserToResult(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            AuthType = "User"
        };

        private static LoginResult MapCustomerToResult(Customer customer) => new()
        {
            Id = customer.Id,
            Username = customer.Email,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            Role = "Customer",
            AuthType = "Customer"
        };

        public async Task<LoginResult?> Login(string identifier, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == identifier);

            if (user != null)
            {
                var verificationResult = _userPasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return null;

                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.PasswordHash = _userPasswordHasher.HashPassword(user, password);
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }

                return MapUserToResult(user);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == identifier);

            if (customer != null)
            {
                var verificationResult = _customerPasswordHasher.VerifyHashedPassword(customer, customer.PasswordHash, password);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return null;

                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, password);
                    _context.Customers.Update(customer);
                    await _context.SaveChangesAsync();
                }

                return MapCustomerToResult(customer);
            }

            return null;
        }

        public async Task<(bool Success, string Message)> Register(string password, string fullName, string? email, string? phone, string? address)
        {
            var checkExist = await _context.Customers.AnyAsync(c => c.Email == email);
            if (checkExist)
            {
                return (false, "Email đã tồn tại!");
            }

            var newCustomer = new Customer
            {
                FullName = fullName,
                Email = email ?? string.Empty,
                Phone = phone,
                Address = address,
                PasswordHash = password
            };

            newCustomer.PasswordHash = _customerPasswordHasher.HashPassword(newCustomer, password);

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return (true, "Đăng ký thành công!");
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string token)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<string> CreateRefreshTokenAsync(int userId, string? deviceInfo = null)
        {
            var rawToken = GenerateRefreshToken();
            var tokenHash = HashToken(rawToken);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                DeviceInfo = deviceInfo
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return rawToken;
        }

        public async Task<int?> ValidateRefreshTokenAsync(string rawToken)
        {
            var tokenHash = HashToken(rawToken);

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash
                    && !t.IsRevoked
                    && t.RevokedAt == null
                    && t.ExpiresAt > DateTime.UtcNow);

            return token?.UserId;
        }

        public async Task RevokeUserTokensAsync(int userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked && t.RevokedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevokeTokenAsync(string rawToken)
        {
            var tokenHash = HashToken(rawToken);

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsRevoked);

            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LoginResult?> GetProfile(string identifier, string authType)
        {
            if (authType == "User")
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == identifier);
                return user != null ? MapUserToResult(user) : null;
            }

            if (authType == "Customer")
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == identifier);
                return customer != null ? MapCustomerToResult(customer) : null;
            }

            return null;
        }

        public async Task<(bool Success, string Message)> ForgotPassword(string email, string clientUrl)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                // Anti email enumeration fallback response
                return (true, "Nếu email tồn tại trên hệ thống, một liên kết đặt lại mật khẩu đã được gửi đi. Vui lòng kiểm tra hộp thư.");
            }

            var rawToken = Guid.NewGuid().ToString("N");
            customer.ResetToken = HashToken(rawToken);
            customer.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            var resetLink = $"{clientUrl.TrimEnd('/')}/reset-password";
            
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendResetPasswordEmailAsync(customer.Email, customer.FullName, resetLink, rawToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send reset password email to {Email}", customer.Email);
                }
            });

            return (true, "Yêu cầu đặt lại mật khẩu đã được gửi đi thành công.");
        }

        public async Task<(bool Success, string Message)> ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(token))
            {
                return (false, "Mã xác thực đặt lại mật khẩu không hợp lệ.");
            }

            var tokenHash = HashToken(token);
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ResetToken == tokenHash);
            if (customer == null)
            {
                return (false, "Mã xác thực đặt lại mật khẩu không hợp lệ.");
            }

            if (!customer.ResetTokenExpiry.HasValue || customer.ResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return (false, "Liên kết đặt lại mật khẩu đã hết hạn. Vui lòng yêu cầu lại.");
            }

            customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, newPassword);
            customer.ResetToken = null;
            customer.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return (true, "Đổi mật khẩu thành công!");
        }

        public async Task<(bool Success, string Message, LoginResult? Result)> UpdateProfile(string identifier, string authType, string fullName, string? phone, string? address)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Họ tên không được để trống", null);

            if (authType == "User")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == identifier);
                if (user == null) return (false, "Không tìm thấy người dùng", null);

                user.FullName = fullName;
                user.Phone = phone;
                user.Address = address;

                await _context.SaveChangesAsync();
                return (true, "Cập nhật thông tin thành công", MapUserToResult(user));
            }

            if (authType == "Customer")
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == identifier);
                if (customer == null) return (false, "Không tìm thấy khách hàng", null);

                customer.FullName = fullName;
                customer.Phone = phone;
                customer.Address = address;

                await _context.SaveChangesAsync();
                return (true, "Cập nhật thông tin thành công", MapCustomerToResult(customer));
            }

            return (false, "Vai trò không hợp lệ", null);
        }

        public async Task<(bool Success, string Message)> ChangePassword(string identifier, string authType, string currentPassword, string newPassword)
        {
            if (authType == "User")
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == identifier);
                if (user == null) return (false, "Không tìm thấy người dùng");

                var verificationResult = _userPasswordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return (false, "Mật khẩu hiện tại không đúng");

                user.PasswordHash = _userPasswordHasher.HashPassword(user, newPassword);
                await _context.SaveChangesAsync();
                return (true, "Đổi mật khẩu thành công");
            }

            if (authType == "Customer")
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == identifier);
                if (customer == null) return (false, "Không tìm thấy khách hàng");

                var verificationResult = _customerPasswordHasher.VerifyHashedPassword(customer, customer.PasswordHash, currentPassword);
                if (verificationResult == PasswordVerificationResult.Failed)
                    return (false, "Mật khẩu hiện tại không đúng");

                customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, newPassword);
                await _context.SaveChangesAsync();
                return (true, "Đổi mật khẩu thành công");
            }

            return (false, "Vai trò không hợp lệ");
        }
    }
}
