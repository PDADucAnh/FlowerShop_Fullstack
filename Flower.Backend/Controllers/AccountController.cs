using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Flower.Backend.Services.Interfaces;
using System.Collections.Generic;
using System;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
    {
        var user = await _authService.Login(username, password);

        if (user != null)
        {
            if (user.AuthType != "User" || (user.Role != "Admin" && user.Role != "Staff"))
            {
                ViewBag.Error = "Tài khoản không có quyền truy cập trang quản trị!";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc ngừng hoạt động!";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim("AuthType", user.AuthType),
                new Claim("LoginTime", DateTime.UtcNow.ToString("o"))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            var rawToken = await _authService.CreateRefreshTokenAsync(user.Id,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            if (rememberMe)
            {
                cookieOptions.Expires = DateTimeOffset.UtcNow.AddDays(30);
            }

            Response.Cookies.Append("X-Refresh-Token", rawToken, cookieOptions);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        var refreshTokenCookie = Request.Cookies["X-Refresh-Token"];
        if (!string.IsNullOrEmpty(refreshTokenCookie))
        {
            await _authService.RevokeTokenAsync(refreshTokenCookie);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            await _authService.RevokeUserTokensAsync(userId);
        }

        Response.Cookies.Delete("X-Refresh-Token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
