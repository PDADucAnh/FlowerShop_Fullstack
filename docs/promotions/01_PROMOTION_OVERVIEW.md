# Promotion Module Overview

## Mục tiêu

Triển khai module Promotion hoàn chỉnh cho Website bán hoa tươi.

Công nghệ sử dụng

- Frontend: Next.js
- Backend: ASP.NET Core Web API
- Database: SQL Server
- ORM: Entity Framework Core

Module Promotion phải hoạt động độc lập với các module hiện tại.

Không sửa đổi hoặc phá vỡ các chức năng đang hoạt động.

Chỉ bổ sung các thành phần mới.

---

## Các chức năng cần có

Website phải hỗ trợ hai hình thức khuyến mãi:

### 1. Flash Sale

Admin tạo chương trình giảm giá theo:

- Dịp lễ
- Theo mùa
- Theo sản phẩm
- Theo danh mục hoa

Ví dụ:

- Valentine
- 8/3
- 20/10
- Noel
- Tết

Flash Sale phải tự động kích hoạt và tự động kết thúc theo thời gian.

---

### 2. Voucher

Khách nhập mã giảm giá khi thanh toán.

Ví dụ:

FLOWER50

LOVE10

WELCOME100

Voucher có thể:

- giảm %
- giảm số tiền

Voucher có thể giới hạn:

- thời gian
- số lượt
- số lần mỗi khách
- giá trị đơn tối thiểu

---

## Nguyên tắc

Promotion phải được tính tại Backend.

Frontend chỉ hiển thị kết quả.

Không tính giá ở Frontend.

---

## Kiến trúc

Promotion bao gồm:

- Flash Sale
- Coupon
- Promotion Dashboard
- Promotion API
- Promotion Service
- Promotion Database

Không sửa các module hiện có.

Chỉ mở rộng hệ thống.