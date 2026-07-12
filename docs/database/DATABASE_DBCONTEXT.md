# Database DbContext & Model Configuration (FlowerShop)

Tài liệu này chi tiết cấu trúc lớp DbContext (`ApplicationDbContext.cs`), cấu hình Fluent API trong `OnModelCreating`, hành vi xóa (Delete Behavior), giá trị mặc định, dữ liệu mẫu (Seed Data) và cơ chế ánh xạ Enum của hệ thống FlowerShop.

---

## 1. Khai báo DbSet

Lớp `ApplicationDbContext` kế thừa từ `DbContext` và triển khai giao diện `IApplicationDbContext`. Dưới đây là danh sách 28 `DbSet` tương ứng với các bảng cơ sở dữ liệu:

1. `Categories` - `DbSet<Category>`
2. `Posts` - `DbSet<Post>`
3. `Users` - `DbSet<User>`
4. `CategoriesProducts` - `DbSet<CategoryProduct>`
5. `Products` - `DbSet<Product>`
6. `Customers` - `DbSet<Customer>`
7. `Orders` - `DbSet<Order>`
8. `OrderDetails` - `DbSet<OrderDetail>`
9. `RefreshTokens` - `DbSet<RefreshToken>`
10. `Advertisements` - `DbSet<Advertisement>`
11. `DeliverySlots` - `DbSet<DeliverySlot>`
12. `Payments` - `DbSet<Payment>`
13. `PhoneBlacklists` - `DbSet<PhoneBlacklist>`
14. `ProductVariants` - `DbSet<ProductVariant>`
15. `CustomerAddresses` - `DbSet<CustomerAddress>`
16. `PaymentMethods` - `DbSet<PaymentMethodDefinition>`
17. `CustomerPaymentPreferences` - `DbSet<CustomerPaymentPreference>`
18. `PaymentAttempts` - `DbSet<PaymentAttempt>`
19. `CancellationPolicies` - `DbSet<CancellationPolicy>`
20. `Refunds` - `DbSet<Refund>`
21. `Notifications` - `DbSet<Notification>`
22. `EmailHistories` - `DbSet<EmailHistory>`
23. `PromotionCampaigns` - `DbSet<PromotionCampaign>`
24. `PromotionProducts` - `DbSet<PromotionProduct>`
25. `Coupons` - `DbSet<Coupon>`
26. `CouponUsages` - `DbSet<CouponUsage>`
27. `FlashSales` - `DbSet<FlashSale>`
28. `FlashSaleProducts` - `DbSet<FlashSaleProduct>`

---

## 2. Fluent API Configurations (OnModelCreating)

Phương thức `OnModelCreating` cấu hình các chỉ mục, mối quan hệ phức tạp và các thuộc tính liên kết nâng cao:

- **Customer Indexes**:
  - `Email` được đánh chỉ mục Unique để chống trùng tài khoản.
  - `Phone` được đánh chỉ mục thường tên `IX_Customers_Phone`.
  - `ResetToken` có chỉ mục lọc (Filtered Index): `[ResetToken] IS NOT NULL` để tối ưu hóa truy vấn tìm kiếm token.
- **Product Index**:
  - `Sku` được thiết lập chỉ mục Unique có bộ lọc: `[Sku] IS NOT NULL`.
- **Order Indexes**:
  - `Status` được đánh chỉ mục `IX_Orders_Status` có bao gồm các trường phủ khác (`IncludeProperties`): `OrderDate`, `PaymentMethod` nhằm tăng tốc độ truy vấn cho Dashboard.
  - Đánh chỉ mục phức hợp `IX_Orders_Status_OrderDate` trên cặp cột `Status` và `OrderDate`.
  - Đánh chỉ mục trên ngày đặt `IX_Orders_OrderDate`.
- **RefreshToken Index**:
  - `TokenHash` có chỉ mục Unique bảo mật `IX_RefreshTokens_TokenHash`.
- **DeliverySlot Index**:
  - Đánh chỉ mục phức hợp trên bộ 4 trường: `ProductId`, `DeliveryDate`, `TimeSlot`, `IsActive`.
  - Đánh chỉ mục phức hợp trên bộ 3 trường: `DeliveryDate`, `TimeSlot`, `IsActive`.
- **PhoneBlacklist Index**:
  - Đánh chỉ mục phức hợp trên cặp trường: `PhoneNumber`, `IsActive`.
- **CustomerAddress Index**:
  - Đánh chỉ mục lọc `IX_CustomerAddresses_CustomerId_IsDefault` trên `CustomerId` và `IsDefault` với điều kiện lọc: `[IsDefault] = 1`.
- **PaymentMethod Index**:
  - Mã thanh toán `Code` được cấu hình chỉ mục Unique.
- **CustomerPaymentPreference Index**:
  - Đánh chỉ mục Unique phức hợp trên bộ đôi: `CustomerId` và `PaymentMethodId`.
- **PaymentAttempt Index**:
  - Đánh chỉ mục Unique phức hợp trên bộ đôi: `PaymentId` và `AttemptNumber`.
- **Refund Index**:
  - Chỉ mục `IX_Refunds_OrderId` trên khóa ngoại `OrderId`.
- **CancellationPolicy Index**:
  - Chỉ mục Unique `IX_CancellationPolicies_OrderStatus` trên cột `OrderStatus`.
- **Notification Indexes**:
  - Chỉ mục trên `CustomerId` và chỉ mục phức hợp trên cặp `CustomerId` & `IsRead` để truy vấn danh sách thông báo chưa đọc.
- **EmailHistory Indexes**:
  - Chỉ mục trên `OrderId` và `EmailType`.
- **PromotionProduct Index**:
  - Chỉ mục Unique phức hợp `IX_PromotionProducts_PromotionId_ProductId` trên `PromotionId` và `ProductId`.
- **PromotionCampaign Index**:
  - Chỉ mục phức hợp trên bộ 3 trường: `IsActive`, `StartDate`, `EndDate` để tối ưu hóa việc lấy các chiến dịch khuyến mãi đang diễn ra.
- **Coupon Index**:
  - Chỉ mục Unique `IX_Coupons_Code` trên `Code`.
- **CouponUsage Index**:
  - Chỉ mục Unique `IX_CouponUsages_OrderId` trên `OrderId`.
  - Chỉ mục phức hợp `IX_CouponUsages_CouponId_CustomerId` trên `CouponId` và `CustomerId`.
- **FlashSale Index**:
  - Chỉ mục phức hợp trên bộ 3 trường: `IsActive`, `StartDate`, `EndDate` để lấy các đợt Flash Sale đang chạy.

---

## 3. Relationships & Delete Behaviors

Hành vi xóa (Delete Behavior) được cấu hình rõ ràng để đảm bảo toàn vẹn dữ liệu trong SQL Server:

### Cascade Delete (Xóa bắc cầu)
Khi một bản ghi cha bị xóa, toàn bộ bản ghi con liên quan sẽ tự động bị xóa:
- **ProductVariant**: Xóa `Product` sẽ tự động xóa các biến thể `ProductVariant` tương ứng (`.OnDelete(DeleteBehavior.Cascade)`).
- **PaymentAttempt**: Xóa bản ghi thanh toán `Payment` sẽ tự động xóa lịch sử các lượt thử `PaymentAttempt` (`.OnDelete(DeleteBehavior.Cascade)`).
- **OrderDetails**: Xóa đơn hàng `Order` sẽ xóa sạch chi tiết đơn hàng `OrderDetail` (`.OnDelete(DeleteBehavior.Cascade)` cấu hình tự động).
- **RefreshTokens**: Xóa tài khoản `User` sẽ xóa các Refresh Token (`.OnDelete(DeleteBehavior.Cascade)` cấu hình tự động).
- **DeliverySlots**: Xóa sản phẩm `Product` sẽ tự động xóa các slot cấu hình giao hàng (`.OnDelete(DeleteBehavior.Cascade)` cấu hình tự động).

### Restrict (Hạn chế xóa)
Không cho phép xóa bản ghi cha nếu đang có bản ghi con tham chiếu đến. Hành vi này ngăn chặn lỗi mồ côi dữ liệu trong cơ sở dữ liệu:
- **Post -> Category**: Ngăn xóa Category nếu có bài viết thuộc danh mục đó.
- **Product -> CategoryProduct**: Ngăn xóa danh mục sản phẩm nếu có chứa hoa thuộc danh mục đó.
- **Order -> Customer**: Không cho xóa tài khoản khách hàng nếu họ đã có lịch sử đơn hàng.
- **OrderDetail -> Product**: Ngăn xóa sản phẩm nếu sản phẩm đó đã được mua trong đơn hàng nào đó.
- **CustomerAddress -> Customer**: Ngăn xóa khách hàng khi chưa dọn sạch bảng địa chỉ giao hàng.
- **CustomerPaymentPreference -> Customer / PaymentMethod**: Ngăn xóa thông tin cấu hình ưu tiên.
- **Payment -> PaymentMethodRef**: Hạn chế xóa phương thức thanh toán định nghĩa.
- **Refund -> Order / Payment**: Hạn chế xóa đơn hàng hoặc thanh toán khi đã có bản ghi hoàn tiền.
- **Notification -> Customer**: Ngăn xóa khách hàng khi có dữ liệu thông báo liên quan.
- **EmailHistory -> Customer**: Ngăn xóa khách hàng khi có lịch sử gửi email.
- **PromotionProduct -> Promotion / Product**: Ngăn xóa sản phẩm hoặc đợt khuyến mãi nếu đang liên kết hoạt động.
- **CouponUsage -> Coupon / Customer / Order**: Ngăn xóa thông tin liên quan nếu đã có lịch sử áp dụng voucher.
- **FlashSaleProduct -> FlashSale / Product**: Ngăn xóa khi liên kết đang hoạt động.
- **Order -> Promotion / Coupon**: Hạn chế xóa coupon hay đợt khuyến mãi khi đơn hàng đã chốt thanh toán.

### NoAction
Trong Fluent API của hệ thống không cấu hình trực tiếp hành vi `NoAction` nào, do cơ chế của SQL Server mặc định chuyển hướng sang Restrict để đảm bảo tính an toàn dữ liệu cao nhất.

---

## 4. Default Value (Giá trị mặc định)

Hệ thống FlowerShop quản lý giá trị mặc định trực tiếp thông qua **C# Property Initializers** trong mã nguồn Entity, thay vì khai báo ràng buộc `DEFAULT` bằng Fluent API trên SQL Server:
- `IsActive = true` trong `Advertisement`, `CancellationPolicy`, `Customer`, `CustomerAddress`, `DeliverySlot`, `FlashSale`, `PaymentMethodDefinition`, `PhoneBlacklist`, `Product`, `PromotionCampaign`.
- `CreatedAt = DateTime.UtcNow` / `OrderDate = DateTime.UtcNow` / `UsedAt = DateTime.UtcNow` / `UsedAt = DateTime.UtcNow` khởi tạo mặc định thời gian hiện tại cho các bản ghi.
- `IsRevoked = false` mặc định chưa thu hồi Refresh Token.
- `PaymentStatus = PaymentStatus.Pending` / `Status = OrderStatus.Pending` thiết lập trạng thái đơn hàng và thanh toán chờ ban đầu.

---

## 5. Seed Data (Dữ liệu mẫu)

Hệ thống triển khai 2 luồng nạp dữ liệu mẫu tự động khi khởi động ứng dụng:

### Seed Admin Account (trong `Program.cs`)
Khi ứng dụng khởi chạy lần đầu tiên, hệ thống sẽ kiểm tra tài khoản `admin` trong bảng `Users`. Nếu chưa tồn tại, hệ thống tự động tạo mới tài khoản quản trị:
```csharp
if (!context.Users.Any(u => u.Username == "admin"))
{
    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
    context.Users.Add(new User
    {
        Username = "admin",
        PasswordHash = hasher.HashPassword(null!, "123456"),
        FullName = "Administrator",
        Role = "Admin"
    });
    context.SaveChanges();
}
```

### Seed Cancellation Policies (trong `OrderCancellationService.cs`)
Phương thức `SeedDefaultPoliciesAsync` tự động nạp danh sách 7 chính sách hủy đơn mặc định khi bảng `CancellationPolicies` trống:
- **Pending**: Hoàn tiền 100%, phí hủy 0%.
- **PendingVerification**: Hoàn tiền 100%, phí hủy 0%.
- **PendingPayment**: Hoàn tiền 100%, phí hủy 0%.
- **Paid**: Hoàn tiền 100%, phí hủy 0%.
- **Confirmed**: Hoàn tiền 80%, phí hủy 20% (Do shop đã nhập/chuẩn bị nguyên liệu).
- **Preparing**: Hoàn tiền 70%, phí hủy 30% (Do hoa đang được cắm).
- **ReadyForDelivery**: Hoàn tiền 50%, phí hủy 50% (Do hoa đã hoàn thành cắm và sẵn sàng giao).

---

## 6. Enum Mapping

Các cấu trúc dữ liệu Enum trong dự án được ánh xạ trực tiếp sang kiểu **integer** trong SQL Server bằng cơ chế mặc định của Entity Framework Core, giúp tối ưu hóa dung lượng lưu trữ và hiệu năng lập chỉ mục:

- **OrderStatus**:
  - `Pending = 0`, `Shipping = 1`, `Completed = 2`, `Cancelled = 3`, `PendingVerification = 4`, `Confirmed = 5`, `Preparing = 6`, `PendingPayment = 7`, `Paid = 8`, `ReadyForDelivery = 9`, `Refunded = 10`, `CancelledByCustomer = 11`, `CancelledByShop = 12`, `RefundPending = 13`
- **PaymentMethod**:
  - `OnlinePayment = 0`, `COD = 1`
- **PaymentStatus**:
  - `Pending = 0`, `Completed = 1`, `Failed = 2`, `Refunded = 3`, `PartialRefund = 4`, `Expired = 5`, `Cancelled = 6`, `RefundPending = 7`, `PartialRefundPending = 8`, `PartialRefunded = 9`
- **PromotionType**:
  - `FlashSale = 0`, `Seasonal = 1`
- **DiscountType**:
  - `Percent = 0`, `FixedAmount = 1`

---

## 7. Value Converter

Hiện tại, `ApplicationDbContext` của dự án không sử dụng bất kỳ `ValueConverter` tùy biến nào qua lệnh `.HasConversion(...)`.
- Tất cả các trường ngày tháng (`DateTime`) được EF Core tự động ánh xạ thành kiểu `datetime2` trong SQL Server.
- Tất cả các trường số thực thập phân (`decimal`) được gán kiểu định dạng `decimal(18,2)` thông qua thuộc tính Data Annotation `[Column(TypeName = "decimal(18,2)")]` trực tiếp trên Entity.
