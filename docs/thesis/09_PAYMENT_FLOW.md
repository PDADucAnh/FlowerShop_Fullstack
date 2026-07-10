# 9. Quy trình và luồng xử lý thanh toán

Tài liệu này mô tả chi tiết quy trình xử lý giao dịch thanh toán trong hệ thống PDA FlowerShop, tập trung vào luồng tích hợp với cổng thanh toán trực tuyến VNPay và cơ chế đối soát, hoàn tiền.

## 9.1. Giai đoạn khởi tạo giao dịch thanh toán trực tuyến (VNPay URL Creation)

Quy trình bắt đầu khi khách hàng hoàn tất việc chọn hoa và tiến hành đặt hàng tại trang thanh toán bằng hình thức thanh toán trực tuyến.

* **Bước 1:** Khách hàng bấm nút đặt hàng trên giao diện. Frontend gửi yêu cầu tạo đơn hàng thông qua API `POST /api/orders`.
* **Bước 2:** Hệ thống kiểm tra tồn kho và tạo bản ghi đơn hàng trong cơ sở dữ liệu với trạng thái đơn hàng ban đầu là `PendingPayment` (Chờ thanh toán) và trạng thái thanh toán là `Pending`.
* **Bước 3:** Hệ thống gọi API `POST /api/payment/create-vnpay-url`. Backend thực hiện tạo một bản ghi giao dịch chờ trong bảng `Payments` để lấy mã giao dịch tạm thời.
* **Bước 4:** Bộ phận xử lý dịch vụ VNPay (`VnPayService`) tiến hành tạo URL thanh toán. Hệ thống thu thập thông tin cấu hình cổng từ tệp `appsettings.json` (BaseUrl, TmnCode, HashSecret, ReturnUrl) và thiết lập các tham số yêu cầu của VNPay:
    * `vnp_TxnRef`: Mã định danh giao dịch duy nhất được ghép dưới dạng `{MãĐơnHàng}_{MãThanhToán}`.
    * `vnp_Amount`: Số tiền thanh toán (đơn giá nhân với 100 theo quy định của VNPay).
    * `vnp_OrderInfo`: Nội dung thanh toán đơn hàng.
    * `vnp_ReturnUrl`: Địa chỉ callback nhận phản hồi kết quả sau khi khách hàng thanh toán xong.
* **Bước 5:** Backend tính toán chữ ký số bảo mật `vnp_SecureHash` bằng thuật toán băm mật mã dựa trên chuỗi cấu hình và mã khóa `HashSecret` của cửa hàng, đính kèm vào cuối URL và trả về liên kết thanh toán cho Frontend.
* **Bước 6:** Frontend nhận liên kết và tự động chuyển hướng khách hàng sang trang thanh toán chính thức của VNPay.

## 9.2. Giai đoạn nhận phản hồi giao dịch và xử lý kết quả (VNPay Callback)

Sau khi khách hàng thực hiện các thao tác xác thực thanh toán tại cổng VNPay, hệ thống VNPay sẽ chuyển hướng trình duyệt của khách hàng quay trở lại địa chỉ callback được cấu hình.

* **Bước 1:** API `GET /api/payment/vnpay-callback` của Backend tiếp nhận yêu cầu chứa các tham số kết quả giao dịch do VNPay gửi về qua Query String.
* **Bước 2:** Backend tiến hành trích xuất chữ ký số bảo mật `vnp_SecureHash` do VNPay gửi kèm. Sau đó, hệ thống sử dụng thuật toán HMAC-SHA512 để tự tính toán chữ ký đối soát từ toàn bộ các tham số phản hồi nhận được và so khớp với chữ ký nhận được nhằm loại bỏ các yêu cầu giả mạo hoặc chỉnh sửa dữ liệu trên đường truyền.
* **Bước 3:** Nếu chữ ký số không hợp lệ, hệ thống ghi nhận cảnh báo bảo mật, ghi nhật ký lỗi và chuyển hướng khách hàng về trang giỏ hàng kèm theo thông báo lỗi thanh toán.
* **Bước 4:** Nếu chữ ký số hoàn toàn chính xác, hệ thống trích xuất thông tin mã đơn hàng từ tham số `vnp_TxnRef` và kiểm tra thông tin số tiền thanh toán thực tế nhận được từ tham số `vnp_Amount` so với tổng giá trị đơn hàng thực tế lưu trong cơ sở dữ liệu.
* **Bước 5:** Xử lý theo mã phản hồi kết quả giao dịch (`vnp_ResponseCode`):
    * **Trường hợp giao dịch thành công (Mã phản hồi `00`):**
        * Hệ thống khởi tạo một giao dịch cơ sở dữ liệu (Database Transaction) để đảm bảo tính toàn vẹn dữ liệu.
        * Cập nhật trạng thái bản ghi trong bảng `Payments` thành `Completed` (Đã hoàn thành), lưu vết mã giao dịch thực tế của VNPay (`vnp_TransactionNo`) và ghi nhận thời gian thanh toán `PaidAt`.
        * Cập nhật trạng thái đơn hàng trong bảng `Orders` sang `Confirmed` (Đã xác nhận), đánh dấu đơn hàng đã được xác minh (`IsVerified = true`).
        * Thực hiện trừ số lượng tồn kho vật lý (`StockQuantity`) của các sản phẩm có trong đơn hàng dựa trên số lượng đã khóa giữ chỗ, đồng thời giải phóng tài nguyên giữ chỗ trong bộ nhớ đệm Memory Cache (Stock Lock).
        * Lưu thông tin nỗ lực thanh toán vào bảng `PaymentAttempts` phục vụ công tác đối soát.
        * Hoàn tất giao dịch cơ sở dữ liệu (Commit Transaction).
        * Gửi thư điện tử tự động thông báo xác nhận đơn hàng và thanh toán thành công đến hòm thư của khách hàng.
        * Chuyển hướng khách hàng về giao diện thông báo thành công `/order-confirmation?orderId={id}&payment=success`.
    * **Trường hợp giao dịch thất bại hoặc bị hủy (Mã phản hồi khác `00`):**
        * Hệ thống cập nhật trạng thái bản ghi trong bảng `Payments` thành `Failed` (Thất bại) và ghi nhận mã lỗi phản hồi từ cổng thanh toán.
        * Đơn hàng vẫn được giữ nguyên ở trạng thái `PendingPayment` để cho phép khách hàng thực hiện thanh toán lại.
        * Hệ thống lưu vết nỗ lực thất bại vào bảng `PaymentAttempts`.
        * Chuyển hướng khách hàng về giao diện thông báo lỗi `/order-confirmation?orderId={id}&payment={vnp_ResponseCode}` để hướng dẫn khách hàng kiểm tra số dư hoặc thực hiện thanh toán lại.

## 9.3. Giai đoạn thanh toán lại đơn hàng (Retry Payment)

Nếu giao dịch trực tuyến ban đầu bị gián đoạn, bị khách hàng chủ động hủy hoặc gặp lỗi thẻ, hệ thống cung cấp quy trình cho phép thanh toán lại.

* Khách hàng truy cập vào lịch sử đơn hàng cá nhân, tìm đơn hàng đang ở trạng thái `PendingPayment` và nhấn nút "Thanh toán lại".
* Frontend gửi yêu cầu đến API `POST /api/payment/retry/{orderId}`.
* Backend kiểm tra tính hợp lệ của đơn hàng (đơn hàng phải thực sự chờ thanh toán và chưa từng thanh toán thành công).
* Backend tạo một bản ghi thanh toán mới trong bảng `Payments` với trạng thái `Pending` và tạo một liên kết thanh toán VNPay mới tương ứng.
* URL thanh toán mới được gửi về cho Frontend để khách hàng tiếp tục thực hiện giao dịch thanh toán.

## 9.4. Giai đoạn xử lý hoàn tiền (Refund Flow)

Quy trình hoàn trả tiền cho khách hàng được thực hiện khi đơn hàng trực tuyến đã thanh toán thành công bị hủy bỏ.

* **Khởi tạo yêu cầu hoàn tiền:** Khi đơn hàng trực tuyến ở trạng thái đã xác nhận hoặc đang chuẩn bị bị hủy (do Admin hủy hoặc khách hàng gửi yêu cầu hủy hợp lệ theo chính sách), hệ thống tự động tạo một bản ghi trong bảng `Refunds` ở trạng thái `0` (Chờ xử lý). Số tiền hoàn được tính toán tự động dựa trên phần trăm hoàn tiền quy định trong chính sách hủy đơn hàng của trạng thái hiện tại.
* **Cập nhật trạng thái đơn hàng:** Đơn hàng được cập nhật sang trạng thái `RefundPending` (Chờ hoàn tiền) và trạng thái thanh toán được chuyển sang `RefundPending`.
* **Phê duyệt hoàn tiền:** Quản trị viên tiến hành kiểm tra thông tin yêu cầu hoàn tiền trên giao diện quản trị Admin Panel. Sau khi thực hiện chuyển khoản hoàn trả tiền cho khách hàng thành công qua cổng thanh toán hoặc tài khoản ngân hàng, Admin nhấn nút phê duyệt hoàn tiền.
* **Cập nhật kết quả hoàn tiền:** Hệ thống cập nhật bản ghi trong bảng `Refunds` thành trạng thái đã xử lý thành công, đồng thời cập nhật trạng thái đơn hàng trong bảng `Orders` sang `Refunded` (Đã hoàn tiền) và trạng thái giao dịch trong bảng `Payments` sang `Refunded`.
* **Gửi thông báo:** Hệ thống tự động gửi thư điện tử thông báo hoàn tiền thành công ghi rõ số tiền hoàn và phương thức hoàn tiền, đồng thời tạo thông báo thời gian thực gửi đến tài khoản khách hàng.
