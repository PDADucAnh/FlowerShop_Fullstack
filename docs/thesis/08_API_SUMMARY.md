# 8. Danh sách và tóm tắt các API Endpoints

Tài liệu này cung cấp danh sách đầy đủ các điểm cuối API (Endpoints) được xây dựng trong phân hệ Backend (ASP.NET Core Web API) phục vụ giao tiếp với ứng dụng Frontend (React SPA). Các API được thiết kế theo chuẩn RESTful và trả về định dạng dữ liệu thuần JSON.

## 8.1. Nhóm API Xác thực và Tài khoản (Auth API)

Đường dẫn cơ sở: `/api/auth`

* **POST /api/auth/register**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Đăng ký tài khoản khách hàng mới.
* **POST /api/auth/login**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Xác thực tài khoản khách hàng, trả về mã Access Token (JWT) và Refresh Token.
* **POST /api/auth/forgot-password**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Gửi mã OTP khôi phục mật khẩu vào email của khách hàng.
* **POST /api/auth/reset-password**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Sử dụng OTP để đặt lại mật khẩu mới.
* **POST /api/auth/refresh**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Sử dụng Refresh Token để gia hạn Access Token mới khi Access Token cũ hết hạn.
* **POST /api/auth/logout**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Thu hồi Refresh Token trong cơ sở dữ liệu và đăng xuất người dùng.
* **PUT /api/auth/change-password**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Thay đổi mật khẩu hiện tại bằng mật khẩu mới.
* **PUT /api/auth/profile**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Cập nhật thông tin cá nhân (họ tên, số điện thoại, địa chỉ mặc định).

## 8.2. Nhóm API Sản phẩm và Danh mục (Products & Categories API)

Đường dẫn cơ sở: `/api/products` và `/api/categories`

* **GET /api/products**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách sản phẩm hoa tươi phân trang, hỗ trợ lọc theo danh mục sản phẩm (CategoryProductId) và khoảng giá (minPrice, maxPrice).
* **GET /api/products/{id}**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy chi tiết thông tin của một sản phẩm hoa tươi và tăng số lượt xem (ViewCount) của sản phẩm đó.
* **GET /api/products/search**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Tìm kiếm sản phẩm theo từ khóa khớp với tên hoặc mã sản phẩm SKU.
* **GET /api/products/trending**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách sản phẩm nổi bật, bán chạy dựa trên thuật toán đánh giá lượt xem và lượt mua trong 30 ngày gần nhất.
* **GET /api/category-products**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách toàn bộ các danh mục sản phẩm hoa tươi.
* **GET /api/categories**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách các danh mục của bài viết blog.

## 8.3. Nhóm API Đơn hàng (Orders API)

Đường dẫn cơ sở: `/api/orders`

* **POST /api/orders**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Tạo đơn đặt hàng mới. Thực hiện kiểm tra tồn kho, giữ chỗ sản phẩm (Stock Lock) và gửi email xác nhận.
* **GET /api/orders**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Lấy danh sách đơn hàng đã đặt của tài khoản khách hàng hiện tại (được lọc tự động dựa trên email trích xuất từ JWT).
* **GET /api/orders/{id}**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Lấy thông tin chi tiết một đơn đặt hàng (chỉ hiển thị nếu đơn hàng thuộc quyền sở hữu của khách hàng yêu cầu).
* **POST /api/orders/{id}/cancel**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Hủy đơn hàng và ghi nhận lý do hủy đơn. Hệ thống tự động tính toán phí hủy đơn và số tiền hoàn lại theo chính sách.
* **POST /api/orders/{id}/verify-otp**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Xác thực mã OTP gửi qua email để hoàn tất quy trình đặt hàng COD có dấu hiệu cần xác minh gian lận.

## 8.4. Nhóm API Thanh toán (Payment API)

Đường dẫn cơ sở: `/api/payment`

* **POST /api/payment/create-vnpay-url**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Tạo liên kết thanh toán chuyển hướng sang cổng thanh toán trực tuyến VNPay Sandbox cho đơn hàng chờ thanh toán.
* **GET /api/payment/vnpay-callback**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Điểm nhận phản hồi kết quả giao dịch tự động của VNPay. Backend thực hiện kiểm tra chữ ký số bảo mật, đối soát thông tin giao dịch và cập nhật trạng thái đơn hàng.
* **POST /api/payment/retry/{orderId}**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* Tạo lại liên kết thanh toán VNPay mới cho đơn hàng có giao dịch trước đó bị lỗi hoặc bị hủy.
* **POST /api/payment/{orderId}/verify**
    * *Quyền truy cập:* Khách hàng đã đăng nhập (Authorize).
    * *Mô tả:* API xác thực mã OTP giao dịch cho các đơn hàng COD.
* **POST /api/payment/webhook**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Tiếp nhận dữ liệu webhook tự động cập nhật trạng thái thanh toán từ các đối tác trung gian (được bảo vệ bằng chữ ký số HMAC-SHA256).

## 8.5. Nhóm API Bài viết (Posts API)

Đường dẫn cơ sở: `/api/posts`

* **GET /api/posts**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách bài viết blog phân trang.
* **GET /api/posts/{slug}**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy nội dung chi tiết bài viết blog theo đường dẫn thân thiện (slug).

## 8.6. Nhóm API Vận chuyển (Delivery API)

Đường dẫn cơ sở: `/api/delivery`

* **GET /api/delivery/slots**
    * *Quyền truy cập:* Khách vãng lai (Anonymous).
    * *Mô tả:* Lấy danh sách các khung giờ giao hoa khả dụng và công suất đặt chỗ thực tế theo ngày cụ thể.
