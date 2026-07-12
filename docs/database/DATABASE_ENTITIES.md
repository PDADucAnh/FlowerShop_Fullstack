# Database Entities (FlowerShop)

Tài liệu này mô tả chi tiết 28 thực thể (Entities) của hệ thống cơ sở dữ liệu FlowerShop, bao gồm các thuộc tính (Properties), thuộc tính điều hướng (Navigation), mối quan hệ (Relationship), chú thích dữ liệu (Annotation), chỉ mục (Index) và các ràng buộc dữ liệu (Constraint).

---

## 1. Advertisement.cs
Bảng lưu trữ thông tin về các banner quảng cáo hiển thị trên trang web.

### Properties
- `Id` (int) - Mã định danh quảng cáo.
- `Title` (string) - Tiêu đề quảng cáo.
- `Subtitle` (string?) - Phụ đề quảng cáo.
- `ImageUrl` (string?) - Đường dẫn hình ảnh quảng cáo.
- `LinkUrl` (string?) - Đường dẫn liên kết khi nhấn vào quảng cáo.
- `SortOrder` (int) - Thứ tự hiển thị.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- None

### Relationship
- None

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(500)]` trên thuộc tính `Title`.
- `[MaxLength(2000)]` trên thuộc tính `ImageUrl`.
- `[MaxLength(1000)]` trên thuộc tính `LinkUrl`.

### Index
- None

### Constraint
- None

---

## 2. CancellationPolicy.cs
Bảng lưu chính sách hủy đơn hàng tương ứng với từng trạng thái của đơn hàng.

### Properties
- `Id` (int) - Mã định danh chính sách hủy đơn.
- `OrderStatus` (string) - Trạng thái đơn hàng áp dụng chính sách hủy.
- `RefundPercent` (int) - Tỷ lệ phần trăm hoàn tiền cho khách.
- `CancellationFeePercent` (int) - Tỷ lệ phần trăm phí phạt hủy đơn.
- `Description` (string?) - Mô tả chính sách.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- None

### Relationship
- None

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `OrderStatus`.
- `[MaxLength(50)]` trên thuộc tính `OrderStatus`.
- `[MaxLength(500)]` trên thuộc tính `Description`.

### Index
- `IX_CancellationPolicies_OrderStatus` (Unique)

### Constraint
- Ràng buộc Unique Constraint duy nhất trên thuộc tính `OrderStatus`.

---

## 3. Category.cs
Bảng danh mục cho bài viết/tin tức (Post).

### Properties
- `Id` (int) - Mã định danh danh mục.
- `Name` (string) - Tên danh mục.
- `Description` (string?) - Mô tả danh mục.
- `Slug` (string?) - Đường dẫn thân thiện URL.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual ICollection<Post> Posts` - Tập các bài viết thuộc danh mục này.

### Relationship
- Category `1 ---- N` Posts (Một danh mục có nhiều bài viết).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `Name`.
- `[MaxLength(200)]` trên thuộc tính `Name`.
- `[MaxLength(2000)]` trên thuộc tính `Description`.
- `[MaxLength(300)]` trên thuộc tính `Slug`.

### Index
- None

### Constraint
- None

---

## 4. CategoryProduct.cs
Bảng danh mục dành cho sản phẩm hoa (Product).

### Properties
- `Id` (int) - Mã định danh danh mục sản phẩm.
- `Name` (string) - Tên danh mục sản phẩm.
- `Description` (string?) - Mô tả danh mục sản phẩm.
- `Slug` (string?) - Đường dẫn thân thiện URL.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual ICollection<Product>? Products` - Danh sách các sản phẩm thuộc danh mục này.

### Relationship
- CategoryProduct `1 ---- N` Products (Một danh mục sản phẩm có nhiều sản phẩm).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `Name`.
- `[MaxLength(200)]` trên thuộc tính `Name`.
- `[MaxLength(2000)]` trên thuộc tính `Description`.
- `[MaxLength(300)]` trên thuộc tính `Slug`.

### Index
- None

### Constraint
- None

---

## 5. Coupon.cs
Bảng lưu trữ thông tin mã giảm giá / khuyến mãi (Coupon).

### Properties
- `Id` (int) - Mã định danh Coupon.
- `Code` (string) - Mã giảm giá dùng để thanh toán.
- `Description` (string?) - Mô tả mã giảm giá.
- `DiscountType` (DiscountType) - Loại giảm giá (Phần trăm hoặc số tiền cố định).
- `DiscountValue` (decimal) - Giá trị giảm giá.
- `MinimumOrderAmount` (decimal?) - Giá trị đơn hàng tối thiểu để áp dụng.
- `MaximumDiscountAmount` (decimal?) - Số tiền giảm giá tối đa (dành cho giảm giá %).
- `UsageLimit` (int?) - Giới hạn tổng số lượt sử dụng.
- `UsedCount` (int) - Số lượt đã sử dụng thực tế.
- `UsagePerCustomer` (int?) - Giới hạn lượt sử dụng trên mỗi khách hàng.
- `CustomerId` (int?) - Mã khách hàng (nếu là coupon độc quyền dành riêng).
- `StartDate` (DateTime?) - Ngày bắt đầu hiệu lực.
- `EndDate` (DateTime?) - Ngày hết hạn hiệu lực.
- `IsPublic` (bool) - Có hiển thị công khai hay không.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual ICollection<CouponUsage>? Usages` - Lịch sử các lần sử dụng Coupon này.

### Relationship
- Coupon `1 ---- N` CouponUsages (Một mã giảm giá được sử dụng nhiều lần).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `Code`.
- `[MaxLength(50)]` trên thuộc tính `Code`.
- `[MaxLength(500)]` trên thuộc tính `Description`.
- `[Column(TypeName = "decimal(18,2)")]` trên `DiscountValue`, `MinimumOrderAmount`, `MaximumDiscountAmount`.

### Index
- `IX_Coupons_Code` (Unique)

### Constraint
- Ràng buộc Unique Constraint trên cột `Code`.

---

## 6. CouponUsage.cs
Bảng lưu lịch sử áp dụng và sử dụng mã giảm giá của từng khách hàng trên đơn hàng cụ thể.

### Properties
- `Id` (int) - Mã định danh lượt sử dụng Coupon.
- `CouponId` (int) - Mã liên kết đến Coupon.
- `CustomerId` (int) - Mã liên kết đến Khách hàng sử dụng.
- `OrderId` (int) - Mã liên kết đến Đơn hàng áp dụng.
- `DiscountAmount` (decimal) - Số tiền đã được giảm trực tiếp trên đơn hàng này.
- `UsedAt` (DateTime) - Thời điểm áp dụng sử dụng.

### Navigation
- `virtual Coupon? Coupon` - Tham chiếu đến Coupon.
- `virtual Customer? Customer` - Tham chiếu đến Khách hàng.
- `virtual Order? Order` - Tham chiếu đến Đơn hàng.

### Relationship
- Coupon `1 ---- N` CouponUsages (BelongsTo Coupon).
- Customer `1 ---- N` CouponUsages (BelongsTo Customer).
- Order `1 ---- 1` CouponUsages (BelongsTo Order - Unique OrderId).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Column(TypeName = "decimal(18,2)")]` trên thuộc tính `DiscountAmount`.
- `[ForeignKey("CouponId")]` cấu hình liên kết thuộc tính điều hướng `Coupon`.
- `[ForeignKey("CustomerId")]` cấu hình liên kết thuộc tính điều hướng `Customer`.
- `[ForeignKey("OrderId")]` cấu hình liên kết thuộc tính điều hướng `Order`.

### Index
- `IX_CouponUsages_OrderId` (Unique)
- `IX_CouponUsages_CouponId_CustomerId`
- `IX_CouponUsages_CustomerId`

### Constraint
- Ràng buộc Unique Constraint trên cột `OrderId` (Mỗi đơn hàng chỉ có tối đa một bản ghi áp dụng coupon).

---

## 7. Customer.cs
Bảng lưu trữ tài khoản của khách mua hàng, trạng thái hoạt động và các thống kê mua hàng.

### Properties
- `Id` (int) - Mã định danh khách hàng.
- `FullName` (string) - Tên đầy đủ của khách hàng.
- `Email` (string) - Địa chỉ Email (dùng làm tài khoản đăng nhập).
- `Phone` (string?) - Số điện thoại liên hệ.
- `Address` (string?) - Địa chỉ của khách hàng.
- `PasswordHash` (string) - Mật khẩu đã mã hóa.
- `DefaultAddressId` (int?) - Mã địa chỉ nhận hàng mặc định.
- `TotalOrders` (int) - Tổng số đơn hàng đã đặt.
- `SuccessfulDeliveries` (int) - Số đơn hàng giao thành công.
- `FailedDeliveries` (int) - Số đơn hàng giao thất bại.
- `IsBlacklisted` (bool) - Khách hàng có nằm trong danh sách đen/chặn hay không.
- `FraudScore` (int) - Điểm đánh giá mức độ gian lận.
- `ResetToken` (string?) - Token dùng để lấy lại mật khẩu.
- `ResetTokenExpiry` (DateTime?) - Thời gian hết hạn của Reset Token.
- `EmailVerified` (bool) - Xác nhận Email đã xác minh.
- `PhoneVerified` (bool) - Xác nhận số điện thoại đã xác minh.
- `LastLogin` (DateTime?) - Thời gian đăng nhập gần nhất.
- `IsActive` (bool) - Trạng thái hoạt động của tài khoản.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual ICollection<Order>? Orders` - Danh sách đơn đặt hàng của khách hàng này.

### Relationship
- Customer `1 ---- N` Orders (Một khách hàng có nhiều đơn hàng).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `FullName`, `Email`, `PasswordHash`.
- `[EmailAddress]` xác thực định dạng email trên thuộc tính `Email`.
- `[Column("Password")]` cấu hình ánh xạ trường C# `PasswordHash` thành cột `Password` trong Database.
- `[MaxLength(100)]` trên thuộc tính `ResetToken`.

### Index
- IX_Customers_Email (Unique)
- IX_Customers_Phone
- IX_Customers_ResetToken (Filtered: `[ResetToken] IS NOT NULL`)

### Constraint
- Ràng buộc Unique Constraint trên cột `Email`.

---

## 8. CustomerAddress.cs
Bảng lưu trữ danh sách địa chỉ nhận hàng của khách hàng.

### Properties
- `Id` (int) - Mã định danh địa chỉ.
- `CustomerId` (int) - Mã liên kết đến Khách hàng sở hữu địa chỉ này.
- `ReceiverName` (string?) - Tên người nhận hàng.
- `ReceiverPhone` (string?) - Số điện thoại người nhận hàng.
- `Province` (string?) - Tỉnh/Thành phố giao hàng.
- `District` (string?) - Quận/Huyện giao hàng.
- `Ward` (string?) - Phường/Xã giao hàng.
- `AddressLine` (string?) - Địa chỉ chi tiết (Số nhà, tên đường...).
- `PostalCode` (string?) - Mã bưu chính.
- `Note` (string?) - Ghi chú cụ thể cho địa chỉ giao hàng này.
- `Latitude` (double?) - Vĩ độ bản đồ.
- `Longitude` (double?) - Kinh độ bản đồ.
- `IsDefault` (bool) - Có phải là địa chỉ giao hàng mặc định của khách không.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual Customer? Customer` - Tham chiếu đến Customer sở hữu địa chỉ này.

### Relationship
- Customer `1 ---- N` CustomerAddresses (BelongsTo Customer).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(200)]` trên thuộc tính `ReceiverName`.
- `[MaxLength(20)]` trên thuộc tính `ReceiverPhone` và `PostalCode`.
- `[MaxLength(100)]` trên `Province`, `District`, `Ward`.
- `[MaxLength(500)]` trên `AddressLine`, `Note`.
- `[ForeignKey("CustomerId")]` cấu hình liên kết thuộc tính điều hướng `Customer`.

### Index
- `IX_CustomerAddresses_CustomerId_IsDefault` (Filtered: `[IsDefault] = 1`)

### Constraint
- None

---

## 9. CustomerPaymentPreference.cs
Bảng lưu phương thức thanh toán ưa thích/mặc định của khách hàng.

### Properties
- `Id` (int) - Mã định danh cấu hình tùy chọn thanh toán.
- `CustomerId` (int) - Mã khách hàng cấu hình.
- `PaymentMethodId` (int) - Mã phương thức thanh toán ưu tiên.
- `IsDefault` (bool) - Xác nhận đây là lựa chọn thanh toán mặc định.
- `LastUsedAt` (DateTime?) - Thời điểm sử dụng gần nhất.
- `CreatedAt` (DateTime) - Thời điểm cấu hình.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cấu hình.

### Navigation
- `virtual Customer? Customer` - Tham chiếu đến Khách hàng.
- `virtual PaymentMethodDefinition? PaymentMethod` - Tham chiếu đến Phương thức thanh toán.

### Relationship
- Customer `1 ---- N` CustomerPaymentPreferences (BelongsTo Customer).
- PaymentMethodDefinition `1 ---- N` CustomerPaymentPreferences (BelongsTo PaymentMethodDefinition).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[ForeignKey("CustomerId")]` trên thuộc tính điều hướng `Customer`.
- `[ForeignKey("PaymentMethodId")]` trên thuộc tính điều hướng `PaymentMethod`.

### Index
- `IX_CustomerPaymentPreferences_PaymentMethodId`
- `IX_CustomerPaymentPreferences_CustomerId_PaymentMethodId` (Unique)

### Constraint
- Ràng buộc Unique Constraint trên cặp cột kép `CustomerId` và `PaymentMethodId`.

---

## 10. DeliverySlot.cs
Bảng quản lý khung giờ và dung lượng (capacity) giao hàng của sản phẩm.

### Properties
- `Id` (int) - Mã định danh Delivery Slot.
- `ProductId` (int) - Mã sản phẩm áp dụng slot giao.
- `DeliveryDate` (DateTime) - Ngày giao hàng cấu hình.
- `TimeSlot` (string) - Khung giờ giao hàng (Ví dụ: "08:00 - 10:00").
- `MaxCapacity` (int) - Số lượng đơn hàng tối đa có thể tiếp nhận giao trong slot này.
- `CurrentBooked` (int) - Số lượng đơn hàng hiện đã được đặt trong slot này.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual Product? Product` - Tham chiếu đến Sản phẩm tương ứng.

### Relationship
- Product `1 ---- N` DeliverySlots (BelongsTo Product).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `TimeSlot`.
- `[MaxLength(50)]` trên thuộc tính `TimeSlot`.
- `[ForeignKey("ProductId")]` trên thuộc tính điều hướng `Product`.

### Index
- `IX_DeliverySlots_Date_TimeSlot_IsActive`
- `IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive`

### Constraint
- None

---

## 11. EmailHistory.cs
Bảng lưu trữ lịch sử các email đã gửi cho khách hàng.

### Properties
- `Id` (int) - Mã định danh lịch sử email.
- `CustomerId` (int?) - Mã khách hàng nhận email (nếu có tài khoản).
- `OrderId` (int?) - Mã đơn hàng liên quan đến email (ví dụ: email xác nhận đơn hàng).
- `EmailType` (string) - Loại email gửi đi (ví dụ: "OrderConfirmation", "ResetPassword").
- `Recipient` (string) - Địa chỉ Email người nhận.
- `Subject` (string?) - Tiêu đề email.
- `Status` (string) - Trạng thái gửi email ("Sent", "Failed"...).
- `SentAt` (DateTime?) - Thời điểm gửi đi thành công.
- `CreatedAt` (DateTime) - Thời điểm ghi nhận hệ thống.

### Navigation
- `virtual Customer? Customer` - Tham chiếu đến Khách hàng nhận email (tùy chọn).

### Relationship
- Customer `1 ---- N` EmailHistories (BelongsTo Customer - Optional).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `EmailType` và `Recipient`.
- `[MaxLength(100)]` trên thuộc tính `EmailType`.
- `[MaxLength(500)]` trên thuộc tính `Recipient` và `Subject`.
- `[MaxLength(50)]` trên thuộc tính `Status`.
- `[ForeignKey("CustomerId")]` trên thuộc tính điều hướng `Customer`.

### Index
- `IX_EmailHistories_CustomerId`
- `IX_EmailHistories_EmailType`
- `IX_EmailHistories_OrderId`

### Constraint
- None

---

## 12. FlashSale.cs
Bảng cấu hình đợt khuyến mãi Flash Sale thời hạn ngắn.

### Properties
- `Id` (int) - Mã định danh đợt Flash Sale.
- `Name` (string) - Tên chương trình Flash Sale.
- `Description` (string?) - Mô tả chi tiết đợt Flash Sale.
- `StartDate` (DateTime) - Ngày giờ bắt đầu.
- `EndDate` (DateTime) - Ngày giờ kết thúc.
- `IsActive` (bool) - Trạng thái kích hoạt đợt sale.
- `CreatedAt` (DateTime) - Thời điểm tạo đợt sale.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual ICollection<FlashSaleProduct>? FlashSaleProducts` - Danh sách các sản phẩm tham gia đợt Flash Sale này.

### Relationship
- FlashSale `1 ---- N` FlashSaleProducts (Một đợt Flash Sale áp dụng cho nhiều sản phẩm).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên thuộc tính `Name`.
- `[MaxLength(200)]` trên thuộc tính `Name`.
- `[MaxLength(2000)]` trên thuộc tính `Description`.

### Index
- `IX_FlashSales_Active_StartDate_EndDate`

### Constraint
- None

---

## 13. FlashSaleProduct.cs
Bảng liên kết sản phẩm tham gia Flash Sale cùng mức chiết khấu và giá sale cụ thể.

### Properties
- `Id` (int) - Mã định danh liên kết.
- `FlashSaleId` (int) - Mã liên kết đến chương trình Flash Sale.
- `ProductId` (int) - Mã liên kết đến Sản phẩm tham gia.
- `SalePrice` (decimal) - Giá bán khuyến mãi thực tế áp dụng trong đợt sale.
- `DiscountPercent` (decimal) - Mức chiết khấu tính theo %.
- `CreatedAt` (DateTime) - Thời điểm thêm sản phẩm vào đợt sale.

### Navigation
- `virtual FlashSale? FlashSale` - Tham chiếu đến chương trình Flash Sale.
- `virtual Product? Product` - Tham chiếu đến Sản phẩm tham gia.

### Relationship
- FlashSale `1 ---- N` FlashSaleProducts (BelongsTo FlashSale).
- Product `1 ---- N` FlashSaleProducts (BelongsTo Product).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Column(TypeName = "decimal(18,2)")]` xác định kiểu dữ liệu decimal(18,2) cho `SalePrice` và `DiscountPercent`.
- `[ForeignKey("FlashSaleId")]` trên thuộc tính điều hướng `FlashSale`.
- `[ForeignKey("ProductId")]` trên thuộc tính điều hướng `Product`.

### Index
- `IX_FlashSaleProducts_FlashSaleId`
- `IX_FlashSaleProducts_ProductId`

### Constraint
- None

---

## 14. Notification.cs
Bảng lưu thông báo hệ thống gửi tới khách hàng.

### Properties
- `Id` (int) - Mã định danh thông báo.
- `CustomerId` (int) - Khách hàng sở hữu thông báo này.
- `OrderId` (int?) - Mã đơn hàng liên quan đến thông báo (nếu có).
- `Title` (string) - Tiêu đề thông báo.
- `Content` (string?) - Nội dung chi tiết thông báo gửi đi.
- `Type` (string) - Loại thông báo (Ví dụ: "OrderUpdate", "Promotion").
- `IsRead` (bool) - Trạng thái đã đọc của khách hàng.
- `CreatedAt` (DateTime) - Thời điểm tạo thông báo.

### Navigation
- `virtual Customer? Customer` - Tham chiếu đến Khách hàng nhận thông báo.

### Relationship
- Customer `1 ---- N` Notifications (BelongsTo Customer).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên các thuộc tính `Title` và `Type`.
- `[MaxLength(500)]` trên thuộc tính `Title`.
- `[MaxLength(2000)]` trên thuộc tính `Content`.
- `[MaxLength(50)]` trên thuộc tính `Type`.
- `[ForeignKey("CustomerId")]` trên thuộc tính điều hướng `Customer`.

### Index
- `IX_Notifications_CustomerId`
- `IX_Notifications_CustomerId_IsRead`

### Constraint
- None

---

## 15. Order.cs
Bảng lưu các giao dịch đặt hàng của khách hàng.

### Properties
- `Id` (int) - Mã đơn hàng.
- `OrderDate` (DateTime) - Ngày giờ tạo đơn hàng.
- `CustomerId` (int) - Khách hàng đặt mua.
- `Status` (OrderStatus) - Trạng thái xử lý đơn hàng (Pending, Confirmed, Shipping...).
- `Notes` (string?) - Ghi chú đơn hàng từ khách hàng.
- `PaymentMethod` (PaymentMethod) - Phương thức thanh toán được chọn (COD, Online).
- `PaymentStatus` (PaymentStatus) - Trạng thái thanh toán (Pending, Completed, Failed...).
- `PaymentTransactionId` (string?) - Mã giao dịch nhận từ cổng thanh toán đối tác (VNPAY).
- `PaymentPaidAt` (DateTime?) - Thời điểm ghi nhận thanh toán hoàn tất.
- `DeliveryDate` (DateTime?) - Ngày yêu cầu giao hàng.
- `DeliveryTimeSlot` (string?) - Khung giờ giao hàng yêu cầu.
- `DeliverySlotId` (int?) - Mã slot giao hàng đăng ký.
- `DeliveryDistrict` (string?) - Quận/Huyện giao nhận.
- `DeliveryAddress` (string?) - Địa chỉ giao nhận tổng hợp.
- `RecipientName` (string?) - Tên người nhận hàng được điền thủ công.
- `RecipientPhone` (string?) - Số điện thoại người nhận được điền thủ công.
- `DeliveryReceiverName` (string?) - Tên người nhận hàng cấu hình từ địa chỉ lưu sẵn.
- `DeliveryReceiverPhone` (string?) - Số điện thoại người nhận cấu hình từ địa chỉ.
- `DeliveryProvince` (string?) - Tỉnh/Thành phố giao nhận.
- `DeliveryWard` (string?) - Phường/Xã giao nhận.
- `DeliveryAddressLine` (string?) - Số nhà, tên đường chi tiết nơi nhận.
- `DeliveryPostalCode` (string?) - Mã bưu chính nơi nhận.
- `CancelledAt` (DateTime?) - Thời điểm đơn hàng bị hủy bỏ.
- `CancellationReason` (string?) - Lý do hủy bỏ đơn đặt hàng.
- `IsVerified` (bool) - Xác thực đơn hàng đã qua bộ phận chăm sóc xác minh.
- `VerifiedAt` (DateTime?) - Thời điểm thực hiện xác minh đơn hàng.
- `RefundAmount` (decimal) - Số tiền hoàn trả (nếu đơn hàng bị hủy bỏ).
- `CancelledBy` (string?) - Đối tượng thực hiện hủy đơn (Khách hàng / Cửa hàng).
- `CancellationFee` (decimal) - Chi phí phát sinh/khấu trừ khi hủy đơn hàng.
- `RefundRequestedAt` (DateTime?) - Thời điểm yêu cầu hoàn tiền.
- `RefundCompletedAt` (DateTime?) - Thời điểm hoàn tất quy trình hoàn tiền.
- `TargetFinishedTime` (DateTime?) - Thời gian dự kiến hoàn thành chuẩn bị đơn hàng.
- `CreatedAt` (DateTime) - Thời điểm tạo bản ghi đơn hàng.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.
- `PromotionId` (int?) - Mã chiến dịch khuyến mãi áp dụng trên đơn.
- `CouponId` (int?) - Mã Coupon giảm giá áp dụng trên đơn.
- `OriginalAmount` (decimal) - Giá trị gốc ban đầu của đơn hàng (chưa giảm).
- `DiscountAmount` (decimal) - Tổng số tiền được giảm giá (khuyến mãi sản phẩm + Coupon).
- `FinalAmount` (decimal) - Số tiền đơn hàng thực tế khách hàng phải trả.

### Navigation
- `virtual Customer? Customer` - Tham chiếu đến Khách hàng đặt đơn.
- `virtual ICollection<OrderDetail>? OrderDetails` - Danh sách chi tiết các món hàng đặt mua.
- `virtual PromotionCampaign? Promotion` - Tham chiếu đến chương trình khuyến mãi áp dụng.
- `virtual Coupon? Coupon` - Tham chiếu đến Coupon áp dụng.

### Relationship
- Customer `1 ---- N` Orders (BelongsTo Customer).
- PromotionCampaign `1 ---- N` Orders (BelongsTo Promotion - Optional).
- Coupon `1 ---- N` Orders (BelongsTo Coupon - Optional).
- Order `1 ---- N` OrderDetails (Một đơn hàng chứa nhiều chi tiết).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(200)]` trên `PaymentTransactionId`, `RecipientName`, `DeliveryReceiverName`.
- `[MaxLength(50)]` trên `DeliveryTimeSlot` và `CancelledBy`.
- `[MaxLength(100)]` trên `DeliveryDistrict`, `DeliveryProvince`, `DeliveryWard`.
- `[MaxLength(500)]` trên `DeliveryAddress`, `DeliveryAddressLine`, `CancellationReason`.
- `[MaxLength(20)]` trên các thuộc tính số điện thoại và mã bưu chính.
- `[Column(TypeName = "decimal(18,2)")]` trên `CancellationFee`, `OriginalAmount`, `DiscountAmount` và `FinalAmount`.
- `[ForeignKey("CustomerId")]` trên thuộc tính điều hướng `Customer`.
- `[ForeignKey("PromotionId")]` trên thuộc tính điều hướng `Promotion`.
- `[ForeignKey("CouponId")]` trên thuộc tính điều hướng `Coupon`.

### Index
- `IX_Orders_CustomerId`
- `IX_Orders_OrderDate`
- `IX_Orders_Status`
- `IX_Orders_Status_OrderDate`
- `IX_Orders_PromotionId`
- `IX_Orders_CouponId`

### Constraint
- None

---

## 16. OrderDetail.cs
Bảng chi tiết các sản phẩm nằm trong đơn hàng.

### Properties
- `Id` (int) - Mã chi tiết đơn hàng.
- `OrderId` (int) - Mã đơn hàng chính chứa chi tiết này.
- `ProductId` (int) - Mã sản phẩm được mua.
- `Quantity` (int) - Số lượng sản phẩm mua.
- `UnitPrice` (decimal) - Đơn giá sản phẩm tại thời điểm mua hàng.
- `ProductName` (string?) - Tên sản phẩm lưu trữ tại thời điểm mua (Tránh lỗi đổi tên sản phẩm sau này).
- `ProductImage` (string?) - Hình ảnh sản phẩm tại thời điểm mua.
- `SizeVariant` (string?) - Biến thể kích thước/style đặt hàng.
- `Discount` (decimal?) - Mức giảm giá áp dụng trực tiếp cho dòng sản phẩm này.
- `Subtotal` (decimal?) - Thành tiền tạm tính của dòng hàng sau chiết khấu.

### Navigation
- `virtual Order? Order` - Tham chiếu đến đơn hàng cha chứa chi tiết này.
- `virtual Product? Product` - Tham chiếu đến sản phẩm tương ứng.

### Relationship
- Order `1 ---- N` OrderDetails (BelongsTo Order).
- Product `1 ---- N` OrderDetails (BelongsTo Product).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Column(TypeName = "decimal(18,2)")]` trên `UnitPrice`, `Discount` và `Subtotal`.
- `[MaxLength(200)]` trên thuộc tính `ProductName`.
- `[MaxLength(1000)]` trên thuộc tính `ProductImage`.
- `[MaxLength(50)]` trên thuộc tính `SizeVariant`.
- `[ForeignKey("OrderId")]` trên thuộc tính điều hướng `Order`.
- `[ForeignKey("ProductId")]` trên thuộc tính điều hướng `Product`.

### Index
- `IX_OrderDetails_OrderId`
- `IX_OrderDetails_ProductId`

### Constraint
- None

---

## 17. Payment.cs
Bảng lưu vết thông tin thanh toán cho đơn hàng.

### Properties
- `Id` (int) - Mã thanh toán.
- `OrderId` (int) - Đơn hàng liên quan cần được thanh toán.
- `Amount` (decimal) - Số tiền giao dịch thanh toán.
- `Method` (PaymentMethod) - Phương thức thanh toán sử dụng (COD/Online).
- `Status` (PaymentStatus) - Trạng thái đợt thanh toán này.
- `PaymentMethodId` (int?) - Mã phương thức thanh toán định nghĩa.
- `Gateway` (string?) - Tên cổng thanh toán xử lý (Ví dụ: "VNPAY").
- `TransactionId` (string?) - Mã giao dịch thành công.
- `GatewayResponseCode` (string?) - Mã phản hồi trả về từ cổng đối tác.
- `BankCode` (string?) - Mã ngân hàng khách sử dụng thanh toán online.
- `PaymentUrl` (string?) - Đường dẫn thanh toán được tạo động.
- `PaidAt` (DateTime?) - Ngày giờ thanh toán hoàn tất.
- `RefundedAt` (DateTime?) - Ngày giờ hoàn tiền giao dịch.
- `RefundTransactionId` (string?) - Giao dịch hoàn trả tiền.
- `RefundResponseCode` (string?) - Mã phản hồi hoàn tiền từ gateway.
- `RefundedBy` (string?) - Người phê duyệt hoàn tiền.
- `RefundNote` (string?) - Ghi chú hoàn tiền phát sinh.
- `Notes` (string?) - Ghi chú thanh toán thông thường.
- `CreatedAt` (DateTime) - Thời điểm tạo bản ghi.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual Order? Order` - Đơn đặt hàng cha.
- `virtual PaymentMethodDefinition? PaymentMethodRef` - Định nghĩa phương thức thanh toán tương ứng.

### Relationship
- Order `1 ---- N` Payments (BelongsTo Order).
- PaymentMethodDefinition `1 ---- N` Payments (BelongsTo PaymentMethodDefinition - Optional).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Column(TypeName = "decimal(18,2)")]` trên thuộc tính `Amount`.
- `[MaxLength(50)]` trên `Gateway`, `GatewayResponseCode`, `BankCode`, `RefundResponseCode`.
- `[MaxLength(200)]` trên `TransactionId`, `RefundTransactionId`.
- `[MaxLength(1000)]` trên `PaymentUrl`.
- `[MaxLength(100)]` trên `RefundedBy`.
- `[MaxLength(500)]` trên `RefundNote`, `Notes`.
- `[ForeignKey("OrderId")]` trên thuộc tính điều hướng `Order`.
- `[ForeignKey("PaymentMethodId")]` trên thuộc tính điều hướng `PaymentMethodRef`.

### Index
- `IX_Payments_OrderId`
- `IX_Payments_PaymentMethodId`

### Constraint
- None

---

## 18. PaymentAttempt.cs
Bảng lưu log chi tiết từng lượt gửi yêu cầu thanh toán sang cổng thanh toán VNPAY/online.

### Properties
- `Id` (int) - Mã lượt thanh toán thử nghiệm.
- `PaymentId` (int) - Mã liên kết đến thanh toán tương ứng.
- `AttemptNumber` (int) - Số thứ tự lần thử thanh toán (1, 2, 3...).
- `GatewayRequest` (string?) - Nội dung chuỗi payload/query gửi đi sang gateway.
- `GatewayResponse` (string?) - Nội dung chuỗi kết quả gateway trả về.
- `IpAddress` (string?) - Địa chỉ IP của khách hàng gửi yêu cầu thanh toán.
- `UserAgent` (string?) - Thông tin trình duyệt/thiết bị gửi yêu cầu.
- `CreatedAt` (DateTime) - Thời điểm xảy ra lượt thanh toán này.

### Navigation
- `virtual Payment? Payment` - Bản ghi thanh toán chính.

### Relationship
- Payment `1 ---- N` PaymentAttempts (BelongsTo Payment).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(50)]` trên thuộc tính `IpAddress`.
- `[MaxLength(500)]` trên thuộc tính `UserAgent`.
- `[ForeignKey("PaymentId")]` trên thuộc tính điều hướng `Payment`.

### Index
- `IX_PaymentAttempts_PaymentId_AttemptNumber` (Unique)

### Constraint
- Ràng buộc Unique Constraint kép trên tổ hợp cột `PaymentId` và `AttemptNumber`.

---

## 19. PaymentMethod.cs (PaymentMethodDefinition)
Bảng định nghĩa danh sách phương thức thanh toán khả dụng.

### Properties
- `Id` (int) - Mã phương thức thanh toán.
- `Code` (string) - Mã nhận dạng (ví dụ: "COD", "VNPAY").
- `Name` (string) - Tên phương thức thanh toán hiển thị cho khách.
- `Description` (string?) - Mô tả phương thức thanh toán.
- `IsOnline` (bool) - Xác định đây là thanh toán trực tuyến qua gateway.
- `IsActive` (bool) - Trạng thái hoạt động/hỗ trợ.
- `DisplayOrder` (int) - Thứ tự hiển thị khi thanh toán.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- None

### Relationship
- None

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `Code` và `Name`.
- `[MaxLength(50)]` trên `Code`.
- `[MaxLength(200)]` trên `Name`.
- `[MaxLength(500)]` trên `Description`.

### Index
- `IX_PaymentMethods_Code` (Unique)

### Constraint
- Ràng buộc Unique Constraint trên cột `Code`.

---

## 20. PhoneBlacklist.cs
Bảng lưu danh sách đen các số điện thoại bị chặn đặt hàng.

### Properties
- `Id` (int) - Mã chặn.
- `PhoneNumber` (string) - Số điện thoại bị liệt vào danh sách chặn.
- `Reason` (string?) - Lý do cụ thể chặn số điện thoại này.
- `CreatedAt` (DateTime) - Thời điểm chặn.
- `IsActive` (bool) - Trạng thái hoạt động của lệnh chặn.

### Navigation
- None

### Relationship
- None

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `PhoneNumber`.
- `[MaxLength(20)]` trên `PhoneNumber`.
- `[MaxLength(500)]` trên `Reason`.

### Index
- `IX_PhoneBlacklist_PhoneNumber_IsActive`

### Constraint
- None

---

## 21. Post.cs
Bảng lưu trữ các bài viết/tin tức đăng tải trên trang web.

### Properties
- `Id` (int) - Mã bài viết.
- `Title` (string) - Tiêu đề bài viết.
- `Content` (string) - Nội dung chi tiết bài viết.
- `Summary` (string?) - Bản tóm tắt nội dung bài viết hiển thị ở trang ngoài.
- `Slug` (string?) - Đường dẫn URL bài viết.
- `ImageUrl` (string?) - Đường dẫn hình ảnh đại diện của bài viết.
- `CreatedDate` (DateTime) - Ngày giờ tạo bài viết.
- `CategoryId` (int) - Danh mục tin tức chứa bài viết.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual Category Category` - Tham chiếu đến Danh mục bài viết.

### Relationship
- Category `1 ---- N` Posts (BelongsTo Category).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `Title` và `Content`.
- `[MaxLength(500)]` trên `Title` và `Summary`.
- `[MaxLength(300)]` trên `Slug`.
- `[MaxLength(1000)]` trên `ImageUrl`.
- `[ForeignKey("CategoryId")]` trên thuộc tính điều hướng `Category`.

### Index
- `IX_Posts_CategoryId`

### Constraint
- None

---

## 22. Product.cs
Bảng lưu thông tin chi tiết các sản phẩm hoa của cửa hàng.

### Properties
- `Id` (int) - Mã sản phẩm hoa.
- `Sku` (string?) - Mã định danh quản lý kho hàng (Stock Keeping Unit).
- `Name` (string) - Tên sản phẩm hoa.
- `Description` (string?) - Mô tả chi tiết sản phẩm.
- `Slug` (string?) - Đường dẫn URL sản phẩm.
- `Price` (decimal) - Giá bán niêm yết/gốc ban đầu.
- `DiscountPrice` (decimal?) - Giá bán khuyến mãi (nếu có giảm giá thủ công).
- `StockQuantity` (int) - Số lượng hàng tồn kho.
- `ImageUrl` (string?) - Đường dẫn ảnh chính của sản phẩm hoa.
- `CategoryProductId` (int) - Mã danh mục của sản phẩm.
- `ViewCount` (int) - Lượt xem sản phẩm của người dùng.
- `AddToCartCount` (int) - Số lần sản phẩm được thêm vào giỏ hàng.
- `IsActive` (bool) - Trạng thái hiển thị bán ngoài trang chủ.
- `FlowerMeaning` (string?) - Ý nghĩa của loài hoa.
- `Origin` (string?) - Nguồn gốc, xuất xứ của hoa.
- `CareInstruction` (string?) - Hướng dẫn bảo quản/chăm sóc hoa giữ tươi lâu.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual CategoryProduct? CategoryProduct` - Danh mục sản phẩm tương ứng.
- `virtual ICollection<ProductVariant>? ProductVariants` - Danh sách biến thể kích thước của hoa.

### Relationship
- CategoryProduct `1 ---- N` Products (BelongsTo CategoryProduct).
- Product `1 ---- N` ProductVariants (Một sản phẩm hoa có nhiều biến thể tùy chọn).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(50)]` trên `Sku`.
- `[Required]` trên `Name`.
- `[MaxLength(200)]` trên `Name` và `Origin`.
- `[MaxLength(300)]` trên `Slug`.
- `[Range(0, double.MaxValue)]` hạn chế khoảng giá trị tối thiểu lớn hơn 0 trên `Price`.
- `[Column(TypeName = "decimal(18,2)")]` trên `Price`.
- `[MaxLength(500)]` trên `FlowerMeaning`.
- `[ForeignKey("CategoryProductId")]` trên thuộc tính điều hướng `CategoryProduct`.

### Index
- `IX_Products_CategoryProductId`
- `IX_Products_Sku` (Unique, Filtered: `[Sku] IS NOT NULL`)

### Constraint
- Ràng buộc Unique Constraint trên cột `Sku` (Loại bỏ các giá trị Null để tránh trùng lặp mã quản lý kho).

---

## 23. ProductVariant.cs
Bảng lưu thông tin các biến thể sản phẩm (size, kiểu cắm...) kèm mức điều chỉnh giá.

### Properties
- `Id` (int) - Mã biến thể.
- `ProductId` (int) - Mã sản phẩm hoa chính.
- `Name` (string) - Tên biến thể (Ví dụ: "Size L", "Bọc hộp quà"...).
- `PriceAdjustment` (decimal) - Mức tiền cộng thêm/điều chỉnh chênh lệch so với giá gốc.
- `IsDefault` (bool) - Xác nhận đây là biến thể hiển thị mặc định của sản phẩm.
- `ProductId1` (int? - Shadow key tự sinh) - Sử dụng trong liên kết ánh xạ nâng cao của EF.

### Navigation
- `virtual Product? Product` - Sản phẩm gốc liên kết.

### Relationship
- Product `1 ---- N` ProductVariants (BelongsTo Product).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `Name`.
- `[MaxLength(50)]` trên `Name`.
- `[Column(TypeName = "decimal(18,2)")]` định kiểu cho cột `PriceAdjustment`.
- `[ForeignKey("ProductId")]` trên thuộc tính điều hướng `Product`.

### Index
- `IX_ProductVariants_ProductId`
- `IX_ProductVariants_ProductId1`

### Constraint
- None

---

## 24. PromotionCampaign.cs
Bảng lưu các chiến dịch khuyến mãi lớn (đợt giảm giá, ưu đãi mùa...).

### Properties
- `Id` (int) - Mã chiến dịch khuyến mãi.
- `Name` (string) - Tên đợt khuyến mãi.
- `Description` (string?) - Mô tả chi tiết đợt khuyến mãi.
- `PromotionType` (PromotionType) - Loại khuyến mãi (FlashSale hoặc Seasonal).
- `DiscountType` (DiscountType) - Cách tính giảm giá (% hoặc số tiền cụ thể).
- `DiscountValue` (decimal) - Giá trị ưu đãi.
- `StartDate` (DateTime) - Thời điểm bắt đầu hiệu lực.
- `EndDate` (DateTime) - Thời điểm kết thúc hiệu lực.
- `Priority` (int) - Độ ưu tiên áp dụng (Trường hợp một sản phẩm nằm trong nhiều campaign cùng lúc).
- `BannerImage` (string?) - Hình ảnh banner quảng cáo đợt khuyến mãi.
- `IsStackable` (bool) - Xác định có thể cộng dồn khuyến mãi hay không.
- `IsActive` (bool) - Trạng thái hoạt động.
- `CreatedAt` (DateTime) - Ngày giờ tạo chiến dịch.
- `UpdatedAt` (DateTime?) - Ngày giờ cập nhật.

### Navigation
- `virtual ICollection<PromotionProduct>? PromotionProducts` - Danh sách các liên kết sản phẩm được áp dụng trong đợt khuyến mãi này.

### Relationship
- PromotionCampaign `1 ---- N` PromotionProducts (Một chiến dịch lớn có nhiều sản phẩm áp dụng).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `Name`.
- `[MaxLength(200)]` trên `Name`.
- `[MaxLength(2000)]` trên `Description`.
- `[Column(TypeName = "decimal(18,2)")]` trên `DiscountValue`.
- `[MaxLength(1000)]` trên `BannerImage`.

### Index
- `IX_PromotionCampaigns_Active_StartDate_EndDate`

### Constraint
- None

---

## 25. PromotionProduct.cs
Bảng liên kết các sản phẩm được áp dụng trong đợt khuyến mãi cụ thể.

### Properties
- `Id` (int) - Mã liên kết khuyến mãi - sản phẩm.
- `PromotionId` (int) - Mã chiến dịch khuyến mãi liên kết.
- `ProductId` (int) - Mã sản phẩm áp dụng liên kết.
- `CreatedAt` (DateTime) - Ngày giờ thiết lập liên kết.

### Navigation
- `virtual PromotionCampaign? Promotion` - Tham chiếu đến chiến dịch khuyến mãi.
- `virtual Product? Product` - Tham chiếu đến sản phẩm.

### Relationship
- PromotionCampaign `1 ---- N` PromotionProducts (BelongsTo Promotion).
- Product `1 ---- N` PromotionProducts (BelongsTo Product).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[ForeignKey("PromotionId")]` trên thuộc tính điều hướng `Promotion`.
- `[ForeignKey("ProductId")]` trên thuộc tính điều hướng `Product`.

### Index
- `IX_PromotionProducts_ProductId`
- `IX_PromotionProducts_PromotionId_ProductId` (Unique)

### Constraint
- Ràng buộc Unique Constraint kép trên tổ hợp cột `PromotionId` và `ProductId`.

---

## 26. RefreshToken.cs
Bảng quản lý token gia hạn đăng nhập cho tài khoản User.

### Properties
- `Id` (int) - Mã token.
- `UserId` (int) - Tài khoản User sở hữu token này.
- `TokenHash` (string) - Chuỗi băm bảo mật của Refresh Token.
- `CreatedAt` (DateTime) - Ngày giờ tạo phiên token.
- `ExpiresAt` (DateTime) - Ngày giờ token hết hạn đăng nhập.
- `RevokedAt` (DateTime?) - Thời điểm thu hồi quyền hoạt động của token (nếu có).
- `IsRevoked` (bool) - Trạng thái đã bị thu hồi.
- `DeviceInfo` (string?) - Thông tin trình duyệt/thiết bị đăng nhập.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual User? User` - Tham chiếu đến tài khoản người dùng tương ứng.

### Relationship
- User `1 ---- N` RefreshTokens (BelongsTo User).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `UserId` và `TokenHash`.
- `[MaxLength(256)]` trên thuộc tính `TokenHash`.
- `[MaxLength(50)]` trên thuộc tính `DeviceInfo`.
- `[ForeignKey("UserId")]` trên thuộc tính điều hướng `User`.

### Index
- `IX_RefreshTokens_TokenHash` (Unique)
- `IX_RefreshTokens_UserId`

### Constraint
- Ràng buộc Unique Constraint trên cột `TokenHash`.

---

## 27. Refund.cs
Bảng lưu vết thông tin hoàn tiền cho đơn hàng bị hủy.

### Properties
- `Id` (int) - Mã bản ghi hoàn tiền.
- `OrderId` (int) - Mã đơn hàng phát sinh yêu cầu hoàn trả tiền.
- `PaymentId` (int?) - Bản ghi thanh toán gốc được hoàn lại.
- `RequestedBy` (string?) - Người yêu cầu thực hiện hoàn tiền (khách hàng / nhân viên).
- `ApprovedBy` (string?) - Người xét duyệt phê duyệt việc hoàn tiền.
- `Reason` (string?) - Lý do hoàn trả tiền.
- `RefundType` (string?) - Hình thức hoàn tiền (Ví dụ: "BankTransfer", "Cash").
- `RefundPercent` (int) - Phần trăm tiền được hoàn theo chính sách.
- `RefundAmount` (decimal) - Tổng số tiền thực tế sẽ thực hiện hoàn trả.
- `RefundStatus` (int) - Trạng thái quy trình hoàn tiền (Pending, Completed...).
- `GatewayRefundId` (string?) - Mã hoàn tiền do bên thứ ba (VNPAY) cung cấp.
- `ProcessedAt` (DateTime?) - Ngày giờ xử lý hoàn tất giao dịch.
- `CreatedAt` (DateTime) - Thời điểm tiếp nhận yêu cầu.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- `virtual Order? Order` - Đơn hàng liên kết hoàn tiền.
- `virtual Payment? Payment` - Bản ghi thanh toán gốc.

### Relationship
- Order `1 ---- N` Refunds (BelongsTo Order).
- Payment `1 ---- N` Refunds (BelongsTo Payment - Optional).

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[MaxLength(100)]` trên `RequestedBy` và `ApprovedBy`.
- `[MaxLength(500)]` trên thuộc tính `Reason`.
- `[MaxLength(50)]` trên thuộc tính `RefundType`.
- `[Column(TypeName = "decimal(18,2)")]` định dạng tiền cho cột `RefundAmount`.
- `[MaxLength(200)]` trên thuộc tính `GatewayRefundId`.
- `[ForeignKey("OrderId")]` trên thuộc tính điều hướng `Order`.
- `[ForeignKey("PaymentId")]` trên thuộc tính điều hướng `Payment`.

### Index
- `IX_Refunds_OrderId`
- `IX_Refunds_PaymentId`

### Constraint
- None

---

## 28. User.cs
Bảng lưu thông tin tài khoản nhân viên / quản trị viên của cửa hàng.

### Properties
- `Id` (int) - Mã người dùng quản trị.
- `Username` (string) - Tên đăng nhập hệ thống quản lý.
- `PasswordHash` (string) - Hhashed password của người dùng.
- `FullName` (string) - Tên đầy đủ hiển thị của nhân viên.
- `Email` (string?) - Email liên hệ.
- `Phone` (string?) - Số điện thoại liên hệ.
- `Address` (string?) - Địa chỉ nơi ở.
- `Role` (string) - Vai trò/quyền hạn trong hệ thống (Admin, Staff, Manager...).
- `IsActive` (bool) - Trạng thái hoạt động tài khoản quản lý.
- `LastLogin` (DateTime?) - Thời điểm đăng nhập gần nhất.
- `CreatedAt` (DateTime) - Thời điểm tạo.
- `UpdatedAt` (DateTime?) - Thời điểm cập nhật cuối cùng.

### Navigation
- None

### Relationship
- None

### Annotation
- `[Key]` trên thuộc tính `Id` (Khóa chính tự tăng).
- `[Required]` trên `Username`, `PasswordHash`, `FullName` và `Role`.
- `[MaxLength(50)]` trên `Username` và `Role`.
- `[MaxLength(200)]` trên `FullName` và `Email`.
- `[MaxLength(20)]` trên `Phone`.
- `[MaxLength(500)]` trên `Address`.

### Index
- None

### Constraint
- None
