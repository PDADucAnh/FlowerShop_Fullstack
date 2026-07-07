# Flower Database Business Rules

## Mục tiêu

Tài liệu này mô tả các quy tắc nghiệp vụ bắt buộc mà AI phải tuân thủ khi thiết kế, sửa đổi hoặc mở rộng cơ sở dữ liệu của hệ thống bán hoa.

AI không được thay đổi các quy tắc này nếu không có yêu cầu rõ ràng từ người dùng.

---

# 1. User và Customer là hai hệ thống độc lập

Hệ thống sử dụng hai bảng riêng biệt.

Users

- Dành cho Admin.
- Dành cho Staff.
- Dành cho Manager.
- Đăng nhập trang quản trị.
- Không đại diện cho khách hàng.

Customers

- Dành cho khách hàng.
- Đăng nhập website mua hoa.
- Không dùng chung bảng Users.

AI không được gộp hai bảng này.

---

# 2. Password

Users

Phải lưu:

PasswordHash

Customers

Phải lưu:

PasswordHash

Không được lưu Password dạng Plain Text.

AI phải sử dụng Password Hash theo chuẩn ASP.NET Core.

---

# 3. Payment

Orders không được lưu dữ liệu thanh toán chi tiết.

Các trường sau không nên tồn tại trong Orders:

- PaymentMethod
- PaymentStatus
- PaymentPaidAt
- PaymentTransactionId

Thông tin thanh toán phải được quản lý tập trung trong bảng Payments.

Orders chỉ cần lưu trạng thái của đơn hàng.

---

# 4. Payments

Payments là bảng quản lý toàn bộ lịch sử giao dịch.

Một Order có thể có nhiều Payment.

Ví dụ

Order

1001

↓

Payment #1

Failed

↓

Payment #2

Failed

↓

Payment #3

Success

Không được tạo Order mới khi khách thanh toán lại.

---

Payments nên hỗ trợ các trường sau:

- Gateway
- Method
- Amount
- Status
- TransactionId
- GatewayResponseCode
- BankCode
- CreatedAt
- UpdatedAt
- PaidAt
- RefundedAt
- Notes

AI có thể mở rộng nếu cần nhưng không được làm mất khả năng tương thích.

---

# 5. Delivery Slot

Delivery Slot không thuộc Product.

AI không được tạo quan hệ:

Product

↓

DeliverySlot

Delivery Slot phải đại diện cho năng lực giao hàng của cửa hàng.

Ví dụ

09:00 - 11:00

MaxCapacity = 20

CurrentBooked = 12

Orders sẽ tham chiếu:

DeliverySlotId

thay vì lưu chuỗi DeliveryTimeSlot.

---

# 6. Product

Product phải hỗ trợ tối thiểu:

- SKU
- Name
- Description
- Price
- DiscountPrice
- StockQuantity
- ImageUrl
- ViewCount
- IsActive
- CreatedAt
- UpdatedAt

Có thể mở rộng:

- FlowerMeaning
- CareInstruction
- Origin

nếu phù hợp.

---

# 7. Customer

Customer nên hỗ trợ:

- CreatedAt
- UpdatedAt
- LastLogin
- EmailVerified
- PhoneVerified
- IsActive

Ngoài ra giữ nguyên các trường:

- FailedDeliveries
- SuccessfulDeliveries
- FraudScore
- IsBlacklisted
- TotalOrders

để phục vụ đánh giá khách hàng.

---

# 8. Order

Order chỉ quản lý nghiệp vụ đơn hàng.

Không lưu thông tin Payment chi tiết.

Order Status nên bao gồm:

- PendingPayment
- Paid
- PreparingBouquet
- ReadyForDelivery
- Shipping
- Delivered
- Completed
- Cancelled
- Expired
- Refunded

---

# 9. Order Detail

OrderDetail nên lưu:

- Quantity
- UnitPrice
- Discount
- Subtotal

để đảm bảo dữ liệu lịch sử không bị thay đổi khi giá sản phẩm thay đổi.

---

# 10. Review

Hệ thống nên có:

ProductReviews

bao gồm:

- ProductId
- CustomerId
- Rating
- Comment
- CreatedAt

---

# 11. Wishlist

Hệ thống nên hỗ trợ:

Wishlists

để khách hàng lưu sản phẩm yêu thích.

---

# 12. Audit Log

Mọi giao dịch quan trọng phải ghi Log.

Bao gồm:

- Payment
- Refund
- Login
- Order Status
- Cancel Order
- Delivery

---

# 13. Security

AI phải đảm bảo:

Không lưu:

- Password
- JWT
- Refresh Token
- Hash Secret
- API Secret

ở dạng Plain Text trong Database nếu không thực sự cần thiết.

---

# 14. Database Convention

Tất cả bảng nghiệp vụ nên có:

- CreatedAt
- UpdatedAt

Nếu hỗ trợ Soft Delete:

- DeletedAt

---

# 15. Business Rules

AI phải luôn ưu tiên:

1. Chuẩn hóa Database.

2. Không lưu dữ liệu trùng.

3. Không vi phạm Normalization.

4. Không thêm bảng dư thừa.

5. Không phá vỡ kiến trúc hiện tại.

6. Luôn giữ khả năng mở rộng cho:

- VNPay
- MoMo
- ZaloPay
- Stripe
- PayPal

7. Không thay đổi Database nếu chưa phân tích ảnh hưởng tới toàn bộ hệ thống.

---

# AI Checklist

Trước khi thay đổi Database, AI phải:

- Đọc toàn bộ Entity.
- Đọc Migration.
- Đọc DbContext.
- Kiểm tra Foreign Key.
- Kiểm tra Index.
- Kiểm tra Cascade Delete.
- Kiểm tra dữ liệu trùng.
- Kiểm tra khả năng mở rộng.
- Đề xuất Migration an toàn.

Sau khi thay đổi:

- Build Project.
- Chạy Migration.
- Kiểm tra dữ liệu.
- Kiểm tra toàn bộ quan hệ.
- Review lại Database.
- Chỉ kết luận hoàn thành khi xác nhận không phá vỡ dữ liệu hiện có.