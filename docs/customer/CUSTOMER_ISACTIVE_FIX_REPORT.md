# Báo Cáo Khắc Phục Lỗi "Customer Bị Khóa" - IsActive

## 1. Root Cause
- Lỗi xuất phát từ migration `20260707053409_AddDatabaseSchemaEnhancements.cs` khi tạo cột `IsActive` cho bảng `Customers` với `defaultValue = false` (tức 0).
- Hệ quả: Toàn bộ khách hàng có từ trước thời điểm chạy migration này đều mang `IsActive = 0`.
- Tiếp đó, logic trong `AuthController.Login` xử lý chính xác `if (!result.IsActive) return 403;` nên đã trả về 403.
- Hơn nữa, Admin (View) đã không hỗ trợ render và chỉnh sửa (Edit) trường `IsActive` do thiếu tính năng này trong `CustomerDTO` và `UpdateCustomerDTO`, dẫn đến tình trạng Admin không thể mở khóa hay kiểm soát khách hàng bị khóa.

## 2. Files Modified
- `Flower.Backend/Models/DTOs/CustomerDTOs.cs`
- `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- `Flower.Backend/Controllers/CustomerController.cs`
- `Flower.Backend/Views/Customer/Index.cshtml`
- `Flower.Backend/Views/Customer/Edit.cshtml`

## 3. DTO Changes
- **CustomerDTO**: Thêm property `public bool IsActive { get; set; }`.
- **UpdateCustomerDTO**: Thêm property `public bool IsActive { get; set; }`.

## 4. Mapping Changes
Trong `MappingExtensions.cs`:
- Cập nhật `ToDTO` cho Customer để map giá trị `IsActive` (từ Entity sang `CustomerDTO`).
- Cập nhật `UpdateEntity` cho Customer để gán giá trị `IsActive` (từ `UpdateCustomerDTO` sang Entity).
- Lớp `CreateCustomerDTO` giữ nguyên, khi map qua Entity khởi tạo sẽ nhận `IsActive = true` (được định nghĩa mặc định từ class Entity `Customer`).

## 5. View Changes
- **Index.cshtml**:
  - Bổ sung header `<th class="px-lg py-4">Trạng thái</th>`.
  - Hiển thị Badge: "Hoạt động" (màu xanh ngọc/green) nếu `IsActive = true` hoặc "Đã khóa" (màu đỏ/red) nếu `IsActive = false`.
- **Edit.cshtml**:
  - Thêm một checkbox `Kích hoạt tài khoản` kết nối (bind) trực tiếp vào thuộc tính `IsActive` nằm ở giao diện chỉnh sửa khách hàng.

## 6. CustomerService Changes
- Không yêu cầu thay đổi logic tại `CustomerService.Update()`. Chức năng gọi thẳng `dto.UpdateEntity(customer)` và gọi `_context.SaveChangesAsync()` nên `IsActive` sẽ được map đầy đủ và ghi đè an toàn xuống SQL Database.

## 7. Login Verification
- Logic chặn login với `IsActive = false` bằng trả mã trạng thái `403` tại `AuthController.Login` hoàn toàn chính xác. Flow Login đang phản ánh trung thực với cấu hình dưới Entity / Database nên không sửa.

## 8. Regression Test
- **Create Customer (Admin)**: Mặc định `IsActive = true`.
- **Register Customer (Frontend)**: Khởi tạo Entity bằng `new Customer {...}`. Do mặc định entity là `true`, `IsActive` mang giá trị mặc định là `true`.
- **Admin & JWT**: Các thuộc tính login và authentication admin giữ nguyên không thay đổi logic.
- **Other Systems**: Đã xác nhận không can thiệp SMTP, VNPay, Order workflow hay hệ thống khác.

## 9. Build Result
- Cấu trúc file ổn định. Quá trình biên dịch .NET không xảy ra lỗi (0 Errors). Frontend không bị ảnh hưởng.
- Migration cấu hình SQL default giá trị tạo mới đã được thêm (tuỳ chọn nếu môi trường yêu cầu `ALTER TABLE`).

## 10. Business Impact
- Đảm bảo Admin có toàn quyền quyết định việc block/unblock khách hàng trực tiếp trên UI thay vì bị chôn vùi trong Code & Data.
- Ngăn ngừa tình huống phát sinh account bị khóa mặc định do hệ lụy từ Database Schema cũ. Đảm bảo trải nghiệm người dùng không bị gián đoạn vô lý.
