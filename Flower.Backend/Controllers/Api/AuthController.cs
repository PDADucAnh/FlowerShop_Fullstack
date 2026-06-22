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
    [AllowAnonymous]
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
            new Claim("FullName", result.FullName ?? ""),
            new Claim("Email", result.Email ?? ""),
            new Claim("Phone", result.Phone ?? ""),
            new Claim("Address", result.Address ?? ""),
            new Claim("AuthType", result.AuthType ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var result = await _authService.Login(login.Username, login.Password);

            if (result != null)
            {
                var claimsIdentity = new ClaimsIdentity(
                    BuildUserClaims(result), CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                var jwtKey = _configuration["Jwt:SecretKey"]
                    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

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
                    username = result.Username,
                    fullName = result.FullName,
                    email = result.Email,
                    phone = result.Phone,
                    address = result.Address,
                    role = result.Role,
                    message = "Login successful"
                });
            }

            return Unauthorized(new { message = "Invalid username or password!" });
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest register)
        {
            var (success, message) = await _authService.Register(
                register.Username, register.Password, register.FullName,
                register.Email, register.Phone, register.Address);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
