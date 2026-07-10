# 5. Các chức năng dành cho khách hàng

Tài liệu này mô tả chi tiết các chức năng được cung cấp cho đối tượng khách hàng trên giao diện website chính thức (React SPA) của hệ thống PDA FlowerShop.

## 5.1. Đăng ký tài khoản (Register)

Chức năng cho phép người dùng đăng ký tài khoản khách hàng mới để mua sắm và quản lý đơn hàng.

* Khách hàng điền các thông tin bắt buộc gồm: Họ tên, Email, Số điện thoại và Mật khẩu.
* Hệ thống kiểm tra tính hợp lệ của định dạng email và số điện thoại thông qua Zod schema ở frontend và kiểm tra trùng lặp email ở backend.
* Khi đăng ký thành công, mật khẩu được băm bảo mật và thông tin tài khoản được lưu vào bảng `Customers`.

## 5.2. Đăng nhập hệ thống (Login)

Chức năng xác thực danh tính khách hàng để truy cập các tiện ích cá nhân hóa.

* Khách hàng đăng nhập bằng cách nhập Email và Mật khẩu.
* Backend kiểm tra thông tin đăng nhập, nếu chính xác sẽ tạo và trả về Access Token (JWT có thời hạn 60 phút) cùng Refresh Token.
* Khách hàng đăng nhập thành công sẽ lưu trữ token trong trình duyệt để tự động đính kèm vào các tiêu đề yêu cầu (HTTP Headers) tiếp theo.

## 5.3. Quên và đặt lại mật khẩu (Forgot & Reset Password)

Quy trình hỗ trợ khách hàng khôi phục tài khoản khi quên mật khẩu.

* Khách hàng nhập email đã đăng ký. Hệ thống tạo một mã Token khôi phục mật khẩu có thời hạn hết hạn ngắn (ResetToken và ResetTokenExpiry lưu trong DB).
* Hệ thống gửi một email khôi phục chứa mã OTP khôi phục trực tiếp đến địa chỉ thư điện tử của khách hàng.
* Khách hàng thực hiện nhập mã khôi phục và mật khẩu mới để đặt lại mật khẩu của mình một cách an toàn.

## 5.4. Duyệt và tìm kiếm sản phẩm (Browse & Search)

Các công cụ hỗ trợ tìm kiếm sản phẩm hoa tươi nhanh chóng và trực quan.

* **Duyệt sản phẩm:** Khách hàng duyệt danh sách sản phẩm phân trang, lọc sản phẩm theo danh mục (CategoryProduct) hoặc lọc theo khoảng giá từ thấp đến cao.
* **Tìm kiếm sản phẩm:** Công cụ tìm kiếm trên thanh điều hướng cho phép tìm nhanh sản phẩm theo tên hoặc mã sản phẩm SKU.
* **Sản phẩm nổi bật (Trending):** Trang chủ hiển thị danh sách sản phẩm nổi bật dựa trên điểm số tính toán từ lượt xem (ViewCount) và lượt thêm vào giỏ hàng (AddToCartCount).

## 5.5. Quản lý giỏ hàng (Cart)

Chức năng quản lý các sản phẩm dự kiến mua trước khi thanh toán.

* Khách hàng có thể thêm sản phẩm vào giỏ hàng từ trang danh sách hoặc trang chi tiết.
* Trong giỏ hàng, khách hàng có thể cập nhật số lượng của từng mặt hàng, hệ thống tự động kiểm tra lượng tồn kho khả dụng để giới hạn số lượng tối đa.
* Khách hàng có thể xóa sản phẩm khỏi giỏ hàng. Thông tin giỏ hàng được đồng bộ hóa lưu trữ tại bộ nhớ LocalStorage.

## 5.6. Tiến hành đặt hàng (Checkout)

Quy trình nhập thông tin giao nhận và xác định hình thức mua hàng.

* Khách hàng điền thông tin người nhận (Họ tên, Số điện thoại) và địa chỉ giao hàng cụ thể (tỉnh/thành, quận/huyện, phường/xã, địa chỉ chi tiết).
* Lựa chọn Ngày giao hàng và Khung giờ giao hàng (Delivery Slots) mong muốn.
* Kiểm tra số lượng tồn kho khả dụng của sản phẩm và áp dụng giữ chỗ tồn kho (Stock Lock) trong thời gian 15 phút để đảm bảo sản phẩm không bị người khác mua mất trong lúc thanh toán.
* Lựa chọn phương thức thanh toán: Thanh toán khi nhận hàng (COD) hoặc Thanh toán trực tuyến (Online Payment).

## 5.7. Thanh toán trực tuyến cổng VNPay (VNPay Payment)

Tích hợp giao dịch thanh toán qua cổng thanh toán VNPay.

* Nếu khách hàng chọn phương thức thanh toán trực tuyến, sau khi tạo đơn hàng ở trạng thái `PendingPayment`, hệ thống sẽ gọi API tạo URL thanh toán và chuyển hướng khách hàng sang cổng thanh toán thử nghiệm VNPay Sandbox.
* Khách hàng thực hiện các bước thanh toán trên cổng VNPay.
* Sau khi hoàn tất giao dịch, VNPay sẽ gọi callback trả về kết quả thanh toán. Hệ thống cập nhật trạng thái giao dịch trong bảng `Payments` và cập nhật đơn hàng thành `Confirmed` nếu thành công, hoặc đánh dấu giao dịch thất bại và cho phép thanh toán lại nếu không thành công.

## 5.8. Lịch sử đơn hàng và theo dõi trạng thái (Order History)

Chức năng giúp khách hàng quản lý và giám sát các đơn hàng đã đặt.

* Khách hàng truy cập danh sách đơn hàng cá nhân để xem danh sách toàn bộ các đơn hàng đã thực hiện kèm trạng thái tương ứng.
* Xem chi tiết từng đơn hàng bao gồm danh sách sản phẩm đã mua, số tiền thanh toán, phí hủy đơn, số tiền được hoàn (nếu đơn hàng bị hủy).
* Thực hiện yêu cầu hủy đơn hàng trực tiếp trên giao diện đối với các đơn hàng chưa chuyển sang trạng thái đang giao hàng (`Shipping`).

## 5.9. Danh sách sản phẩm yêu thích (Wishlist)

Chức năng lưu trữ sản phẩm khách hàng quan tâm.

* Khách hàng nhấp vào biểu tượng yêu thích để thêm hoặc xóa sản phẩm khỏi danh sách yêu thích.
* Danh sách sản phẩm yêu thích được hiển thị tại trang riêng và lưu trữ tại bộ nhớ LocalStorage của trình duyệt.

## 5.10. Đánh giá sản phẩm (Reviews)

* **Chưa triển khai:** Tính năng đánh giá sản phẩm (viết nhận xét, chấm điểm sao và đính kèm hình ảnh sau khi nhận hàng) chưa được triển khai trong phiên bản hiện tại.

## 5.11. Cập nhật thông tin cá nhân (Profile)

Chức năng hỗ trợ quản lý dữ liệu tài khoản khách hàng.

* Khách hàng đăng nhập có thể thực hiện cập nhật họ tên, số điện thoại và địa chỉ giao hàng mặc định của mình.
* Thực hiện đổi mật khẩu tài khoản bằng cách nhập mật khẩu hiện tại và mật khẩu mới.

## 5.12. Đọc tin tức và chia sẻ bài viết (Blog)

Chức năng cung cấp thông tin hữu ích và kết nối khách hàng.

* Khách hàng có thể đọc danh sách các bài viết chia sẻ kinh nghiệm, hướng dẫn cắm hoa, ý nghĩa các loài hoa.
* Xem chi tiết nội dung bài viết định dạng HTML an toàn và chia sẻ liên kết bài viết lên các mạng xã hội.
