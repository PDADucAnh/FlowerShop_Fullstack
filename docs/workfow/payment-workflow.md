Đúng. Đây là **một thiếu sót khá lớn** trong workflow hiện tại, và khi bảo vệ đồ án giảng viên thường sẽ hỏi:

> "Nếu khách hàng đóng tab VNPAY thì sao?"
>
> "Nếu khách thanh toán thất bại thì có tạo lại đơn không?"
>
> "Nếu khách muốn thanh toán lại thì xử lý như thế nào?"

Nếu không có luồng này thì hệ thống chưa hoàn chỉnh.

---

# Kiến trúc chuẩn của Website bán hoa

## Trạng thái Order

Mình khuyên dùng enum như sau:

```text
PendingPayment        Chờ thanh toán

Paid                  Đã thanh toán

Processing            Đang chuẩn bị hoa

ReadyForDelivery      Đã chuẩn bị xong

Delivering            Đang giao

Completed             Hoàn thành

Cancelled             Đã hủy

Refunded              Đã hoàn tiền
```

---

## Trạng thái Payment

```text
Pending

Success

Failed

Expired

Cancelled

Refunded
```

---

# Luồng 1 — Khách tạo đơn

```
Checkout

↓

Tạo Order

↓

Status = PendingPayment

↓

Tạo Payment

↓

Status = Pending

↓

Redirect VNPay
```

**Lưu ý:**

Order được tạo **trước khi redirect**.

Đây là chuẩn của mọi hệ thống TMĐT.

---

# Luồng 2 — Thanh toán thành công

```
VNPay Callback

↓

Validate Signature

↓

ResponseCode = 00

↓

Payment = Success

↓

Order = Paid

↓

Giảm tồn kho

↓

Gửi Email

↓

Thông báo thành công
```

---

# Luồng 3 — Khách bấm Back

```
Checkout

↓

Redirect VNPay

↓

Khách bấm Back

↓

Không callback

↓

Payment vẫn Pending

↓

Order vẫn PendingPayment
```

Không xóa Order.

---

# Luồng 4 — Khách đóng trình duyệt

```
Checkout

↓

Redirect VNPay

↓

Đóng Browser

↓

Không callback

↓

Payment Pending

↓

Order PendingPayment
```

---

# Luồng 5 — Thanh toán thất bại

```
VNPay Callback

↓

Response != 00

↓

Payment = Failed

↓

Order = PendingPayment
```

Quan trọng:

Không chuyển Order thành Cancelled ngay.

---

# Luồng 6 — Khách hết thời gian thanh toán

Ví dụ sau

30 phút

Scheduler chạy

↓

```
PendingPayment

↓

quá hạn

↓

Payment = Expired

↓

Order = Cancelled
```

Đồng thời

```
Trả lại Stock
```

---

# Luồng 7 — Thanh toán lại (Thiếu trong workflow của bạn)

Đây là luồng cực kỳ quan trọng.

```
Order History

↓

Đơn hàng

↓

PendingPayment

↓

Hiện nút

"Thanh toán lại"

↓

Click

↓

Sinh Payment mới

↓

Redirect VNPay

↓

Thanh toán

↓

Success

↓

Order = Paid
```

Không tạo Order mới.

Chỉ tạo Payment mới.

---

# Database

## Orders

```
Id

Status

...
```

---

## Payments

Một Order có thể có nhiều Payment.

Ví dụ

Order

```
1001
```

Payments

| Id | OrderId | Status  |
| -- | ------- | ------- |
| 1  | 1001    | Failed  |
| 2  | 1001    | Expired |
| 3  | 1001    | Success |

Đây là thiết kế đúng.

Không cập nhật đè Payment cũ.

---

# Luồng Retry Payment

```
Customer

↓

Order History

↓

PendingPayment

↓

Retry Payment

↓

Create Payment Record

↓

Status = Pending

↓

Redirect VNPay

↓

Success

↓

Update Payment

↓

Update Order

↓

Done
```

---

# UI cần bổ sung

Trong trang

```
My Orders
```

Nếu

```
Order.Status = PendingPayment
```

Hiện

```
[ Thanh toán ngay ]
```

Nếu

```
Payment Failed
```

Hiện

```
Thanh toán thất bại

[ Thanh toán lại ]
```

Nếu

```
Expired
```

Hiện

```
Hết hạn thanh toán

[ Thanh toán lại ]
```

---

# Sau khi quá hạn

Ví dụ

```
Sau 24 giờ
```

Cron Job

↓

```
PendingPayment

↓

Cancelled
```

↓

```
Stock++
```

↓

```
Email khách
```

---

# Mình còn khuyên bổ sung một bảng mới

Để đồ án đạt chất lượng cao hơn, hãy thêm:

## PaymentAttempts

```
Id

PaymentId

AttemptNumber

GatewayRequest

GatewayResponse

IpAddress

UserAgent

CreatedAt
```

Quan hệ:

```
Order

    │

    ▼

Payments

    │

    ▼

PaymentAttempts
```

Điều này giúp:

* Lưu lịch sử từng lần khách bấm "Thanh toán lại".
* Phục vụ kiểm tra lỗi, audit và chống gian lận.
* Không ghi đè thông tin giao dịch cũ.

---

# Mình đề xuất bổ sung tài liệu

Bạn nên tạo thêm file:

```
docs/integrations/vnpay/

VNPAY_PAYMENT_LIFECYCLE.md
```

Nội dung nên bao gồm đầy đủ:

* Luồng tạo Order.
* Luồng tạo Payment.
* Callback thành công.
* Callback thất bại.
* Người dùng đóng trình duyệt.
* Người dùng hủy thanh toán.
* Thanh toán lại (Retry Payment).
* Đơn hàng hết hạn thanh toán.
* Khôi phục tồn kho.
* Gửi Email.
* Quy tắc không tạo Order mới khi thanh toán lại.
* Quy tắc một Order có nhiều Payment.
* Sequence Diagram.
* Activity Diagram.
* Business Rules.

Đây sẽ là tài liệu để AI CLI luôn tuân thủ khi triển khai VNPAY và giúp hệ thống xử lý đầy đủ các tình huống thực tế mà một website bán hoa thương mại điện tử cần có.
