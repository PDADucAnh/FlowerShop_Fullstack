Đây chính là **Refund Workflow**. Thực tế, đây là phần mà **90% đồ án TMĐT bị thiếu**.

Với **website bán hoa tươi**, mình **không khuyến khích hoàn tiền 100% trong mọi trường hợp**, vì hoa là mặt hàng dễ hư hỏng và có chi phí chuẩn bị rất lớn.

Mình đề xuất quy trình theo chuẩn nghiệp vụ của các shop hoa chuyên nghiệp.

---

# Refund Workflow - Flower E-commerce

## Trường hợp 1 - Shop hủy đơn

Ví dụ

* Hết hoa
* Nhà cung cấp hết nguyên liệu
* Không giao được do thiên tai
* Hoa bị lỗi
* Không đủ nhân sự

Luồng

```text
Customer

↓

Đặt hàng

↓

Thanh toán VNPay

↓

Payment = Success

↓

Order = Paid

↓

Admin kiểm tra

↓

Không thể thực hiện

↓

Admin chọn

Cancel Order

↓

Nhập lý do

↓

Refund = 100%

↓

Order = CancelledByShop

↓

Payment = RefundPending

↓

Gửi Email

↓

Khách nhận thông báo

↓

Admin thực hiện hoàn tiền

↓

Payment = Refunded

↓

Order = Refunded
```

---

## Email gửi khách

```
Đơn hàng #DH000123 đã được hủy.

Lý do:

Rất tiếc hiện cửa hàng không đủ hoa để thực hiện mẫu bó hoa bạn đã đặt.

Chúng tôi sẽ hoàn lại:

500.000 VNĐ

Thời gian hoàn tiền:

Trong vòng 24 giờ.

Xin lỗi vì sự bất tiện này.
```

---

# Trường hợp 2 - Khách hủy trước khi shop làm hoa

Ví dụ

Khách vừa đặt

↓

Shop chưa cắm

↓

Khách đổi ý

Luồng

```text
Order = Paid

↓

Customer

↓

Cancel Order

↓

Admin xác nhận

↓

Refund = 100%

↓

RefundPending

↓

Refunded
```

Không mất phí.

---

# Trường hợp 3 - Shop đang cắm hoa

Đây là trường hợp đặc biệt.

Ví dụ

Shop đã:

* mua hoa
* cắt hoa
* cắm 70%

Khách hủy.

Không thể bán nguyên giá.

Lúc này nên thu phí.

---

## Chính sách đề xuất

| Tiến độ                 |              Phí |
| ----------------------- | ---------------: |
| Chưa chuẩn bị           |               0% |
| Đã chuẩn bị nguyên liệu |              20% |
| Đang cắm hoa            |              30% |
| Đã hoàn thành           |              50% |
| Đang giao               | Không hỗ trợ hủy |

Đây là mức phí hợp lý để bảo vệ cả khách hàng và cửa hàng.

---

## Luồng

```text
Order = Preparing

↓

Customer

↓

Cancel

↓

Admin xác nhận

↓

Tiến độ = Đang cắm

↓

Cancellation Fee = 30%

↓

Refund = 70%

↓

Payment = PartialRefundPending

↓

Email

↓

Hoàn tiền

↓

PartialRefunded
```

---

## Email

```
Đơn hàng #DH000123 đã được hủy theo yêu cầu.

Đơn hàng đang trong quá trình cắm hoa.

Theo chính sách của cửa hàng:

Chi phí nguyên liệu và nhân công đã phát sinh.

Giá trị đơn hàng:

500.000 VNĐ

Phí hủy:

30%

150.000 VNĐ

Số tiền hoàn:

350.000 VNĐ

Tiền sẽ được hoàn trong vòng 24 giờ.

Xin cảm ơn.
```

---

# Trường hợp 4 - Đang giao

Ví dụ

Shipper đã lấy hoa.

```text
Order

↓

Delivering

↓

Customer

↓

Cancel
```

Không nên cho hủy.

Thông báo

```
Đơn hàng đang được giao.

Bạn không thể hủy ở thời điểm này.

Nếu cần hỗ trợ vui lòng liên hệ cửa hàng.
```

---

# Database cần bổ sung

## Orders

Thêm

```text
CancelledBy

CancellationFee

RefundAmount

RefundReason

RefundRequestedAt

RefundCompletedAt
```

---

## Payments

Thêm

```text
RefundStatus

RefundTransactionId

RefundResponseCode

RefundedBy

RefundNote
```

---

## Refunds (Khuyến nghị)

Mình khuyên tách riêng bảng Refunds.

```text
Refunds

Id

OrderId

PaymentId

RequestedBy

ApprovedBy

Reason

RefundType

RefundPercent

RefundAmount

RefundStatus

GatewayRefundId

ProcessedAt

CreatedAt

UpdatedAt
```

---

# Trạng thái Order

```text
PendingPayment

Paid

Preparing

ReadyForDelivery

Delivering

Completed

CancelledByCustomer

CancelledByShop

RefundPending

Refunded
```

---

# Trạng thái Payment

```text
Pending

Success

Failed

RefundPending

PartialRefundPending

Refunded

PartialRefunded

Expired
```

---

# Luồng tổng thể

```text
Thanh toán thành công

        │

        ▼

Paid

        │

────────┼──────────────────────────

        │

        ▼

Admin hủy

↓

Refund 100%

↓

RefundPending

↓

Refunded

──────────────────────────────

Customer hủy

↓

Chưa làm hoa

↓

Refund 100%

──────────────────────────────

Customer hủy

↓

Đang chuẩn bị

↓

Refund 80%

──────────────────────────────

Customer hủy

↓

Đang cắm

↓

Refund 70%

──────────────────────────────

Customer hủy

↓

Đang giao

↓

Không cho phép
```

# Khuyến nghị cho đồ án

Mình khuyên **không hardcode tỷ lệ hoàn tiền** (ví dụ 30% hay 50%) trong mã nguồn. Thay vào đó, tạo thêm một bảng cấu hình như:

```text
CancellationPolicies
```

Ví dụ:

| OrderStatus      | RefundPercent | CancellationFeePercent |
| ---------------- | ------------: | ---------------------: |
| Pending          |           100 |                      0 |
| Preparing        |            80 |                     20 |
| FlowerArranging  |            70 |                     30 |
| ReadyForDelivery |            50 |                     50 |
| Delivering       |             0 |                    100 |

Khi admin hoặc khách hủy đơn, hệ thống sẽ đọc chính sách từ bảng này để tính tiền hoàn và phí hủy. Cách làm này có nhiều ưu điểm:

* Dễ thay đổi chính sách mà không cần sửa code.
* Admin có thể điều chỉnh mức phí theo quy định của cửa hàng.
* Dễ mở rộng nếu sau này có nhiều loại sản phẩm hoặc chương trình khuyến mãi.
* Thể hiện tư duy thiết kế hệ thống tốt hơn khi bảo vệ đồ án.

Đây là kiến trúc mà mình đánh giá phù hợp hơn cho một đồ án tốt nghiệp vì vừa phản ánh đúng nghiệp vụ của cửa hàng hoa, vừa có khả năng mở rộng giống các hệ thống thương mại điện tử thực tế.
