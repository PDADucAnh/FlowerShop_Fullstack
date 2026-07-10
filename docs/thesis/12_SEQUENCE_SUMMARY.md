# 12. Kịch bản tuần tự của các chức năng chính

Tài liệu này mô tả chi tiết bằng văn bản luồng tương tác tuần tự giữa Khách hàng, Giao diện người dùng (Frontend), Máy chủ ứng dụng (Backend), Cơ sở dữ liệu (Database) và Cổng thanh toán (VNPay) cho các chức năng cốt lõi của hệ thống.

## 12.1. Kịch bản Đăng ký tài khoản khách hàng

Quy trình tuần tự thực hiện việc tạo mới một tài khoản khách hàng trên hệ thống.

* **Bước 1:** Khách hàng điền thông tin đăng ký (họ tên, email, số điện thoại, mật khẩu) trên Form đăng ký của giao diện Frontend.
* **Bước 2:** Frontend thực hiện kiểm tra định dạng dữ liệu đầu vào. Nếu dữ liệu không hợp lệ, Frontend hiển thị thông báo lỗi trực tiếp cho Khách hàng. Nếu dữ liệu hợp lệ, Frontend gửi yêu cầu đăng ký (dữ liệu JSON chứa thông tin đăng ký) đến API `POST /api/auth/register` của Backend.
* **Bước 3:** Backend tiếp nhận yêu cầu, gọi dịch vụ khách hàng `CustomerService` kiểm tra sự tồn tại của Email trong cơ sở dữ liệu.
* **Bước 4:** `CustomerService` thực hiện truy vấn cơ sở dữ liệu. Cơ sở dữ liệu trả về kết quả tồn tại hay chưa.
* **Bước 5:** Nếu email đã được đăng ký trước đó, Backend trả về mã lỗi `400 Bad Request` kèm thông báo email đã tồn tại. Frontend nhận kết quả và hiển thị thông báo lỗi lên giao diện.
* **Bước 6:** Nếu email chưa tồn tại, Backend khởi tạo bộ băm mật khẩu `PasswordHasher` của ASP.NET Core để băm mật khẩu của khách hàng thành chuỗi an toàn.
* **Bước 7:** Backend tạo một đối tượng khách hàng `Customer` mới và gửi yêu cầu lưu trữ đến Cơ sở dữ liệu.
* **Bước 8:** Cơ sở dữ liệu thực hiện thêm mới bản ghi và trả về trạng thái lưu thành công.
* **Bước 9:** Backend nhận phản hồi thành công từ Cơ sở dữ liệu, gửi trả về Frontend phản hồi trạng thái `200 OK` kèm thông báo đăng ký tài khoản thành công.
* **Bước 10:** Frontend hiển thị thông báo thành công cho Khách hàng và tự động chuyển hướng khách hàng sang trang đăng nhập.

## 12.2. Kịch bản Đặt hàng và Thanh toán trực tuyến qua cổng VNPay

Quy trình xử lý tuần tự từ lúc chọn sản phẩm đến khi thanh toán thành công qua VNPay.

* **Bước 1:** Khách hàng nhấn nút tiến hành đặt hàng tại trang Checkout của Frontend sau khi điền đầy đủ thông tin giao nhận và lựa chọn phương thức thanh toán trực tuyến.
* **Bước 2:** Frontend gửi yêu cầu đặt đơn hàng đến API `POST /api/orders` của Backend kèm theo danh sách sản phẩm và thông tin vận chuyển.
* **Bước 3:** Backend khởi động một Database Transaction. Gọi dịch vụ tồn kho để kiểm tra số lượng tồn kho khả dụng của từng sản phẩm.
* **Bước 4:** Nếu số lượng tồn kho không đáp ứng đủ, Backend hủy bỏ Transaction và gửi trả về lỗi `400 Bad Request` kèm tên sản phẩm thiếu hàng. Frontend nhận mã lỗi và hiển thị cảnh báo tồn kho cho Khách hàng.
* **Bước 5:** Nếu kho đủ hàng, Backend gọi dịch vụ giữ chỗ tồn kho `StockLockService` để thực hiện khóa tạm thời số lượng sản phẩm cắm hoa trong thời gian 15 phút (lưu trữ trong Memory Cache).
* **Bước 6:** Backend tạo bản ghi đơn hàng trong bảng `Orders` với trạng thái là `PendingPayment` và lưu trữ vào Cơ sở dữ liệu.
* **Bước 7:** Backend gọi API `POST /api/payment/create-vnpay-url` để khởi tạo bản ghi thanh toán trạng thái `Pending` trong bảng `Payments` và sử dụng `VnPayService` để tạo đường dẫn thanh toán VNPay Sandbox có chứa chữ ký số bảo mật.
* **Bước 8:** Backend cam kết Transaction (Commit) và trả về Frontend đường dẫn URL thanh toán VNPay kèm theo ID đơn hàng mới tạo.
* **Bước 9:** Frontend nhận URL thanh toán và thực hiện chuyển hướng trình duyệt của Khách hàng sang cổng thanh toán trực tuyến VNPay.
* **Bước 10:** Khách hàng nhập thông tin tài khoản ngân hàng thử nghiệm và mã xác thực trên cổng thanh toán VNPay.
* **Bước 11:** VNPay xử lý giao dịch thành công và chuyển hướng trình duyệt của Khách hàng quay lại địa chỉ callback `GET /api/payment/vnpay-callback` của Backend kèm theo các tham số kết quả giao dịch và chữ ký số.
* **Bước 12:** Backend tiếp nhận callback, sử dụng khóa bí mật cấu hình để đối soát chữ ký số từ VNPay. Nếu chữ ký số hợp lệ và giao dịch thành công (Mã phản hồi `00`):
    * Backend bắt đầu một Database Transaction mới.
    * Cập nhật trạng thái đơn hàng thành `Confirmed` (Đã xác nhận), trạng thái thanh toán thành `Completed` (Đã hoàn thành) và lưu vết mã giao dịch VNPay.
    * Thực hiện trừ số lượng tồn kho vật lý chính thức của các sản phẩm trong cơ sở dữ liệu và giải phóng bộ giữ chỗ tồn kho tạm thời trong Memory Cache.
    * Ghi nhận nỗ lực giao dịch thành công vào bảng `PaymentAttempts`.
    * Commit Transaction cơ sở dữ liệu.
    * Kích hoạt dịch vụ `EmailService` chạy ngầm gửi email xác nhận đặt hàng thành công đến email của Khách hàng.
* **Bước 13:** Backend trả về chỉ thị chuyển hướng (Redirect) trình duyệt của Khách hàng quay lại trang `/order-confirmation?orderId={id}&payment=success` trên Frontend.
* **Bước 14:** Giao diện Frontend hiển thị thông báo đặt hàng và thanh toán trực tuyến thành công, liệt kê thông tin chi tiết hóa đơn đơn hàng cho Khách hàng.

## 12.3. Kịch bản Xác minh mã OTP đối với đơn hàng COD có dấu hiệu gian lận

Quy trình tuần tự gửi và kiểm tra mã OTP đối với các khách hàng thuộc diện cần xác minh trước khi xác nhận đơn hàng COD.

* **Bước 1:** Khách hàng bấm đặt hàng bằng hình thức thanh toán khi nhận hàng (COD) trên Frontend.
* **Bước 2:** Frontend gửi yêu cầu đặt đơn đến API `POST /api/orders` của Backend.
* **Bước 3:** Backend tiếp nhận yêu cầu đặt đơn, tiến hành kiểm tra thông tin khách hàng qua dịch vụ chống gian lận `FraudDetectionService`.
* **Bước 4:** `FraudDetectionService` kiểm tra số điện thoại khách hàng. Nếu số điện thoại nằm trong danh sách đen (`PhoneBlacklist`), Backend lập tức từ chối đơn hàng và trả về yêu cầu khách hàng thanh toán online.
* **Bước 5:** Nếu số điện thoại không bị chặn nhưng điểm số đánh giá độ tin cậy thấp, Backend lưu đơn hàng ở trạng thái `PendingVerification` (Chờ xác minh).
* **Bước 6:** Backend sinh ngẫu nhiên một mã OTP gồm 6 chữ số, lưu trữ mã OTP vào bộ nhớ cache `IMemoryCache` kèm thời hạn hết hạn là 10 phút với khóa định danh đơn hàng.
* **Bước 7:** Backend gọi `EmailService` gửi email chứa mã OTP xác minh trực tiếp đến địa chỉ email của Khách hàng.
* **Bước 8:** Backend gửi trả về Frontend phản hồi thông báo đơn hàng đã được tạo nhưng đang ở trạng thái chờ xác minh OTP.
* **Bước 9:** Frontend nhận phản hồi, hiển thị hộp thoại yêu cầu Khách hàng nhập mã OTP xác minh đơn hàng.
* **Bước 10:** Khách hàng kiểm tra email cá nhân, sao chép mã OTP và nhập vào ô nhập mã trên giao diện Frontend rồi nhấn nút gửi.
* **Bước 11:** Frontend gửi mã OTP kèm mã đơn hàng lên API `POST /api/payment/{orderId}/verify` của Backend.
* **Bước 12:** Backend nhận yêu cầu, trích xuất mã OTP lưu trong bộ nhớ cache dựa trên ID đơn hàng để đối khớp.
* **Bước 13:** Nếu mã OTP không khớp hoặc đã quá hạn 10 phút, Backend trả về lỗi `400 Bad Request` thông báo mã xác minh không hợp lệ. Frontend hiển thị thông báo lỗi yêu cầu Khách hàng kiểm tra và nhập lại mã.
* **Bước 14:** Nếu mã OTP chính xác, Backend cập nhật trạng thái đơn hàng sang `Confirmed` (Đã xác nhận), đánh dấu đơn hàng đã xác minh thành công (`IsVerified = true`), gửi email xác nhận đặt hàng thành công đến khách hàng và trả về Frontend phản hồi trạng thái thành công.
* **Bước 15:** Frontend đóng hộp thoại nhập OTP, hiển thị thông báo xác thực đơn hàng thành công và hiển thị thông tin chi tiết đơn hàng cho Khách hàng.
