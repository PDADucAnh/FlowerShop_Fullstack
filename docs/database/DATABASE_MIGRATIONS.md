# Database Migrations History (FlowerShop)

Tài liệu này lưu trữ lịch sử phát triển cơ sở dữ liệu của dự án FlowerShop thông qua các bản EF Core Migration xếp theo thứ tự thời gian từ cũ nhất đến mới nhất.

---

## Danh sách Migration theo thứ tự thời gian

### 001. InitialCreate
- **Tên**: `InitialCreate`
- **Tập tin**: `20260701101110_InitialCreate.cs`
- **Ngày tạo**: 01/07/2026 10:11:10
- **Thay đổi chính**: Khởi tạo cấu trúc cơ sở dữ liệu ban đầu cho các tính năng cơ bản của cửa hàng.
- **Table thêm mới** (10 bảng):
  - `Advertisements`
  - `Categories`
  - `CategoriesProducts`
  - `Customers`
  - `Users`
  - `Posts`
  - `Products`
  - `Orders`
  - `RefreshTokens`
  - `OrderDetails`
- **Primary Key thêm**:
  - `PK_Advertisements` (Id)
  - `PK_Categories` (Id)
  - `PK_CategoriesProducts` (Id)
  - `PK_Customers` (Id)
  - `PK_Users` (Id)
  - `PK_Posts` (Id)
  - `PK_Products` (Id)
  - `PK_Orders` (Id)
  - `PK_RefreshTokens` (Id)
  - `PK_OrderDetails` (Id)
- **Foreign Key thêm**:
  - `FK_Posts_Categories_CategoryId` -> `Categories(Id)` (Cascade Delete)
  - `FK_Products_CategoriesProducts_CategoryProductId` -> `CategoriesProducts(Id)` (Cascade Delete)
  - `FK_Orders_Customers_CustomerId` -> `Customers(Id)` (Cascade Delete)
  - `FK_RefreshTokens_Users_UserId` -> `Users(Id)` (Cascade Delete)
  - `FK_OrderDetails_Orders_OrderId` -> `Orders(Id)` (Cascade Delete)
  - `FK_OrderDetails_Products_ProductId` -> `Products(Id)` (Cascade Delete)
- **Index thêm**:
  - `IX_Customers_Email` (Unique)
  - `IX_OrderDetails_OrderId`
  - `IX_OrderDetails_ProductId`
  - `IX_Orders_CustomerId`
  - `IX_Posts_CategoryId`
  - `IX_Products_CategoryProductId`
  - `IX_Products_Sku` (Unique, Lọc `[Sku] IS NOT NULL`)
  - `IX_RefreshTokens_UserId`

---

### 002. AddOrderWorkflowFields
- **Tên**: `AddOrderWorkflowFields`
- **Tập tin**: `20260701173141_AddOrderWorkflowFields.cs`
- **Ngày tạo**: 01/07/2026 17:31:41
- **Thay đổi chính**: Bổ sung các trường phục vụ quy trình xử lý đơn hàng, danh sách đen, và thanh toán.
- **Table thêm mới** (3 bảng):
  - `DeliverySlots`
  - `Payments`
  - `PhoneBlacklists`
- **Column thêm**:
  - `Orders`: `CancellationReason` (nvarchar(500)), `CancelledAt` (datetime2), `DeliveryAddress` (nvarchar(500)), `DeliveryDate` (datetime2), `DeliveryDistrict` (nvarchar(100)), `DeliveryTimeSlot` (nvarchar(50)), `IsVerified` (bit), `PaymentMethod` (int), `PaymentPaidAt` (datetime2), `PaymentStatus` (int), `PaymentTransactionId` (nvarchar(200)), `RefundAmount` (decimal(18,2)), `VerifiedAt` (datetime2).
  - `Customers`: `FailedDeliveries` (int), `FraudScore` (int), `IsBlacklisted` (bit), `SuccessfulDeliveries` (int), `TotalOrders` (int).
- **Primary Key thêm**:
  - `PK_DeliverySlots` (Id)
  - `PK_Payments` (Id)
  - `PK_PhoneBlacklists` (Id)
- **Foreign Key thêm**:
  - `FK_DeliverySlots_Products_ProductId` -> `Products(Id)` (Cascade Delete)
  - `FK_Payments_Orders_OrderId` -> `Orders(Id)` (Cascade Delete)
- **Index thêm**:
  - `IX_DeliverySlots_ProductId`
  - `IX_Payments_OrderId`

---

### 003. AddOrderTargetFinishedTime
- **Tên**: `AddOrderTargetFinishedTime`
- **Tập tin**: `20260701185309_AddOrderTargetFinishedTime.cs`
- **Ngày tạo**: 01/07/2026 18:53:09
- **Thay đổi chính**: Thêm thời hạn hoàn thành mục tiêu cho đơn hàng để quản lý SLA.
- **Column thêm**:
  - `Orders`: `TargetFinishedTime` (datetime2, nullable)

---

### 004. AddCustomerResetToken
- **Tên**: `AddCustomerResetToken`
- **Tập tin**: `20260702065852_AddCustomerResetToken.cs`
- **Ngày tạo**: 02/07/2026 06:58:52
- **Thay đổi chính**: Thêm token reset mật khẩu cho khách hàng.
- **Column thêm**:
  - `Customers`: `ResetToken` (nvarchar(100)), `ResetTokenExpiry` (datetime2)

---

### 005. AddTrendingScoreFields
- **Tên**: `AddTrendingScoreFields`
- **Tập tin**: `20260704013940_AddTrendingScoreFields.cs`
- **Ngày tạo**: 04/07/2026 01:39:40
- **Thay đổi chính**: Thêm các chỉ số thống kê lượt xem, thêm vào giỏ hàng và giá khuyến mãi để hỗ trợ tính điểm xu hướng sản phẩm.
- **Column thêm**:
  - `Products`: `AddToCartCount` (int), `DiscountPrice` (decimal(18,2)), `ViewCount` (int)

---

### 006. FixCascadeDeleteRestrict
- **Tên**: `FixCascadeDeleteRestrict`
- **Tập tin**: `20260704220154_FixCascadeDeleteRestrict.cs`
- **Ngày tạo**: 04/07/2026 22:01:54
- **Thay đổi chính**: Sửa đổi cơ chế xóa từ Cascade sang Restrict ở các bảng trung gian cốt lõi nhằm tránh mất dữ liệu diện rộng.
- **Foreign Key sửa đổi**:
  - `FK_OrderDetails_Products_ProductId` (Chuyển sang `Restrict`)
  - `FK_Orders_Customers_CustomerId` (Chuyển sang `Restrict`)
  - `FK_Posts_Categories_CategoryId` (Chuyển sang `Restrict`)
  - `FK_Products_CategoriesProducts_CategoryProductId` (Chuyển sang `Restrict`)

---

### 007. AddOrderIndexes
- **Tên**: `AddOrderIndexes`
- **Tập tin**: `20260704221519_AddOrderIndexes.cs`
- **Ngày tạo**: 04/07/2026 22:15:19
- **Thay đổi chính**: Tối ưu hóa hiệu năng tìm kiếm đơn hàng bằng cách tạo thêm chỉ mục phủ và chỉ mục phức hợp.
- **Index thêm**:
  - `IX_Orders_Status` on `Orders(Status)` (Include: `OrderDate`, `PaymentMethod`)
  - `IX_Orders_Status_OrderDate` on `Orders(Status, OrderDate)`

---

### 008. AddRecipientAndProductSnapshot
- **Tên**: `AddRecipientAndProductSnapshot`
- **Tập tin**: `20260705053142_AddRecipientAndProductSnapshot.cs`
- **Ngày tạo**: 05/07/2026 05:31:42
- **Thay đổi chính**: Hỗ trợ lưu thông tin người nhận khác người đặt, chụp ảnh thông tin sản phẩm lúc mua (ProductName, ProductImage) để tránh lỗi đổi giá/ảnh sản phẩm về sau, và thêm cấu hình biến thể sản phẩm.
- **Table thêm mới** (1 bảng):
  - `ProductVariants`
- **Column sửa đổi**:
  - `Products.Price`: Thay đổi kiểu từ `decimal(18,0)` thành `decimal(18,2)`.
- **Column thêm**:
  - `Orders`: `RecipientName` (nvarchar(200)), `RecipientPhone` (nvarchar(20))
  - `OrderDetails`: `ProductImage` (nvarchar(1000)), `ProductName` (nvarchar(200)), `SizeVariant` (nvarchar(50))
- **Primary Key thêm**:
  - `PK_ProductVariants` (Id)
- **Foreign Key thêm**:
  - `FK_ProductVariants_Products_ProductId` -> `Products(Id)` (Cascade Delete)
- **Index thêm**:
  - `IX_ProductVariants_ProductId`
  - Chỉ mục chạy bằng Raw SQL: `IX_Products_BasePrice` on `Products(Price)` (Include: `Name`, `Slug`, `Sku`, `ImageUrl`)

---

### 009. AddDatabaseSchemaEnhancements
- **Tên**: `AddDatabaseSchemaEnhancements`
- **Tập tin**: `20260707053409_AddDatabaseSchemaEnhancements.cs`
- **Ngày tạo**: 07/07/2026 05:34:09
- **Thay đổi chính**: Đợt nâng cấp cơ sở dữ liệu diện rộng bổ sung cột ghi nhận thời gian (`CreatedAt`, `UpdatedAt`), quản lý địa chỉ của khách hàng, tích hợp phương thức thanh toán động và tối ưu hóa hệ thống chỉ mục.
- **Table thêm mới** (3 bảng):
  - `CustomerAddresses`
  - `PaymentMethods` (Tên entity: `PaymentMethodDefinition`)
  - `CustomerPaymentPreferences`
- **Column thêm**:
  - `Users`: `CreatedAt` (datetime2), `IsActive` (bit), `LastLogin` (datetime2), `UpdatedAt` (datetime2).
  - `RefreshTokens`: `UpdatedAt` (datetime2).
  - `ProductVariants`: `ProductId1` (int, nullable - EF shadow key).
  - `Products`: `CareInstruction` (nvarchar(max)), `CreatedAt` (datetime2), `FlowerMeaning` (nvarchar(500)), `IsActive` (bit), `Origin` (nvarchar(200)), `UpdatedAt` (datetime2).
  - `Posts`: `UpdatedAt` (datetime2).
  - `Payments`: `BankCode` (nvarchar(50)), `CreatedAt` (datetime2), `Gateway` (nvarchar(50)), `GatewayResponseCode` (nvarchar(50)), `PaymentMethodId` (int, nullable), `PaymentUrl` (nvarchar(1000)), `UpdatedAt` (datetime2).
  - `Orders`: `CreatedAt` (datetime2), `DeliveryAddressLine` (nvarchar(500)), `DeliveryPostalCode` (nvarchar(20)), `DeliveryProvince` (nvarchar(100)), `DeliveryReceiverName` (nvarchar(200)), `DeliveryReceiverPhone` (nvarchar(20)), `DeliverySlotId` (int, nullable), `DeliveryWard` (nvarchar(100)), `UpdatedAt` (datetime2).
  - `OrderDetails`: `Discount` (decimal(18,2)), `Subtotal` (decimal(18,2)).
  - `DeliverySlots`: `CreatedAt` (datetime2), `UpdatedAt` (datetime2).
  - `Customers`: `CreatedAt` (datetime2), `DefaultAddressId` (int, nullable), `EmailVerified` (bit), `IsActive` (bit), `LastLogin` (datetime2), `PhoneVerified` (bit), `UpdatedAt` (datetime2).
  - `CategoriesProducts`: `CreatedAt` (datetime2), `UpdatedAt` (datetime2).
  - `Categories`: `CreatedAt` (datetime2), `UpdatedAt` (datetime2).
  - `Advertisements`: `UpdatedAt` (datetime2) (Đồng thời đổi tên cột `CreatedDate` -> `CreatedAt`).
- **Column sửa đổi**:
  - `ProductVariants.PriceAdjustment`: Chuyển kiểu dữ liệu sang `decimal(18,2)`.
  - `Payments.Amount`: Chuyển kiểu dữ liệu sang `decimal(18,2)`.
  - `OrderDetails.UnitPrice`: Chuyển kiểu dữ liệu sang `decimal(18,2)`.
  - `Customers.Phone`: Chuyển kiểu dữ liệu sang `nvarchar(450)`.
- **Primary Key thêm**:
  - `PK_CustomerAddresses` (Id)
  - `PK_PaymentMethods` (Id)
  - `PK_CustomerPaymentPreferences` (Id)
- **Foreign Key thêm**:
  - `FK_CustomerAddresses_Customers_CustomerId` -> `Customers(Id)` (Restrict)
  - `FK_CustomerPaymentPreferences_Customers_CustomerId` -> `Customers(Id)` (Restrict)
  - `FK_CustomerPaymentPreferences_PaymentMethods_PaymentMethodId` -> `PaymentMethods(Id)` (Restrict)
  - `FK_Payments_PaymentMethods_PaymentMethodId` -> `PaymentMethods(Id)` (Restrict)
  - `FK_ProductVariants_Products_ProductId1` -> `Products(Id)` (No Action)
- **Index xóa**:
  - `IX_DeliverySlots_ProductId` (Để thay thế bằng chỉ mục đa cột tối ưu hơn)
- **Index thêm**:
  - `IX_RefreshTokens_TokenHash` (Unique)
  - `IX_ProductVariants_ProductId1`
  - `IX_PhoneBlacklist_PhoneNumber_IsActive`
  - `IX_Payments_PaymentMethodId`
  - `IX_Orders_OrderDate`
  - `IX_DeliverySlots_Date_TimeSlot_IsActive`
  - `IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive`
  - `IX_Customers_Phone`
  - `IX_Customers_ResetToken` (Chỉ mục lọc `[ResetToken] IS NOT NULL`)
  - `IX_CustomerAddresses_CustomerId_IsDefault` (Chỉ mục lọc `[IsDefault] = 1`)
  - `IX_CustomerPaymentPreferences_CustomerId_PaymentMethodId` (Unique)
  - `IX_CustomerPaymentPreferences_PaymentMethodId`
  - `IX_PaymentMethods_Code` (Unique)

---

### 010. AddPaymentAttemptTable
- **Tên**: `AddPaymentAttemptTable`
- **Tập tin**: `20260707060100_AddPaymentAttemptTable.cs`
- **Ngày tạo**: 07/07/2026 06:01:00
- **Thay đổi chính**: Thêm bảng ghi nhận chi tiết lịch sử từng lượt cố gắng thanh toán trực tuyến qua Gateway (IP, UserAgent, Request/Response thô).
- **Table thêm mới** (1 bảng):
  - `PaymentAttempts`
- **Primary Key thêm**:
  - `PK_PaymentAttempts` (Id)
- **Foreign Key thêm**:
  - `FK_PaymentAttempts_Payments_PaymentId` -> `Payments(Id)` (Cascade Delete)
- **Index thêm**:
  - `IX_PaymentAttempts_PaymentId_AttemptNumber` (Unique)

---

### 011. AddCancelRefundWorkflow
- **Tên**: `AddCancelRefundWorkflow`
- **Tập tin**: `20260707141444_AddCancelRefundWorkflow.cs`
- **Ngày tạo**: 07/07/2026 14:14:44
- **Thay đổi chính**: Thiết lập quy trình hủy đơn và hoàn trả tiền tích hợp cổng thanh toán trực tuyến.
- **Table thêm mới** (2 bảng):
  - `CancellationPolicies`
  - `Refunds`
- **Column thêm**:
  - `Payments`: `RefundNote` (nvarchar(500)), `RefundResponseCode` (nvarchar(50)), `RefundTransactionId` (nvarchar(200)), `RefundedBy` (nvarchar(100)).
  - `Orders`: `CancellationFee` (decimal(18,2)), `CancelledBy` (nvarchar(50)), `RefundCompletedAt` (datetime2), `RefundRequestedAt` (datetime2).
- **Primary Key thêm**:
  - `PK_CancellationPolicies` (Id)
  - `PK_Refunds` (Id)
- **Foreign Key thêm**:
  - `FK_Refunds_Orders_OrderId` -> `Orders(Id)` (Restrict)
  - `FK_Refunds_Payments_PaymentId` -> `Payments(Id)` (Restrict)
- **Index thêm**:
  - `IX_CancellationPolicies_OrderStatus` (Unique)
  - `IX_Refunds_OrderId`
  - `IX_Refunds_PaymentId`

---

### 012. AddNotificationsAndEmailHistory
- **Tên**: `AddNotificationsAndEmailHistory`
- **Tập tin**: `20260710083407_AddNotificationsAndEmailHistory.cs`
- **Ngày tạo**: 10/07/2026 08:34:07
- **Thay đổi chính**: Quản lý lịch sử gửi Email thông báo trạng thái đơn hàng và các thông báo đẩy trong ứng dụng cho khách hàng.
- **Table thêm mới** (2 bảng):
  - `EmailHistories`
  - `Notifications`
- **Primary Key thêm**:
  - `PK_EmailHistories` (Id)
  - `PK_Notifications` (Id)
- **Foreign Key thêm**:
  - `FK_EmailHistories_Customers_CustomerId` -> `Customers(Id)` (Restrict, Nullable)
  - `FK_Notifications_Customers_CustomerId` -> `Customers(Id)` (Restrict)
- **Index thêm**:
  - `IX_EmailHistories_CustomerId`
  - `IX_EmailHistories_EmailType`
  - `IX_EmailHistories_OrderId`
  - `IX_Notifications_CustomerId`
  - `IX_Notifications_CustomerId_IsRead`

---

### 013. AddPromotionModule
- **Tên**: `AddPromotionModule`
- **Tập tin**: `20260711125154_AddPromotionModule.cs`
- **Ngày tạo**: 11/07/2026 12:51:54
- **Thay đổi chính**: Thiết lập hệ thống chiến dịch khuyến mãi (Promotion Campaigns) và mã giảm giá (Coupons) hỗ trợ đa dạng cấu hình giảm giá.
- **Table thêm mới** (4 bảng):
  - `Coupons`
  - `PromotionCampaigns`
  - `CouponUsages`
  - `PromotionProducts`
- **Primary Key thêm**:
  - `PK_Coupons` (Id)
  - `PK_PromotionCampaigns` (Id)
  - `PK_CouponUsages` (Id)
  - `PK_PromotionProducts` (Id)
- **Foreign Key thêm**:
  - `FK_CouponUsages_Coupons_CouponId` -> `Coupons(Id)` (Restrict)
  - `FK_CouponUsages_Customers_CustomerId` -> `Customers(Id)` (Restrict)
  - `FK_CouponUsages_Orders_OrderId` -> `Orders(Id)` (Restrict)
  - `FK_PromotionProducts_Products_ProductId` -> `Products(Id)` (Restrict)
  - `FK_PromotionProducts_PromotionCampaigns_PromotionId` -> `PromotionCampaigns(Id)` (Restrict)
- **Index thêm**:
  - `IX_Coupons_Code` (Unique)
  - `IX_CouponUsages_CouponId_CustomerId`
  - `IX_CouponUsages_CustomerId`
  - `IX_CouponUsages_OrderId` (Unique)
  - `IX_PromotionCampaigns_Active_StartDate_EndDate`
  - `IX_PromotionProducts_ProductId`
  - `IX_PromotionProducts_PromotionId_ProductId` (Unique)

---

### 014. AddOrderPromotionFieldsAndFlashSale
- **Tên**: `AddOrderPromotionFieldsAndFlashSale`
- **Tập tin**: `20260712032329_AddOrderPromotionFieldsAndFlashSale.cs`
- **Ngày tạo**: 12/07/2026 03:23:29
- **Thay đổi chính**: Tích hợp thông tin khuyến mãi và mã giảm giá trực tiếp vào đơn đặt hàng, đồng thời triển khai cấu trúc cơ sở dữ liệu cho module Flash Sale bán hàng giờ vàng.
- **Table thêm mới** (2 bảng):
  - `FlashSales`
  - `FlashSaleProducts`
- **Column thêm**:
  - `Orders`: `CouponId` (int, nullable), `DiscountAmount` (decimal(18,2)), `FinalAmount` (decimal(18,2)), `OriginalAmount` (decimal(18,2)), `PromotionId` (int, nullable).
- **Primary Key thêm**:
  - `PK_FlashSales` (Id)
  - `PK_FlashSaleProducts` (Id)
- **Foreign Key thêm**:
  - `FK_FlashSaleProducts_FlashSales_FlashSaleId` -> `FlashSales(Id)` (Restrict)
  - `FK_FlashSaleProducts_Products_ProductId` -> `Products(Id)` (Restrict)
  - `FK_Orders_Coupons_CouponId` -> `Coupons(Id)` (Restrict)
  - `FK_Orders_PromotionCampaigns_PromotionId` -> `PromotionCampaigns(Id)` (Restrict)
- **Index thêm**:
  - `IX_Orders_CouponId`
  - `IX_Orders_PromotionId`
  - `IX_FlashSaleProducts_FlashSaleId`
  - `IX_FlashSaleProducts_ProductId`
  - `IX_FlashSales_Active_StartDate_EndDate`
