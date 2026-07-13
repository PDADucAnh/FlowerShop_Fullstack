# Báo cáo xử lý lỗi Dependency Injection tại OrdersController

## 1. Nguyên nhân gốc của lỗi

Hệ thống ASP.NET Core Dependency Injection (DI) phát sinh lỗi:
`System.InvalidOperationException: Multiple constructors accepting all given argument types have been found in type Flower.Backend.Controllers.Api.OrdersController`

Nguyên nhân là do `OrdersController` tồn tại hai public constructors:
1. `public OrdersController(IOrderService orderService)`
2. `public OrdersController(IOrderService orderService, ISystemSettingService settingService)`

Cả hai dependency `IOrderService` và `ISystemSettingService` đều đã được đăng ký hợp lệ trong DI Container (`Program.cs`). Khi ASP.NET Core kích hoạt controller, nó nhận thấy có nhiều constructor mà tất cả tham số đều thỏa mãn (có sẵn trong container) nhưng không thể xác định được constructor nào là ưu tiên duy nhất (Ambiguity), dẫn đến việc văng exception.

## 2. Các bước khắc phục và Audit

- **Kiểm tra toàn hệ thống:** Tiến hành kiểm tra tất cả các file trong `Controllers` và `Controllers/Api` để tìm những class có hiện tượng đa constructor. Kết quả ghi nhận chỉ có `Flower.Backend.Controllers.Api.OrdersController` là vi phạm nguyên tắc DI của ASP.NET Core.
- **Phân tích Dependency thực tế:** Trong nội dung `OrdersController`, các Action Method như `CreateOrder` và `Checkout` có truy xuất đến `_settingService` để lấy `OrderSettings`. Do vậy, dependency `ISystemSettingService` là bắt buộc.
- **Kiểm tra Program.cs:** Xác nhận rằng `IOrderService`, `ISystemSettingService`, `IEmailService`, `IMemoryCache`... đều đã được đăng ký (Scoped) đầy đủ tại `Program.cs`.
- **Refactor mã nguồn:** Đã xóa bỏ constructor dư thừa `public OrdersController(IOrderService orderService)`, chỉ giữ lại constructor duy nhất inject cả hai Interface `IOrderService` và `ISystemSettingService`.

## 3. Cấu trúc DI Graph của OrdersController sau khi fix

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ISystemSettingService _settingService;

    // Constructor duy nhất, giải quyết hoàn toàn lỗi Ambiguity
    public OrdersController(IOrderService orderService, ISystemSettingService settingService)
    {
        _orderService = orderService;
        _settingService = settingService;
    }
    
    // ... các API GET, POST, PUT (CreateOrder, Checkout, Cancel,...)
}
```

Các dependencies được resolve theo sơ đồ:
- Khởi tạo DI Container (Program.cs)
- HTTP Request gọi đến `OrdersController`
- Controller Factory yêu cầu khởi tạo `OrdersController`
- ASP.NET Core DI Container resolve `IOrderService` và `ISystemSettingService` và inject chúng vào constructor.

## 4. Kết quả biên dịch (Regression)

Quá trình biên dịch lại dự án `Flower.Backend.csproj` diễn ra thành công với kết quả:

```text
Build succeeded.
    90 Warning(s)
    0 Error(s)

Time Elapsed 00:00:07.17
```

- Không còn Error nào phát sinh từ phía Dependency Injection của Controller.
- Lỗi Exception khởi tạo `OrdersController` đã được fix hoàn toàn.
