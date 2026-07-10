# 14. Cơ chế bảo mật hệ thống

Tài liệu này tổng hợp và phân tích chi tiết các biện pháp, kỹ thuật bảo mật được áp dụng trong thiết kế và lập trình hệ thống PDA FlowerShop nhằm bảo vệ tài nguyên dữ liệu và ngăn chặn các nguy cơ tấn công mạng phổ biến.

## 14.1. Xác thực bảo mật bằng mã JWT và Refresh Token

Hệ thống bảo vệ các đường dẫn API bằng cơ chế xác thực không trạng thái (Stateless Authentication) sử dụng mã thông báo JWT (JSON Web Token).

* **Access Token ngắn hạn:** Mã truy cập JWT được cấu hình thời gian hết hạn ngắn (60 phút) để giảm thiểu rủi ro khi token bị đánh cắp. Token chứa thông tin định danh (Claim) gồm email và vai trò của khách hàng đã được ký số bảo mật bằng khóa bí mật `SecretKey` của máy chủ.
* **Refresh Token dài hạn:** Thiết lập Refresh Token có thời hạn sử dụng 7 ngày được lưu trữ dưới dạng bản băm trong cơ sở dữ liệu. Khi Access Token hết hạn, ứng dụng khách sử dụng Refresh Token gửi yêu cầu gia hạn tự động qua API `/api/auth/refresh`. Hệ thống thực hiện kiểm tra trạng thái thu hồi (`IsRevoked`), thời gian hết hạn (`ExpiresAt`) và thiết bị gửi yêu cầu để phát hiện kịp thời các hành vi giả mạo phiên đăng nhập.

## 14.2. Phân quyền truy cập theo vai trò (Role Authorization)

* Hệ thống áp dụng chính sách phân quyền theo vai trò (Role-Based Access Control - RBAC) tại tầng bộ điều khiển bằng các chính sách bảo vệ (`Authorization Policies`).
* Các API yêu cầu xác thực được bảo vệ bằng thuộc tính `[Authorize]`. Các chính sách truy cập `AdminOnly` và `StaffOnly` được cấu hình để kiểm tra thông tin vai trò trích xuất từ JWT (đối với API) hoặc Session Cookie (đối với trang quản trị), bảo đảm người dùng không thể truy cập các tài nguyên vượt quá thẩm quyền cho phép.

## 14.3. Cơ chế băm mật khẩu một chiều (Password Hashing)

* Hệ thống tuyệt đối không lưu trữ mật khẩu của người dùng (bao gồm tài khoản nhân viên quản trị và tài khoản khách hàng) dưới dạng văn bản thô (Plain Text) trong cơ sở dữ liệu.
* Sử dụng bộ băm mật khẩu `PasswordHasher` tiêu chuẩn của ASP.NET Core (ứng dụng thuật toán PBKDF2 kết hợp mã hóa băm HMAC-SHA256 với chuỗi muối ngẫu nhiên - Salt) để thực hiện băm mật khẩu trước khi lưu trữ vào trường `PasswordHash`, bảo đảm an toàn trước các cuộc tấn công dò quét mật khẩu bằng bảng cầu vồng (Rainbow Table).

## 14.4. Mã hóa đường truyền bằng giao thức HTTPS

* Mọi thông tin truyền tải giữa trình duyệt của người dùng và máy chủ Backend được mã hóa bảo mật thông qua giao thức HTTPS.
* Trong cấu hình khởi tạo của Backend, middleware `UseHttpsRedirection()` được kích hoạt để tự động chuyển hướng toàn bộ các yêu cầu HTTP không an toàn sang giao thức HTTPS bảo mật, ngăn chặn nguy cơ bị nghe lén hoặc đánh cắp dữ liệu trên đường truyền (Man-in-the-Middle Attacks).

## 14.5. Kiểm soát dữ liệu đầu vào (Input Validation)

Hệ thống thực hiện kiểm tra dữ liệu đầu vào nghiêm ngặt tại cả hai đầu ứng dụng.

* **Phía Client-side:** Sử dụng thư viện `Zod` để xây dựng các biểu mẫu kiểm tra dữ liệu đầu vào (email hợp lệ, độ dài mật khẩu tối thiểu, số điện thoại đúng định dạng số điện thoại Việt Nam) trước khi gửi yêu cầu lên máy chủ, nâng cao trải nghiệm người dùng và giảm tải xử lý cho Backend.
* **Phía Server-side:** Sử dụng các thuộc tính kiểm tra của ASP.NET Core (`Model Validation Attributes` như `[Required]`, `[MaxLength]`, `[Range]`) tại các lớp DTO để tự động kiểm duyệt dữ liệu yêu cầu. Hệ thống tự động từ chối xử lý và trả về mã lỗi `400 Bad Request` nếu dữ liệu không thỏa mãn điều kiện ràng buộc.

## 14.6. Cơ chế chia sẻ tài nguyên chéo nguồn (CORS)

* Backend thiết lập chính sách CORS (`Cross-Origin Resource Sharing`) nghiêm ngặt để chỉ định rõ ràng các nguồn được phép gửi yêu cầu API đến hệ thống.
* Sử dụng chính sách CORS `AllowReactApp` chỉ chấp nhận các yêu cầu gửi đến từ địa chỉ nguồn chính thức của giao diện khách hàng (`http://localhost:3000`), cho phép truyền nhận đầy đủ Header, Method và thông tin xác thực (Credentials), ngăn chặn các website lạ thực hiện gửi mã script khai thác dữ liệu từ API của hệ thống.

## 14.7. Phòng chống tấn công SQL Injection

* Tầng truy cập dữ liệu của hệ thống sử dụng thư viện Entity Framework Core làm cầu nối truy vấn.
* EF Core tự động chuyển đổi các truy vấn LINQ thành các câu lệnh SQL được tham số hóa (Parameterized Queries) trước khi gửi đến máy chủ SQL Server. Cơ chế liên kết tham số này bảo đảm dữ liệu nhập vào từ người dùng được xử lý như một giá trị tham số thuần túy thay vì một phần của mã lệnh SQL, ngăn ngừa hoàn toàn nguy cơ tấn công tiêm mã độc SQL Injection.

## 14.8. Phòng chống tấn công chèn mã script chéo trang (XSS)

* **React Auto-escaping:** Giao diện khách hàng React mặc định tự động mã hóa (escape) toàn bộ các giá trị chuỗi trước khi kết xuất lên giao diện HTML, ngăn chặn mã độc JavaScript nhúng trong dữ liệu hiển thị tự động thực thi.
* **DOMPurify Sanitization:** Đối với các dữ liệu đặc thù chứa mã HTML lớn (như nội dung bài viết blog cắm hoa được biên tập từ CKEditor), Frontend sử dụng thư viện kiểm duyệt `DOMPurify` để lọc sạch toàn bộ các mã lệnh độc hại (như thẻ `<script>`, các thuộc tính sự kiện `onload`, `onerror`) trước khi chèn vào trang chi tiết bài viết blog.

## 14.9. Phòng chống giả mạo yêu cầu chéo trang (CSRF)

* Đối với giao diện quản trị Admin Panel sử dụng cơ chế Cookie xác thực, hệ thống kích hoạt cơ chế Antiforgery của ASP.NET Core.
* Máy chủ sinh mã token bảo mật `X-CSRF-TOKEN` được cấu hình HttpOnly và chính sách SameSite nghiêm ngặt (`SameSiteMode.Strict`) lưu trong cookie của trình duyệt. Mỗi biểu mẫu POST yêu cầu gửi lên phải đính kèm mã token tương ứng để đối khớp trên máy chủ, ngăn chặn triệt để tấn công giả mạo yêu cầu chéo trang CSRF.
