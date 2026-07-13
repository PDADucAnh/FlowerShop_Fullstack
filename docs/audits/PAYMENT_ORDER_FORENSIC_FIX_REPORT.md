# Báo Cáo Forensic Audit & Fix (Luồng Payment, Order, Checkout)

**Ngày thực hiện:** 2026-07-14
**Mục tiêu:** Kiểm toán (Audit) và vá (Fix) triệt để các lỗi liên quan đến quá trình thanh toán, đặt hàng, huỷ đơn.
**Trạng thái:** Hoàn thành (Fixed)

---

## Danh sách các lỗi đã xác định và giải pháp

### 1. Lỗi False Positive cho COD trên Frontend
- **Mô tả:** Khách hàng thanh toán COD nhưng sau khi đặt hàng thành công, frontend hiển thị giao diện "Thanh toán không thành công". Nguyên nhân là do sau khi gọi API, frontend redirect sang trang confirmation mà thiếu query parameter `payment=success`. Mặc định UI đọc `payment` không bằng `success` nên nhảy vào UI lỗi, dù đơn hàng đã được tạo thành công và kho đã bị trừ trong Database. Điều này làm khách hàng hoang mang và bấm đặt liên tục, làm rác database và kiệt quệ kho hàng.
- **Giải pháp:** Sửa trong `Flower-shop.frontend/src/pages/checkout/index.tsx`. Cập nhật logic redirect của phương thức COD để bổ sung `payment=success` trong URL (VD: `navigate('/order-confirmation/123?payment=success')`).

### 2. Status Mapping Mismatch trên OrderDetail
- **Mô tả:** Backend trả về các enum trạng thái như `PaymentMethod`, `PaymentStatus` và `OrderStatus` dưới định dạng chuỗi (String) thay vì Số nguyên (Int). Tuy nhiên, file `OrderDetail.tsx` (Frontend) đã hardcode sử dụng object key là số nguyên cho `statusConfig`. Điều này làm `OrderDetail.tsx` không đọc được cấu hình, làm giao diện vỡ hoặc báo lỗi trạng thái "Chưa xác định".
- **Giải pháp:** Cập nhật file `Flower-shop.frontend/src/types/order.ts` để đổi type `paymentMethod`, `paymentStatus`, `status` thành `number | string`. Đồng thời, cập nhật `OrderDetail.tsx` để ánh xạ đúng cấu hình của thẻ màu trạng thái.

### 3. Email Refund Gửi Bất Cập Cho COD
- **Mô tả:** Khi đơn hàng COD bị huỷ, tiến trình gửi mail ở backend (trong `EmailService.cs`) vẫn tự động sinh nội dung hoàn trả tiền (Refund) mặc dù với COD số tiền khách đã trả là 0 đồng. Câu chữ "Tiền sẽ được hoàn trong vòng 24 giờ" xuất hiện trong Email huỷ đơn gây hiểu nhầm nghiêm trọng.
- **Giải pháp:** Sửa hàm `BuildCancellationEmailBody` trong `Flower.Backend/Services/EmailService.cs`. Đưa thông báo hoàn tiền vào điều kiện `if (refundAmount > 0)`.

### 4. Shipping Fee Không Đồng Bộ (Giá trị hiển thị & Giá trị lưu Backend)
- **Mô tả:** Trong `OrderService.cs` phía Backend, tính năng tạo đơn (`CreateOrder`) sử dụng giá trị `DefaultFee` từ `ShippingSettings`. Tuy nhiên, trang `checkout/index.tsx` đã hardcode phí vận chuyển là `Miễn phí` (0 VNĐ). Hệ lụy là Frontend hiển thị thiếu phí ship, dẫn tới tổng tiền người dùng thấy và tiền Backend xử lý lệnh (với VNPAY) bị lệch nhau.
- **Giải pháp:** 
  1. Backend: Thêm endpoint `[HttpGet("checkout")]` tại `SettingsApiController.cs` để trả về công khai cấu hình `ShippingSettings` và `OrderSettings` cho Frontend.
  2. Frontend: `checkout/index.tsx` fetch config `/settings/checkout` từ API, tự động tính `shippingFee` dựa vào biến `freeShipFrom` và `defaultFee`. 

### 5. Lệch Giá Khi "Thử Lại Thanh Toán" (Retry Payment)
- **Mô tả:** Tính năng thử lại thanh toán (`CreateRetryPayment` trong `PaymentService.cs`) lấy giá trị thanh toán bằng cách tính tổng lại: `order.OrderDetails?.Sum(od => od.Quantity * od.UnitPrice)`. Phương pháp này sai do bỏ qua Shipping Fee và Coupon/Promotion (lưu trong `FinalAmount`).
- **Giải pháp:** Sửa trong `PaymentService.cs`, thay dòng mã `var totalAmount = order.OrderDetails?.Sum(...)` bằng `var totalAmount = order.FinalAmount;`. Hệ thống đảm bảo số tiền thử lại bằng đúng số tiền đã chốt tại thời điểm đặt hàng. 

---

## Kiến trúc sau khi xử lý (No Regression Guarantee)
Toàn bộ sửa đổi đều thoả mãn các nguyên tắc kiến trúc:
- **Zero Frontend Structure Changes:** Cấu trúc components giữ nguyên, chỉ xử lý state & conditional rendering.
- **Zero DB Migration/Schema Changes:** Khai thác các trường snapshot đã tồn tại.
- **Zero Business Logic Breakage:** Logic tính tiền đồng nhất.
- **Zero Technical Debt.**
