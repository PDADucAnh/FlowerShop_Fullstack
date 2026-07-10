# 7. Tóm tắt cơ cấu cơ sở dữ liệu

Tài liệu này mô tả chi tiết sơ đồ cơ sở dữ liệu (Database Schema) của hệ thống PDA FlowerShop dựa trên các thực thể thực tế (Entity Classes) được định nghĩa trong mã nguồn của dự án (Entity Framework Core).

## 7.1. Bảng Users (Người dùng hệ trị)

Bảng lưu trữ thông tin các tài khoản có quyền truy cập vào giao diện quản trị CMS (Admin, Editor).

* **Mục đích:** Quản lý thông tin đăng nhập và phân quyền của cán bộ nhân viên cửa hàng.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Ràng buộc và chỉ mục:** Tên đăng nhập `Username` không được trùng lặp.
* **Các trường quan trọng:**
    * `Username` (string, độ dài tối đa 50): Tên đăng nhập.
    * `PasswordHash` (string): Mật khẩu đã được mã hóa băm.
    * `FullName` (string, độ dài tối đa 200): Họ tên đầy đủ của nhân viên.
    * `Email` (string, độ dài tối đa 200): Địa chỉ email liên lạc.
    * `Phone` (string, độ dài tối đa 20): Số điện thoại liên lạc.
    * `Role` (string, độ dài tối đa 50): Vai trò hệ thống (Admin, Editor).
* **Quy tắc nghiệp vụ:** Bảng này tách biệt hoàn toàn với bảng Khách hàng. Nhân viên không mua hàng trực tiếp bằng tài khoản quản trị.

## 7.2. Bảng Customers (Khách hàng)

Bảng lưu trữ thông tin tài khoản người mua hoa trên hệ thống trực tuyến.

* **Mục đích:** Quản lý dữ liệu người mua hàng, thông tin bảo mật và đánh giá mức độ uy tín.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Ràng buộc và chỉ mục:** Email đăng ký của khách hàng phải là duy nhất.
* **Các trường quan trọng:**
    * `FullName` (string): Họ và tên khách hàng.
    * `Email` (string): Địa chỉ thư điện tử dùng làm tài khoản đăng nhập.
    * `Phone` (string): Số điện thoại liên lạc của khách hàng.
    * `PasswordHash` (string): Mật khẩu đăng nhập đã băm.
    * `FraudScore` (int): Điểm số đánh giá hành vi gian lận (bùng đơn).
    * `IsBlacklisted` (bool): Đánh dấu khách hàng bị đưa vào danh sách đen.
    * `FailedDeliveries` (int): Số đơn giao thất bại.
    * `SuccessfulDeliveries` (int): Số đơn giao thành công.
    * `TotalOrders` (int): Tổng số đơn hàng đã đặt.
    * `ResetToken` (string, độ dài tối đa 100): Token khôi phục mật khẩu.
    * `ResetTokenExpiry` (DateTime): Thời gian hết hạn của token khôi phục mật khẩu.

## 7.3. Bảng Products (Sản phẩm hoa)

Bảng lưu trữ thông tin chi tiết về các sản phẩm hoa được bày bán trên trang web.

* **Mục đích:** Quản lý dữ liệu sản phẩm hoa, giá cả và tồn kho thực tế.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `CategoryProductId` tham chiếu đến khóa chính `Id` của bảng `CategoriesProducts`.
* **Chỉ mục:** Chỉ mục duy nhất trên cột `Sku` và cột `Slug`.
* **Các trường quan trọng:**
    * `Sku` (string, độ dài tối đa 50): Mã định danh quản lý kho của sản phẩm.
    * `Name` (string, độ dài tối đa 200): Tên sản phẩm hoa tươi.
    * `Description` (string): Mô tả chi tiết về sản phẩm hoa.
    * `Slug` (string, độ dài tối đa 300): Đường dẫn thân thiện dùng cho SEO.
    * `Price` (decimal): Giá bán gốc.
    * `DiscountPrice` (decimal, nullable): Giá khuyến mãi.
    * `StockQuantity` (int): Số lượng sản phẩm khả dụng trong kho.
    * `ImageUrl` (string): Đường dẫn ảnh sản phẩm.
    * `ViewCount` (int): Lượt xem chi tiết sản phẩm.
    * `AddToCartCount` (int): Lượt thêm vào giỏ hàng.

## 7.4. Bảng CategoriesProducts (Danh mục sản phẩm)

Bảng định nghĩa các nhóm sản phẩm hoa tươi để phân loại.

* **Mục đích:** Hỗ trợ phân loại và tìm kiếm sản phẩm hoa nhanh chóng.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Các trường quan trọng:**
    * `Name` (string, độ dài tối đa 200): Tên danh mục (ví dụ: Hoa sinh nhật, Hoa khai trương).
    * `Description` (string, độ dài tối đa 2000): Mô tả danh mục.
    * `Slug` (string, độ dài tối đa 300): Đường dẫn thân thiện của danh mục.

## 7.5. Bảng Orders (Đơn hàng)

Bảng chứa thông tin cốt lõi của các đơn đặt hàng trong hệ thống.

* **Mục đích:** Theo dõi thông tin đặt hàng, trạng thái xử lý, phương thức thanh toán và thông tin giao nhận.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `CustomerId` tham chiếu đến khóa chính `Id` của bảng `Customers`.
* **Các trường quan trọng:**
    * `OrderDate` (DateTime): Thời gian khách hàng đặt đơn.
    * `Status` (int): Trạng thái đơn hàng (lưu dưới dạng số nguyên ánh xạ từ OrderStatus Enum).
    * `Notes` (string): Ghi chú giao hàng của khách hàng.
    * `DeliveryDate` (DateTime): Ngày yêu cầu giao hàng.
    * `DeliveryTimeSlot` (string, độ dài tối đa 50): Khung giờ giao hàng yêu cầu.
    * `DeliveryAddress` (string, độ dài tối đa 500): Địa chỉ giao hàng đầy đủ.
    * `RecipientName` (string, độ dài tối đa 200): Họ tên người nhận hoa.
    * `RecipientPhone` (string, độ dài tối đa 20): Số điện thoại người nhận hoa.
    * `PaymentMethod` (int): Phương thức thanh toán (COD hoặc OnlinePayment).
    * `PaymentStatus` (int): Trạng thái thanh toán giao dịch.
    * `IsVerified` (bool): Trạng thái xác minh đơn hàng (đặc biệt với COD).
    * `CancellationReason` (string, độ dài tối đa 500): Lý do hủy đơn hàng.
    * `RefundAmount` (decimal): Số tiền cửa hàng phải hoàn lại cho khách.
* **Quy tắc nghiệp vụ:** Địa chỉ giao hàng, họ tên và số điện thoại người nhận được sao chép trực tiếp dưới dạng Snapshot tại thời điểm đặt đơn. Nếu khách hàng cập nhật hồ sơ sau này, thông tin đơn hàng cũ vẫn giữ nguyên.

## 7.6. Bảng OrderDetails (Chi tiết đơn hàng)

Bảng lưu danh sách các sản phẩm cụ thể và số lượng tương ứng trong mỗi đơn hàng.

* **Mục đích:** Lưu giữ chi tiết hóa đơn mua hàng thời điểm đặt hàng.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:**
    * `OrderId` tham chiếu đến bảng `Orders`.
    * `ProductId` tham chiếu đến bảng `Products`.
* **Các trường quan trọng:**
    * `Quantity` (int): Số lượng mua.
    * `UnitPrice` (decimal): Đơn giá bán tại thời điểm đặt hàng.
    * `ProductName` (string, độ dài tối đa 200): Tên sản phẩm lưu vết thời điểm mua.
    * `ProductImage` (string, độ dài tối đa 1000): Đường dẫn ảnh sản phẩm lưu vết.

## 7.7. Bảng Payments (Giao dịch thanh toán)

Bảng quản lý lịch sử thanh toán chi tiết của các đơn hàng.

* **Mục đích:** Quản lý toàn bộ giao dịch thanh toán VNPay và COD, hỗ trợ thanh toán lại khi giao dịch lỗi.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `OrderId` tham chiếu đến bảng `Orders`.
* **Các trường quan trọng:**
    * `Amount` (decimal): Số tiền thanh toán.
    * `Method` (int): Phương thức thanh toán thực tế.
    * `Status` (int): Trạng thái giao dịch (Pending, Completed, Failed, Refunded).
    * `TransactionId` (string, độ dài tối đa 200): Mã giao dịch của VNPay.
    * `PaidAt` (DateTime): Thời gian thanh toán thành công.
* **Quy tắc nghiệp vụ:** Một đơn hàng có thể có nhiều bản ghi giao dịch thanh toán trong bảng này (ví dụ: giao dịch đầu tiên bị thất bại hoặc khách hủy thanh toán giữa chừng, sau đó khách thực hiện thanh toán lại thành công ở bản ghi giao dịch thứ hai).

## 7.8. Bảng DeliverySlots (Khung giờ giao hàng)

Bảng quản lý năng lực phân phối và giới hạn số lượng chuẩn bị hoa.

* **Mục đích:** Phân bổ nhân lực cắm hoa và vận chuyển tránh quá tải.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `ProductId` tham chiếu đến bảng `Products`.
* **Các trường quan trọng:**
    * `DeliveryDate` (DateTime): Ngày giao hàng.
    * `TimeSlot` (string, độ dài tối đa 50): Chuỗi mô tả khung giờ (ví dụ: 09:00-11:00).
    * `MaxCapacity` (int): Số lượng đơn hàng tối đa có thể nhận trong khung giờ.
    * `CurrentBooked` (int): Số lượng đơn hàng thực tế khách đã đặt.
    * `IsActive` (bool): Trạng thái hoạt động của khung giờ.

## 7.9. Bảng Categories (Danh mục bài viết blog)

* **Mục đích:** Quản lý nhóm bài viết.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Các trường quan trọng:** `Name` (độ dài tối đa 200), `Description` (độ dài tối đa 2000), `Slug` (độ dài tối đa 300).

## 7.10. Bảng Posts (Bài viết blog)

* **Mục đích:** Lưu trữ các bài viết giới thiệu ý nghĩa hoa, cẩm nang cắm hoa.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `CategoryId` tham chiếu đến bảng `Categories`.
* **Các trường quan trọng:**
    * `Title` (string, độ dài tối đa 500): Tiêu đề bài viết.
    * `Content` (string): Nội dung định dạng HTML.
    * `Summary` (string, độ dài tối đa 500): Tóm tắt nội dung bài viết.
    * `Slug` (string, độ dài tối đa 300): Đường dẫn SEO.
    * `ImageUrl` (string, độ dài tối đa 1000): Ảnh đại diện bài viết.
    * `CreatedDate` (DateTime): Ngày đăng bài.

## 7.11. Bảng PhoneBlacklists (Danh sách đen số điện thoại)

* **Mục đích:** Lưu trữ các số điện thoại có dấu hiệu gian lận hoặc bùng hàng.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Các trường quan trọng:**
    * `PhoneNumber` (string, độ dài tối đa 20): Số điện thoại bị khóa.
    * `Reason` (string, độ dài tối đa 500): Lý do khóa.
    * `CreatedAt` (DateTime): Ngày đưa vào danh sách đen.
    * `IsActive` (bool): Trạng thái hoạt động của lệnh cấm.

## 7.12. Bảng RefreshTokens (Phiên token làm mới)

* **Mục đích:** Lưu vết các refresh token để hỗ trợ cơ chế bảo mật đăng nhập tự động.
* **Khóa chính:** `Id` (kiểu dữ liệu int, tự tăng).
* **Khóa ngoại:** `UserId` tham chiếu đến bảng `Users` (hoặc liên kết gián tiếp).
* **Các trường quan trọng:**
    * `TokenHash` (string, độ dài tối đa 256): Bản băm của refresh token.
    * `ExpiresAt` (DateTime): Thời gian token hết hạn.
    * `IsRevoked` (bool): Đã bị thu hồi hay chưa.
    * `RevokedAt` (DateTime): Thời gian thu hồi token.
    * `DeviceInfo` (string, độ dài tối đa 50): Thiết bị thực hiện đăng nhập.
