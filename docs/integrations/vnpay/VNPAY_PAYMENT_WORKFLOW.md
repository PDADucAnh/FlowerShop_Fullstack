# VNPAY Payment Workflow (Flower E-Commerce)

## Mục tiêu

Tích hợp VNPAY vào hệ thống bán hoa tươi theo đúng quy trình nghiệp vụ.

AI phải thiết kế hệ thống đảm bảo:

- Không mất đơn hàng khi thanh toán thất bại.
- Không tạo Order trùng.
- Một Order chỉ được thanh toán thành công một lần.
- Một Order có thể thanh toán lại nhiều lần nếu chưa thành công.
- Không cập nhật Order trước khi xác minh callback.
- Toàn bộ giao dịch phải an toàn và có thể mở rộng.

---

# Quy trình tổng thể

```text
Customer

↓

Shopping Cart

↓

Checkout

↓

Validate Request

↓

Create Order

↓

Order = PendingPayment

↓

Create Payment Record

↓

Payment = Pending

↓

Generate VNPay Payment URL

↓

Redirect VNPay
```

---

# Trước khi tạo Payment

AI phải kiểm tra:

- Người dùng đã đăng nhập (nếu yêu cầu)
- Giỏ hàng không rỗng
- Sản phẩm còn tồn tại
- Sản phẩm đang hoạt động
- Giá sản phẩm hợp lệ
- Voucher hợp lệ
- Địa chỉ giao hàng hợp lệ
- Phương thức giao hàng hợp lệ
- Tổng tiền được tính từ Database
- Không sử dụng dữ liệu giá từ Frontend

Nếu bất kỳ bước nào thất bại:

Không tạo Payment.

---

# Khi khách đang thanh toán

Order:

PendingPayment

Payment:

Pending

Không được:

- Trừ tồn kho.
- Gửi Email xác nhận.
- Bắt đầu chuẩn bị hoa.
- Tạo hóa đơn.

---

# Callback từ VNPAY

Workflow

```text
VNPay Callback

↓

Validate Signature

↓

Validate Merchant

↓

Validate Transaction

↓

Validate ResponseCode

↓

Validate Order

↓

Validate Amount

↓

Check Duplicate Callback

↓

Begin Database Transaction
```

---

# Thanh toán thành công

Điều kiện:

ResponseCode = 00

Sau khi xác minh thành công:

```text
Update Payment

↓

Payment = Success

↓

Update Order

↓

Order = Paid

↓

Giảm tồn kho

↓

Lưu Audit Log

↓

Commit Transaction

↓

Gửi Email xác nhận

↓

Thông báo thành công

↓

Chuyển Order sang:

PreparingBouquet
```

---

# Thanh toán thất bại

Bao gồm:

- User Cancel
- Sai OTP
- Không đủ tiền
- Timeout
- ResponseCode khác 00

Workflow

```text
Update Payment

↓

Payment = Failed

↓

Order vẫn giữ

Status = PendingPayment

↓

Không giảm tồn kho

↓

Không gửi Email xác nhận

↓

Hiển thị nút

Thanh toán lại
```

---

# Thanh toán lại

Nếu Order vẫn:

PendingPayment

AI phải:

```text
Kiểm tra Order

↓

Kiểm tra chưa Paid

↓

Tạo Payment Record mới

↓

Payment = Pending

↓

Generate Payment URL mới

↓

Redirect VNPay
```

Không được:

- Tạo Order mới.
- Xóa Order cũ.

---

# Một Order có nhiều Payment

Ví dụ

Order

OrderId = 1001

Payment

Payment #1

Failed

Payment #2

Failed

Payment #3

Success

Order

Paid

Đây là thiết kế bắt buộc.

---

# Sau khi thanh toán thành công

Order sẽ đi theo quy trình:

```text
Paid

↓

PreparingBouquet

↓

ReadyForDelivery

↓

Shipping

↓

Delivered

↓

Completed
```

---

# Order Status

Hệ thống phải hỗ trợ tối thiểu:

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

# Payment Status

Hệ thống phải hỗ trợ:

- Pending
- Success
- Failed
- Cancelled
- Expired
- Refunded

---

# Order Expired

Nếu Order:

PendingPayment

quá thời gian cấu hình

Ví dụ:

30 phút

AI phải:

```text
Order

↓

Expired

↓

Payment

↓

Expired

↓

Cho phép người dùng

Đặt hàng lại
```

Không tự động thanh toán lại.

---

# Callback Rules

AI phải đảm bảo:

- Verify Signature.
- Verify Merchant.
- Verify Amount.
- Verify Order.
- Verify Transaction.
- Verify Duplicate Callback.

Nếu bất kỳ bước nào thất bại:

Không cập nhật Order.

Không cập nhật Payment thành Success.

Ghi Log.

---

# Idempotency

Callback có thể được gọi nhiều lần.

Nếu Order đã Paid:

Không cập nhật lần nữa.

Không giảm tồn kho lần nữa.

Không gửi Email lần nữa.

Không tạo Notification lần nữa.

Chỉ ghi Log.

---

# Database Transaction

Các bước sau phải nằm trong cùng Transaction:

- Update Payment
- Update Order
- Update Inventory
- Save Audit Log

Nếu lỗi:

Rollback toàn bộ.

---

# Logging

Phải ghi:

- Create Payment
- Redirect VNPay
- Callback
- Verify Signature
- Verify Amount
- Verify Order
- Payment Success
- Payment Failed
- Retry Payment
- Exception

Không ghi:

- HashSecret
- AccessToken
- JWT
- Password
- Sensitive Data

---

# Security

AI phải tuân thủ:

- Không tin dữ liệu từ Frontend.
- Không tin Amount từ Client.
- Luôn lấy Amount từ Database.
- Luôn Verify Signature.
- Luôn Verify Merchant.
- Không Hardcode Secret.
- Secret phải lưu trong cấu hình hoặc Secret Manager.

---

# Frontend

Checkout

↓

Redirect VNPay

↓

Nếu Success

↓

Thông báo

Thanh toán thành công

↓

Đi tới Chi tiết đơn hàng

---

Nếu Failed

↓

Thông báo

Thanh toán không thành công

↓

Hiển thị lý do (nếu có)

↓

Hiển thị nút

Thanh toán lại

---

Nếu Expired

↓

Thông báo

Đơn hàng đã hết hạn

↓

Quay lại Checkout

---

# AI Checklist

Sau khi hoàn thành tích hợp:

- Build Backend.
- Build Frontend.
- Kiểm tra toàn bộ workflow.
- Kiểm tra callback.
- Kiểm tra Retry Payment.
- Kiểm tra Duplicate Callback.
- Kiểm tra Invalid Signature.
- Kiểm tra Rollback Transaction.
- Kiểm tra Logging.
- Tự review code.
- Chỉ kết luận hoàn thành khi toàn bộ workflow hoạt động đúng.