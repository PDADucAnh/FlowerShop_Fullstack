# Báo Cáo Khắc Phục Lỗi "Bắt Buộc Nhập Lại Mật Khẩu Khi Edit Customer"

## 1. Root Cause
- Trong phiên bản trước, view `Edit.cshtml` đặt thuộc tính `required` ở thẻ `<input asp-for="PasswordHash">`, khiến cho client-side validation luôn chặn form nếu để trống.
- Ngoài ra, ở phía backend (`MappingExtensions.cs` và `CustomerService.cs`), mặc dù có kiểm tra logic nhưng `CustomerService` bị trùng lặp thao tác băm (hash) mật khẩu và dẫn tới nguy cơ lỗi nếu không xử lý khoảng trắng. Lỗi UX chính là bắt người dùng (Admin) phải liên tục cập nhật mật khẩu mặc dù họ chỉ muốn đổi thông tin khác (như Số điện thoại, Trạng thái).

## 2. Files Modified
- `Flower.Backend/Views/Customer/Edit.cshtml`
- `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- `Flower.Backend/Services/CustomerService.cs`
- *Lưu ý:* File `CustomerDTOs.cs` (cụ thể là `UpdateCustomerDTO`) đã định nghĩa `public string? PasswordHash { get; set; }` (không có `[Required]`) nên được giữ nguyên.

## 3. DTO Changes
- `UpdateCustomerDTO` không yêu cầu thay đổi gì thêm do thuộc tính `PasswordHash` đã có thể null (`string?`) và không dính attribute `[Required]`.

## 4. View Changes
- **Edit.cshtml**:
  - Gỡ bỏ thuộc tính `required` ở input `PasswordHash`.
  - Đổi Label từ "Mật khẩu hiện tại" thành "Mật khẩu mới (để trống nếu không đổi)".
  - Đổi type của input sang `type="password"`.
  - Thêm `placeholder="Để trống nếu không muốn thay đổi mật khẩu"` để rõ ràng trải nghiệm UX.
  - Bảo đảm `PasswordHash` không được truyền ngược từ Controller xuống View (logic Controller hiện tại tạo `UpdateCustomerDTO` mới và không map `customer.PasswordHash`, do đó View không hiển thị các chuỗi hash nguy hiểm).

## 5. Mapping Changes & Password Update Flow
- **MappingExtensions.cs**: Cập nhật phương thức `UpdateEntity` dùng `!string.IsNullOrWhiteSpace` để đảm bảo chuỗi rỗng/chứa khoảng trắng không bị lọt vào DB. Khi mật khẩu hợp lệ (có thay đổi), chính MappingExtensions sẽ khởi tạo `PasswordHasher<Customer>` và gán `PasswordHash` đã được băm an toàn vào Entity.
- **CustomerService.cs**: Gỡ bỏ block code xử lý băm mật khẩu do MappingExtensions đã chịu trách nhiệm trọn vẹn bước này. Việc thay đổi này giải quyết tình trạng double-hash (nếu có) và tuân thủ đúng nguyên tắc Single Responsibility.

## 6. Regression Test
- **Sửa thông tin không nhập mật khẩu**: Edit tên, số điện thoại, địa chỉ, trạng thái IsActive. Trưởng hợp này `PasswordHash` từ Form gửi về là `null` hoặc `""`. MappingExtensions bỏ qua khối lệnh if. Mật khẩu không bị đổi/xóa => Pass.
- **Sửa thông tin có nhập mật khẩu**: Nhập giá trị mới vào ô mật khẩu. MappingExtensions nhận giá trị có nghĩa, khởi chạy hàm HashPassword và gán Hash mới cho Entity. Sau khi SaveChanges, password cũ vô tác dụng, password mới đăng nhập thành công => Pass.
- **Bảo mật**: View hiện tại không phơi bày chuỗi hash. Server không bao giờ trả về PasswordHash hiện tại cho Edit form => Pass.

## 7. Build Result
- Quá trình biên dịch .NET không xảy ra lỗi (0 Errors). Lệnh `dotnet build Flower.Backend/Flower.Backend.csproj` đã hoàn tất thành công.
- Dự án giữ nguyên cấu trúc MVC truyền thống mà không làm ảnh hưởng đến các Module Authentication hay Dynamic Settings hiện hành.
