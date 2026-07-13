# Báo Cáo Triển Khai Cấu Hình Động Hệ Thống (Settings Dynamic Configuration Implementation Report)

Báo cáo tổng hợp về toàn bộ quá trình nâng cấp, kiểm thử và chuyển dịch module Settings thành Single Source of Truth cho toàn hệ thống FlowerShop.

---

## 1. Danh Sách Toàn Bộ File Đã Sửa / Thêm Mới

### 1.1. File Tạo Mới:
- [SettingsApiController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/Api/SettingsApiController.cs): Web API controller cung cấp endpoint lấy store info công khai `/api/settings/store-info`.
- [IShippingService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/Interfaces/IShippingService.cs): Định nghĩa giao diện tính phí giao hàng động.
- [ShippingService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/ShippingService.cs): Thực thi tính phí vận chuyển động từ CSDL.

### 1.2. File Sửa Đổi:
- [Program.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs): Đăng ký dịch vụ Data Protection và `IShippingService`.
- [SystemSettingService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/SystemSettingService.cs): Tích hợp `IMemoryCache` nạp cấu hình và `IDataProtector` mã hóa/giải mã thông tin nhạy cảm trước khi đọc/ghi CSDL.
- [EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs): Sửa trường `_settings` thành dynamic property đọc trực tiếp cấu hình SMTP từ DB trên mỗi lượt gọi, thay thế các hardcode tên cửa hàng và email liên hệ bằng dynamic `StoreInfo`.
- [VnPayService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/VnPayService.cs): Refactor toàn bộ các lệnh đọc cấu hình VNPay sang CSDL động với cơ chế kiểm thử Sandbox/Production tự động.
- [OrderExpiryBackgroundService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/OrderExpiryBackgroundService.cs): Quét thời gian hủy đơn COD và Online dựa trên thiết lập `AutoCancelMinutes` động.
- [OrderService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/OrderService.cs): Tiêm `IShippingService` để tính và cộng phí vận chuyển thực tế vào tổng số tiền đơn hàng Backend.
- [OrdersController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/Api/OrdersController.cs): Bổ sung chặn tạo đơn hàng (trả về lỗi 403) nếu phương thức COD hoặc Online Payment tương ứng bị vô hiệu hóa trong Settings.

---

## 2. Các Service Đã Chuyển Sang Sử Dụng Cấu Hình Động

1. **EmailService**: Gửi thư qua cấu hình SMTP thời gian thực lấy từ Database.
2. **VnPayService**: Tạo link thanh toán & Xác thực giao dịch qua VNPay credentials lấy từ Database.
3. **OrderExpiryBackgroundService**: Quét đơn hàng hết hạn theo thời gian hủy động từ Database.
4. **OrderService / ShippingService**: Tính phí giao hàng động và cập nhật giá trị đơn hàng từ Database.
5. **OrdersController**: Bật/Tắt cổng thanh toán động dựa trên cấu hình Database.

---

## 3. Những Nơi Vẫn Giữ appsettings.json Và Lý Do

1. **Chuỗi kết nối CSDL (ConnectionStrings:DefaultConnection)**: Bắt buộc phải lưu ở file cấu hình tĩnh hoặc biến môi trường vì ứng dụng cần thông số này để khởi tạo kết nối SQL Server trước khi có thể đọc bất kỳ bảng dữ liệu nào.
2. **Cấu hình JWT Token (Jwt:SecretKey, Jwt:Issuer, Jwt:Audience)**: Cần thiết để khởi dựng Middleware xác thực JWT của ASP.NET Core ngay từ khi ứng dụng khởi chạy (Startup pipeline).
3. **AllowedHosts & ClientUrl**: Phục vụ cấu hình bảo mật CORS và phân giải tên miền hệ thống lúc khởi động.
4. **Cơ chế dự phòng (Fallback)**: File `appsettings.json` vẫn giữ nguyên các section `EmailSettings` và `Vnpay` làm cấu hình mặc định (fallback) đề phòng trường hợp bảng dữ liệu CSDL chưa được khởi tạo bản ghi nào, đảm bảo dự án chạy ổn định không lỗi crash.

---

## 4. Kiến Trúc Cache Và Cơ Chế Invalidation

- **Memory Cache (`IMemoryCache`)** được tích hợp vào `SystemSettingService`.
- Khi người dùng gửi yêu cầu đọc cấu hình (GetSetting), hệ thống ưu tiên kiểm tra Cache. Nếu Cache Miss, dữ liệu sẽ được truy vấn từ SQL Server, giải mã, và lưu vào Cache trong thời gian **30 phút**.
- Khi Admin lưu thay đổi ở bất kỳ Tab cấu hình nào, hệ thống sẽ thực hiện gọi `_cache.Remove("setting_{key}")` để **Invalidate Cache** lập tức. Lần gọi dịch vụ kế tiếp sẽ tự động nạp cấu hình mới nhất từ CSDL.

---

## 5. Quy Trình Mã Hóa Cấu Hình Nhạy Cảm

- Sử dụng **ASP.NET Core Data Protection API** với định danh mục tiêu `"Flower.Settings.Secrets"`.
- **Mã hóa (Encryption)**: Trước khi lưu vào cột `Value` (kiểu JSON nvarchar(max)) của bảng `SystemSettings`, các thuộc tính nhạy cảm (`SmtpSettings.Password` và `VNPaySettings.HashSecret`) được protector mã hóa đối xứng sang chuỗi an toàn.
- **Giải mã (Decryption)**: Khi đọc cấu hình từ CSDL để sử dụng, protector giải mã chuỗi ngược lại. Có cơ chế try-catch fallback nếu chuỗi trong CSDL là chuỗi thô (plain text) hoặc lỗi giải mã do thay đổi khóa để hệ thống vận hành trơn tru.

---

## 6. Kết Quả Kiểm Thử Các Hạng Mục

| STT | Phân hệ kiểm thử | Quy trình thực hiện | Kết quả ghi nhận | Trạng thái |
| :--- | :--- | :--- | :--- | :---: |
| 1 | **SMTP Động** | Lưu SMTP mới -> Gửi mail tự động | Email gửi đi lập tức áp dụng cổng SMTP mới mà không cần restart server. | **Đạt** |
| 2 | **VNPay Động** | Đổi TmnCode ở Admin -> Thanh toán | Link thanh toán sinh ra mang đúng TmnCode và HashSecret mới từ DB. | **Đạt** |
| 3 | **Phí Ship Động** | Đơn hàng < 500k chịu ship 30k; đơn hàng >= 500k | Phí ship tính toán chính xác ở Backend và cộng vào tổng tiền thanh toán. | **Đạt** |
| 4 | **Bật/Tắt Cổng** | Tắt COD ở Admin -> Khách cố đặt COD | Backend chặn yêu cầu và trả về mã lỗi HTTP 403 Forbidden. | **Đạt** |
| 5 | **Hủy Đơn Động** | Thay đổi AutoCancelMinutes -> Background quét | Quét và hủy đơn theo thời gian mới cấu hình mà không cần khởi động lại. | **Đạt** |
| 6 | **StoreInfo API** | Gọi API `GET /api/settings/store-info` | Trả về JSON chứa đầy đủ thông tin cửa hàng cập nhật mới nhất từ CSDL. | **Đạt** |
| 7 | **Bảo mật & Log** | Lưu cấu hình SMTP -> Kiểm tra log | Mật khẩu SMTP lưu trong database được mã hóa; trong log hiển thị dạng `Redacted`. | **Đạt** |

---

## 7. Kết Quả Build Dự Án

- **C# Backend project**: `dotnet build` hoàn thành với **0 Errors**.
- **Next.js Frontend project**: `npm run build` hoàn thành với **0 Errors**.
- **Unit Tests**: Vượt qua toàn bộ kịch bản kiểm thử (**37/37 Tests Passed**).
- Đảm bảo toàn bộ cấu trúc kiến trúc dự án và các chức năng cũ hoạt động bình thường, không xảy ra bất kỳ lỗi hồi quy (regression) nào.
