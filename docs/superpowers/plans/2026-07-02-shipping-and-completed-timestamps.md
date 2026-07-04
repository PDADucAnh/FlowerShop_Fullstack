# Kế hoạch bổ sung mốc thời gian Đang giao & Đã giao vào Email

> **Dành cho Agent:** SỬ DỤNG skill `superpowers:subagent-driven-development` để thực hiện kế hoạch này từng bước.

**Mục tiêu:** Bổ sung mốc thời gian **Đang giao** và **Đã giao** vào email thông báo của khách hàng, đồng thời tự động gửi email thông báo khi đơn hàng chuyển sang trạng thái đang giao hàng (`Shipping`).

---

### Các thay đổi chính:

1. **Giao diện Email (Timeline mốc thời gian)**:
   * **Thời gian đặt hàng**: `order.OrderDate`
   * **Thời gian xác nhận**: `order.VerifiedAt` / `order.PaymentPaidAt` / `order.OrderDate.AddMinutes(5)`
   * **Thời gian đang giao (Shipping)**:
     * Nếu trạng thái `>= OrderStatus.Shipping`: Hiển thị thời gian chuyển trạng thái (hoặc thời gian thực tế nếu là lúc gửi email).
     * Nếu trạng thái `< OrderStatus.Shipping`: Hiển thị `"Chờ bàn giao"`.
   * **Thời gian đã giao (Completed)**:
     * Nếu trạng thái là `Completed`: Hiển thị thời điểm hoàn thành (thời gian thực tế gửi email).
     * Nếu trạng thái `< Completed`: Hiển thị `"Chờ hoàn thành"`.

2. **Backend Service**:
   * Thêm phương thức `SendOrderShippingEmailAsync` vào `IEmailService` và `EmailService`.
   * Tích hợp sự kiện thay đổi trạng thái sang `Shipping` trong `OrderService.Update` để gọi gửi email thông báo đang giao hàng.

---

### Danh sách các Task cần thực hiện:

#### Task 1: Cấu trúc hóa phương thức lấy mốc thời gian động trong EmailService.cs
- [ ] **Mô tả**: Trong `EmailService.cs`, viết logic xác định mốc thời gian cho từng chặng trạng thái dựa trên `order.Status`:
  * Xác nhận: `order.VerifiedAt` hoặc `order.PaymentPaidAt` hoặc `order.OrderDate.AddMinutes(5)`.
  * Đang giao: `DateTime.Now` (nếu đang ở trạng thái Shipping), hoặc `order.VerifiedAt.Value.AddHours(2)` (nếu đã hoàn thành), hoặc `"Chờ bàn giao"`.
  * Đã giao: `DateTime.Now` (nếu đang ở trạng thái Completed), hoặc `"Chờ hoàn thành"`.

#### Task 2: Thêm Email thông báo đang giao hàng (`SendOrderShippingEmailAsync`)
- [ ] **Mô tả**:
  * Cập nhật `IEmailService.cs` thêm khai báo `SendOrderShippingEmailAsync`.
  * Triển khai hàm `SendOrderShippingEmailAsync` trong `EmailService.cs` tương tự các hàm gửi email khác, tiêu đề email: `Đơn hàng #{order.Id} đang được giao - AnhCMS Boutique`.

#### Task 3: Tích hợp sự kiện gửi email Đang giao trong `OrderService.cs`
- [ ] **Mô tả**: Cập nhật hàm `Update` của `OrderService.cs` để lắng nghe sự kiện `statusChangedToShipping` và gửi email thông báo đến khách hàng.

#### Task 4: Kiểm tra và Push Git
- [ ] **Mô tả**: Biên dịch dự án Backend và push code lên nhánh `Buoi_12`.
