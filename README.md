# 🚀 TÀI LIỆU DỰ ÁN: AnhCMS_Solution
### 🌐 Full-Stack ASP.NET Core & ReactJS

> **💡 Tổng quan:** Dự án AnhCMS_Solution là một hệ thống Website Full-Stack kết hợp giữa nền tảng Thương mại điện tử (cửa hàng hoa tươi **PDA FLOWER**) và Hệ thống quản trị nội dung tin tức/blog, xây dựng trên ASP.NET Core 8.0 + React 19.

---

## 📚 Tài liệu Chi tiết

| Tài liệu | Mô tả |
|----------|-------|
| [📖 Hướng dẫn Cài đặt](./docs/setup-guide.md) | Yêu cầu hệ thống, cài đặt môi trường, cấu hình Backend & Frontend, tạo Database, chạy dự án, xử lý sự cố |
| [🏗️ Kiến trúc & Workflow Chi tiết](./docs/architecture-overview.md) | Sơ đồ kiến trúc, ER Diagram, workflow từng chức năng (mua hàng, đơn hàng, hủy đơn, giỏ hàng, auth, v.v.), danh sách API |
| [📧 Cấu hình Gmail SMTP](./docs/gmail-setup-guide.md) | Hướng dẫn tạo App Password Google và cấu hình gửi email tự động |
| [❌ Hủy đơn hàng](./docs/workflow/order-cancellation-workflow.md) | Workflow chi tiết chức năng hủy đơn hàng |
| [🔍 Tìm kiếm & Lọc](./docs/workflow/Search.md) | Chi tiết chức năng tìm kiếm và lọc sản phẩm |
| [💲 Lọc theo giá](./docs/workflow/Price-Range-Filter.md) | Chi tiết chức năng lọc sản phẩm theo khoảng giá |

---

## 📑 Mục lục
1. [Tài liệu Chi tiết](#-tài-liệu-chi-tiết)
2. [Định hướng và Mục tiêu dự án](#-1-định-hướng-và-mục-tiêu-dự-án)
3. [Môi trường & Công nghệ (Tech Stack)](#️-2-môi-trường--công-nghệ-tech-stack)
4. [Kiến trúc Solution](#️-3-kiến-trúc-solution)
5. [Cấu trúc Cơ sở dữ liệu (Database Schema)](#-4-cấu-trúc-cơ-sở-dữ-liệu-database-schema)
6. [Tính năng chính](#-5-tính-năng-chính)
7. [Lộ trình phát triển dự án](#-6-lộ-trình-phát-triển-dự-án-theo-từng-buổi)
8. [Những lưu ý kỹ thuật cốt lõi](#️-7-những-lưu-ý-kỹ-thuật-cốt-lõi)

---

## 📌 1. ĐỊNH HƯỚNG VÀ MỤC TIÊU DỰ ÁN

Hệ thống được thiết kế theo **kiến trúc Lai (Hybrid)** và **phân tách trách nhiệm (Decoupling)**:

* 🖥️ **Backend:** Đóng vai trò cung cấp giao diện quản trị Admin bằng kiến trúc ASP.NET Core MVC truyền thống, đồng thời mở các cổng RESTful Web API để cung cấp dữ liệu thô (chuẩn JSON) cho các nền tảng khác. Việc Backend chỉ trả về dữ liệu thô giúp tiết kiệm băng thông và tối ưu tốc độ tải trang.
* ⚛️ **Frontend:** Chuyển đổi tư duy từ Render phía Server sang Render phía Client bằng ReactJS (Single Page Application). ReactJS tự động gọi API lấy dữ liệu thực tế từ Database thông qua Axios và hiển thị lên giao diện.

---

## ⚙️ 2. MÔI TRƯỜNG & CÔNG NGHỆ (TECH STACK)

| Phân hệ | Công nghệ & Công cụ sử dụng |
| :--- | :--- |
| **Backend** | .NET 8.0 SDK, ASP.NET Core MVC, ASP.NET Core Web API, SignalR |
| **Frontend** | React 19, TypeScript 6, Tailwind CSS 3.4, TanStack React Query 5, Axios |
| **Database** | SQL Server (Express/LocalDB), Entity Framework Core (Code-First Migration) |
| **Auth** | JWT Bearer (API) + Cookie Authentication (Admin MVC), Dual Policy Scheme |
| **Real-time** | SignalR WebSocket (`/hubs/notifications`) |
| **Testing** | xUnit + InMemory Database |
| **Công cụ** | Visual Studio 2022, SSMS, Node.js LTS, Postman, Swagger UI |

---

## 🏗 3. KIẾN TRÚC SOLUTION

Dự án được chia thành **4 Project** độc lập:

1. 📦 **CMS.Data (Lớp Dữ liệu):** Class Library chứa `ApplicationDbContext` và định nghĩa toàn bộ **13 thực thể (Entities)** của Database.
2. ⚙️ **CMS.Backend (Lớp Xử lý):** "Trạm điều khiển" cung cấp giao diện quản trị Admin (17 MVC Controllers) và RESTful Web API (12 API Controllers), kèm SignalR Hub, Middleware, Background Services.
3. 🎨 **cms.frontend (Lớp Giao diện):** React SPA với 17 trang, TanStack Query, Context API (Auth, Cart, Wishlist), SignalR client.
4. ✅ **CMS.Tests (Lớp Kiểm thử):** xUnit unit tests với InMemory Database.

---

## 🗄 4. CẤU TRÚC CƠ SỞ DỮ LIỆU (DATABASE SCHEMA)

Hệ thống gồm **13 bảng** với các mối quan hệ phức tạp:

| Tên Bảng | Mô tả |
| :--- | :--- |
| **Users** | Quản trị viên (Admin/Editor), phân quyền theo Role |
| **Categories** | Danh mục tin tức blog (phân cấp ParentId) |
| **Posts** | Bài viết blog với nội dung HTML (CKEditor 5) |
| **CategoriesProducts** | Danh mục sản phẩm hoa |
| **Products** | Sản phẩm hoa (SKU unique, có index trên Price) |
| **Customers** | Khách hàng (Email unique, hỗ trợ reset password token) |
| **Orders** | Đơn hàng với 7 trạng thái, thông tin giao hàng, thanh toán |
| **OrderDetails** | Chi tiết đơn hàng (sản phẩm + số lượng + giá) |
| **DeliverySlots** | Khung giờ giao hàng theo sản phẩm (capacity, day-of-week) |
| **Advertisements** | Banner quảng cáo (vị trí, thời gian hiệu lực) |
| **Payments** | Giao dịch thanh toán (webhook, 4 trạng thái) |
| **RefreshTokens** | JWT Refresh Token quản lý phiên đăng nhập |
| **PhoneBlacklists** | SĐT nằm trong danh sách đen (chống gian lận) |

> 📖 **Xem chi tiết:** [Sơ đồ ER Diagram và mô tả đầy đủ](./docs/architecture-overview.md#3-database-schema)

---

## 🚀 5. TÍNH NĂNG CHÍNH

### 👤 Người dùng (Frontend React SPA)

| Tính năng | Mô tả |
|-----------|-------|
| **Trang chủ** | Hero banner quảng cáo (Slider), sản phẩm bán chạy (top 3), danh sách sản phẩm phân trang, bài viết mới nhất |
| **Cửa hàng** | Lọc theo danh mục, khoảng giá (preset + custom), sắp xếp, phân trang 9 sản phẩm/trang |
| **Tìm kiếm** | Tìm kiếm toàn văn theo tên/SKU sản phẩm |
| **Chi tiết sản phẩm** | Hình ảnh + lightbox, chọn kích thước (Classic/Deluxe/Grand), số lượng, thêm giỏ hàng/mua ngay |
| **Giỏ hàng** | LocalStorage persistence, kiểm tra tồn kho, cập nhật số lượng, xóa |
| **Thanh toán** | Form đa bước (người mua, người nhận, thời gian giao, phương thức thanh toán) |
| **Đơn hàng** | Lịch sử, chi tiết, hủy đơn (nếu hợp lệ) |
| **Yêu thích** | Wishlist lưu trong localStorage |
| **Xác thực** | Đăng nhập/đăng ký, quên mật khẩu, đặt lại mật khẩu qua email |
| **Blog** | Danh sách bài viết phân trang, sidebar danh mục, chi tiết bài viết với rich content |
| **Liên hệ** | Form gửi tin nhắn + thông tin cửa hàng |

### 🔧 Quản trị (Admin Panel MVC)

| Tính năng | Mô tả |
|-----------|-------|
| **Dashboard** | Thống kê tổng quan (đơn hàng, doanh thu, sản phẩm, khách hàng) |
| **Quản lý Sản phẩm** | Thêm/sửa/xóa, upload hình ảnh |
| **Quản lý Đơn hàng** | Duyệt đơn, xác nhận, cập nhật trạng thái, hủy đơn |
| **Quản lý Bài viết** | Soạn thảo nội dung với CKEditor 5 |
| **Quản lý Danh mục** | Danh mục blog & danh mục sản phẩm |
| **Quản lý Người dùng** | Quản trị viên (Admin/Editor), khách hàng |
| **Quản lý Quảng cáo** | Banner slider, vị trí hiển thị, thời gian hiệu lực |
| **Phân quyền** | Admin (full) / Staff (Editor - giới hạn) |

### 🧠 Tính năng Nâng cao

| Tính năng | Mô tả |
|-----------|-------|
| **SignalR Real-time** | Thông báo CRUD đến tất cả client đang kết nối |
| **Fraud Detection** | Kiểm tra SĐT blacklist, buộc OnlinePayment nếu nghi ngờ |
| **Stock Locking** | Giữ chỗ tạm thời khi checkout (15 phút TTL) |
| **Auto-expiry** | Tự động hủy đơn chưa xác minh sau 30 phút |
| **Email Notifications** | Gửi email SMTP qua Gmail khi đơn hàng thay đổi trạng thái |
| **Kích thước sản phẩm** | Classic (giá gốc), Deluxe (+300k), Grand (+600k) |
| **Delivery Slots** | Khung giờ giao hàng linh hoạt, kiểm tra lead-time 2 tiếng |


## 🗓 6. LỘ TRÌNH PHÁT TRIỂN DỰ ÁN (THEO TỪNG BUỔI)

### 🟢 Giai đoạn 1: Nền tảng Backend & Cơ sở dữ liệu
* **Buổi 1: Khởi tạo kiến trúc dự án**
  * Tạo Blank Solution và thiết lập cấu trúc 3 lớp (Data, Backend, Frontend).
  * Viết mã các Class Entity chuẩn hóa.
  * Tạo dữ liệu giả (Mock Data) trong Controller để hiển thị lên View `.cshtml` nhằm kiểm tra kết nối giữa các project.
* **Buổi 2: Kết nối Database với EF Core**
  * Cài đặt NuGet Packages (`SqlServer`, `Tools`, `Design`).
  * Tạo `ApplicationDbContext` và cấu hình Connection String trong `appsettings.json`.
  * Sử dụng kỹ thuật Code-First Migration (`Add-Migration InitialCreate` và `Update-Database`) để sinh tự động các bảng vào SQL Server.
* **Buổi 3: Truy vấn LINQ & Thao tác dữ liệu (CRUD)**
  * Áp dụng LINQ (`Where`, `OrderByDescending`, `Take`) để lọc và sắp xếp dữ liệu.
  * Sử dụng lệnh `.Include()` (Eager Loading) để Join bảng (ví dụ: lấy tên Danh mục kèm theo Bài viết) tránh lỗi dữ liệu rỗng.
  * Thực hiện đầy đủ quy trình Thêm - Xóa - Sửa và lưu vào SQL bằng `SaveChanges()`.

### 🟡 Giai đoạn 2: Xây dựng Admin Panel, Bảo mật & Web API
* **Buổi 4: Giao diện Quản trị Toàn diện (Admin Panel)**
  * Xây dựng `_LayoutAdmin.cshtml` kết hợp Bootstrap với Sidebar điều hướng tĩnh.
  * Xử lý logic Upload hình ảnh (`IFormFile`) vào thư mục `wwwroot/uploads` với tên file sinh ngẫu nhiên bằng `Guid`.
  * Tích hợp trình soạn thảo văn bản CKEditor 5 để biên tập nội dung, dùng `@Html.Raw()` hiển thị mã HTML.
* **Buổi 5: Bảo mật & Phân quyền (Security & Identity)**
  * Cấu hình dịch vụ xác thực `CookieAuthentication` trong `Program.cs`.
  * Xây dựng luồng Đăng nhập (Login Flow) cấp thẻ bài qua Claims (lưu Username, Role).
  * Áp dụng Attribute `[Authorize]` để khóa trang quản trị, và `[Authorize(Roles="Admin")]` để phân quyền chặt chẽ.
* **Buổi 6: Khởi tạo Web API chuẩn RESTful**
  * Cấu hình kiến trúc Lai (Hybrid) chạy song song MVC và API trên cùng một Backend.
  * Cài đặt Swagger sinh tài liệu API tự động để kiểm thử.
  * Thiết kế API Controllers (`[ApiController]`, `[Route("api/[controller]")]`) với các phương thức GET, POST.
  * Áp dụng kỹ thuật Projection (`.Select()`) để gọt tỉa dữ liệu thừa trước khi trả về chuỗi JSON.
  * Test API nhận Đơn hàng (POST) qua công cụ Postman bằng chuỗi JSON thô.

### 🔵 Giai đoạn 3: Kết nối Frontend ReactJS (Client-Side)
* **Buổi 7: Khởi tạo ReactJS & Cấu hình CORS**
  * Mở cửa bảo mật CORS trên Backend (`AllowAnyOrigin`, `AllowAnyMethod`).
  * Khởi tạo ReactJS (`npx create-react-app`), cài đặt thư viện `axios`.
  * Tạo file cấu hình tập trung `axiosClient.js` và viết Service lấy danh mục sản phẩm từ API.
* **Buổi 8: Vòng đời Component & Hook useEffect**
  * Nắm vững cơ chế `useEffect` để kích hoạt việc gọi API (Side Effects).
  * Sử dụng Hook `useState` kết hợp `async/await` để quản lý trạng thái dữ liệu và vòng tải (Loading).
  * Gọi API lấy danh sách Bài viết và Danh mục blog thời trang, xử lý định dạng tiền tệ và ngày tháng (`toLocaleDateString('vi-VN')`).

### 🟣 Giai đoạn 4: Tính năng Nâng cao (Dự kiến lộ trình chuẩn)
* **Buổi 9:** Ứng dụng React Router Dom để làm chức năng điều hướng chi tiết trang bài viết (SPA).
* **Buổi 10:** Xây dựng Giỏ hàng (Cart), truyền đối tượng đơn hàng lên Web API; giới thiệu gRPC Service.
* **Buổi 11:** Kỹ năng gỡ lỗi (Debug) bằng Chrome DevTools, sửa lỗi API, tối ưu SEO cơ bản.
* **Buổi 12:** Tổng kết, Demo ứng dụng E-Commerce Full-stack và bảo vệ đồ án.

---

## ⚠️ 7. NHỮNG LƯU Ý KỸ THUẬT CỐT LÕI

> ⚠️ **Cảnh báo:** Việc bỏ qua các lưu ý dưới đây có thể dẫn đến các lỗi nghiêm trọng về bảo mật, hiệu năng hoặc sập hệ thống.

### 🔴 1. Cấu hình CORS
Middleware `app.UseCors("TênPolicy")` bắt buộc phải nằm **chính xác** giữa `app.UseRouting()` và `app.UseAuthorization()` trong file `Program.cs`. Nếu sai vị trí, ReactJS sẽ bị lỗi *Network Error*.
```csharp
app.UseRouting();
app.UseCors("AllowAll"); // <-- BẮT BUỘC nằm ở đây
app.UseAuthorization();
```

### 🔴 2. Chống lặp vô hạn ở ReactJS
Khi sử dụng Hook `useEffect` để fetch data, bắt buộc phải truyền **mảng phụ thuộc rỗng `[]`** vào tham số thứ hai để API chỉ gọi 1 lần duy nhất khi render.
```javascript
useEffect(() => {
    fetchDataFromAPI();
}, []); // Mảng rỗng ngăn chặn vòng lặp vô hạn
```

### 🔴 3. Tối ưu gói tin JSON
Khi viết Web API lấy danh sách dữ liệu, nên sử dụng cú pháp LINQ `.Select()` để loại bỏ các cột không cần thiết, giúp web tải nhanh hơn.
```csharp
// Chỉ lấy những trường cần thiết
var data = await _context.Posts.Select(x => new { x.Id, x.Title }).ToListAsync();
```

### 🔴 4. Kiểm soát cập nhật Mật khẩu
Trong chức năng Sửa (Edit) User Admin, phải xử lý logic `if (!string.IsNullOrEmpty(NewPassword))` để quyết định sử dụng mật khẩu mới hay giữ nguyên mã Hash cũ, tránh làm mất mật khẩu khi người dùng để trống form.
```csharp
if (!string.IsNullOrEmpty(userDto.NewPassword))
{
    // Mã hóa và cập nhật mật khẩu mới
}
// Ngược lại: Giữ nguyên PasswordHash cũ trong DB
```

### 🔴 5. Ràng buộc toàn vẹn SQL
Việc gọi lệnh xóa một Danh mục (Category) đang có Bài viết (Post) bên trong sẽ gây lỗi hệ thống (Foreign Key Constraint). **Cần xóa hết các bài viết liên kết trước khi xóa danh mục cha.**

---
<p align="center">
  <i>© 2026 AnhCMS_Solution - Full-Stack ASP.NET Core & ReactJS Documentation</i>
</p>
