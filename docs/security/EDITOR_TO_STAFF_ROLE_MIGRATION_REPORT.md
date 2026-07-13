# Báo Cáo Chuẩn Hóa Vai Trò Người Dùng (Editor To Staff Role Migration Report)

Tài liệu chi tiết về quá trình chuẩn hóa vai trò người quản trị phụ từ "Editor" thành "Staff" trên toàn bộ dự án FlowerShop, bao gồm cập nhật mã nguồn, thiết lập migration dữ liệu và cấu hình phân quyền.

---

## 1. Nguyên nhân cần đổi Role
Để đồng nhất với tài liệu thiết kế, nghiệp vụ thực tế của dự án FlowerShop và báo cáo đồ án tốt nghiệp, vai trò nhân viên vận hành cần sử dụng tên gọi chuẩn hóa duy nhất là **`Staff`** thay vì **`Editor`** (Biên tập viên) như trước. 
Yêu cầu đặt ra là phải rà soát, loại bỏ hoàn toàn tên gọi cũ `"Editor"` ở mọi lớp hạ tầng, CSDL, Claims, Policy và Views mà không làm gián đoạn hệ thống hoặc mất dữ liệu người dùng.

---

## 2. Danh sách tất cả file đã sửa

### Backend & Database:
- [Flower.Backend/Controllers/AccountController.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Controllers/AccountController.cs): Cập nhật kiểm tra vai trò khi đăng nhập chỉ cho phép `"Admin"` và `"Staff"`.
- [Flower.Backend/Program.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Program.cs): Đổi vai trò yêu cầu của Policy `StaffOnly` từ `"Editor"` sang `"Staff"`.
- [Flower.Data/Migrations/20260713084052_UpdateEditorToStaffRole.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Data/Migrations/20260713084052_UpdateEditorToStaffRole.cs): Tệp migration CSDL thực hiện đổi vai trò của các bản ghi có sẵn trong DB.

### Unit Tests:
- [Flower.Tests/UserServiceTests.cs](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Tests/UserServiceTests.cs): Cập nhật dữ liệu giả lập và các câu lệnh khẳng định (Assert) từ `"Editor"` sang `"Staff"`.

### Razor Views (UI):
- [Flower.Backend/Views/User/Create.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/User/Create.cshtml): Đổi giá trị option của select dropdown từ `"Editor"` sang `"Staff"`.
- [Flower.Backend/Views/User/Edit.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/User/Edit.cshtml): Đổi giá trị option của select dropdown từ `"Editor"` sang `"Staff"`.

### Tài liệu & Tài nguyên dự án:
- [README.md](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/README.md): Cập nhật tài liệu vai trò người dùng trong hệ thống.
- [docs/architecture-overview.md](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/docs/architecture-overview.md): Cập nhật mô tả thực thể `User` và Policy `StaffOnly`.
- [docs/promotions/PROMOTION_ANALYSIS_RESULT.md](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/docs/promotions/PROMOTION_ANALYSIS_RESULT.md): Cập nhật tài liệu chính sách xác thực và các loại người dùng.
- [docs/security/AUTHENTICATION_SECURITY_FIX_REPORT.md](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/docs/security/AUTHENTICATION_SECURITY_FIX_REPORT.md): Cập nhật mô tả luồng phân quyền và chính sách `StaffOnly`.
- [docs/security/STAFF_ROLE_PERMISSION_IMPLEMENTATION_REPORT.md](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/docs/security/STAFF_ROLE_PERMISSION_IMPLEMENTATION_REPORT.md): Cập nhật báo cáo phân quyền Staff và biểu đồ luồng RBAC.

---

## 3. Các bảng Database đã cập nhật
- **Bảng `Users`**: Cập nhật giá trị cột `Role` của toàn bộ bản ghi người dùng từ `"Editor"` sang `"Staff"`.
- Việc cập nhật được thực thi thông qua câu lệnh SQL Update an toàn, không tạo mới tài khoản, không làm mất mật khẩu đã mã hóa và giữ nguyên Refresh Token.

---

## 4. Migration đã tạo
Mã migration Entity Framework Core `20260713084052_UpdateEditorToStaffRole.cs` đã được sinh ra và áp dụng thành công vào cơ sở dữ liệu:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("UPDATE Users SET Role = 'Staff' WHERE Role = 'Editor'");
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.Sql("UPDATE Users SET Role = 'Editor' WHERE Role = 'Staff'");
}
```

---

## 5. Các Entity đã sửa
- Thực thể `User` sử dụng kiểu dữ liệu string cho trường `Role`. Do đó, không cần thay đổi định nghĩa Class mà chỉ thay đổi cách xử lý dữ liệu và thiết lập giá trị mặc định/lựa chọn trong view.

---

## 6. Các Controller đã sửa
- **AccountController**: Đổi cờ lọc vai trò khi đăng nhập Admin MVC từ `Editor` thành `Staff`.

---

## 7. Các API đã sửa
- Cơ chế sinh token JWT trong `AuthController.cs` trích xuất vai trò trực tiếp từ thuộc tính `Role` trong Database của User. Vì vai trò trong CSDL đã chuyển dịch hoàn toàn sang `"Staff"`, JWT Claims mới sinh ra sẽ tự động chứa claim `Role = "Staff"` mà không cần thay đổi mã nguồn sinh Token.

---

## 8. Các Razor Views đã sửa
- Dropdown chọn vai trò khi Tạo/Sửa thông tin người dùng quản trị ([Views/User/Create.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/User/Create.cshtml) và [Views/User/Edit.cshtml](file:///D:/TrenLop/ThucTapTaiTruong/FlowerShop/Flower.Backend/Views/User/Edit.cshtml)) đã chuyển giá trị value thành `"Staff"` và nhãn hiển thị thành `"Nhân viên"`.

---

## 9. Các Policy đã sửa
- Policy **`StaffOnly`** trong `Program.cs` đã được cập nhật vai trò yêu cầu:
  ```csharp
  options.AddPolicy("StaffOnly", policy =>
      policy.RequireRole("Admin", "Staff"));
  ```

---

## 10. Các JWT Claims đã sửa
- Khi người dùng đăng nhập thành công, claim vai trò (`ClaimTypes.Role`) được trích xuất từ cột `Role` của bảng `Users`. Token mới được sinh ra sẽ tự động nhận giá trị `"Staff"`, giúp phân hệ Frontend/API xác thực quyền chính xác.

---

## 11. Các tài liệu đã cập nhật
- Cập nhật toàn bộ các file markdown liên quan trong thư mục `/docs` và file `README.md` ở gốc dự án, thay thế các khái niệm "Editor" bằng "Staff" để tránh gây nhầm lẫn khi bàn giao hoặc chấm điểm đồ án.

---

## 12. Kết quả kiểm thử (Test Cases & Results)
- **Unit Tests**: Chạy toàn bộ bộ test `Flower.Tests.csproj` thành công 100% (**37/37 Tests Passed**).
- **Admin Login & Staff Login**: Đăng nhập bằng tài khoản admin hoặc tài khoản có role `Staff` đều truy cập được bình thường.
- **Customer Blocked**: Tài khoản khách hàng đăng nhập vào trang Admin MVC sẽ bị báo lỗi phân quyền thích hợp.
- **RBAC Checks**: Staff đăng nhập chỉ nhìn thấy các menu được cấp và bị chặn từ chối truy cập (403) đối với các hành động CRUD (Create, Update, Delete) của Promotion, Coupon, User, v.v.

---

## 13. Kết quả build
- **Backend Build**: Biên dịch thành công với `0 Errors`.
- **Frontend Build**: Biên dịch thành công với `0 Errors`.
