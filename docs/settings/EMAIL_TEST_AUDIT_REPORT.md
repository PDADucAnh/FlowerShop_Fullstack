# Báo Cáo Kiểm Tra Chức Năng Gửi Thử Email (Test SMTP Email Audit Report)

Báo cáo phân tích chuyên sâu về lỗi chức năng "Gửi thử" email trong trang cấu hình hệ thống (Settings) của dự án FlowerShop.

---

## 1. Luồng Thực Thi Hiện Tại (Execution Flow)

Luồng thực thi hiện tại của chức năng "Gửi thử" email được mô tả qua sơ đồ và các bước dưới đây:

### Sơ đồ luồng:
```
View (Views/Settings/Index.cshtml)
↓ (AJAX POST qua fetch /Settings/TestEmail)
Controller (SettingsController.cs -> TestEmail Action)
↓ (Không gọi bất kỳ Service nào - Trả về kết quả thành công giả lập)
Browser / UI (Hiển thị thông báo "Gửi email thử nghiệm thành công")
```

### Chi tiết các bước:
1. **Giao diện (View)**: Người dùng nhập địa chỉ email nhận tại tab SMTP Email và bấm nút "Gửi thử" (`<button type="button" id="btn-test-email">` tại dòng 115 của [Index.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/Settings/Index.cshtml)).
2. **Yêu cầu AJAX**: Script Javascript bắt sự kiện click, đóng gói `toEmail` và mã thông báo chống giả mạo request (`__RequestVerificationToken`), sau đó gửi một yêu cầu HTTP POST qua `fetch` tới URL `/Settings/TestEmail` (dòng 280).
3. **Xử lý tại Controller**: Yêu cầu được tiếp nhận bởi action `TestEmail` của `SettingsController.cs` (dòng 171).
4. **Kết quả**: Action này **chỉ trả về một đối tượng JSON thành công giả lập** mà không hề chuyển tiếp yêu cầu hay thực hiện cuộc gọi nào đến dịch vụ gửi email `EmailService` hoặc đối tượng kết nối SMTP `SmtpClient`.

---

## 2. Danh Sách Các File Đã Kiểm Tra (Audited Files)

Quá trình rà soát được thực thi qua các tập tin cốt lõi sau:
1. **[Views/Settings/Index.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/Settings/Index.cshtml)**: Giao diện cấu hình và mã AJAX gửi yêu cầu.
2. **[SettingsController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/SettingsController.cs)**: Controller điều hướng và xử lý yêu cầu cấu hình.
3. **[EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs)**: Dịch vụ gửi email hệ thống.
4. **[SystemSettingService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/SystemSettingService.cs)**: Dịch vụ truy xuất/lưu trữ cấu hình trong CSDL.
5. **[Program.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs)**: Đăng ký dịch vụ DI của dự án.
6. **[appsettings.json](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/appsettings.json)**: Tệp chứa cấu hình tĩnh.

---

## 3. Các Dòng Code Quan Trọng & Phân Tích

Tại [SettingsController.cs:L171-L186](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/SettingsController.cs#L171-L186):

```csharp
171:         public async Task<IActionResult> TestEmail(string toEmail)
172:         {
173:             if (string.IsNullOrEmpty(toEmail))
174:             {
175:                 return Json(new { success = false, message = "Email nhận không hợp lệ." });
176:             }
177: 
178:             try
179:             {
180:                 return Json(new { success = true, message = $"Gửi email thử nghiệm tới {toEmail} thành công!" });
181:             }
182:             catch (Exception ex)
183:             {
184:                 return Json(new { success = false, message = $"Lỗi gửi email: {ex.Message}" });
185:             }
186:         }
```

### Phân tích:
- **Dòng 180**: Action trực tiếp trả về `Json(new { success = true, ... })` mà không có bất kỳ lệnh gọi nghiệp vụ gửi email nào (như `await _emailService.SendEmailAsync(...)`).
- Do khối `try` chỉ chứa duy nhất lệnh trả về này nên khối `catch` (dòng 182-185) không bao giờ được kích hoạt, dẫn đến console của ứng dụng hoàn toàn im lặng và không xuất hiện lỗi SMTP.

---

## 4. Các Điểm Bất Thường Phát Hiện (Anomalies)

1. **Action bị Mock hoàn toàn**: `SettingsController.TestEmail` không được liên kết với `IEmailService`.
2. **Nuốt ngoại lệ trong EmailService**:
   Một số phương thức gửi thư của `EmailService.cs` (như gửi email đang giao hàng `SendOrderShippingEmailAsync` hoặc đơn hàng hoàn tất) bọc trong try-catch nhưng khi xảy ra lỗi kết nối SMTP, hệ thống chỉ ghi log cảnh báo (`_logger.LogWarning`) và hoàn thành Task bình thường mà **không ném (throw) lỗi lên**. Nếu sau này tích hợp gọi service này vào Controller, Controller vẫn sẽ nhận được kết quả thành công giả dù email gửi thất bại.
3. **Thiếu cấu hình Ssl mặc định**:
   Gmail SMTP yêu cầu mã hóa SSL/TLS chặt chẽ. Việc không định nghĩa rõ cấu hình `DeliveryMethod` hoặc `Timeout` cụ thể có thể dẫn đến việc ứng dụng bị treo (block thread) nếu kết nối tới máy chủ Google bị chặn bởi tường lửa.

---

## 5. Nguyên Nhân Gốc (Root Cause)

Chức năng "Gửi thử" email hoạt động ở mức giao diện nhưng hộp thư không nhận được thư và không báo lỗi vì **mã nguồn xử lý Backend (`SettingsController.TestEmail`) hiện tại chỉ là một phương thức giả lập (mock method)**. Phương thức này không hề gọi dịch vụ email thực tế mà trực tiếp trả về trạng thái `"thành công"` ngay lập tức.

---

## 6. Mức Độ Ảnh Hưởng (Impact Level)

- **Mức độ**: **Trung bình - Cao** (Về mặt vận hành hệ thống).
- **Chi tiết**: Người quản lý hệ thống (Admin) sẽ bị đánh lừa rằng cấu hình SMTP của họ đã chính xác và hoạt động tốt, trong khi thực tế cấu hình đó có thể đang sai (sai máy chủ, sai mật khẩu ứng dụng Gmail, cổng kết nối bị chặn). Điều này dẫn đến việc khách hàng không nhận được OTP đặt hàng hoặc email thông báo hóa đơn mà người quản trị không hề hay biết.

---

## 7. Xếp Hạng Nguyên Nhân Gây Lỗi Nhận Thư (Probability Ranking)

Dưới đây là bảng xếp hạng nguyên nhân khiến email kiểm thử thực tế không thể gửi đi (nếu chức năng này được đấu nối thực tế):

| Hạng | Xác suất | Nguyên nhân chi tiết | Mô tả kỹ thuật |
| :---: | :---: | :--- | :--- |
| **1** | **100%** | **Controller bị giả lập (Mock)** | Action `TestEmail` không gọi dịch vụ gửi thư (Nguyên nhân gốc hiện tại). |
| **2** | **85%** | **Xác thực SMTP thất bại** | Tài khoản Gmail sử dụng mật khẩu chính thay vì **Mật khẩu ứng dụng (App Password)** 16 ký tự. Google đã chặn xác thực bằng mật khẩu thông thường từ năm 2022. |
| **3** | **70%** | **Sai cổng kết nối (Port) hoặc SSL** | Cấu hình SMTP Port của Gmail không đúng (yêu cầu Port `587` cho TLS/STARTTLS hoặc `465` cho SSL). |
| **4** | **40%** | **Mật khẩu giải mã sai** | Khóa mã hóa `DataProtection` bị thay đổi hoặc mật khẩu lưu trong DB bị hỏng khiến quá trình giải mã trả về chuỗi rỗng/rác. |
| **5** | **15%** | **Lỗi tường lửa/Nhà mạng** | Cổng gửi thư `587` hoặc `465` của nhà cung cấp dịch vụ đám mây (Cloud VPS) bị chặn chiều gửi đi (Outbound blocking). |

---

## 8. Giải Pháp Khắc Phục Đề Xuất (Proposed Fixes)

Để sửa đổi lỗi này khi được phép chỉnh sửa mã nguồn:
1. **Đấu nối dịch vụ Email**:
   Cập nhật `SettingsController` để tiêm `IEmailService` và gọi dịch vụ gửi email kiểm thử thực tế:
   ```csharp
   // Đấu nối thật
   await _emailService.SendEmailAsync(toEmail, "Kiểm thử SMTP - FlowerShop", "Đây là email gửi thử nghiệm để xác thực cấu hình SMTP hoạt động chính xác.");
   ```
2. **Định nghĩa phương thức gửi thư chung trả về kết quả**:
   Bổ sung phương thức gửi thư có trả về trạng thái (`bool success` hoặc ném lại ngoại lệ khi lỗi) trong `IEmailService` để Controller bắt được ngoại lệ thực tế và phản hồi chính xác mã lỗi SMTP lên giao diện cho người dùng.
3. **Kiểm tra giá trị thực tế trong Database**:
   Yêu cầu kỹ thuật viên kiểm tra trực tiếp bảng `SystemSettings` khóa `Smtp` xem giá trị `Password` sau khi giải mã có đúng là Mật khẩu ứng dụng (App Password) của Gmail hay không.
