using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Flower.Backend.Services;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flower.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();

        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Register_NewCustomer_ShouldSucceed()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);

            // Act
            var result = await authService.Register("Password123!", "Test User", "test@example.com", "0123456789", "123 Street");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Đăng ký thành công!", result.Message);

            var customerInDb = await context.Customers.FirstOrDefaultAsync(c => c.Email == "test@example.com");
            Assert.NotNull(customerInDb);
            Assert.Equal("Test User", customerInDb.FullName);
            Assert.Equal("0123456789", customerInDb.Phone);
            Assert.Equal("123 Street", customerInDb.Address);
        }

        [Fact]
        public async Task Register_ExistingEmail_ShouldFail()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            
            // Seed existing customer
            await authService.Register("Password123!", "Test User", "test@example.com", null, null);

            // Act
            var result = await authService.Register("DifferentPassword!", "Another Name", "test@example.com", null, null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email đã tồn tại!", result.Message);
        }

        [Fact]
        public async Task Login_CustomerCorrectCredentials_ShouldReturnResult()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            await authService.Register("Password123!", "Login User", "login@example.com", null, null);

            // Act
            var result = await authService.Login("login@example.com", "Password123!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("login@example.com", result.Username);
            Assert.Equal("Login User", result.FullName);
            Assert.Equal("login@example.com", result.Email);
            Assert.Equal("Customer", result.Role);
            Assert.Equal("Customer", result.AuthType);
        }

        [Fact]
        public async Task Login_IncorrectPassword_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            await authService.Register("Password123!", "Login User", "login@example.com", null, null);

            // Act
            var result = await authService.Login("login@example.com", "WrongPassword!");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_NonExistentUser_ShouldReturnNull()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);

            // Act
            var result = await authService.Login("nonexistent@example.com", "Password123!");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_AdminUser_ShouldReturnResult()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);

            // Seed an admin User directly in the database
            var admin = new User
            {
                Username = "admin",
                FullName = "Admin User",
                Role = "Admin",
                PasswordHash = new PasswordHasher<User>().HashPassword(new User(), "Admin@123")
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();

            // Act
            var result = await authService.Login("admin", "Admin@123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
            Assert.Equal("Admin User", result.FullName);
            Assert.Equal("Admin", result.Role);
            Assert.Equal("User", result.AuthType);
        }

        [Fact]
        public async Task ForgotPassword_NonExistentEmail_ShouldSucceedWithFallbackMessage()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);

            // Act
            var result = await authService.ForgotPassword("nonexistent@example.com", "http://client.com");

            // Assert
            Assert.True(result.Success);
            Assert.Contains("Nếu email tồn tại trên hệ thống", result.Message);
            _emailServiceMock.Verify(e => e.SendResetPasswordEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_ExistingEmail_ShouldUpdateCustomerAndSendEmail()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            await authService.Register("Password123!", "Test User", "test@example.com", null, null);

            var emailSentTcs = new TaskCompletionSource<(string link, string? token)>();
            _emailServiceMock
                .Setup(e => e.SendResetPasswordEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>()))
                .Callback<string, string, string, string?>((email, name, link, token) => emailSentTcs.TrySetResult((link, token)))
                .Returns(Task.CompletedTask);

            // Act
            var result = await authService.ForgotPassword("test@example.com", "http://client.com");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Yêu cầu đặt lại mật khẩu đã được gửi đi thành công.", result.Message);

            // Wait for fire-and-forget task to execute
            var completedTask = await Task.WhenAny(emailSentTcs.Task, Task.Delay(2000));
            Assert.Same(emailSentTcs.Task, completedTask);

            var (resetLink, rawToken) = await emailSentTcs.Task;

            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Email == "test@example.com");
            Assert.NotNull(customer);
            Assert.NotNull(customer.ResetToken);
            Assert.NotNull(customer.ResetTokenExpiry);
            Assert.True(customer.ResetTokenExpiry > DateTime.UtcNow);

            // Verify resetLink no longer contains the token in the URL (security fix)
            Assert.Equal("http://client.com/reset-password", resetLink);

            // Verify rawToken is provided as a separate parameter
            Assert.False(string.IsNullOrEmpty(rawToken));

            // Verify that the hashed token in database matches the hashed rawToken
            Assert.Equal(HashToken(rawToken), customer.ResetToken);

            // Verify using ResetPassword that the raw token works
            var resetResult = await authService.ResetPassword(rawToken, "NewPassword123!");
            Assert.True(resetResult.Success);
        }

        [Fact]
        public async Task ResetPassword_InvalidToken_ShouldFail()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);

            // Act
            var result = await authService.ResetPassword("invalid-token", "NewPassword123!");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Mã xác thực đặt lại mật khẩu không hợp lệ.", result.Message);
        }

        [Fact]
        public async Task ResetPassword_ExpiredToken_ShouldFail()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            
            var customer = new Customer
            {
                FullName = "Expired User",
                Email = "expired@example.com",
                PasswordHash = "old-hash",
                ResetToken = HashToken("expired-token"),
                ResetTokenExpiry = DateTime.UtcNow.AddMinutes(-5) // Expired 5 minutes ago
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Act
            var result = await authService.ResetPassword("expired-token", "NewPassword123!");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Liên kết đặt lại mật khẩu đã hết hạn. Vui lòng yêu cầu lại.", result.Message);
        }

        [Fact]
        public async Task ResetPassword_ValidToken_ShouldSucceedAndHashNewPassword()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var authService = new AuthService(context, _emailServiceMock.Object, _loggerMock.Object);
            
            var customer = new Customer
            {
                FullName = "Valid User",
                Email = "valid@example.com",
                PasswordHash = "old-hash",
                ResetToken = HashToken("valid-token"),
                ResetTokenExpiry = DateTime.UtcNow.AddMinutes(5)
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Act
            var result = await authService.ResetPassword("valid-token", "NewPassword123!");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Đổi mật khẩu thành công!", result.Message);

            var updatedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == "valid@example.com");
            Assert.NotNull(updatedCustomer);
            Assert.Null(updatedCustomer.ResetToken);
            Assert.Null(updatedCustomer.ResetTokenExpiry);
            Assert.NotEqual("old-hash", updatedCustomer.PasswordHash);
            
            // Verify we can log in with new password
            var loginResult = await authService.Login("valid@example.com", "NewPassword123!");
            Assert.NotNull(loginResult);
        }

        private static string HashToken(string token)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
