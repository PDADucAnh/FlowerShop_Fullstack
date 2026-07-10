# ADMIN_DASHBOARD_WORKFLOW.md

# Dashboard Admin Workflow

## Mục tiêu

Dashboard là trang đầu tiên sau khi Admin đăng nhập thành công.

Dashboard giúp người quản trị theo dõi toàn bộ hoạt động của cửa hàng theo thời gian thực mà không cần truy cập từng module riêng lẻ.

Dashboard chỉ dành cho tài khoản có quyền Admin.

---

# Luồng hoạt động

## Bước 1. Admin đăng nhập

Admin nhập:

- Username
- Password

↓

Backend xác thực JWT Authentication.

↓

Nếu thành công

↓

Sinh JWT Token

↓

Chuyển đến Dashboard.

Nếu thất bại

↓

Hiển thị thông báo đăng nhập không thành công.

---

# Bước 2. Dashboard khởi tạo

Sau khi Dashboard mở

Frontend gọi đồng thời nhiều API.

Ví dụ

GET

/api/dashboard/summary

/api/dashboard/orders

/api/dashboard/revenue

/api/dashboard/products

/api/dashboard/customers

/api/dashboard/notifications

/api/dashboard/charts

---

# Bước 3. Backend tổng hợp dữ liệu

Dashboard KHÔNG đọc trực tiếp từ một bảng.

Dashboard lấy dữ liệu từ nhiều bảng:

Customers

Orders

OrderDetails

Products

Payments

Advertisements

Posts

DeliverySlots

ProductReviews

Wishlists

AuditLogs

...

Sau đó tổng hợp thành DashboardDTO.

---

# Dashboard hiển thị

## Tổng doanh thu

Hiển thị

- Doanh thu hôm nay

- Doanh thu tuần

- Doanh thu tháng

- Doanh thu năm

Chỉ tính

Orders

Status = Completed

Payments

Status = Paid

Không tính

Cancelled

Refunded

Pending Payment

Failed Payment

---

## Tổng số đơn hàng

Hiển thị

- Đơn mới

- Chờ xác nhận

- Đang chuẩn bị

- Đang cắm hoa

- Chờ giao

- Đang giao

- Hoàn thành

- Hủy

---

## Tổng khách hàng

Hiển thị

- Tổng khách hàng

- Khách mới

- Khách hoạt động

- Khách bị khóa

---

## Tổng sản phẩm

Hiển thị

- Tổng sản phẩm

- Đang bán

- Hết hàng

- Ngừng kinh doanh

---

## Thanh toán

Hiển thị

- Thanh toán VNPAY

- Chuyển khoản

- Tiền mặt

- Chờ thanh toán

- Thanh toán thất bại

- Hoàn tiền

---

## Tình trạng kho

Hiển thị

- Hoa còn nhiều

- Hoa sắp hết

- Hoa hết hàng

Có cảnh báo màu đỏ.

---

## Khuyến mãi đang chạy

Hiển thị

- Số chương trình

- Thời gian còn lại

- Sản phẩm áp dụng

---

## Đánh giá khách hàng

Hiển thị

- Rating trung bình

- Tổng lượt đánh giá

- Đánh giá mới nhất

---

## Banner

Hiển thị

- Banner đang hoạt động

- Banner hết hạn

---

# Biểu đồ

Dashboard hiển thị nhiều biểu đồ.

---

## Biểu đồ doanh thu

Theo

Ngày

↓

Tuần

↓

Tháng

↓

Năm

Nguồn

Orders

Payments

---

## Biểu đồ đơn hàng

Hiển thị số lượng đơn

Pending

Preparing

Delivering

Completed

Cancelled

---

## Biểu đồ phương thức thanh toán

Ví dụ

VNPAY

Transfer

Cash

---

## Biểu đồ doanh thu theo danh mục

Ví dụ

Hoa sinh nhật

Hoa khai trương

Hoa cưới

Hoa chia buồn

Hoa tình yêu

---

## Top sản phẩm bán chạy

Top 10

Theo

Số lượng bán

Hoặc

Doanh thu

---

## Khách hàng thân thiết

Top khách hàng

Theo

- Tổng đơn

- Tổng tiền

- Điểm tích lũy

---

# Thông báo

Dashboard hiển thị Notification.

Ví dụ

Đơn mới

Khách vừa thanh toán

Hoa sắp hết

Khuyến mãi sắp hết hạn

Khách vừa đánh giá

Yêu cầu hoàn tiền

---

# Công việc cần làm

Dashboard có shortcut.

Ví dụ

Quản lý đơn hàng

↓

Đi đến

/admin/orders

---

Quản lý sản phẩm

↓

/admin/products

---

Quản lý khách hàng

↓

/admin/customers

---

Quản lý thanh toán

↓

/admin/payments

---

Quản lý khuyến mãi

↓

/admin/promotions

---

Quản lý bài viết

↓

/admin/posts

---

Quản lý Banner

↓

/admin/advertisements

---

# Refresh dữ liệu

Dashboard

Auto Refresh

Có thể

30 giây

Hoặc

60 giây

Hoặc

Refresh thủ công.

---

# Phân quyền

Dashboard chỉ cho phép

Role

Admin

Nếu

Customer

↓

403 Forbidden

---

# Logging

Mỗi lần Admin truy cập Dashboard

AuditLogs

Lưu

UserId

Action

VIEW_DASHBOARD

IP

Browser

Login Time

CreatedAt

---

# API

GET

/api/dashboard/summary

Tổng hợp dữ liệu.

---

GET

/api/dashboard/revenue

Doanh thu.

---

GET

/api/dashboard/orders

Thống kê đơn hàng.

---

GET

/api/dashboard/products

Thống kê sản phẩm.

---

GET

/api/dashboard/customers

Thống kê khách hàng.

---

GET

/api/dashboard/charts

Biểu đồ.

---

GET

/api/dashboard/notifications

Thông báo.

---

# Quy tắc nghiệp vụ

- Chỉ tính doanh thu từ các đơn hàng đã hoàn thành và đã thanh toán thành công.
- Đơn hàng bị hủy hoặc hoàn tiền không được tính vào doanh thu.
- Cảnh báo khi số lượng tồn kho của sản phẩm thấp hơn ngưỡng cấu hình.
- Thống kê được cập nhật theo thời gian thực hoặc sau mỗi lần làm mới Dashboard.
- Dashboard phải tải nhanh bằng cách sử dụng các truy vấn tổng hợp và tối ưu hóa dữ liệu.
- Tất cả API Dashboard đều yêu cầu JWT hợp lệ và quyền Admin.