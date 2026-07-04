# Tài liệu Đặc tả Thiết kế: Chức năng Quên Mật khẩu (Forgot Password)

Tài liệu này đặc tả chi tiết kiến trúc, cơ sở dữ liệu, API Backend và giao diện Frontend cho tính năng đặt lại mật khẩu cho thực thể **Customer** bằng cơ chế **One-Time Reset Token**.

---

## 1. Thiết kế Cơ sở dữ liệu (Database Design)

Bổ sung 2 cột mới vào bảng `Customers` của SQL Server để lưu trữ trạng thái token đặt lại mật khẩu tạm thời:

```sql
ALTER TABLE Customers 
ADD ResetToken NVARCHAR(100) NULL,
    ResetTokenExpiry DATETIME NULL;
```

### Cập nhật Model Entity `Customer.cs` (`CMS.Data/Entities/Customer.cs`)
```csharp
[MaxLength(100)]
public string? ResetToken { get; set; }

public DateTime? ResetTokenExpiry { get; set; }
```

---

## 2. API Backend & Nghiệp vụ (ASP.NET Core)

### A. Interface & Service Authentication

#### `IAuthService.cs` (`CMS.Backend/Services/Interfaces/IAuthService.cs`)
```csharp
Task<(bool Success, string Message)> ForgotPassword(string email, string clientUrl);
Task<(bool Success, string Message)> ResetPassword(string token, string newPassword);
```

#### `AuthService.cs` (`CMS.Backend/Services/AuthService.cs`)
*   **`ForgotPassword(string email, string clientUrl)`**:
    1. Tìm kiếm `Customer` theo `email`.
    2. Nếu không tìm thấy: Trả về `(true, "Nếu email tồn tại trên hệ thống, một liên kết đặt lại mật khẩu đã được gửi đi. Vui lòng kiểm tra hộp thư.")` (Chống dò quét Email).
    3. Nếu tìm thấy:
        * Sinh mã GUID: `string token = Guid.NewGuid().ToString("N");`
        * Thiết lập thời gian hết hạn: `ResetTokenExpiry = DateTime.Now.AddMinutes(15);` (TTL: 15 phút).
        * Cập nhật vào DB.
        * Tạo liên kết: `string resetLink = $"{clientUrl}/reset-password?token={token}";`
        * Gọi `IEmailService.SendResetPasswordEmailAsync` để gửi thư.
        * Trả về kết quả thành công.

*   **`ResetPassword(string token, string newPassword)`**:
    1. Tìm kiếm `Customer` có `ResetToken == token`.
    2. Nếu `customer == null` -> Trả về `(false, "Mã xác thực không hợp lệ.")`.
    3. Nếu `customer.ResetTokenExpiry < DateTime.Now` -> Trả về `(false, "Liên kết đã hết hạn, vui lòng yêu cầu lại.")`.
    4. Tiến hành đổi mật khẩu:
        * Mã hóa mật khẩu mới: `customer.PasswordHash = _customerPasswordHasher.HashPassword(customer, newPassword);`
        * Vô hiệu hóa token đã dùng: `customer.ResetToken = null; customer.ResetTokenExpiry = null;`
        * Lưu thay đổi vào Database.
        * Trả về `(true, "Đặt lại mật khẩu thành công!")`.

### B. Email Service (`EmailService.cs`)

#### `IEmailService.cs` & `EmailService.cs`
```csharp
Task SendResetPasswordEmailAsync(string email, string name, string resetLink);
```
*   **Template Email**:
    *   Định dạng HTML chuẩn, sử dụng logo `AnhCMS Boutique` và các nút bấm chuyên nghiệp với màu sắc hồng cánh sen chủ đạo `#ab2c5d`.
    *   Nút bấm dẫn trực tiếp tới `resetLink`.

### C. Auth Controller Endpoints (`AuthController.cs`)

```csharp
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
{
    // clientUrl được gửi từ frontend hoặc tự động lấy từ Origin Header
    var clientUrl = Request.Headers["Origin"].ToString();
    if (string.IsNullOrEmpty(clientUrl)) clientUrl = "http://localhost:5173"; // fallback

    var (success, message) = await _authService.ForgotPassword(request.Email, clientUrl);
    return Ok(new { message });
}

[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
{
    var (success, message) = await _authService.ResetPassword(request.Token, request.NewPassword);
    if (!success) return BadRequest(new { message });
    return Ok(new { message });
}
```

---

## 3. Giao diện và Luồng Frontend (React SPA)

### A. Định tuyến mới trong `App.tsx`
```tsx
import ForgotPassword from './pages/forgot-password';
import ResetPassword from './pages/reset-password';

// Thêm routes
<Route path="/forgot-password" element={<ForgotPassword />} />
<Route path="/reset-password" element={<ResetPassword />} />
```

### B. Trang Yêu cầu đặt lại mật khẩu (`pages/forgot-password/index.tsx`)
*   Hiển thị biểu mẫu nhập Email.
*   Nút "Gửi yêu cầu" gọi API `POST /api/Auth/forgot-password`.
*   Hiển thị thông báo hướng dẫn khách kiểm tra hòm thư.

### C. Trang Đặt lại mật khẩu (`pages/reset-password/index.tsx`)
*   Bóc tách `token` từ URL query (`?token=...`).
*   Form nhập **Mật khẩu mới** và **Xác nhận mật khẩu**.
*   Khi submit thành công, gọi API `POST /api/Auth/reset-password`.
*   Thông báo thành công và chuyển hướng về trang `/login` sau 3 giây.
