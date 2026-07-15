using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Claims;
using Flower.Data;

namespace Flower.Backend.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService, IMemoryCache cache, IApplicationDbContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/hubs"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? context.User.FindFirst("Id")?.Value;
                var authType = context.User.FindFirst("AuthType")?.Value ?? "User";

                if (int.TryParse(userIdClaim, out var userId))
                {
                    bool isActive = false;
                    string currentRole = "";
                    
                    if (authType == "User")
                    {
                        var dbUser = await dbContext.Users.FindAsync(userId);
                        if (dbUser != null)
                        {
                            isActive = dbUser.IsActive;
                            currentRole = dbUser.Role;
                        }
                    }
                    else if (authType == "Customer")
                    {
                        var dbCustomer = await dbContext.Customers.FindAsync(userId);
                        if (dbCustomer != null)
                        {
                            isActive = dbCustomer.IsActive;
                            currentRole = "Customer";
                        }
                    }

                    var claimRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

                    if (!isActive || (currentRole != "" && currentRole != claimRole))
                    {
                        var refreshTokenCookie = context.Request.Cookies["X-Refresh-Token"];
                        if (!string.IsNullOrEmpty(refreshTokenCookie))
                        {
                            await authService.RevokeTokenAsync(refreshTokenCookie);
                        }

                        context.Response.Cookies.Delete("X-Refresh-Token");
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Tài khoản đã bị khóa hoặc không hợp lệ.\"}");
                            return;
                        }
                        else
                        {
                            context.Response.Redirect("/Account/Login");
                            return;
                        }
                    }
                }

                var loginTimeClaim = context.User.FindFirst("LoginTime")?.Value;
                if (loginTimeClaim != null && DateTime.TryParse(loginTimeClaim, out var loginTime))
                {
                    if (DateTime.UtcNow - loginTime > TimeSpan.FromDays(30))
                    {
                        var refreshTokenCookie = context.Request.Cookies["X-Refresh-Token"];
                        if (!string.IsNullOrEmpty(refreshTokenCookie))
                        {
                            await authService.RevokeTokenAsync(refreshTokenCookie);
                        }

                        context.Response.Cookies.Delete("X-Refresh-Token");
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }

                var rawToken = context.Request.Cookies["X-Refresh-Token"];
                if (!string.IsNullOrEmpty(rawToken))
                {
                    var cacheKey = $"session_valid_{rawToken}";
                    if (!cache.TryGetValue(cacheKey, out int? _))
                    {
                        var dbUserId = await authService.ValidateRefreshTokenAsync(rawToken);
                        if (dbUserId == null)
                        {
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            context.Response.Cookies.Delete("X-Refresh-Token");
                            context.Response.Redirect("/Account/Login");
                            return;
                        }

                        cache.Set(cacheKey, dbUserId.Value, _cacheDuration);
                    }
                }
            }

            await _next(context);
        }
    }
}
