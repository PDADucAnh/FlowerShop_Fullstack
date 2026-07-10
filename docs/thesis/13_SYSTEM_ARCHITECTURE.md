# 13. Kiến trúc hệ thống

Tài liệu này mô tả chi tiết kiến trúc kỹ thuật tổng thể của hệ thống PDA FlowerShop, phân tích cơ cấu hoạt động của các thành phần và cách thức các thành phần liên kết với nhau trong một hệ thống đồng bộ.

## 13.1. Kiến trúc phân tầng tổng thể (Hybrid Architecture)

Hệ thống được phát triển theo mô hình kiến trúc lai (Hybrid Architecture) nhằm tối ưu hóa khả năng vận hành của hai phân hệ chính.

* **Kiến trúc tách rời (Decoupled SPA):** Giao diện dành cho khách hàng được xây dựng dưới dạng ứng dụng trang đơn (React SPA), chạy hoàn toàn độc lập trên trình duyệt client và giao tiếp với máy chủ thông qua REST API gửi nhận dữ liệu JSON.
* **Kiến trúc Server-Side Rendering (Razor Views):** Phân hệ quản trị (Admin Panel) được xây dựng trực tiếp trên nền tảng ASP.NET Core MVC sử dụng cơ chế dựng trang phía máy chủ để bảo đảm tốc độ truy cập dữ liệu quản lý nhanh và tăng cường tính bảo mật cho hoạt động quản lý.

## 13.2. Phân hệ Frontend (Client-Side)

Ứng dụng phía máy khách được triển khai bằng công nghệ React 19 kết hợp ngôn ngữ lập trình TypeScript để kiểm soát lỗi kiểu dữ liệu nghiêm ngặt.

* **Vite Build Tool:** Sử dụng công cụ Vite thay thế cho Webpack truyền thống nhằm đẩy nhanh tốc độ biên dịch mã nguồn và tối ưu dung lượng đóng gói khi phát hành sản phẩm.
* **Quản lý định tuyến (Routing):** Sử dụng thư viện `react-router-dom` phiên bản 7 để thiết lập cơ chế định tuyến động phía máy khách, giúp chuyển trang không cần tải lại toàn bộ tài liệu HTML.
* **Quản lý dữ liệu và Cache (Data Fetching):** Sử dụng thư viện `TanStack Query` (React Query) phiên bản 5 để quản lý các trạng thái bất đồng bộ của dữ liệu API, tự động lưu cache dữ liệu và thực hiện truy vấn lại (refetching) khi dữ liệu phía máy chủ thay đổi.
* **Quản lý trạng thái toàn cục (State Management):** Sử dụng cơ chế React Context API để lưu trữ và phân phát thông tin đăng nhập (`AuthContext`), thông tin giỏ hàng (`CartContext`) và danh sách sản phẩm yêu thích (`WishlistContext`).
* **Định dạng giao diện (Styling):** Áp dụng thiết kế đáp ứng (Responsive Web Design) bằng mã CSS viết tay thuần túy (Custom Vanilla CSS), hạn chế tối đa các thư viện giao diện cồng kềnh để tối ưu hóa thời gian tải trang trên các thiết bị di động (Mobile-First).

## 13.3. Phân hệ Backend (Server-Side)

Máy chủ ứng dụng được xây dựng trên nền tảng framework ASP.NET Core 8.0, cung cấp dịch vụ Web API tốc độ cao.

* **Cơ chế tiêm phụ thuộc (Dependency Injection):** Toàn bộ các dịch vụ nghiệp vụ (Services) và tầng dữ liệu được quản lý tập trung và tiêm tự động vào các bộ điều khiển (Controllers) qua DI Container của ASP.NET Core với các vòng đời phù hợp (Transient, Scoped, Singleton).
* **SignalR Real-Time Hub:** Tích hợp SignalR Server để duy trì kết nối WebSocket hai chiều giữa máy chủ và các trình duyệt quản trị, cho phép phát đi thông báo tức thời khi có đơn hàng mới phát sinh.
* **Xử lý ngầm (HostedService):** Sử dụng `OrderExpiryBackgroundService` kế thừa từ `BackgroundService` chạy ngầm theo chu kỳ 60 giây để thực hiện các tác vụ tự động quét dọn đơn hàng quá hạn mà không ảnh hưởng đến hiệu năng xử lý của luồng API chính.

## 13.4. Cơ sở dữ liệu (Database Layer)

Hệ thống lưu trữ dữ liệu tập trung trên hệ quản trị cơ sở dữ liệu Microsoft SQL Server.

* **Entity Framework Core (Code-First):** Sử dụng ORM EF Core để thiết lập ánh xạ đối tượng thực thể C# thành cấu trúc bảng quan hệ trong cơ sở dữ liệu. Mọi thay đổi về cấu trúc dữ liệu được quản lý lịch sử thông qua EF Core Migrations.
* **Tính toàn vẹn dữ liệu:** Thiết lập ràng buộc khóa ngoại (Foreign Keys) nghiêm ngặt với chính sách xóa hạn chế (`DeleteBehavior.Restrict`) trên các liên kết quan hệ quan trọng (như giữa Khách hàng và Đơn hàng) để bảo vệ tính lịch sử giao dịch. Các cột thường xuyên tìm kiếm được tạo chỉ mục độc lập (Unique Indexes) để đẩy nhanh tốc độ truy vấn.

## 13.5. Cơ chế xác thực và phân quyền (Authentication & Authorization)

Hệ thống triển khai cơ chế xác thực kép (Dual Authentication Scheme) bảo đảm an toàn cho từng phân hệ.

* **Đối với API (/api/*):** Sử dụng cơ chế xác thực không trạng thái qua JWT Bearer Token. Khi đăng nhập thành công, khách hàng nhận được Access Token (hạn 60 phút) dùng để đính kèm vào HTTP Header và Refresh Token lưu trữ dưới dạng bản băm trong cơ sở dữ liệu để gia hạn tự động.
* **Đối với trang quản trị (/Admin/*):** Sử dụng cơ chế xác thực Cookie Authentication truyền thống. Trình duyệt lưu trữ cookie đăng nhập được mã hóa an toàn và tự động gửi lên máy chủ trong mỗi yêu cầu.
* **Phân quyền (Authorization Policies):** Phân chia cụ thể các chính sách truy cập `AdminOnly` (yêu cầu vai trò Admin) và `StaffOnly` (yêu cầu vai trò Admin hoặc Editor).

## 13.6. Tích hợp thanh toán và xử lý tài chính (Payment Integration)

Hệ thống tích hợp trực tiếp với cổng thanh toán điện tử VNPay thông qua môi trường VNPay Sandbox.

* **Chữ ký số bảo mật:** Áp dụng thuật toán HMAC-SHA512 kết hợp với khóa bí mật `HashSecret` để mã hóa và kiểm tra tính toàn vẹn của dữ liệu giao dịch gửi đi và nhận về từ VNPay.
* **Tính không thể phủ nhận:** Lưu trữ mọi kết quả phản hồi của cổng thanh toán trong bảng giao dịch `Payments` độc lập, đồng thời lưu chi tiết lịch sử từng nỗ lực thanh toán vào bảng `PaymentAttempts` phục vụ công tác đối soát.

## 13.7. Bộ nhớ lưu trữ vật lý (Storage Layer)

* **Hình ảnh tĩnh:** Các tệp hình ảnh sản phẩm do quản trị viên tải lên được xử lý bằng thư viện `SixLabors.ImageSharp` để chuẩn hóa kích thước, định dạng và lưu trữ trực tiếp trên hệ thống tệp vật lý của máy chủ tại thư mục `/wwwroot/uploads/products/`. Đường dẫn tương đối được lưu vào cơ sở dữ liệu.
* **Nội dung bài viết:** Các nội dung định dạng HTML lớn (từ trình soạn thảo CKEditor 5) được lưu trữ trực tiếp trong trường kiểu dữ liệu văn bản lớn (`nvarchar(max)`) của cơ sở dữ liệu.
