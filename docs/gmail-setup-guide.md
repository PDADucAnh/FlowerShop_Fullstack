# Hướng dẫn cấu hình Gmail SMTP

## 1. Lấy App Password từ Google

1. Bật 2-Step Verification tại [Google Security](https://myaccount.google.com/security)
2. Vào thẳng [App Passwords](https://myaccount.google.com/apppasswords)
3. Chọn **Mail** + **Windows Computer** → Generate
4. Copy mật khẩu 16 chữ được hiện ra

> Lưu ý: App Password chỉ hiện **1 lần duy nhất** sau khi tạo.

## 2. Cấu hình (chọn 1 trong 2 cách)

### Cách A — User Secrets (khuyên dùng, an toàn hơn)

Chạy từ terminal tại thư mục `Flower.Backend/`:

```bash
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

Chạy **1 lần duy nhất**, dữ liệu lưu vĩnh viễn trên máy, không bị mất khi restart.

### Cách B — Thêm trực tiếp vào appsettings.json (chạy tạm)

Mở `Flower.Backend/appsettings.json`, sửa phần `EmailSettings` thành:

```json
"EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "SenderEmail": "noreply@flowershop.com",
    "SenderName": "FlowerShop",
    "EnableSsl": true
}
```

> **Cảnh báo:** Không commit file có chứa mật khẩu lên git. Sau khi chạy thử, chuyển qua User Secrets.

## 3. Kiểm tra hoạt động

Khi nào app gọi `IEmailService.SendEmailAsync()` hoặc `SendOtpEmailAsync()`, nếu thiếu cấu hình hệ thống vẫn chạy bình thường nhưng không gửi được mail (log warning).

## 4. File liên quan

- `Flower.Backend/Services/EmailService.cs` — implementation SMTP
- `Flower.Backend/appsettings.json` — cấu hình EmailSettings
- `Flower.Backend/Models/EmailSettings.cs` — model mapping
