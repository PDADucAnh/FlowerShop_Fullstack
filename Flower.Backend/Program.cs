/* Họ tên: Phạm Đức Anh
 * Mã SV: 2123110135
 * Lớp: CCQ2311D
 * Ngày tạo: 06/06/2026
 * Mô tả:   1.Xác thức Authentication và phân quyền Authorization
 *          2. Cấu hình hệ thống để sử dụng Cookie Authentication trong ASP.NET Core
 *          3. Thiết lập các middleware cần thiết để bảo vệ các trang quản trị và đảm bảo chỉ người dùng đã đăng nhập mới có thể truy cập vào các chức năng quản lý trong CMS
 *          4. Tạo tài khoản người dùng với vai trò Admin và Editor để kiểm tra chức năng phân quyền trong hệ thống CMS
 *          5. Xử lý bảo mật: Không hiển thị mật khẩu trong danh sách thành viên, và có chức năng đổi mật khẩu riêng biệt trong UserController
 *          6. Áp dụng chính sách CORS để cho phép các ứng dụng frontend (ví dụ: React, Angular) có thể gọi API của hệ thống CMS một cách an toàn và hiệu quả
 *          7. Sử dụng Swagger để tạo tài liệu API tự động cho các endpoint trong hệ thống CMS, giúp cho việc phát triển và tích hợp với frontend trở nên dễ dàng hơn
 *          8. Tối ưu hiệu suất: Sử dụng các kỹ thuật như caching, pagination, và tối ưu hóa truy vấn để cải thiện hiệu suất của hệ thống CMS khi xử lý lượng lớn dữ liệu hoặc nhiều yêu cầu đồng thời
 */
using Flower.Backend.Models;
using Flower.Backend.Services;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? jwtSettings["SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("JWT SecretKey is not configured. Set JWT_SECRET_KEY environment variable or Jwt:SecretKey in user secrets.");

var webhookSecret = Environment.GetEnvironmentVariable("WEBHOOK_SECRET_KEY")
    ?? builder.Configuration["WebhookSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(webhookSecret))
    throw new InvalidOperationException("Webhook SecretKey is not configured. Set WEBHOOK_SECRET_KEY environment variable.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Flower.Auth";
})
.AddPolicyScheme("Flower.Auth", "Bearer or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            return JwtBearerDefaults.AuthenticationScheme;
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
{
    cookieOptions.LoginPath = "/Account/Login";
    cookieOptions.AccessDeniedPath = "/Account/AccessDenied";
    cookieOptions.Cookie.IsEssential = true;
    cookieOptions.SlidingExpiration = false;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
{
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
    jwtOptions.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var error = new ApiErrorResponse
            {
                Code = "UNAUTHORIZED",
                Message = context.ErrorDescription ?? "Authentication required"
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            var error = new ApiErrorResponse
            {
                Code = "FORBIDDEN",
                Message = "You do not have permission to access this resource"
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    };
});
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IPostService, Flower.Backend.Services.PostService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IProductService, Flower.Backend.Services.ProductService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICategoryService, Flower.Backend.Services.CategoryService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICategoryProductService, Flower.Backend.Services.CategoryProductService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IUserService, Flower.Backend.Services.UserService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IAuthService, Flower.Backend.Services.AuthService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICustomerService, Flower.Backend.Services.CustomerService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IOrderService, Flower.Backend.Services.OrderService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IOrderCancellationService, Flower.Backend.Services.OrderCancellationService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IOrderDetailService, Flower.Backend.Services.OrderDetailService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.INotificationService, Flower.Backend.Services.NotificationService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IAdvertisementService, Flower.Backend.Services.AdvertisementService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IDeliverySlotService, Flower.Backend.Services.DeliverySlotService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IPaymentService, Flower.Backend.Services.PaymentService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IVnPayService, Flower.Backend.Services.VnPayService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IFraudDetectionService, Flower.Backend.Services.FraudDetectionService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IDashboardService, Flower.Backend.Services.DashboardService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IPromotionService, Flower.Backend.Services.PromotionService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.ICouponService, Flower.Backend.Services.CouponService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IPriceCalculationService, Flower.Backend.Services.PriceCalculationService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IFlashSaleService, Flower.Backend.Services.FlashSaleService>();
builder.Services.AddHostedService<Flower.Backend.Services.PromotionScheduler>();
builder.Services.Configure<Flower.Backend.Models.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var timeSettings = builder.Configuration.GetSection("TimeSettings").Get<TimeSettings>() ?? new TimeSettings();
builder.Services.AddSingleton(timeSettings);
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IEmailService, Flower.Backend.Services.EmailService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<Flower.Backend.Services.StockLockService>();
builder.Services.AddHostedService<Flower.Backend.Services.OrderExpiryBackgroundService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiGlobal", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

// ---- CẤU HÌNH CORS (THÊM VÀO TRƯỚC builder.Build()) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Cho phép ReactJS ở port 3000 gọi tới
              .AllowAnyHeader()                     // Cho phép mọi loại Header (Content-Type, Authorization...)
              .AllowAnyMethod()                     // Cho phép mọi phương thức HTTP (GET, POST, PUT, DELETE)
              .AllowCredentials();                  // Hỗ trợ truyền Cookie/Session nếu cần sau này
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Editor"));
});

var app = builder.Build();

// Auto-apply pending migrations (dev only) or run manually via update-database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Environment.IsDevelopment())
    {
        await context.Database.MigrateAsync();
    }

    // Seed admin account
    if (!context.Users.Any(u => u.Username == "admin"))
    {
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Flower.Data.Entities.User>();
        context.Users.Add(new Flower.Data.Entities.User
        {
            Username = "admin",
            PasswordHash = hasher.HashPassword(null!, "123456"),
            FullName = "Administrator",
            Role = "Admin"
        });
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception on API request {Path}", context.Request.Path);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var error = new ApiErrorResponse
            {
                Code = "INTERNAL_ERROR",
                Message = "An internal error occurred"
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(error));
        }
    }
    else
    {
        await next();
    }
});

app.UseAuthentication();
app.UseMiddleware<Flower.Backend.Middleware.SessionValidationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.MapHub<Flower.Backend.Hubs.NotificationHub>("/hubs/notifications");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();