# BÁO CÁO: KHẮC PHỤC LỖI DEPENDENCY INJECTION TẠI ORDERSERVICE

*Ngày thực hiện: 14/07/2026*

## 1. Root Cause (Nguyên nhân gốc)
Sau khi fix lỗi tại `OrdersController`, tiến trình phân giải DI tiếp tục thất bại khi khởi tạo `IOrderService` do `Flower.Backend.Services.OrderService` vi phạm nguyên tắc Dependency Injection của ASP.NET Core: **Tồn tại nhiều hơn 1 public constructor có thể được resolve**.

Cụ thể, `OrderService` có 2 constructor:
1. Constructor cũ (14 tham số), không có `IShippingService`.
2. Constructor mới (15 tham số), được thêm vào trong quá trình refactor, có chứa `IShippingService`.

Vì toàn bộ các interface (bao gồm cả `IShippingService`) đều đã được đăng ký hợp lệ trong `Program.cs`, ASP.NET Core DI Container không thể quyết định constructor nào sẽ được sử dụng (Ambiguity), dẫn đến Exception: 
`System.InvalidOperationException: Multiple constructors accepting all given argument types have been found in type 'Flower.Backend.Services.OrderService'.`

Hậu quả: Lỗi này chặn đứng request ngay tại Controller Factory, khiến API `POST /api/Orders` chết trước khi kịp tiến vào Action.

## 2. Constructor Trước Khi Sửa
Tồn tại constructor thừa kế gọi lại `this(...)`:

```csharp
public OrderService(
    IApplicationDbContext context,
    ILogger<OrderService> logger,
    IHttpContextAccessor httpContextAccessor,
    IDeliverySlotService deliverySlotService,
    IPaymentService paymentService,
    IFraudDetectionService fraudDetectionService,
    StockLockService stockLockService,
    IEmailService emailService,
    TimeSettings timeSettings,
    IMemoryCache memoryCache,
    IOrderCancellationService orderCancellationService,
    IPromotionService promotionService,
    ICouponService couponService,
    IAdminNotificationService adminNotificationService)
    : this(context, logger, httpContextAccessor, deliverySlotService, paymentService,
           fraudDetectionService, stockLockService, emailService, timeSettings, memoryCache,
           orderCancellationService, promotionService, couponService, adminNotificationService, null!)
{
}
```

## 3. Constructor Sau Khi Sửa
Đã gỡ bỏ constructor legacy ở trên, chỉ giữ lại constructor đầy đủ nhất (15 tham số). Tất cả 15 dependency đều được gán cho 15 private readonly field tương ứng và tất cả các field này đều được dùng trong class.

```csharp
public OrderService(
    IApplicationDbContext context,
    ILogger<OrderService> logger,
    IHttpContextAccessor httpContextAccessor,
    IDeliverySlotService deliverySlotService,
    IPaymentService paymentService,
    IFraudDetectionService fraudDetectionService,
    StockLockService stockLockService,
    IEmailService emailService,
    TimeSettings timeSettings,
    IMemoryCache memoryCache,
    IOrderCancellationService orderCancellationService,
    IPromotionService promotionService,
    ICouponService couponService,
    IAdminNotificationService adminNotificationService,
    IShippingService shippingService)
{
    _context = context;
    // ... các gán biến khác
    _shippingService = shippingService;
}
```

## 4. Dependency Graph
Sơ đồ khởi tạo DI hiện tại cho API Orders:

```text
[HTTP POST /api/Orders]
       ↓
(Routing & Authorization Middleware)
       ↓
Controller Factory (Yêu cầu khởi tạo OrdersController)
       ↓
DI Container: Resolve OrdersController dependencies
       ├─> Resolve ISystemSettingService (Thành công -> SystemSettingService)
       └─> Resolve IOrderService 
               ↓
           DI Container: Resolve OrderService dependencies
               ├─> IApplicationDbContext
               ├─> ILogger<OrderService>
               ├─> IHttpContextAccessor
               ├─> IDeliverySlotService
               ├─> IPaymentService
               ├─> IFraudDetectionService
               ├─> StockLockService
               ├─> IEmailService
               ├─> TimeSettings
               ├─> IMemoryCache
               ├─> IOrderCancellationService
               ├─> IPromotionService
               ├─> ICouponService
               ├─> IAdminNotificationService
               └─> IShippingService
               (Tất cả 15 services đều resolve thành công vì đã có trong Program.cs)
               ↓
           Khởi tạo OrderService thành công
       ↓
Khởi tạo OrdersController thành công
       ↓
Kích hoạt Action CreateOrder([FromBody] OrderInputDTO input)
```

## 5. Kết quả Build
Biên dịch dự án thành công, đảm bảo mã nguồn tuân thủ cú pháp và các quy chuẩn kiểu dữ liệu:
```text
Build succeeded.
    90 Warning(s)
    0 Error(s)
Time Elapsed 00:00:06.66
```

## 6. Kết quả Runtime
- **Lỗi Ambiguity:** Đã được khắc phục triệt để trên cả Controller và Service layer.
- **Request `POST /api/Orders`:** Hiện tại hệ thống đã có thể vượt qua bước `Controller Factory` và **chính thức đi vào Action `CreateOrder`** (với tham số `OrderInputDTO` được deserialize từ JSON body).

## 7. Regression Analysis
- **Phạm vi tác động:** Thay đổi chỉ diễn ra ở khai báo DI Constructor của `OrderService`. Không có tác động đến cấu trúc Database, API Contract, hay business logic cốt lõi.
- **Tính ổn định của code hiện tại:** Cả `OrderController` (MVC) và `OrdersController` (API) đều dùng chung `IOrderService`. Việc fix `OrderService` giúp giải cứu toàn bộ mọi chức năng gọi qua service này.
- **Bảo mật & Cấu hình:** Các dependencies trong `OrderService` (như `IFraudDetectionService`, `IEmailService`, `IShippingService`) được khởi tạo an toàn, sẵn sàng phục vụ cho quá trình kiểm tra đơn hàng, gửi email và tính phí giao hàng. Không có dependency nào bị `null` do thiếu truyền tải tham số.
