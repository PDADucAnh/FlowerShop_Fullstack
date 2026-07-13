# Báo Cáo Bảo Mật Cấu Hình Nhạy Cảm (Settings Security Report)

Tài liệu chi tiết về giải pháp mã hóa dữ liệu cấu hình nhạy cảm và bảo mật thông tin tài khoản quản trị trong hệ thống FlowerShop.

---

## 1. Nguy Cơ Bảo Mật Dữ Liệu Tĩnh (Plain Text Leakage)

Trong các thiết kế thông thường, mật khẩu SMTP và khóa bảo mật HashSecret của cổng thanh toán VNPay thường được lưu trữ dưới dạng chuỗi thô (Plain Text) tại file `appsettings.json` hoặc trong CSDL. Điều này dẫn đến nguy cơ lớn:
- Kỹ thuật viên hoặc người có quyền truy cập CSDL có thể xem trộm mật khẩu SMTP để spam mail.
- Kẻ tấn công khai thác lỗ hổng SQL Injection có thể lấy cắp khóa HashSecret VNPay để tạo các giao dịch giả mạo thanh toán.

---

## 2. Giải Pháp Mã Hóa Bằng ASP.NET Core Data Protection

Hệ thống FlowerShop đã áp dụng giải pháp mã hóa động sử dụng **ASP.NET Core Data Protection API**:
- **Cơ chế mã hóa đối xứng**: Khi lưu dữ liệu nhạy cảm, hệ thống sử dụng một `IDataProtector` với định danh mục tiêu `"Flower.Settings.Secrets"` để mã hóa thông tin thô trước khi lưu vào cột `Value` trong bảng `SystemSettings`.
- **Cơ chế giải mã tự động**: Khi nạp dữ liệu cấu hình từ CSDL lên bộ nhớ đệm, protector sẽ thực hiện giải mã ngược lại để trả về thông số thật cho các dịch vụ gửi email hoặc thanh toán.
- **Tự phục hồi lỗi**: Nếu dữ liệu trong DB là chuỗi thô chưa mã hóa (do nâng cấp từ hệ thống cũ) hoặc khóa bảo mật của Data Protection bị thay đổi, hệ thống sẽ tự động bắt ngoại lệ và dùng lại chuỗi thô làm fallback để tránh crash hệ thống.

```csharp
private string SafeDecrypt(string encryptedText)
{
    if (string.IsNullOrEmpty(encryptedText)) return string.Empty;
    try {
        return _protector.Unprotect(encryptedText);
    } catch {
        return encryptedText; // Fallback nếu chuỗi chưa mã hóa
    }
}
```

---

## 3. Bảo Vệ Hiển Thị Và Log Nhật Ký (Secrets Masking & Audit Logging)

### 3.1. Che dấu thông tin trên Giao diện (UI Masking)
- Khi quản trị viên truy cập trang cấu hình ở Admin, các trường mật khẩu SMTP và HashSecret VNPay sẽ được che dấu hoàn toàn dưới chuỗi mặt nạ `••••••••••••`.
- Nếu Admin không thay đổi các trường này (giữ nguyên mặt nạ `••••••••••••`), Controller sẽ tự động giữ lại giá trị cũ đã mã hóa trong CSDL mà không ghi đè chuỗi mặt nạ vào DB.

### 3.2. Không log dữ liệu nhạy cảm (Zero Sensitive Logging)
- Hệ thống ghi nhận mọi hoạt động sửa đổi cấu hình của Admin qua Audit Log.
- Tuy nhiên, trước khi chuyển đổi đối tượng cấu hình thành chuỗi JSON để lưu nhật ký thông qua `_logger.LogInformation`, các thuộc tính nhạy cảm như `Password` và `HashSecret` đều bị ghi đè cứng thành chuỗi `"Redacted"` hoặc `"******"`.
- Đảm bảo tuyệt đối không có thông tin mật khẩu hay khóa bảo mật nào bị rò rỉ vào file log hoặc console của ứng dụng.
- Ví dụ log mẫu:
  `AUDIT LOG: User admin@flowershop.com updated SMTP Settings. Old Value: {"Host":"smtp.gmail.com","Port":587,"Username":"pdahoctap@gmail.com","Password":"Redacted"...}`
