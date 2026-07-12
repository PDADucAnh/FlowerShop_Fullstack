# Database Current Schema (FlowerShop)

Tài liệu này phân tích và mô tả chi tiết toàn bộ cấu trúc cơ sở dữ liệu hiện tại của hệ thống FlowerShop dựa trên source code Backend ASP.NET Core và Database thực tế.

---

# Table: Advertisements

Bảng lưu trữ thông tin về các banner quảng cáo hiển thị trên trang web.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Title | nvarchar(500) | No | No | No | |
| Subtitle | nvarchar(max) | Yes | No | No | |
| ImageUrl | nvarchar(2000) | Yes | No | No | |
| LinkUrl | nvarchar(1000) | Yes | No | No | |
| SortOrder | int | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Advertisements(Id)

Foreign Keys
- None

Indexes
- None

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
- None

Rows Count
- 2

---

# Table: CancellationPolicies

Bảng lưu trữ chính sách hủy đơn hàng tương ứng với từng trạng thái của đơn hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| OrderStatus | nvarchar(50) | No | No | No | |
| RefundPercent | int | No | No | No | |
| CancellationFeePercent | int | No | No | No | |
| Description | nvarchar(500) | Yes | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_CancellationPolicies(Id)

Foreign Keys
- None

Indexes
- IX_CancellationPolicies_OrderStatus

Unique Constraints
- Unique index IX_CancellationPolicies_OrderStatus on OrderStatus

Check Constraints
- None

Navigation Properties
- None

Rows Count
- 0

---

# Table: Categories

Bảng danh mục cho bài viết/tin tức (Post).

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(2000) | Yes | No | No | |
| Slug | nvarchar(300) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Categories(Id)

Foreign Keys
- None

Indexes
- None

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Categories
HasMany(Posts)

Rows Count
- 1

---

# Table: CategoriesProducts

Bảng danh mục dành cho sản phẩm hoa (Product).

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(2000) | Yes | No | No | |
| Slug | nvarchar(300) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_CategoriesProducts(Id)

Foreign Keys
- None

Indexes
- None

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
CategoriesProducts
HasMany(Products)

Rows Count
- 2

---

# Table: Coupons

Bảng lưu trữ thông tin mã giảm giá / khuyến mãi (Coupon).

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Code | nvarchar(50) | No | No | No | |
| Description | nvarchar(500) | Yes | No | No | |
| DiscountType | int | No | No | No | |
| DiscountValue | decimal(18,2) | No | No | No | |
| MinimumOrderAmount | decimal(18,2) | Yes | No | No | |
| MaximumDiscountAmount | decimal(18,2) | Yes | No | No | |
| UsageLimit | int | Yes | No | No | |
| UsedCount | int | No | No | No | |
| UsagePerCustomer | int | Yes | No | No | |
| CustomerId | int | Yes | No | No | |
| StartDate | datetime2 | Yes | No | No | |
| EndDate | datetime2 | Yes | No | No | |
| IsPublic | bit | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Coupons(Id)

Foreign Keys
- None

Indexes
- IX_Coupons_Code

Unique Constraints
- Unique index IX_Coupons_Code on Code

Check Constraints
- None

Navigation Properties
Coupons
HasMany(Usages)

Rows Count
- 3

---

# Table: CouponUsages

Bảng lưu lịch sử áp dụng và sử dụng mã giảm giá của từng khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CouponId | int | No | No | Yes | |
| CustomerId | int | No | No | Yes | |
| OrderId | int | No | No | Yes | |
| DiscountAmount | decimal(18,2) | No | No | No | |
| UsedAt | datetime2 | No | No | No | |

Primary Key
- PK_CouponUsages(Id)

Foreign Keys
- CouponId -> Coupons(Id)
- CustomerId -> Customers(Id)
- OrderId -> Orders(Id)

Indexes
- IX_CouponUsages_OrderId
- IX_CouponUsages_CouponId_CustomerId
- IX_CouponUsages_CustomerId

Unique Constraints
- Unique index IX_CouponUsages_OrderId on OrderId

Check Constraints
- None

Navigation Properties
CouponUsages
BelongsTo(Coupon)
BelongsTo(Customer)
BelongsTo(Order)

Rows Count
- 0

---

# Table: Customers

Bảng lưu trữ tài khoản của khách mua hàng, trạng thái hoạt động và các thống kê mua hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| FullName | nvarchar(max) | No | No | No | |
| Email | nvarchar(450) | No | No | No | |
| Phone | nvarchar(450) | Yes | No | No | |
| Address | nvarchar(max) | Yes | No | No | |
| Password | nvarchar(max) | No | No | No | |
| DefaultAddressId | int | Yes | No | No | |
| TotalOrders | int | No | No | No | |
| SuccessfulDeliveries | int | No | No | No | |
| FailedDeliveries | int | No | No | No | |
| IsBlacklisted | bit | No | No | No | |
| FraudScore | int | No | No | No | |
| ResetToken | nvarchar(100) | Yes | No | No | |
| ResetTokenExpiry | datetime2 | Yes | No | No | |
| EmailVerified | bit | No | No | No | |
| PhoneVerified | bit | No | No | No | |
| LastLogin | datetime2 | Yes | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Customers(Id)

Foreign Keys
- None

Indexes
- IX_Customers_Email
- IX_Customers_Phone
- IX_Customers_ResetToken

Unique Constraints
- Unique index IX_Customers_Email on Email

Check Constraints
- None

Navigation Properties
Customers
HasMany(Orders)

Rows Count
- 1

---

# Table: CustomerAddresses

Bảng lưu trữ danh sách địa chỉ nhận hàng của khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CustomerId | int | No | No | Yes | |
| ReceiverName | nvarchar(200) | Yes | No | No | |
| ReceiverPhone | nvarchar(20) | Yes | No | No | |
| Province | nvarchar(100) | Yes | No | No | |
| District | nvarchar(100) | Yes | No | No | |
| Ward | nvarchar(100) | Yes | No | No | |
| AddressLine | nvarchar(500) | Yes | No | No | |
| PostalCode | nvarchar(20) | Yes | No | No | |
| Note | nvarchar(500) | Yes | No | No | |
| Latitude | float | Yes | No | No | |
| Longitude | float | Yes | No | No | |
| IsDefault | bit | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_CustomerAddresses(Id)

Foreign Keys
- CustomerId -> Customers(Id)

Indexes
- IX_CustomerAddresses_CustomerId_IsDefault

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
CustomerAddresses
BelongsTo(Customer)

Rows Count
- 0

---

# Table: CustomerPaymentPreferences

Bảng lưu phương thức thanh toán ưa thích/mặc định của khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CustomerId | int | No | No | Yes | |
| PaymentMethodId | int | No | No | Yes | |
| IsDefault | bit | No | No | No | |
| LastUsedAt | datetime2 | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_CustomerPaymentPreferences(Id)

Foreign Keys
- CustomerId -> Customers(Id)
- PaymentMethodId -> PaymentMethods(Id)

Indexes
- IX_CustomerPaymentPreferences_PaymentMethodId
- IX_CustomerPaymentPreferences_CustomerId_PaymentMethodId

Unique Constraints
- Unique index on CustomerId & PaymentMethodId

Check Constraints
- None

Navigation Properties
CustomerPaymentPreferences
BelongsTo(Customer)
BelongsTo(PaymentMethod)

Rows Count
- 0

---

# Table: DeliverySlots

Bảng quản lý khung giờ và dung lượng (capacity) giao hàng của sản phẩm.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| ProductId | int | No | No | Yes | |
| DeliveryDate | datetime2 | No | No | No | |
| TimeSlot | nvarchar(50) | No | No | No | |
| MaxCapacity | int | No | No | No | |
| CurrentBooked | int | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_DeliverySlots(Id)

Foreign Keys
- ProductId -> Products(Id)

Indexes
- IX_DeliverySlots_Date_TimeSlot_IsActive
- IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
DeliverySlots
BelongsTo(Product)

Rows Count
- 12

---

# Table: EmailHistories

Bảng lưu trữ lịch sử các email đã gửi cho khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CustomerId | int | Yes | No | Yes | |
| OrderId | int | Yes | No | No | |
| EmailType | nvarchar(100) | No | No | No | |
| Recipient | nvarchar(500) | No | No | No | |
| Subject | nvarchar(500) | Yes | No | No | |
| Status | nvarchar(50) | No | No | No | |
| SentAt | datetime2 | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |

Primary Key
- PK_EmailHistories(Id)

Foreign Keys
- CustomerId -> Customers(Id)

Indexes
- IX_EmailHistories_CustomerId
- IX_EmailHistories_EmailType
- IX_EmailHistories_OrderId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
EmailHistories
BelongsTo(Customer)

Rows Count
- 0

---

# Table: FlashSales

Bảng cấu hình đợt khuyến mãi Flash Sale thời hạn ngắn.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(2000) | Yes | No | No | |
| StartDate | datetime2 | No | No | No | |
| EndDate | datetime2 | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_FlashSales(Id)

Foreign Keys
- None

Indexes
- IX_FlashSales_Active_StartDate_EndDate

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
FlashSales
HasMany(FlashSaleProducts)

Rows Count
- 0

---

# Table: FlashSaleProducts

Bảng liên kết sản phẩm tham gia Flash Sale cùng mức chiết khấu và giá sale cụ thể.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| FlashSaleId | int | No | No | Yes | |
| ProductId | int | No | No | Yes | |
| SalePrice | decimal(18,2) | No | No | No | |
| DiscountPercent | decimal(18,2) | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |

Primary Key
- PK_FlashSaleProducts(Id)

Foreign Keys
- FlashSaleId -> FlashSales(Id)
- ProductId -> Products(Id)

Indexes
- IX_FlashSaleProducts_FlashSaleId
- IX_FlashSaleProducts_ProductId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
FlashSaleProducts
BelongsTo(FlashSale)
BelongsTo(Product)

Rows Count
- 0

---

# Table: Notifications

Bảng lưu thông báo hệ thống của khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CustomerId | int | No | No | Yes | |
| OrderId | int | Yes | No | No | |
| Title | nvarchar(500) | No | No | No | |
| Content | nvarchar(2000) | Yes | No | No | |
| Type | nvarchar(50) | No | No | No | |
| IsRead | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |

Primary Key
- PK_Notifications(Id)

Foreign Keys
- CustomerId -> Customers(Id)

Indexes
- IX_Notifications_CustomerId
- IX_Notifications_CustomerId_IsRead

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Notifications
BelongsTo(Customer)

Rows Count
- 0

---

# Table: Orders

Bảng lưu các giao dịch đặt hàng của khách hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| OrderDate | datetime2 | No | No | No | |
| CustomerId | int | No | No | Yes | |
| Status | int | No | No | No | |
| Notes | nvarchar(max) | Yes | No | No | |
| PaymentMethod | int | No | No | No | |
| PaymentStatus | int | No | No | No | |
| PaymentTransactionId | nvarchar(200) | Yes | No | No | |
| PaymentPaidAt | datetime2 | Yes | No | No | |
| DeliveryDate | datetime2 | Yes | No | No | |
| DeliveryTimeSlot | nvarchar(50) | Yes | No | No | |
| DeliverySlotId | int | Yes | No | No | |
| DeliveryDistrict | nvarchar(100) | Yes | No | No | |
| DeliveryAddress | nvarchar(500) | Yes | No | No | |
| RecipientName | nvarchar(200) | Yes | No | No | |
| RecipientPhone | nvarchar(20) | Yes | No | No | |
| DeliveryReceiverName | nvarchar(200) | Yes | No | No | |
| DeliveryReceiverPhone | nvarchar(20) | Yes | No | No | |
| DeliveryProvince | nvarchar(100) | Yes | No | No | |
| DeliveryWard | nvarchar(100) | Yes | No | No | |
| DeliveryAddressLine | nvarchar(500) | Yes | No | No | |
| DeliveryPostalCode | nvarchar(20) | Yes | No | No | |
| CancelledAt | datetime2 | Yes | No | No | |
| CancellationReason | nvarchar(500) | Yes | No | No | |
| IsVerified | bit | No | No | No | |
| VerifiedAt | datetime2 | Yes | No | No | |
| RefundAmount | decimal(18,2) | No | No | No | |
| CancelledBy | nvarchar(50) | Yes | No | No | |
| CancellationFee | decimal(18,2) | No | No | No | |
| RefundRequestedAt | datetime2 | Yes | No | No | |
| RefundCompletedAt | datetime2 | Yes | No | No | |
| TargetFinishedTime | datetime2 | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |
| PromotionId | int | Yes | No | Yes | |
| CouponId | int | Yes | No | Yes | |
| OriginalAmount | decimal(18,2) | No | No | No | |
| DiscountAmount | decimal(18,2) | No | No | No | |
| FinalAmount | decimal(18,2) | No | No | No | |

Primary Key
- PK_Orders(Id)

Foreign Keys
- CustomerId -> Customers(Id)
- PromotionId -> PromotionCampaigns(Id)
- CouponId -> Coupons(Id)

Indexes
- IX_Orders_CustomerId
- IX_Orders_OrderDate
- IX_Orders_Status
- IX_Orders_Status_OrderDate
- IX_Orders_PromotionId
- IX_Orders_CouponId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Orders
BelongsTo(Customer)
BelongsTo(Promotion)
BelongsTo(Coupon)
HasMany(OrderDetails)

Rows Count
- 17

---

# Table: OrderDetails

Bảng chi tiết các sản phẩm nằm trong đơn hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| OrderId | int | No | No | Yes | |
| ProductId | int | No | No | Yes | |
| Quantity | int | No | No | No | |
| UnitPrice | decimal(18,2) | No | No | No | |
| ProductName | nvarchar(200) | Yes | No | No | |
| ProductImage | nvarchar(1000) | Yes | No | No | |
| SizeVariant | nvarchar(50) | Yes | No | No | |
| Discount | decimal(18,2) | Yes | No | No | |
| Subtotal | decimal(18,2) | Yes | No | No | |

Primary Key
- PK_OrderDetails(Id)

Foreign Keys
- OrderId -> Orders(Id)
- ProductId -> Products(Id)

Indexes
- IX_OrderDetails_OrderId
- IX_OrderDetails_ProductId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
OrderDetails
BelongsTo(Order)
BelongsTo(Product)

Rows Count
- 17

---

# Table: Payments

Bảng lưu vết thông tin thanh toán cho đơn hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| OrderId | int | No | No | Yes | |
| Amount | decimal(18,2) | No | No | No | |
| Method | int | No | No | No | |
| Status | int | No | No | No | |
| PaymentMethodId | int | Yes | No | Yes | |
| Gateway | nvarchar(50) | Yes | No | No | |
| TransactionId | nvarchar(200) | Yes | No | No | |
| GatewayResponseCode | nvarchar(50) | Yes | No | No | |
| BankCode | nvarchar(50) | Yes | No | No | |
| PaymentUrl | nvarchar(1000) | Yes | No | No | |
| PaidAt | datetime2 | Yes | No | No | |
| RefundedAt | datetime2 | Yes | No | No | |
| RefundTransactionId | nvarchar(200) | Yes | No | No | |
| RefundResponseCode | nvarchar(50) | Yes | No | No | |
| RefundedBy | nvarchar(100) | Yes | No | No | |
| RefundNote | nvarchar(500) | Yes | No | No | |
| Notes | nvarchar(500) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Payments(Id)

Foreign Keys
- OrderId -> Orders(Id)
- PaymentMethodId -> PaymentMethods(Id)

Indexes
- IX_Payments_OrderId
- IX_Payments_PaymentMethodId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Payments
BelongsTo(Order)
BelongsTo(PaymentMethodRef)

Rows Count
- 30

---

# Table: PaymentAttempts

Bảng lưu log chi tiết từng lượt gửi yêu cầu thanh toán sang cổng thanh toán VNPAY/online.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| PaymentId | int | No | No | Yes | |
| AttemptNumber | int | No | No | No | |
| GatewayRequest | nvarchar(max) | Yes | No | No | |
| GatewayResponse | nvarchar(max) | Yes | No | No | |
| IpAddress | nvarchar(50) | Yes | No | No | |
| UserAgent | nvarchar(500) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |

Primary Key
- PK_PaymentAttempts(Id)

Foreign Keys
- PaymentId -> Payments(Id)

Indexes
- IX_PaymentAttempts_PaymentId_AttemptNumber

Unique Constraints
- Unique index on PaymentId & AttemptNumber

Check Constraints
- None

Navigation Properties
PaymentAttempts
BelongsTo(Payment)

Rows Count
- 11

---

# Table: PaymentMethods

Bảng định nghĩa danh sách phương thức thanh toán khả dụng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Code | nvarchar(50) | No | No | No | |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(500) | Yes | No | No | |
| IsOnline | bit | No | No | No | |
| IsActive | bit | No | No | No | |
| DisplayOrder | int | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_PaymentMethods(Id)

Foreign Keys
- None

Indexes
- IX_PaymentMethods_Code

Unique Constraints
- Unique index IX_PaymentMethods_Code on Code

Check Constraints
- None

Navigation Properties
- None

Rows Count
- 0

---

# Table: PhoneBlacklists

Bảng lưu danh sách đen các số điện thoại bị chặn đặt hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| PhoneNumber | nvarchar(20) | No | No | No | |
| Reason | nvarchar(500) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| IsActive | bit | No | No | No | |

Primary Key
- PK_PhoneBlacklists(Id)

Foreign Keys
- None

Indexes
- IX_PhoneBlacklist_PhoneNumber_IsActive

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
- None

Rows Count
- 0

---

# Table: Posts

Bảng lưu trữ các bài viết/tin tức trên trang web.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| CategoryId | int | No | No | Yes | |
| Title | nvarchar(500) | No | No | No | |
| Content | nvarchar(max) | No | No | No | |
| Summary | nvarchar(500) | Yes | No | No | |
| Slug | nvarchar(300) | Yes | No | No | |
| ImageUrl | nvarchar(1000) | Yes | No | No | |
| CreatedDate | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Posts(Id)

Foreign Keys
- CategoryId -> Categories(Id)

Indexes
- IX_Posts_CategoryId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Posts
BelongsTo(Category)

Rows Count
- 3

---

# Table: Products

Bảng lưu thông tin chi tiết các sản phẩm hoa của cửa hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Sku | nvarchar(50) | Yes | No | No | |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(max) | Yes | No | No | |
| Slug | nvarchar(300) | Yes | No | No | |
| Price | decimal(18,2) | No | No | No | |
| DiscountPrice | decimal(18,2) | Yes | No | No | |
| StockQuantity | int | No | No | No | |
| ImageUrl | nvarchar(max) | Yes | No | No | |
| CategoryProductId | int | No | No | Yes | |
| ViewCount | int | No | No | No | |
| AddToCartCount | int | No | No | No | |
| IsActive | bit | No | No | No | |
| FlowerMeaning | nvarchar(500) | Yes | No | No | |
| Origin | nvarchar(200) | Yes | No | No | |
| CareInstruction | nvarchar(max) | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Products(Id)

Foreign Keys
- CategoryProductId -> CategoriesProducts(Id)

Indexes
- IX_Products_CategoryProductId
- IX_Products_Sku

Unique Constraints
- Unique index IX_Products_Sku on Sku

Check Constraints
- None

Navigation Properties
Products
BelongsTo(CategoryProduct)
HasMany(ProductVariants)

Rows Count
- 10

---

# Table: ProductVariants

Bảng lưu thông tin các biến thể sản phẩm (size, kiểu cắm...) kèm mức điều chỉnh giá.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| ProductId | int | No | No | Yes | |
| Name | nvarchar(50) | No | No | No | |
| PriceAdjustment | decimal(18,2) | No | No | No | |
| IsDefault | bit | No | No | No | |
| ProductId1 | int | Yes | No | Yes | |

Primary Key
- PK_ProductVariants(Id)

Foreign Keys
- ProductId -> Products(Id)
- ProductId1 -> Products(Id)

Indexes
- IX_ProductVariants_ProductId
- IX_ProductVariants_ProductId1

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
ProductVariants
BelongsTo(Product)

Rows Count
- 0

---

# Table: PromotionCampaigns

Bảng lưu các chiến dịch khuyến mãi (đợt giảm giá, ưu đãi mùa...).

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Name | nvarchar(200) | No | No | No | |
| Description | nvarchar(2000) | Yes | No | No | |
| PromotionType | int | No | No | No | |
| DiscountType | int | No | No | No | |
| DiscountValue | decimal(18,2) | No | No | No | |
| StartDate | datetime2 | No | No | No | |
| EndDate | datetime2 | No | No | No | |
| Priority | int | No | No | No | |
| BannerImage | nvarchar(1000) | Yes | No | No | |
| IsStackable | bit | No | No | No | |
| IsActive | bit | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_PromotionCampaigns(Id)

Foreign Keys
- None

Indexes
- IX_PromotionCampaigns_Active_StartDate_EndDate

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
PromotionCampaigns
HasMany(PromotionProducts)

Rows Count
- 0

---

# Table: PromotionProducts

Bảng liên kết các sản phẩm được áp dụng trong đợt khuyến mãi cụ thể.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| PromotionId | int | No | No | Yes | |
| ProductId | int | No | No | Yes | |
| CreatedAt | datetime2 | No | No | No | |

Primary Key
- PK_PromotionProducts(Id)

Foreign Keys
- PromotionId -> PromotionCampaigns(Id)
- ProductId -> Products(Id)

Indexes
- IX_PromotionProducts_ProductId
- IX_PromotionProducts_PromotionId_ProductId

Unique Constraints
- Unique index IX_PromotionProducts_PromotionId_ProductId on PromotionId & ProductId

Check Constraints
- None

Navigation Properties
PromotionProducts
BelongsTo(Promotion)
BelongsTo(Product)

Rows Count
- 0

---

# Table: RefreshTokens

Bảng quản lý token gia hạn đăng nhập cho tài khoản User.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| UserId | int | No | No | Yes | |
| TokenHash | nvarchar(256) | No | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| ExpiresAt | datetime2 | No | No | No | |
| RevokedAt | datetime2 | Yes | No | No | |
| IsRevoked | bit | No | No | No | |
| DeviceInfo | nvarchar(50) | Yes | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_RefreshTokens(Id)

Foreign Keys
- UserId -> Users(Id)

Indexes
- IX_RefreshTokens_TokenHash
- IX_RefreshTokens_UserId

Unique Constraints
- Unique index IX_RefreshTokens_TokenHash on TokenHash

Check Constraints
- None

Navigation Properties
RefreshTokens
BelongsTo(User)

Rows Count
- 17

---

# Table: Refunds

Bảng lưu vết thông tin hoàn tiền cho đơn hàng bị hủy.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| OrderId | int | No | No | Yes | |
| PaymentId | int | Yes | No | Yes | |
| RequestedBy | nvarchar(100) | Yes | No | No | |
| ApprovedBy | nvarchar(100) | Yes | No | No | |
| Reason | nvarchar(500) | Yes | No | No | |
| RefundType | nvarchar(50) | Yes | No | No | |
| RefundPercent | int | No | No | No | |
| RefundAmount | decimal(18,2) | No | No | No | |
| RefundStatus | int | No | No | No | |
| GatewayRefundId | nvarchar(200) | Yes | No | No | |
| ProcessedAt | datetime2 | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Refunds(Id)

Foreign Keys
- OrderId -> Orders(Id)
- PaymentId -> Payments(Id)

Indexes
- IX_Refunds_OrderId
- IX_Refunds_PaymentId

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
Refunds
BelongsTo(Order)
BelongsTo(Payment)

Rows Count
- 2

---

# Table: Users

Bảng lưu thông tin tài khoản nhân viên / quản trị viên của cửa hàng.

| Column | Data Type | Nullable | PK | FK | Default |
|--------|-----------|----------|----|----|---------|
| Id | int | No | Yes | No | Identity |
| Username | nvarchar(50) | No | No | No | |
| PasswordHash | nvarchar(max) | No | No | No | |
| FullName | nvarchar(200) | No | No | No | |
| Email | nvarchar(200) | Yes | No | No | |
| Phone | nvarchar(20) | Yes | No | No | |
| Address | nvarchar(500) | Yes | No | No | |
| Role | nvarchar(50) | No | No | No | |
| IsActive | bit | No | No | No | |
| LastLogin | datetime2 | Yes | No | No | |
| CreatedAt | datetime2 | No | No | No | |
| UpdatedAt | datetime2 | Yes | No | No | |

Primary Key
- PK_Users(Id)

Foreign Keys
- None

Indexes
- None

Unique Constraints
- None

Check Constraints
- None

Navigation Properties
- None

Rows Count
- 2

---

## Bảng thống kê toàn bộ Database

Dưới đây là bảng tổng hợp số lượng bản ghi (rows), trạng thái khóa chính (PK) và khóa ngoại (FK) của toàn bộ 28 bảng trong Database của dự án:

| Table | Rows | PK | FK |
|-------|------|----|----|
| Advertisements | 2 | Yes (Id) | No |
| CancellationPolicies | 0 | Yes (Id) | No |
| Categories | 1 | Yes (Id) | No |
| CategoriesProducts | 2 | Yes (Id) | No |
| Coupons | 3 | Yes (Id) | No |
| CouponUsages | 0 | Yes (Id) | Yes (CouponId, CustomerId, OrderId) |
| Customers | 1 | Yes (Id) | No |
| CustomerAddresses | 0 | Yes (Id) | Yes (CustomerId) |
| CustomerPaymentPreferences | 0 | Yes (Id) | Yes (CustomerId, PaymentMethodId) |
| DeliverySlots | 12 | Yes (Id) | Yes (ProductId) |
| EmailHistories | 0 | Yes (Id) | Yes (CustomerId) |
| FlashSales | 0 | Yes (Id) | No |
| FlashSaleProducts | 0 | Yes (Id) | Yes (FlashSaleId, ProductId) |
| Notifications | 0 | Yes (Id) | Yes (CustomerId) |
| Orders | 17 | Yes (Id) | Yes (CustomerId, PromotionId, CouponId) |
| OrderDetails | 17 | Yes (Id) | Yes (OrderId, ProductId) |
| Payments | 30 | Yes (Id) | Yes (OrderId, PaymentMethodId) |
| PaymentAttempts | 11 | Yes (Id) | Yes (PaymentId) |
| PaymentMethods | 0 | Yes (Id) | No |
| PhoneBlacklists | 0 | Yes (Id) | No |
| Posts | 3 | Yes (Id) | Yes (CategoryId) |
| Products | 10 | Yes (Id) | Yes (CategoryProductId) |
| ProductVariants | 0 | Yes (Id) | Yes (ProductId, ProductId1) |
| PromotionCampaigns | 0 | Yes (Id) | No |
| PromotionProducts | 0 | Yes (Id) | Yes (PromotionId, ProductId) |
| RefreshTokens | 17 | Yes (Id) | Yes (UserId) |
| Refunds | 2 | Yes (Id) | Yes (OrderId, PaymentId) |
| Users | 2 | Yes (Id) | No |
