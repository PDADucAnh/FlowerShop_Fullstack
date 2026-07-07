Đây là đề xuất của mình sau khi xem toàn bộ ERD và xét theo **đồ án tốt nghiệp Website bán hoa (Next.js + ASP.NET Core + SQL Server + VNPAY)**.

Mục tiêu là:

* Không thay đổi quá nhiều kiến trúc hiện tại.
* Chỉ bổ sung những gì thực sự cần.
* Dễ bảo vệ đồ án.
* Dễ mở rộng sau này.

---

# 1. Users

### Hiện tại

```text
Id
Username
PasswordHash
FullName
Email
Phone
Address
Role
```

### Đề xuất

```text
Id
Username
PasswordHash
FullName
Email
Phone
Address
Role
IsActive              (NEW)
LastLogin             (NEW)
CreatedAt             (NEW)
UpdatedAt             (NEW)
```

### Giải thích

| Trường    | Lý do                                |
| --------- | ------------------------------------ |
| IsActive  | Khóa tài khoản nhân viên thay vì xóa |
| LastLogin | Theo dõi lần đăng nhập cuối          |
| CreatedAt | Audit                                |
| UpdatedAt | Audit                                |

---

# 2. Customers

### Hiện tại

```text
Id
FullName
Email
Phone
Address
Password
FailedDeliveries
FraudScore
IsBlacklisted
SuccessfulDeliveries
TotalOrders
ResetToken
ResetTokenExpiry
```

### Đề xuất

```text
Id
FullName
Email
Phone

PasswordHash          (Đổi từ Password)

DefaultAddressId      (NEW)

FailedDeliveries
FraudScore
IsBlacklisted
SuccessfulDeliveries
TotalOrders

ResetToken
ResetTokenExpiry

EmailVerified         (NEW)
PhoneVerified         (NEW)

LastLogin             (NEW)

IsActive              (NEW)

CreatedAt             (NEW)
UpdatedAt             (NEW)
```

### Giải thích

| Trường | Lý do |
|----------|-------------------------------|
| PasswordHash | Không lưu mật khẩu thô |
| DefaultAddressId | Địa chỉ mặc định của khách |
| EmailVerified | Xác thực Email |
| PhoneVerified | Xác thực SĐT |
| LastLogin | Lần đăng nhập cuối |
| IsActive | Khóa tài khoản |
| CreatedAt | Audit |
| UpdatedAt | Audit |

> Lưu ý:
>
> Customers không còn lưu trực tiếp Address.
>
> Mỗi khách có thể lưu nhiều địa chỉ trong bảng CustomerAddresses.
# 3. Products

### Hiện tại

```text
Id
Sku
Name
Description
Slug
Price
StockQuantity
ImageUrl
CategoryProductId
AddToCartCount
DiscountPrice
ViewCount
```

### Đề xuất

```text
Id
Sku
Name
Description
Slug
Price
DiscountPrice
StockQuantity
ImageUrl
CategoryProductId
AddToCartCount
ViewCount
IsActive             (NEW)
CreatedAt            (NEW)
UpdatedAt            (NEW)

FlowerMeaning        (OPTION)
Origin               (OPTION)
CareInstruction      (OPTION)
```

### Giải thích

| Trường          | Lý do              |
| --------------- | ------------------ |
| IsActive        | Ẩn sản phẩm        |
| CreatedAt       | Audit              |
| UpdatedAt       | Audit              |
| FlowerMeaning   | Ý nghĩa hoa        |
| Origin          | Xuất xứ            |
| CareInstruction | Hướng dẫn chăm sóc |

---

# 4. CategoriesProducts

### Hiện tại

```text
Id
Name
Description
Slug
```

### Đề xuất

```text
Id
Name
Description
Slug
CreatedAt
UpdatedAt
```

---

# 5. Orders

## Hiện tại

```text
Id
OrderDate
CustomerId
Status
Notes
CancelationReason
CanceledAt
DeliveryAddress
DeliveryDate
DeliveryDistrict
DeliveryTimeSlot
IsVerified
PaymentMethod
PaymentPaidAt
PaymentStatus
PaymentTransactionId
RefundAmount
VerifiedAt
TargetFinishedTime
```

---

## Đề xuất

```text
Id

OrderDate

CustomerId

Status

Notes

CancellationReason

CancelledAt

DeliveryReceiverName      (NEW)

DeliveryReceiverPhone     (NEW)

DeliveryProvince          (NEW)

DeliveryDistrict

DeliveryWard              (NEW)

DeliveryAddressLine       (NEW)

DeliveryPostalCode        (NEW)

DeliveryDate

DeliverySlotId            (NEW)

IsVerified

VerifiedAt

RefundAmount

TargetFinishedTime

CreatedAt                 (NEW)

UpdatedAt                 (NEW)
```

## Xóa

```text
PaymentMethod

PaymentPaidAt

PaymentStatus

PaymentTransactionId

DeliveryAddress
```

### Giải thích

Orders lưu Snapshot địa chỉ giao hàng.

Không tham chiếu trực tiếp CustomerAddresses.

Nếu khách sửa địa chỉ sau khi đặt hàng thì đơn cũ vẫn giữ nguyên.
# 6. OrderDetails

### Hiện tại

```text
Id
OrderId
ProductId
Quantity
UnitPrice
```

### Đề xuất

```text
Id
OrderId
ProductId
Quantity
UnitPrice

Discount      (NEW)

Subtotal      (NEW)
```

### Giải thích

Ví dụ

```text
Giá

500

Giảm

100

Subtotal

400
```

Sau này giá sản phẩm thay đổi vẫn giữ lịch sử.

---

# 7. Payments

Hiện tại

```text
Id
OrderId
Amount
Method
Status
TransactionId
PaidAt
RefundedAt
Notes
```

---

Đề xuất

```text
Id

OrderId

PaymentMethodId        (NEW)

Gateway

Amount

Status

TransactionId

GatewayResponseCode

BankCode

PaymentUrl

CreatedAt

UpdatedAt

PaidAt

RefundedAt

Notes
```

### Giải thích

| Trường | Lý do |
|----------|-----------------------|
| PaymentMethodId | FK sang PaymentMethods |
| Gateway | VNPay, MoMo... |
| GatewayResponseCode | ResponseCode |
| BankCode | Ngân hàng |
| PaymentUrl | Debug |
| CreatedAt | Audit |
| UpdatedAt | Audit |

Payment Method không lưu dạng text.

Luôn lấy từ bảng PaymentMethods.
# 8. DeliverySlots

Hiện tại

```text
Id

ProductId

DeliveryDate

TimeSlot

MaxCapacity

CurrentBooked

IsActive
```

---

Đề xuất

```text
Id

DeliveryDate

TimeSlot

MaxCapacity

CurrentBooked

IsActive

CreatedAt

UpdatedAt
```

### Xóa

```text
ProductId
```

### Lý do

Delivery Slot thuộc hệ thống giao hàng.

Không thuộc Product.

---

# 9. Advertisements

Hiện tại

```text
Id

Title

Subtitle

ImageUrl

LinkUrl

SortOrder

IsActive

CreateDate
```

Đề xuất

```text
Id
Title
Subtitle
ImageUrl
LinkUrl
SortOrder
IsActive
CreatedAt
UpdatedAt
```

---

# 10. Posts

Đề xuất

```text
Id
Title
Content
Summary
Slug
ImageUrl
CategoryId
CreatedAt
UpdatedAt
```

---

# 11. Categories

Đề xuất

```text
Id
Name
Description
Slug
CreatedAt
UpdatedAt
```

---

# 12. RefreshTokens

Hiện tại

Khá chuẩn.

Chỉ thêm

```text
UpdatedAt
```

---

# 13. PhoneBlacklists

Hiện tại

```text
Id
PhoneNumber
Reason
CreatedAt
IsActive
```

Giữ nguyên.

---
---

# 14. CustomerAddresses

```text
Id

CustomerId

ReceiverName

ReceiverPhone

Province

District

Ward

AddressLine

PostalCode

Note

Latitude

Longitude

IsDefault

IsActive

CreatedAt

UpdatedAt
```

### Lý do

Một khách hàng có thể lưu nhiều địa chỉ giao hoa.

Ví dụ

- Nhà
- Công ty
- Nhà người thân
- Người nhận quà

Orders sẽ Copy địa chỉ này thành Snapshot.

Không tham chiếu trực tiếp.

---

# 15. PaymentMethods

```text
Id

Code

Name

Description

IsOnline

IsActive

DisplayOrder

CreatedAt

UpdatedAt
```

### Lý do

Không Hardcode

- VNPay
- COD
- MoMo

AI phải đọc từ Database.

---

# 16. CustomerPaymentPreferences

```text
Id

CustomerId

PaymentMethodId

IsDefault

LastUsedAt

CreatedAt

UpdatedAt
```

### Lý do

Ghi nhớ khách thường dùng phương thức nào.

Không lưu thông tin thẻ.

Ví dụ

✓ VNPay

✓ COD

---

# 17. CustomerPaymentMethods (Future Extension)

```text
Id

CustomerId

Gateway

PaymentToken

MaskedCardNumber

CardBrand

CardHolderName

ExpiryMonth

ExpiryYear

IsDefault

CreatedAt

UpdatedAt
```

### Chỉ dùng khi

Gateway hỗ trợ Tokenization.

Không lưu

- Card Number

- CVV

- OTP
# Bảng nên thêm

# Bảng nên thêm

- CustomerAddresses

- PaymentMethods

- CustomerPaymentPreferences

- CustomerPaymentMethods (Future Extension)

- ProductReviews

- Wishlists

- AuditLogs---

# Tóm tắt thay đổi

| Bảng               | Thay đổi                                                                              |
| ------------------ | ------------------------------------------------------------------------------------- |
| Users              | + IsActive, LastLogin, CreatedAt, UpdatedAt                                           |
| Customers          | PasswordHash, EmailVerified, PhoneVerified, LastLogin, IsActive, CreatedAt, UpdatedAt |
| Products           | + IsActive, CreatedAt, UpdatedAt, (FlowerMeaning, Origin, CareInstruction tùy chọn)   |
| CategoriesProducts | + CreatedAt, UpdatedAt                                                                |
| Orders             | Bỏ thông tin Payment, thêm DeliverySlotId, CreatedAt, UpdatedAt                       |
| OrderDetails       | + Discount, Subtotal                                                                  |
| Payments           | + Gateway, GatewayResponseCode, BankCode, PaymentUrl, CreatedAt, UpdatedAt            |
| DeliverySlots      | Bỏ ProductId, + CreatedAt, UpdatedAt                                                  |
| Advertisements     | Chuẩn hóa thành CreatedAt, UpdatedAt                                                  |
| Posts              | + UpdatedAt                                                                           |
| Categories         | + CreatedAt, UpdatedAt                                                                |
| RefreshTokens      | + UpdatedAt                                                                           |
| Mới                | ProductReviews, Wishlists, AuditLogs       
| Customers | + DefaultAddressId |
| Orders | Snapshot Delivery Address |
| Payments | + PaymentMethodId |
| Mới | CustomerAddresses |
| Mới | PaymentMethods |
| Mới | CustomerPaymentPreferences |
| Mới | CustomerPaymentMethods (Future Extension) |                                           |

## Khuyến nghị thêm

Nếu bạn hướng tới đồ án đạt chất lượng cao, mình còn đề xuất:

* Chuẩn hóa tất cả các cột thời gian theo một quy ước thống nhất (`CreatedAt`, `UpdatedAt`, `DeletedAt` nếu dùng soft delete).
* Sử dụng `enum` hoặc bảng tra cứu cho các trường trạng thái (`OrderStatus`, `PaymentStatus`) thay vì lưu chuỗi tự do để tránh sai lệch dữ liệu.
* Thêm các chỉ mục (indexes) cho các cột được tìm kiếm nhiều như `Sku`, `Slug`, `Email`, `Phone`, `OrderDate`, `CustomerId`, `Status`, `TransactionId`.
* Thiết lập các ràng buộc (`UNIQUE`, `CHECK`, `FOREIGN KEY`) để đảm bảo tính toàn vẹn dữ liệu.

Với những điều chỉnh trên, cơ sở dữ liệu sẽ phù hợp với quy mô của một đồ án tốt nghiệp, đồng thời vẫn đủ khả năng mở rộng khi bổ sung các tính năng như MoMo, Stripe, đánh giá sản phẩm hoặc quản lý khuyến mãi trong tương lai.
---

# Kiến trúc thanh toán

Customers
        │
        ├──────────────┐
        │              │
        ▼              ▼

CustomerAddresses

CustomerPaymentPreferences

        │              │
        │              ▼

        │        PaymentMethods

        │              │

        └──────────────┴───────────────┐

                                       ▼

Orders

        │

        ▼

Payments

        │

        ▼

OrderDetails

---

## Quy tắc

- Customers có thể lưu nhiều địa chỉ giao hàng.
- Orders luôn lưu Snapshot địa chỉ.
- Khách có thể chọn địa chỉ khác nhau cho mỗi đơn.
- Payment Method lấy từ bảng PaymentMethods.
- Hệ thống ghi nhớ phương thức thanh toán ưa thích bằng CustomerPaymentPreferences.
- Không lưu Card Number.
- Không lưu CVV.
- Không lưu OTP.
- Nếu sau này Gateway hỗ trợ Tokenization thì chỉ lưu Payment Token.
- Kiến trúc phải hỗ trợ mở rộng sang VNPay, MoMo, Stripe, PayPal mà không cần thay đổi Database.