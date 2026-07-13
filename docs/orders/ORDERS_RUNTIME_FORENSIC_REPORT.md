# BÁO CÁO FORENSIC AUDIT: API ORDERS

*Ngày phân tích: 14/07/2026*

## 1. Số lượng Public Constructor của OrdersController
Sau khi fix tại thời điểm hiện tại, `Flower.Backend.Controllers.Api.OrdersController` chỉ còn **1 public constructor** duy nhất. 
- **Constructor 1:** `public OrdersController(IOrderService orderService, ISystemSettingService settingService)`
- **Vị trí file:** `Flower.Backend\Controllers\Api\OrdersController.cs`
- **Số dòng:** Khai báo tại dòng 20.

*(Lưu ý: Nếu bạn vẫn gặp lỗi "Multiple constructors accepting all given argument types have been found in type OrdersController", điều này có nghĩa là ứng dụng đang chạy một bản DLL compile cũ trước khi file code này được fix).*

## 2. Kiểm tra Partial Class OrdersController
- **Không có** bất kỳ `partial class OrdersController` nào trong toàn bộ dự án.

## 3. Kiểm tra các file OrdersController khác trong Project
- Đã quét toàn bộ thư mục gốc, `Controllers`, `Controllers/Api`, `obj`, `bin`, `Generated`.
- **Kết quả:** Không có file nào khác mang tên `OrdersController.cs`. 
- *(Lưu ý: Hệ thống có tồn tại một file `OrderController.cs` số ít dành cho giao diện MVC Admin, nằm tại `Flower.Backend\Controllers\OrderController.cs`, nhưng nó không xung đột với `OrdersController` API).*

## 4. Kiểm tra Program.cs và DI Registrations
Đã verify quá trình đăng ký dịch vụ trong `Program.cs`:
- **IOrderService:** Đã đăng ký (Scoped - Line 140)
- **ISystemSettingService:** Đã đăng ký (Scoped - Line 145)
- **ILogger:** Đã đăng ký (Tích hợp sẵn của ASP.NET Core Framework)
- **IMemoryCache:** Đã đăng ký (`AddMemoryCache` - Line 162)
- **EmailService:** Đã đăng ký (Scoped - Line 161)
- **IMapper (AutoMapper):** ⚠️ **KHÔNG ĐƯỢC ĐĂNG KÝ**. (Không có `AddAutoMapper` trong `Program.cs`. Tuy API hiện tại không gọi trực tiếp nhưng nếu module nào inject `IMapper`, nó sẽ gây Exception "Unable to resolve service").

## 5. Phân tích Request POST /api/Orders
**Q:** Request đã đi vào Action `CreateOrder` hay chết trước khi tạo Controller?
**A:** **Chết trước khi tạo Controller** (Dừng ở bước DI -> Controller Factory).

## 6. Lỗi Exception Đầu Tiên (Nếu Action Đã Chạy)
Action `CreateOrder` **CHƯA ĐƯỢC CHẠY**. Sẽ không có exception nào bắt nguồn từ logic code bên trong hàm `CreateOrder` lúc này.

## 7. Giải Thích Nguyên Nhân Action Chưa Chạy (LỖI THỰC SỰ HIỆN TẠI)
Mặc dù `OrdersController` đã được fix chỉ còn 1 constructor, nhưng quá trình resolve của DI Container vẫn sẽ thất bại vì dependency bên dưới của nó là `OrderService` **lại đang có chứa lỗi Multiple Constructors**.

Cụ thể, class `Flower.Backend.Services.OrderService` đang sở hữu **2 public constructors**:
- **Constructor 1 (14 arguments):** Dòng 36-55. 
- **Constructor 2 (15 arguments):** Dòng 57-73 (Chứa thêm tham số `IShippingService`).

Vì cả hai Interface `IAdminNotificationService` và `IShippingService` đều đã được đăng ký hợp lệ trong `Program.cs`, ASP.NET Core DI Container sẽ không biết phải chọn constructor nào của `OrderService` để khởi tạo.
=> **Exception hiện tại (nếu chạy mã mới nhất):**
`System.InvalidOperationException: Multiple constructors accepting all given argument types have been found in type 'Flower.Backend.Services.OrderService'.`

## 8. Trạng Thái Của Visual Studio và Dll
Dự án được biên dịch qua lệnh `dotnet build` trả về **0 Error(s)** và **2 Warning(s)**. Code hoàn toàn hợp lệ về cú pháp. 
Tuy nhiên, lỗi Ambiguity Constructor là lỗi **Runtime** (chỉ văng ra khi chạy app gọi Route hoặc khi tạo Scope API).
Nếu bạn vẫn gặp lỗi `Multiple constructors... in type 'Flower.Backend.Controllers.Api.OrdersController'`, đó là bằng chứng cho thấy **Visual Studio / IIS Express chưa load DLL mới nhất** (Có thể do lỗi cache build hoặc IDE chưa restart). Tuy nhiên sau khi clear cache và load dll mới, bạn sẽ gặp ngay lỗi tương tự tại `OrderService`.

## 9. Kết Luận
- `OrdersController` đã sạch lỗi đa constructor.
- Request `/api/Orders` hiện tại đang bị chặn đứng ở `Controller Factory` do quá trình inject `IOrderService` thất bại (Vì `OrderService` có 2 constructor).
- Cần tiếp tục refactor xóa bỏ constructor thứ nhất của `OrderService` để API có thể tiến vào Action. 

*(Báo cáo được thực hiện bằng cách đọc mã nguồn thuần túy, không tạo commit hay sửa đổi dữ liệu DB).*
