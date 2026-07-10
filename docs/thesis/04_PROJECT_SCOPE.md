# 4. Phạm vi dự án

Tài liệu này xác định rõ ranh giới của dự án PDA FlowerShop bao gồm các tính năng được triển khai, các tính năng nằm ngoài phạm vi nghiên cứu, các công nghệ được áp dụng và môi trường triển khai thực tế.

## 4.1. Giới hạn của dự án

Dự án tập trung xây dựng một hệ thống bán hoa tươi trực tuyến hoàn chỉnh cho một cửa hàng hoa tươi đơn lẻ (PDA FLOWER), phục vụ hai đối tượng chính: khách hàng mua sắm trực tuyến (qua giao diện React SPA) và đội ngũ nhân viên quản lý cửa hàng (qua giao diện MVC Admin Panel). Hệ thống không giải quyết bài toán chuỗi cửa hàng đa chi nhánh hoặc sàn thương mại điện tử đa người bán (Multi-vendor Marketplace).

## 4.2. Các tính năng được hỗ trợ (Supported Features)

Hệ thống triển khai đầy đủ các nghiệp vụ thương mại điện tử cốt lõi và các tính năng đặc thù cho ngành hoa.

* **Phân hệ khách hàng:**
    * Duyệt danh mục, tìm kiếm và xem chi tiết sản phẩm hoa.
    * Đăng ký, đăng nhập tài khoản khách hàng, đổi mật khẩu và cập nhật hồ sơ cá nhân.
    * Quản lý giỏ hàng và danh sách sản phẩm yêu thích (Wishlist).
    * Áp dụng mã giảm giá trực tiếp vào đơn hàng.
    * Đặt hàng trực tuyến, lựa chọn ngày giao hàng và khung giờ giao hàng (Delivery Slots).
    * Thanh toán trực tuyến qua cổng VNPay Sandbox hoặc thanh toán khi nhận hàng (COD).
    * Xác thực mã OTP gửi qua email đối với đơn hàng COD có dấu hiệu cần xác minh.
    * Tra cứu lịch sử mua hàng, theo dõi trạng thái đơn hàng thời gian thực và thực hiện yêu cầu hủy đơn hàng.
    * Đọc tin tức, kiến thức ý nghĩa các loài hoa từ phân hệ Blog.
* **Phân hệ quản trị (CMS):**
    * Giao diện quản trị Admin Panel sử dụng công nghệ ASP.NET Core MVC Razor Views tích hợp DataTables.
    * Quản lý danh mục và danh sách sản phẩm hoa tươi (CRUD).
    * Quản lý danh mục bài viết và bài viết Blog hỗ trợ trình soạn thảo CKEditor 5.
    * Quản lý danh sách khách hàng, cập nhật điểm gian lận (Fraud Score) và danh sách đen số điện thoại (Phone Blacklist).
    * Tiếp nhận đơn hàng, xem chi tiết và cập nhật trạng thái đơn hàng (xác nhận đơn, chuẩn bị hoa, bàn giao vận chuyển, hoàn thành).
    * Quản lý các giao dịch thanh toán trực tuyến, đối soát mã giao dịch và xử lý phê duyệt hoàn tiền.
    * Cấu hình thời gian giới hạn giữ chỗ tồn kho (Stock Lock) và quản lý năng lực giao hàng của các khung giờ giao hoa.
    * Quản lý tài khoản quản trị viên và nhân viên vận hành hệ thống.
    * Quản lý banner quảng cáo hiển thị trên trang chủ.
    * Dashboard hiển thị biểu đồ thống kê doanh số, số lượng đơn hàng và các mặt hàng bán chạy nhất.
* **Phân hệ nền tảng:**
    * Tự động gửi email thông báo trạng thái đơn hàng (đã nhận đơn, đã xác nhận, đã giao hàng, đã hủy đơn, đã hoàn tiền).
    * Dịch vụ chạy ngầm tự động quét và hủy các đơn hàng chờ xác minh quá hạn 30 phút mà khách hàng không thực hiện xác thực OTP.
    * SignalR Hub phát thông báo đồng bộ dữ liệu tức thời khi có thay đổi trạng thái đơn hàng hoặc sản phẩm.

## 4.3. Các tính năng chưa được hỗ trợ (Unsupported Features)

Các tính năng sau nằm ngoài phạm vi thực hiện của dự án trong giai đoạn hiện tại và được ghi nhận là "Chưa triển khai".

* **Chưa triển khai:** Tích hợp API định vị GPS của các hãng vận chuyển (Ahamove/Grab) để theo dõi thời gian thực vị trí shipper di chuyển trên bản đồ.
* **Chưa triển khai:** Tự động tính toán phí vận chuyển động dựa trên khoảng cách địa lý thực tế từ cửa hàng đến người nhận thông qua API Google Maps. Phí giao hàng hiện tại được cấu hình cố định theo quận/huyện.
* **Chưa triển khai:** Chức năng cho phép khách hàng tự thiết kế mẫu hoa tươi trực tuyến (Custom Bouquet Builder).
* **Chưa triển khai:** Hệ thống ví điện tử nội bộ tích điểm đổi quà trực tiếp.
* **Chưa triển khai:** Thanh toán quốc tế qua thẻ tín dụng quốc tế trực tiếp không qua cổng VNPay.

## 4.4. Phạm vi công nghệ (Technology Scope)

Dự án áp dụng các công nghệ hiện đại, ổn định và có tính kế thừa cao.

* **Công nghệ Backend:** ASP.NET Core Web API 8.0, Entity Framework Core 8.0, SQL Server LocalDB, SignalR, JWT Bearer Authentication, Cookie Authentication, MailKit/MimeKit.
* **Công nghệ Frontend:** React 19, TypeScript 6.0, Vite Build Tool, react-router-dom v7, TanStack Query v5 (React Query), React Context API, Zod Validation, TailwindCSS.
* **Công nghệ kiểm thử:** xUnit, Moq Framework, EF Core InMemory Database.

## 4.5. Phạm vi triển khai (Deployment Scope)

Dự án được cấu hình và kiểm thử vận hành ổn định trong môi trường cục bộ (Local Environment).

* **Backend Web API & CMS:** Chạy trên IIS Express hoặc Kestrel Server tại cổng `http://localhost:5064` (hoặc cổng HTTPS tương ứng).
* **Frontend React Client:** Chạy trên Vite Dev Server tại địa chỉ `http://localhost:3000`.
* **Cơ sở dữ liệu:** Sử dụng SQL Server LocalDB kết nối qua chuỗi cấu hình `DefaultConnection`.
* **Cổng thanh toán:** Kết nối với hệ thống VNPay Sandbox (môi trường kiểm thử dành cho nhà phát triển).
* **Hệ thống gửi thư:** Kết nối với máy chủ SMTP của Gmail sử dụng tài khoản Gmail thật cấu hình qua Mật khẩu ứng dụng (App Password) được lưu trữ an toàn trong cấu hình hệ thống.
