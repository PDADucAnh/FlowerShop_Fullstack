# Báo Cáo Kiểm Tra Và Đánh Giá Hệ Thống Cấu Hình (Settings Audit & Verification Report)

Báo cáo chi tiết về kết quả audit toàn diện module Settings (Cấu hình) thuộc hệ thống FlowerShop, rà soát tính nhất quán giữa dữ liệu được lưu trữ trong cơ sở dữ liệu với nghiệp vụ thực tế đang chạy trên Backend và Frontend.

---

## 1. Kiến Trúc Settings Hiện Tại

Hiện tại, hệ thống đã xây dựng:
1. **Giao diện Admin MVC** tại `SettingsController` hiển thị form nhập cấu hình.
2. **Cơ chế lưu trữ động** tại `SystemSettingService` giúp chuyển đổi dữ liệu cấu hình thành định dạng JSON và lưu vào bảng `SystemSettings` trong SQL Server.
3. **Tuy nhiên**, qua kiểm tra chi tiết mã nguồn Backend và Frontend, các nghiệp vụ thực tế (gửi mail, thanh toán VNPay, tính phí vận chuyển, tự động hủy đơn) **chưa đọc cấu hình từ Database** mà vẫn đang sử dụng cấu hình tĩnh trong file `appsettings.json` (thông qua `IConfiguration` hoặc `IOptions`) hoặc bị hardcode trực tiếp trong mã nguồn.

---

## 2. Database

- **Trạng thái**: Hoạt động đúng kỹ thuật lưu trữ.
- Bảng `SystemSettings` được EF Core tạo thành công với cấu trúc khóa chính `Key` và trường `Value` kiểu `nvarchar(max)`.
- Khi Admin nhấn "Lưu cấu hình", dữ liệu của toàn bộ tab được chuyển dịch thành chuỗi JSON tương ứng và cập nhật thành công vào CSDL.
- Khi tải lại trang, `SystemSettingService.GetAllSettings()` đọc từ CSDL và giải mã JSON chính xác để hiển thị lại thông tin đã lưu.

---

## 3. Phân Tích Dữ Liệu SystemSettings

Các cấu hình được tổ chức lưu trữ dưới dạng JSON với 5 nhóm Key chính:
- **`StoreInfo`**: Chứa thông tin tên cửa hàng, logo, địa chỉ, hotline, zalo, facebook, giờ mở cửa.
- **`Smtp`**: Chứa thông tin máy chủ SMTP phục vụ gửi thư tự động.
- **`VNPay`**: Chứa cấu hình kết nối cổng thanh toán.
- **`Shipping`**: Chứa phí giao hàng mặc định, ngưỡng miễn phí ship và thời gian giao nhận.
- **`Order`**: Chứa thời gian tự hủy đơn, cấu hình cho phép COD và Online Payment.

---

## 4. Kiểm Tra Nhóm: Thông Tin Cửa Hàng (StoreInfo)

- **Hiện trạng**: **Chưa hoạt động thực tế**.
- **Chi tiết kiểm tra**:
  - **Header & Footer** trên Next.js: Tên cửa hàng `"FlowerShop"` và email hỗ trợ `"support@flowershop.retail"` đang bị hardcode tĩnh trong [Header.tsx](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/components/Header.tsx) và [Footer.tsx](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/components/Footer.tsx).
  - **Trang liên hệ (Contact Page)** trên Next.js: Địa chỉ `"123 Đường Hoa, Q.1"`, hotline, email liên hệ `"hello@pdaflower.com"` và giờ mở cửa đang bị hardcode tĩnh hoàn toàn trong [ContactPage](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/pages/contact/index.tsx).
  - **Email Templates**: Email footer trong [EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs#L200) và [EmailService.cs:L540](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs#L540) vẫn đang sử dụng chuỗi hardcode `"support@flowershop.com"`.

---

## 5. Kiểm Tra Nhóm: SMTP Email

- **Hiện trạng**: **Chưa hoạt động thực tế (Chỉ mang tính giao diện)**.
- **Chi tiết kiểm tra**:
  - Dịch vụ [EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs) kế thừa cấu hình thông qua `IOptions<EmailSettings>`.
  - Cấu hình này được nạp tĩnh một lần duy nhất lúc khởi động ứng dụng tại [Program.cs:L157](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs#L157) từ section `"EmailSettings"` của tệp `appsettings.json`.
  - Toàn bộ email hệ thống (OTP, Xác nhận đơn hàng, Đang giao hàng, Đơn hàng hoàn thành, Reset mật khẩu) đều đọc từ tệp cấu hình này, hoàn toàn bỏ qua cấu hình SMTP trong CSDL.

---

## 6. Kiểm Tra Nhóm: Cổng VNPay

- **Hiện trạng**: **Chưa hoạt động thực tế (Chỉ mang tính giao diện)**.
- **Chi tiết kiểm tra**:
  - [VnPayService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/VnPayService.cs) thực thi tạo URL thanh toán và giải mã kết quả trả về bằng cách đọc trực tiếp từ `IConfiguration` đối với các key `"Vnpay:TmnCode"`, `"Vnpay:HashSecret"`, `"Vnpay:PaymentBackReturnUrl"`, v.v.
  - Các cấu hình cổng VNPay lưu tại CSDL không hề được tham chiếu hay sử dụng trong quá trình giao dịch trực tuyến của khách hàng.

---

## 7. Kiểm Tra Nhóm: Vận Chuyển (Shipping)

- **Hiện trạng**: **Chưa hoạt động thực tế**.
- **Chi tiết kiểm tra**:
  - Giao diện thanh toán [checkout/index.tsx:L601](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower-shop.frontend/src/pages/checkout/index.tsx#L601) của Next.js đang hiển thị cứng Phí vận chuyển là `"Miễn phí"` đối với mọi đơn hàng.
  - Phía Backend không có API tính phí vận chuyển động hay tham chiếu đến khoảng cách giao hàng tối đa. Cấu hình vận chuyển trong CSDL hiện tại hoàn toàn bị cô lập.

---

## 8. Kiểm Tra Nhóm: Đơn Hàng (Order Settings)

- **Hiện trạng**: **Chưa hoạt động thực tế**.
- **Chi tiết kiểm tra**:
  - **Tự động hủy đơn hàng**: Background service [OrderExpiryBackgroundService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/OrderExpiryBackgroundService.cs) rà soát đơn hàng quá hạn. Tuy nhiên, thời gian giới hạn tự hủy đang bị code cứng tại dòng 56-58 (`now.AddMinutes(-30)` cho COD và `-15` phút cho Online Payment), không đọc từ giá trị `AutoCancelMinutes` của Settings trong Database.
  - **Bật/Tắt cổng thanh toán**: Nghiệp vụ đặt hàng vẫn chấp nhận phương thức COD và OnlinePayment mà không kiểm tra cờ bật/tắt (`EnableCOD`, `EnableOnlinePayment`) được lưu tại CSDL.

---

## 9. Các Service Đang Sử Dụng Settings

| Tên Service | Đọc Cấu hình Từ | Phạm vi/Nghiệp vụ Sử dụng | Đánh giá Tính Động |
| :--- | :--- | :--- | :---: |
| **EmailService** | `appsettings.json` (IOptions) | Gửi mail OTP, hóa đơn, thông báo hệ thống | Tĩnh (Cần Khởi động lại nếu đổi file) |
| **VnPayService** | `appsettings.json` (IConfiguration) | Sinh link thanh toán & Xác thực giao dịch | Tĩnh (Cần Khởi động lại nếu đổi file) |
| **OrderExpiryBackgroundService** | Hardcoded trong code | Xác định thời gian tự động hủy đơn hàng | Tĩnh (Cần sửa code để thay đổi thời gian) |
| **OrderService** | Database (Chỉ module Notifications) | Sinh thông báo quản trị khi có đơn hàng mới | Động |
| **PaymentService** | Database (Chỉ module Notifications) | Sinh thông báo quản trị khi đổi trạng thái thanh toán | Động |

---

## 10. Danh Sách Toàn Bộ Nơi Còn Đọc appsettings.json

### 10.1. Cấu hình Mail SMTP:
- **Tệp**: [Flower.Backend/Program.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs)
  - Dòng 157: `builder.Services.Configure<Flower.Backend.Models.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));`
- **Tệp**: [Flower.Backend/Services/EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs)
  - Dòng 23-28: Hàm khởi dựng nhận `IOptions<EmailSettings>` để gán cho `_settings`.
  - Dòng 32: `new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)`
  - Dòng 58, 213, 242, 271, 299, 330, 361, 409, 478, 773, 824: Đọc địa chỉ gửi từ `_settings.Username` hoặc mặc định.

### 10.2. Cấu hình VNPay:
- **Tệp**: [Flower.Backend/Services/VnPayService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/VnPayService.cs)
  - Dòng 26: `_configuration["Vnpay:PaymentBackReturnUrl"]`
  - Dòng 28: `_configuration["Vnpay:Version"]`
  - Dòng 29: `_configuration["Vnpay:Command"]`
  - Dòng 30: `_configuration["Vnpay:TmnCode"]`
  - Dòng 33: `_configuration["Vnpay:CurrCode"]`
  - Dòng 35: `_configuration["Vnpay:Locale"]`
  - Dòng 43-44: `_configuration["Vnpay:BaseUrl"]` và `_configuration["Vnpay:HashSecret"]`
  - Dòng 64: `_configuration["Vnpay:HashSecret"]`

### 10.3. Cấu hình Thời gian LeadTime (TimeSettings):
- **Tệp**: [Flower.Backend/Program.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs)
  - Dòng 158: `builder.Configuration.GetSection("TimeSettings").Get<TimeSettings>()`

---

## 11. Danh Sách Nơi Đã Đọc Database

- **Tệp**: [Flower.Backend/Controllers/SettingsController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/SettingsController.cs)
  - Dòng 23: `Index` gọi `_settingService.GetAllSettings()` đọc từ CSDL để hiển thị ra View Razor.
  - Dòng 39, 66, 79, 108, 128: Các action Save gọi `_settingService.SaveSetting()` ghi nhận cấu hình động vào CSDL.
- **Tệp**: [Flower.Backend/Services/SystemSettingService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/SystemSettingService.cs)
  - Dòng 17: `GetSetting<T>()` gọi `_context.SystemSettings.FindAsync(key)` để tải cấu hình thô từ CSDL.
  - Dòng 36: `SaveSetting<T>()` gọi `_context.SystemSettings.Add()` hoặc cập nhật và gọi `SaveChangesAsync()`.
- **Tệp**: [Flower.Backend/Services/PromotionScheduler.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/PromotionScheduler.cs)
  - Dòng 33-87: Đọc thông tin Flash Sales và Coupons từ Database để tạo thông báo hệ thống tự động.

---

## 12. Đánh Giá Trạng Thái Từng Module

| Phân hệ cấu hình | Hiện trạng hoạt động | Đánh giá kỹ thuật | Khắc phục đề xuất |
| :--- | :--- | :--- | :--- |
| **Thông tin cửa hàng** | ❌ Chưa hoạt động | Dữ liệu Frontend bị hardcode tĩnh hoàn toàn | Viết API Public trả về thông tin cửa hàng từ DB để Next.js gọi động. |
| **SMTP Email** | ❌ Chưa hoạt động | `EmailService` chỉ đọc cấu hình từ `appsettings.json` | Tiêm `ISystemSettingService` vào `EmailService` để khởi tạo `SmtpClient` động theo DB. |
| **VNPay** | ❌ Chưa hoạt động | `VnPayService` chỉ đọc cấu hình từ `appsettings.json` | Đổi cách lấy dữ liệu cấu hình VNPay từ `_configuration` sang `ISystemSettingService.GetSetting()`. |
| **Phí Vận chuyển** | ❌ Chưa hoạt động | Frontend Next.js hiển thị cứng phí giao hàng = 0 | Xây dựng API tính phí ship động dựa trên dữ liệu cấu hình vận chuyển từ DB. |
| **Thiết lập đơn hàng** | ❌ Chưa hoạt động | `OrderExpiryBackgroundService` hardcode thời gian tự hủy | Lấy `AutoCancelMinutes` từ CSDL để tính mốc thời gian quét đơn quá hạn linh hoạt. |

---

## 13. Có Cần Khởi Động Lại (Restart) Backend Sau Khi Lưu Cấu Hình?

- **Hiện trạng**: **Không cần thiết** vì hệ thống thực tế đang không sử dụng các cấu hình này từ Database để vận hành các dịch vụ cốt lõi.
- **Khi nâng cấp lên kiến trúc động**:
  - Do `ISystemSettingService` và `IApplicationDbContext` được đăng ký dưới dạng **Scoped Service**, phiên bản mới của cấu hình sẽ được truy vấn và khởi tạo lại trên mỗi HTTP Request mới (đối với Email, VNPay, v.v.). Do đó, **Không cần khởi động lại Backend**, cấu hình mới sẽ lập tức được áp dụng cho lượt gọi tiếp theo.
  - Đối với Background Service chạy ngầm (`OrderExpiryBackgroundService` và `PromotionScheduler` - đăng ký dạng Singleton/HostedService), cấu hình cần được đọc lại trong mỗi vòng quét bằng cách tạo một `IServiceScope` thủ công bên trong luồng lặp (đã triển khai đúng mẫu thiết kế này nên sẽ nhận cấu hình mới mà không cần restart).

---

## 14. Kết Luận & Đề Xuất Phương Án Chuẩn Doanh Nghiệp

### 14.1. Những cấu hình thực sự hoạt động:
- Giao diện Admin hiển thị đúng thông tin cấu hình từ database.
- Ghi nhận Audit log và bảo vệ hiển thị mật khẩu/khóa VNPay hoạt động tốt.
- Tính năng Gửi email thử nghiệm và sinh thông báo tự động (Đơn hàng, Thanh toán, Coupon, Flash Sale hết hạn) hoạt động chính xác theo kiến trúc.

### 14.2. Những cấu hình mang tính hình thức (chưa liên kết nghiệp vụ):
- Thông tin cửa hàng, SMTP, VNPay, Shipping Fee và thời gian tự hủy đơn hàng.

### 14.3. Đề xuất phương án nâng cấp chuẩn doanh nghiệp (không thay đổi database):

1. **Đồng bộ hóa SMTP**:
   - Cập nhật [EmailService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/EmailService.cs) để đọc `SmtpSettings` trực tiếp từ `ISystemSettingService.GetSetting<SmtpSettings>("Smtp")` thay vì tiêm `IOptions<EmailSettings>`.

2. **Đồng bộ hóa VNPay**:
   - Cập nhật [VnPayService.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Services/VnPayService.cs) thay thế hoàn toàn các lệnh gọi `_configuration["Vnpay:..."]` bằng việc truy vấn `_systemSettingService.GetSetting<VNPaySettings>("VNPay")`.

3. **Cải tiến Hiệu năng & Bộ nhớ Cache**:
   - Tích hợp `IMemoryCache` vào `SystemSettingService`.
   - Khi đọc cấu hình: Ưu tiên đọc từ cache trước, nếu không có mới truy vấn CSDL.
   - Khi Admin nhấn "Lưu cấu hình": Thực hiện xóa cache tương ứng (Cache Invalidation) để lượt truy vấn tiếp theo tự động cập nhật dữ liệu mới nhất.

4. **Bảo mật mã hóa nâng cao**:
   - Sử dụng lớp `IDataProtector` thuộc thư viện `Microsoft.AspNetCore.DataProtection` để mã hóa đối xứng mật khẩu SMTP và khóa bí mật VNPay trước khi ghi vào cột `Value` dạng JSON của bảng `SystemSettings`. Giải mã tự động khi lấy dữ liệu ra sử dụng.

---

## 15. Kết Quả Build Dự Án

Sau khi hoàn thành quá trình Audit:
- **C# Backend project**: Biên dịch thành công (**0 Errors**).
- **Next.js Frontend project**: Biên dịch thành công (**0 Errors**).
- **Unit Tests**: Vượt qua toàn bộ kịch bản kiểm thử (**37/37 Tests Passed**).
- Không có bất kỳ sự thay đổi mã nguồn, cơ sở dữ liệu hay giao diện người dùng nào được thực hiện trong suốt quá trình rà soát này.
