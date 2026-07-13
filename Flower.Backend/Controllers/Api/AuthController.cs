using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        private static Claim[] BuildUserClaims(LoginResult result) => new[]
        {
            new Claim(ClaimTypes.Name, result.Username),
            new Claim(ClaimTypes.Role, result.Role),
            new Claim("Id", result.Id.ToString()),
            new Claim("FullName", result.FullName ?? ""),
            new Claim("Email", result.Email ?? ""),
            new Claim("Phone", result.Phone ?? ""),
            new Claim("Address", result.Address ?? ""),
            new Claim("AuthType", result.AuthType ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            try
            {
                var result = await _authService.Login(login.Username, login.Password);

                if (result != null)
                {
                    if (!result.IsActive)
                    {
                        return StatusCode(403, new { success = false, message = "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động." });
                    }

                    // NOTE: Jwt:SecretKey must be >= 32 characters (256 bits) for HS256
                    var jwtKey = _configuration["Jwt:SecretKey"]
                        ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
                    var issuer = _configuration["Jwt:Issuer"]
                        ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
                    var audience = _configuration["Jwt:Audience"]
                        ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
                    if (!int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var expiryMinutes))
                        expiryMinutes = 60;

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(jwtKey);
                    var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(BuildUserClaims(result)),
                        Expires = expiration,
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new
                    {
                        token = tokenString,
                        expiresAt = expiration.ToString("o"),
                        id = result.Id,
                        username = result.Username,
                        fullName = result.FullName,
                        email = result.Email,
                        phone = result.Phone,
                        address = result.Address,
                        role = result.Role,
                        message = "Login successful"
                    });
                }

                return Unauthorized(new { success = false, message = "Email hoặc mật khẩu không đúng." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra trong quá trình đăng nhập." });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Invalid token" });

            var authType = User.FindFirst("AuthType")?.Value ?? "User";

            var result = await _authService.GetProfile(username, authType);
            if (result == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                id = result.Id,
                username = result.Username,
                fullName = result.FullName,
                email = result.Email,
                phone = result.Phone,
                address = result.Address,
                role = result.Role
            });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Invalid token" });

            var authType = User.FindFirst("AuthType")?.Value ?? "User";

            var (success, message, result) = await _authService.UpdateProfile(username, authType, request.FullName, request.Phone, request.Address);
            if (!success || result == null)
                return BadRequest(new { message });

            // Generate a new JWT token to update claims
            var jwtKey = _configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
            if (!int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var expiryMinutes))
                expiryMinutes = 60;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);
            var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(BuildUserClaims(result)),
                Expires = expiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    id = result.Id,
                    username = result.Username,
                    fullName = result.FullName,
                    email = result.Email,
                    phone = result.Phone,
                    address = result.Address,
                    role = result.Role
                },
                message
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            var (success, message) = await _authService.Register(
                register.Password, register.FullName,
                register.Email, register.Phone, register.Address);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Invalid token" });

            var authType = User.FindFirst("AuthType")?.Value ?? "User";

            var (success, message) = await _authService.ChangePassword(username, authType, request.CurrentPassword, request.NewPassword);
            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var clientUrl = _configuration["ClientUrl"] ?? "http://localhost:3000";

            var (success, message) = await _authService.ForgotPassword(request.Email, clientUrl);
            return Ok(new { message });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (success, message) = await _authService.ResetPassword(request.Token, request.NewPassword);
            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }
    }
}
