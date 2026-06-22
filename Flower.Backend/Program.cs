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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "AnhCMS.Auth";
})
.AddPolicyScheme("AnhCMS.Auth", "Bearer or Cookie", options =>
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
builder.Services.AddControllersWithViews();

// Swagger
builder.Services.AddControllers();
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
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.IOrderDetailService, Flower.Backend.Services.OrderDetailService>();
builder.Services.AddScoped<Flower.Backend.Services.Interfaces.INotificationService, Flower.Backend.Services.NotificationService>();
builder.Services.AddSignalR();

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
        policy.RequireRole("Admin", "Administrator"));
    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Admin", "Administrator", "Editor"));
});

var app = builder.Build();


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
app.UseAuthorization();

app.MapControllers();

app.MapHub<Flower.Backend.Hubs.NotificationHub>("/hubs/notifications");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (!context.Users.Any())
    {
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Flower.Data.Entities.User>();
        context.Users.Add(new Flower.Data.Entities.User
        {
            Username = "admin",
            PasswordHash = "",
            FullName = "Administrator",
            Role = "Admin"
        });
        context.SaveChanges();
        var admin = context.Users.First(u => u.Username == "admin");
        admin.PasswordHash = hasher.HashPassword(admin, "admin123");
        context.SaveChanges();
    }

    if (!context.CategoriesProducts.Any())
    {
        context.CategoriesProducts.AddRange(
            new Flower.Data.Entities.CategoryProduct { Name = "Hoa hồng", Description = "Bộ sưu tập hoa hồng cao cấp" },
            new Flower.Data.Entities.CategoryProduct { Name = "Hoa cúc", Description = "Hoa cúc tươi thắm" },
            new Flower.Data.Entities.CategoryProduct { Name = "Hoa lan", Description = "Lan nhiệt đới sang trọng" },
            new Flower.Data.Entities.CategoryProduct { Name = "Hoa mùa xuân", Description = "Sắc xuân rực rỡ" },
            new Flower.Data.Entities.CategoryProduct { Name = "Bó hoa cưới", Description = "Bó hoa cô dâu" }
        );
        context.SaveChanges();
    }

    if (!context.Products.Any())
    {
        var hoaHong = context.CategoriesProducts.First(c => c.Name == "Hoa hồng").Id;
        var hoaCuc = context.CategoriesProducts.First(c => c.Name == "Hoa cúc").Id;
        var hoaLan = context.CategoriesProducts.First(c => c.Name == "Hoa lan").Id;
        var muaXuan = context.CategoriesProducts.First(c => c.Name == "Hoa mùa xuân").Id;
        var cuoi = context.CategoriesProducts.First(c => c.Name == "Bó hoa cưới").Id;

        context.Products.AddRange(
            new Flower.Data.Entities.Product { Name = "Hoa hồng đỏ tình yêu", Price = 350000, StockQuantity = 50, CategoryProductId = hoaHong, ImageUrl = "https://images.unsplash.com/photo-1563241527-3004b7be0ffd?w=600", Description = "Hoa hồng đỏ nhập khẩu, tươi lâu." },
            new Flower.Data.Entities.Product { Name = "Hoa hồng phấn pastel", Price = 280000, StockQuantity = 30, CategoryProductId = hoaHong, ImageUrl = "https://images.unsplash.com/photo-1518621736915-f3b1c41bfd00?w=600", Description = "Sắc hồng phấn nhẹ nhàng, lãng mạn." },
            new Flower.Data.Entities.Product { Name = "Cúc họa mi trắng", Price = 180000, StockQuantity = 0, CategoryProductId = hoaCuc, ImageUrl = "https://images.unsplash.com/photo-1496062031456-07b8f162a322?w=600", Description = "Cúc họa mi trắng tinh khôi." },
            new Flower.Data.Entities.Product { Name = "Lan hồ điệp tím", Price = 550000, StockQuantity = 20, CategoryProductId = hoaLan, ImageUrl = "https://images.unsplash.com/photo-1567095761054-7a02e69e5c43?w=600", Description = "Lan hồ điệp tím cao cấp trong chậu gốm." },
            new Flower.Data.Entities.Product { Name = "Bó hoa xuân rực rỡ", Price = 420000, StockQuantity = 15, CategoryProductId = muaXuan, ImageUrl = "https://images.unsplash.com/photo-1463648067503-3e0e1e48b3c0?w=600", Description = "Hoa xuân tươi thắm nhiều sắc màu." },
            new Flower.Data.Entities.Product { Name = "Bó hoa cưới cổ điển", Price = 890000, StockQuantity = 10, CategoryProductId = cuoi, ImageUrl = "https://images.unsplash.com/photo-1519378058457-4c29c1e4c76c?w=600", Description = "Bó hoa cưới hồng - trắng thanh lịch." },
            new Flower.Data.Entities.Product { Name = "Hoa hồng xanh dương", Price = 450000, StockQuantity = 25, CategoryProductId = hoaHong, ImageUrl = "https://images.unsplash.com/photo-1597733336794-12d05021d510?w=600", Description = "Hoa hồng xanh dương độc đáo." },
            new Flower.Data.Entities.Product { Name = "Lan vũ nữ vàng", Price = 380000, StockQuantity = 12, CategoryProductId = hoaLan, ImageUrl = "https://images.unsplash.com/photo-1567427017947-545c5f8d16ad?w=600", Description = "Lan vũ nữ vàng rực rỡ." }
        );
        context.SaveChanges();
    }

    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Flower.Data.Entities.Category { Name = "Chia sẻ", Description = "Chia sẻ kinh nghiệm và câu chuyện hoa" },
            new Flower.Data.Entities.Category { Name = "Mẹo & Thủ thuật", Description = "Mẹo chăm hoa tươi lâu" },
            new Flower.Data.Entities.Category { Name = "Sự kiện", Description = "Sự kiện và chương trình đặc biệt" }
        );
        context.SaveChanges();
    }

    if (!context.Posts.Any())
    {
        var share = context.Categories.First(c => c.Name == "Chia sẻ").Id;
        var tips = context.Categories.First(c => c.Name == "Mẹo & Thủ thuật").Id;

        context.Posts.AddRange(
            new Flower.Data.Entities.Post { Title = "Nghệ thuật cắm hoa Nhật Bản – Ikebana", Content = "<p>Ikebana là nghệ thuật cắm hoa truyền thống của Nhật Bản, nơi mỗi cành hoa đều mang một ý nghĩa riêng.</p><p>Bài viết này sẽ giúp bạn hiểu thêm về triết lý đằng sau những tác phẩm Ikebana tinh tế.</p>", Summary = "Khám phá triết lý và kỹ thuật của nghệ thuật cắm hoa Ikebana.", ImageUrl = "https://images.unsplash.com/photo-1567696917-6ba0c5d0b761?w=800", CategoryId = share, CreatedDate = DateTime.Parse("2026-06-15") },
            new Flower.Data.Entities.Post { Title = "Mẹo giữ hoa tươi lâu đến 2 tuần", Content = "<p>Bạn có biết chỉ cần thay nước mỗi ngày và cắt gốc chéo 45 độ là hoa có thể tươi đến 2 tuần?</p><p>Hãy cùng tìm hiểu thêm nhiều mẹo hay khác trong bài viết này!</p>", Summary = "Bí quyết đơn giản giúp hoa luôn tươi thắm.", ImageUrl = "https://images.unsplash.com/photo-1508610030471-5eba6c0a15a5?w=800", CategoryId = tips, CreatedDate = DateTime.Parse("2026-06-10") },
            new Flower.Data.Entities.Post { Title = "Ý nghĩa các loài hoa trong văn hóa Việt", Content = "<p>Mỗi loài hoa đều gắn liền với những câu chuyện và biểu tượng riêng trong văn hóa Việt Nam.</p><p>Từ hoa sen tượng trưng cho sự thanh cao đến hoa đào báo hiệu mùa xuân.</p>", Summary = "Tìm hiểu ý nghĩa đặc biệt của các loài hoa trong văn hóa người Việt.", ImageUrl = "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?w=800", CategoryId = share, CreatedDate = DateTime.Parse("2026-06-05") }
        );
        context.SaveChanges();
    }
}

app.Run();