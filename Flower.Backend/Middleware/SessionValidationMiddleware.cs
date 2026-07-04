using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;

namespace Flower.Backend.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
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
                    var userId = await authService.ValidateRefreshTokenAsync(rawToken);
                    if (userId == null)
                    {
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        context.Response.Cookies.Delete("X-Refresh-Token");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
