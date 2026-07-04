# Kế hoạch bổ sung đầy đủ thông tin đơn hàng vào Email thông báo khách hàng

> **Dành cho Agent:** SỬ DỤNG skill `superpowers:subagent-driven-development` để thực hiện kế hoạch này từng bước.

**Mục tiêu:** Nâng cấp nội dung email thông báo xác nhận đơn hàng (`SendOrderConfirmedEmailAsync`) và email hoàn thành đơn hàng (`SendOrderCompletedEmailAsync`) trong `EmailService.cs` để chứa đầy đủ chi tiết người mua, người nhận, thời gian, địa điểm, lời chúc thiệp và ghi chú.

---

### Các thông tin sẽ được bổ sung vào Email:
1. **Thông tin khách hàng & Người nhận** (được bóc tách từ trường `order.Notes` có cấu trúc dạng `Người mua: ... | Người nhận: ... | Lời chúc: ...`):
   * Họ tên, Số điện thoại, Email người mua.
   * Họ tên, Số điện thoại người nhận.
2. **Thông tin giao hàng chi tiết**:
   * Địa chỉ nhận hoa chi tiết (`order.DeliveryAddress`).
   * Quận/Huyện giao hàng (`order.DeliveryDistrict`).
   * Ngày giao hàng (`order.DeliveryDate` định dạng `dd/MM/yyyy`).
   * Khung giờ giao hoa (`order.DeliveryTimeSlot`).
3. **Thông điệp thiệp tặng kèm & Ghi chú thêm**:
   * Lời chúc trên thiệp chúc mừng.
   * Ghi chú giao nhận đặc biệt của khách hàng.

---

### Danh sách các Task cần thực hiện:

#### Task 1: Bổ sung phương thức phân tách thông tin từ trường `Notes`
- [ ] **Mô tả**: Viết một hàm helper private `ParseNotesInfo` trong `EmailService.cs` để phân tích chuỗi `order.Notes` (dạng phân cách bởi ký tự `|`) thành một đối tượng chứa các thuộc tính:
  * `BuyerInfo` (Thông tin người mua)
  * `RecipientInfo` (Thông tin người nhận)
  * `GreetingCard` (Lời chúc trên thiệp)
  * `ExtraNotes` (Ghi chú thêm)

#### Task 2: Thiết kế lại Layout HTML Email mẫu chuyên nghiệp hơn
- [ ] **Mô tả**: Cập nhật hàm `BuildOrderEmailBody` trong `EmailService.cs`:
  * Áp dụng giao diện sang trọng hơn với tông màu chủ đạo hồng cánh sen `#ab2c5d` của FlowerShop.
  * Thêm phần **"Thông tin giao nhận"** chứa: Địa chỉ, Ngày giờ giao.
  * Thêm phần **"Thông tin người gửi & người nhận"** và **"Lời chúc kèm thiệp"**.
  * Bố cục hóa đơn sản phẩm hiển thị gọn gàng, có căn chỉnh lề phải cho cột Đơn giá và Thành tiền.

#### Task 3: Biên dịch kiểm tra và chạy thử
- [ ] **Mô tả**: Chạy biên dịch dự án Backend để đảm bảo không bị lỗi cú pháp:
  ```bash
  dotnet build CMS.Backend/CMS.Backend.csproj
  ```

#### Task 4: Commit và Push code lên nhánh `Buoi_12`
- [ ] **Mô tả**: Ghi nhận toàn bộ thay đổi cấu trúc email vào Git và push lên nhánh làm việc.
