# Báo Cáo Cấu Hình Động Hệ Thống (Settings Dynamic Configuration Report)

Tài liệu chi tiết về quá trình rà soát, tái cấu trúc và áp dụng cấu hình động cho toàn bộ hệ thống quản lý của FlowerShop từ CSDL SQL Server thay thế cho file `appsettings.json`.

---

## 1. Phạm Vi Kiểm Tra Ban Đầu (Audit Scope)

Hệ thống đã được quét qua toàn bộ mã nguồn để định vị các vị trí đọc cấu hình tĩnh.
- **Trước nâng cấp**: 
  - Cấu hình SMTP và VNPay được nạp tĩnh qua `appsettings.json` tại thời điểm khởi chạy ứng dụng.
  - Phí vận chuyển và cờ phương thức thanh toán bị hardcode hoặc bỏ qua.
  - Thời gian tự động hủy đơn được lập trình cứng.
- **Sau nâng cấp**:
  - Toàn bộ tham chiếu cấu hình được tập trung hóa qua thực thể `SystemSetting` trong CSDL SQL Server.
  - Áp dụng cấu chế tự động phục hồi cấu hình cũ khi bảo mật, và hỗ trợ hoàn hảo cơ chế dự phòng (fallback) để tránh crash hệ thống.

---

## 2. Các Phân Hệ Được Refactor Sang Cấu Hình Động

### 2.1. SMTP Email
- **Service**: `EmailService`
- **Cơ chế**: Getter property `_settings` tự động truy vấn `ISystemSettingService.GetSetting<SmtpSettings>("Smtp")`.
- **Tác động**: Khi thay đổi cấu hình SMTP ở trang quản trị, các email gửi đi tiếp theo (gửi mã OTP, xác nhận đơn hàng, hoàn tiền, v.v.) sẽ áp dụng ngay thông số SMTP mới mà không cần restart server.

### 2.2. Cổng Thanh Toán VNPay
- **Service**: `VnPayService`
- **Cơ chế**: Nhận diện cấu hình VNPay qua `ISystemSettingService.GetSetting<VNPaySettings>("VNPay")` động.
- **Tác động**: Tự động chuyển đổi địa chỉ Endpoint của VNPay tùy thuộc cờ kiểm thử `IsSandbox` (Sandbox: `sandbox.vnpayment.vn`, Production: `pay.vnpayment.vn`).

### 2.3. Cấu Hình Vận Chuyển
- **Service**: `ShippingService` mới lập.
- **Cơ chế**: Đọc phí ship mặc định và ngưỡng miễn phí ship để tính phí ship trực tiếp tại Backend.
- **Tác động**: Đồng bộ hóa phí giao hàng thực tế lúc đặt hàng ở Backend thay vì hardcode 0đ, tăng tính bảo mật cho giao dịch.

### 2.4. Thời Gian Tự Hủy Đơn Hàng
- **Service**: `OrderExpiryBackgroundService` (Background Worker quét đơn hàng quá hạn).
- **Cơ chế**: Đọc `AutoCancelMinutes` từ DB. COD cutoff tính theo đúng giá trị này, Online cutoff tính bằng 1/2 giá trị này.
- **Tác động**: background service tự động áp dụng thời gian hủy đơn mới ngay trong vòng lặp quét kế tiếp mà không cần khởi động lại.

### 2.5. Bật / Tắt Phương Thức Thanh Toán (Payment Control)
- **Controller**: `OrdersController` (Api actions: `CreateOrder`, `Checkout`)
- **Cơ chế**: Đọc cờ `EnableCOD` và `EnableOnlinePayment`.
- **Tác động**: Trả về mã lỗi HTTP 403 Forbidden lập tức nếu khách hàng cố tình đặt hàng qua phương thức bị Admin vô hiệu hóa.

---

## 3. Cơ Chế Dự Phòng (Fallback Mechanism)

Để đảm bảo hệ thống không bao giờ bị crash nếu CSDL trống hoặc thiếu bản ghi cấu hình, `SystemSettingService` đã được trang bị cơ chế tự động phục hồi (Fallback):
- Khi không tìm thấy bản ghi khóa trong bảng `SystemSettings`:
  - **SMTP**: Đọc dữ liệu dự phòng từ mục `EmailSettings` trong file `appsettings.json`.
  - **VNPay**: Đọc dữ liệu dự phòng từ mục `Vnpay` trong file `appsettings.json`.
  - **Order**: Đọc thời gian hủy đơn dự phòng từ `TimeSettings:PreShippingMinutes` trong `appsettings.json`.
  - **Store/Shipping**: Tự động khởi tạo đối tượng mặc định với giá trị an toàn đã được định nghĩa trước.
