# 10. Vòng đời và luồng xử lý đơn hàng

Tài liệu này mô tả chi tiết vòng đời của một đơn hàng (Order Lifecycle) trong hệ thống PDA FlowerShop, các trạng thái đơn hàng và các quy tắc chuyển đổi trạng thái tương ứng dựa trên mã nguồn thực tế của hệ thống.

## 10.1. Sơ đồ các trạng thái đơn hàng

Đơn hàng được quản lý chặt chẽ thông qua các trạng thái số nguyên đại diện cho các bước trong vòng đời sản xuất và giao nhận hoa tươi.

* `PendingPayment` (Chờ thanh toán): Đơn hàng thanh toán trực tuyến được tạo thành công và đang chờ kết quả giao dịch từ cổng thanh toán VNPay.
* `PendingVerification` (Chờ xác minh): Đơn hàng thanh toán khi nhận hàng (COD) đang chờ khách hàng xác thực mã OTP gửi qua email.
* `Confirmed` (Đã xác nhận): Đơn hàng đã được xác minh thành công và được cửa hàng tiếp nhận để chuẩn bị.
* `Preparing` (Đang chuẩn bị): Nhân viên cắm hoa đang lựa chọn nguyên liệu, cắm hoa thiết kế và chuẩn bị thiệp kèm theo.
* `ReadyForDelivery` (Sẵn sàng giao): Hoa tươi đã cắm xong, được đóng gói bảo quản và sẵn sàng bàn giao cho nhân viên giao hàng.
* `Shipping` (Đang giao hàng): Shipper đang vận chuyển hoa đến địa chỉ người nhận.
* `Completed` (Hoàn thành): Người nhận đã nhận hoa tươi thành công.
* `Cancelled` / `CancelledByCustomer` / `CancelledByShop` (Đã hủy): Đơn hàng bị hủy bỏ bởi khách hàng, cửa hàng hoặc do hệ thống tự động hủy.
* `RefundPending` (Chờ hoàn tiền): Đơn hàng đã thanh toán trực tuyến bị hủy và đang chờ quản trị viên phê duyệt hoàn tiền.
* `Refunded` (Đã hoàn tiền): Đơn hàng đã hoàn tất quy trình hoàn trả tiền cho khách hàng.

## 10.2. Luồng xử lý chi tiết tại từng giai đoạn

### Giai đoạn 1: Giỏ hàng và Đặt hàng (Cart to Checkout)

Khách hàng lựa chọn các sản phẩm hoa tươi và tiến hành điền thông tin đặt hàng tại giao diện Checkout.

* **Phân nhánh Đơn hàng trực tuyến (Online Payment):**
    * Hệ thống tạo đơn hàng với trạng thái `PendingPayment`.
    * Áp dụng cơ chế khóa tồn kho tạm thời (`StockLockService`) trong thời gian 15 phút để đảm bảo giữ chỗ sản phẩm cho khách hàng.
    * Khách hàng thực hiện thanh toán trên VNPay. Nếu thành công, trạng thái đơn hàng chuyển sang `Confirmed` và giải phóng khóa tồn kho tạm thời để trừ tồn kho vật lý chính thức. Nếu thất bại, đơn hàng giữ trạng thái `PendingPayment` và tự động bị hủy sau khi hết hạn 15 phút.
* **Phân nhánh Đơn hàng thanh toán khi nhận hàng (COD):**
    * Hệ thống thực hiện kiểm tra số điện thoại của khách hàng có nằm trong danh sách đen (`PhoneBlacklist`) hay không. Nếu có, bắt buộc khách hàng phải chọn phương thức thanh toán trực tuyến.
    * Hệ thống kiểm tra mức độ tin cậy của khách hàng qua điểm số gian lận (`FraudScore`). Nếu tài khoản có độ tin cậy thấp, hệ thống đặt đơn hàng ở trạng thái `PendingVerification` và gửi mã OTP 6 số qua email của khách. Khách hàng phải nhập mã OTP này để xác thực đơn hàng.
    * Nếu tài khoản khách hàng có độ tin cậy cao (không cần xác minh OTP) hoặc khách hàng nhập đúng mã OTP xác thực, đơn hàng lập tức chuyển sang trạng thái `Confirmed`.
    * Dịch vụ chạy ngầm `OrderExpiryBackgroundService` quét hệ thống mỗi 60 giây và tự động chuyển các đơn hàng `PendingVerification` quá hạn 30 phút sang trạng thái `Cancelled` để giải phóng khung giờ giao hàng.

### Giai đoạn 2: Chuẩn bị sản phẩm (Preparing)

Cửa hàng tiếp nhận đơn hàng đã được xác nhận (`Confirmed`) và chuyển sang trạng thái `Preparing`.

* Nhân viên cắm hoa tiến hành thiết kế mẫu hoa tươi theo yêu cầu đơn hàng.
* **Quy tắc hủy đơn:** Trong giai đoạn này, khách hàng vẫn được phép yêu cầu hủy đơn hàng trực tuyến, tuy nhiên sẽ phải chịu mức phí phạt hủy đơn là 30% giá trị đơn hàng do cửa hàng đã tiến hành cắt tỉa nguyên liệu hoa tươi (loại nguyên liệu nhanh hỏng). Khách hàng nhận lại 70% số tiền hoàn.

### Giai đoạn 3: Sẵn sàng bàn giao (Ready for Delivery)

Sản phẩm hoa tươi sau khi cắm xong được chuyển sang trạng thái `ReadyForDelivery`.

* Hoa được đóng gói kèm thiệp chúc mừng và lưu trữ ở khu vực bảo quản mát.
* **Quy tắc hủy đơn:** Nếu khách hàng yêu cầu hủy đơn ở giai đoạn này, mức phí phạt hủy đơn là 50% giá trị đơn hàng do sản phẩm đã hoàn thiện hoàn toàn. Khách hàng nhận lại 50% tiền hoàn.

### Giai đoạn 4: Vận chuyển và Hoàn thành (Shipping to Completed)

Đơn hàng được bàn giao cho nhân viên giao hàng và chuyển sang trạng thái `Shipping`.

* Shipper thực hiện vận chuyển hoa tươi đến địa chỉ người nhận yêu cầu.
* **Quy tắc hủy đơn:** Khi đơn hàng đã chuyển sang trạng thái `Shipping` hoặc `Completed`, khách hàng **không được phép hủy đơn hàng** dưới bất kỳ hình thức nào.
* Sau khi giao hoa thành công, nhân viên cập nhật đơn hàng thành `Completed`. Hệ thống tự động ghi nhận điểm số uy tín của khách hàng (`SuccessfulDeliveries` tăng thêm 1). Nếu đơn hàng COD bị khách hàng từ chối nhận hoa (bùng đơn), nhân viên cập nhật trạng thái đơn hàng sang `Cancelled` và hệ thống tự động tăng điểm số gian lận (`FailedDeliveries` và `FraudScore` tăng thêm 1).
