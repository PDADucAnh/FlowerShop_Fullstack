# Kiến trúc & Workflow Chi tiết - AnhCMS_Solution

> **Phiên bản:** 1.0 | **Cập nhật:** 07/2026  
> **Mô tả:** Tài liệu kiến trúc tổng thể, sơ đồ luồng dữ liệu, và workflow chi tiết từng chức năng của hệ thống CMS Thương mại điện tử PDA FLOWER.

---

## Mục lục

1. [Tổng quan kiến trúc](#1-tổng-quan-kiến-trúc)
2. [Sơ đồ kiến trúc hệ thống](#2-sơ-đồ-kiến-trúc-hệ-thống)
3. [Database Schema](#3-database-schema)
4. [Authentication & Authorization](#4-authentication--authorization)
5. [Workflow: Mua hàng (Checkout)](#5-workflow-mua-hàng-checkout)
6. [Workflow: Quản lý Đơn hàng](#6-workflow-quản-lý-đơn-hàng)
7. [Workflow: Hủy đơn hàng](#7-workflow-hủy-đơn-hàng)
8. [Workflow: Giỏ hàng & Yêu thích](#8-workflow-giỏ-hàng--yêu-thích)
9. [Workflow: Blog & Tin tức](#9-workflow-blog--tin-tức)
10. [Workflow: Tìm kiếm & Lọc sản phẩm](#10-workflow-tìm-kiếm--lọc-sản-phẩm)
11. [Workflow: Xác thực & Bảo mật](#11-workflow-xác-thực--bảo-mật)
12. [SignalR Real-time Notifications](#12-signalr-real-time-notifications)
13. [Fraud Detection & Stock Locking](#13-fraud-detection--stock-locking)
14. [Admin Panel](#14-admin-panel)
15. [Danh sách API Endpoints](#15-danh-sách-api-endpoints)

---

## 1. Tổng quan kiến trúc

Hệ thống **AnhCMS_Solution** là một nền tảng Thương mại điện tử kết hợp Hệ thống Quản trị Nội dung (CMS) dành cho cửa hàng hoa tươi **PDA FLOWER**, được xây dựng theo mô hình **Hybrid Architecture**:

```
┌─────────────────────────────────────────────────────────┐
│                    NGƯỜI DÙNG                          │
│            (Trình duyệt Web / Postman)                  │
└────────────────────────┬────────────────────────────────┘
                         │
          ┌──────────────┴──────────────┐
          ▼                             ▼
┌──────────────────┐      ┌──────────────────────────┐
│  React SPA       │      │  Admin MVC (Razor Views) │
│  localhost:3000   │      │  localhost:5064/Admin    │
│  (Khách hàng)    │      │  (Quản trị viên)         │
└────────┬─────────┘      └───────────┬──────────────┘
         │ REST API (JSON)            │ Cookie Auth
         ▼                            ▼
┌────────────────────────────────────────────────────────┐
│              CMS.Backend (ASP.NET Core 8.0)            │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────┐ │
│  │ API Ctrl    │  │ MVC Ctrl     │  │ SignalR Hub   │ │
│  │ /api/*      │  │ /Admin/*     │  │ /hubs/notify  │ │
│  └──────┬──────┘  └──────┬───────┘  └──────┬────────┘ │
│         │                │                  │         │
│  ┌──────┴────────────────┴──────────────────┴──────┐ │
│  │              Services Layer                      │ │
│  │  Product, Order, Auth, Customer, Category, Post,…│ │
│  └──────┬──────────────────────────────────────────┘ │
│         │                                            │
│  ┌──────┴──────────────────────────────────────────┐ │
│  │         CMS.Data (EF Core DbContext)             │ │
│  └──────┬──────────────────────────────────────────┘ │
└─────────┼────────────────────────────────────────────┘
          │
          ▼
┌──────────────────┐
│  SQL Server      │
│  LocalDB         │
│  AnhCMS_DB       │
└──────────────────┘
```

### Đặc điểm kiến trúc

| Đặc điểm | Mô tả |
|----------|-------|
| **Hybrid** | Backend vừa là MVC Server (Admin Panel), vừa là REST API Server (React SPA) |
| **Decoupled** | Frontend React độc lập, giao tiếp qua REST API thuần JSON |
| **Code-First** | Database được sinh tự động từ Entity Classes qua EF Core Migration |
| **Real-time** | SignalR cho thông báo CRUD đến tất cả client đang kết nối |
| **InMemory Cache** | Stock Lock tạm thời khi checkout, tự động hết hạn sau 15 phút |

---

## 2. Sơ đồ kiến trúc hệ thống

### 2.1 Layers & Dependencies

```
┌─────────────────────────────────────────────────────┐
│                   CMS.Tests                         │
│           (xUnit + InMemory Database)               │
├─────────────────────────────────────────────────────┤
│                  cms.frontend                       │
│  React 19 + TypeScript + Tailwind + TanStack Query │
│  ┌──────────┬──────────┬──────────┬──────────────┐ │
│  │ Pages    │ Context  │ Hooks    │ Services     │ │
│  │ (17)     │ (3)      │ (6)      │ (4)          │ │
│  └──────────┴──────────┴──────────┴──────────────┘ │
├─────────────────────────────────────────────────────┤
│                  CMS.Backend                        │
│  ┌───────────────────────────────────────────────┐ │
│  │ Controllers (MVC: 17 + API: 12)              │ │
│  ├───────────────────────────────────────────────┤ │
│  │ Services (14 interfaces + implementations)   │ │
│  ├───────────────────────────────────────────────┤ │
│  │ Middleware / Hubs / Utils                     │ │
│  └───────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│                  CMS.Data                          │
│  ┌───────────────────────────────────────────────┐ │
│  │ ApplicationDbContext + 13 Entity classes      │ │
│  │ + 5 Migrations                                │ │
│  └───────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

### 2.2 Service Dependencies

```
┌──────────────────────────────────────────────────────┐
│ Program.cs (DI Container)                            │
│                                                      │
│ IAuthService ─────────────┐                          │
│ IUserService ─────────────┤                          │
│ IProductService ──────────┤───► ApplicationDbContext │
│ ICategoryService ─────────┤       (Scoped)           │
│ ICategoryProductService ──┤                          │
│ IPostService ─────────────┤                          │
│ ICustomerService ─────────┤                          │
│ IOrderService ────────────┤                          │
│ IOrderDetailService ──────┤                          │
│ IAdvertisementService ────┤                          │
│ IDeliverySlotService ─────┤                          │
│ IPaymentService ──────────┤                          │
│ IFraudDetectionService ───┤                          │
│ IEmailService ────────────┤                          │
│ INotificationService ─────┤                          │
│ StockLockService ─────────┤(MemoryCache, Singleton)  │
│ OrderExpiryBgService ─────┤(HostedService, Singleton)│
│                         (Scoped)                     │
└──────────────────────────────────────────────────────┘
```

---

## 3. Database Schema

### 3.1 Mô hình quan hệ thực thể (ER Diagram)

```
┌──────────────┐     ┌────────────────┐
│   Users      │     │   Categories   │
│──────────────│     │────────────────│
│ PK Id (int)  │     │ PK Id (int)    │
│ Username     │     │ Name           │
│ PasswordHash │     │ Description    │
│ FullName     │     │ Slug           │
│ Email        │     │ ParentId (FK)──┼──┐
│ Phone        │     └────────┬───────┘  │
│ Role         │              │          │
│ IsActive     │              │          │
│ CreatedAt    │              ▼          │
└──────────────┘     ┌────────────────┐ │
                     │    Posts       │ │
                     │────────────────│ │
                     │ PK Id (int)    │ │
                     │ Title          │ │
                     │ Content (HTML) │ │
                     │ ImageUrl       │ │
                     │ Slug           │ │
                     │ CategoryId(FK)─┼─┘
                     │ CreatedDate    │
                     │ Author         │
                     │ IsPublished    │
                     └────────────────┘

┌──────────────────┐
│ CategoriesProduct │
│──────────────────│
│ PK Id (int)      │
│ Name             │
│ Description      │
│ Slug             │
└────────┬─────────┘
         │ 1
         │
         │ *
         ▼
┌──────────────────┐     ┌──────────────────┐
│    Products      │     │   DeliverySlots  │
│──────────────────│     │──────────────────│
│ PK Id (int)     │──┐  │ PK Id (int)      │
│ Sku (unique)    │  │  │ ProductId (FK)──┼─┘
│ Name             │  │  │ DayOfWeek        │
│ Description      │  │  │ StartTime        │
│ Slug             │  │  │ EndTime          │
│ Price (decimal)  │  │  │ Capacity         │
│ DiscountPrice    │  │  └──────────────────┘
│ StockQuantity    │  │
│ ImageUrl         │  │
│ CategoryProductId│  │
│ FK)             │  │
└──────────────────┘  │
         │ *          │ *
         │            │
         ▼            │
┌──────────────────┐  │
│   OrderDetails   │  │
│──────────────────│  │
│ PK Id (int)      │  │
│ OrderId (FK) ────┼──┼───┐
│ ProductId (FK)───┼──┘   │
│ Quantity          │      │
│ UnitPrice         │      │
└──────────────────┘      │
         │ *              │
         │                │
         ▼ 1              │
┌──────────────────┐      │
│     Orders       │      │
│──────────────────│      │
│ PK Id (int)     │      │
│ OrderDate         │      │
│ CustomerId (FK)──────┐  │
│ Status (enum)       │  │  │
│ Notes               │  │  │
│ PaymentMethod       │  │  │
│ PaymentStatus       │  │  │
│ PaymentTransactionId│  │  │
│ DeliveryDate        │  │  │
│ DeliveryTimeSlot    │  │  │
│ DeliveryDistrict    │  │  │
│ DeliveryAddress     │  │  │
│ CancelledAt         │  │  │
│ CancellationReason  │  │  │
│ IsVerified          │  │  │
│ VerifiedAt          │  │  │
│ RefundAmount        │  │  │
│ TargetFinishedTime  │  │  │
└──────────────────┘     │
         │ *              │
         │ 1              │
         ▼                │
┌──────────────────┐      │
│   Customers      │      │
│──────────────────│      │
│ PK Id (int)     │      │
│ FullName          │      │
│ Email (unique)   │      │
│ Phone             │      │
│ Address           │      │
│ PasswordHash      │      │
│ ResetToken        │      │
│ ResetTokenExpiry  │      │
│ CreatedAt         │      │
└──────────────────┘      │

┌──────────────────┐      │
│  RefreshTokens   │      │
│──────────────────│      │
│ PK Id (int)      │      │
│ Token             │      │
│ JwtId             │      │
│ IsUsed            │      │
│ IsRevoked         │      │
│ AddedDate         │      │
│ ExpiryDate        │      │
│ CustomerId (FK)───┼──────┘
│ UserId (FK)       │
└──────────────────┘

┌──────────────────┐
│  Advertisements  │
│──────────────────│
│ PK Id (int)      │
│ Title             │
│ ImageUrl          │
│ LinkUrl           │
│ Position          │
│ IsActive          │
│ SortOrder         │
│ StartDate         │
│ EndDate           │
└──────────────────┘

┌──────────────────┐     ┌──────────────────┐
│   Payments       │     │  PhoneBlacklists │
│──────────────────│     │──────────────────│
│ PK Id (int)      │     │ PK Id (int)      │
│ OrderId (FK)     │     │ Phone             │
│ Amount            │     │ Reason            │
│ PaymentMethod     │     │ CreatedAt         │
│ Status (enum)    │     └──────────────────┘
│ TransactionId     │
│ PaidAt            │
│ CreatedAt         │
└──────────────────┘
```

### 3.2 Danh sách 13 Entities

| Entity | DbSet | Mô tả | Quan hệ |
|--------|-------|-------|---------|
| `User` | `Users` | Quản trị viên Admin/Editor | - |
| `Category` | `Categories` | Danh mục Blog | 1-N với Post |
| `Post` | `Posts` | Bài viết Blog | N-1 với Category |
| `CategoryProduct` | `CategoriesProducts` | Danh mục Sản phẩm | 1-N với Product |
| `Product` | `Products` | Sản phẩm hoa | N-1 với CategoryProduct, 1-N với OrderDetail |
| `Customer` | `Customers` | Khách hàng | 1-N với Order |
| `Order` | `Orders` | Đơn hàng | N-1 với Customer, 1-N với OrderDetail |
| `OrderDetail` | `OrderDetails` | Chi tiết đơn hàng | N-1 với Order & Product |
| `DeliverySlot` | `DeliverySlots` | Khung giờ giao hàng | N-1 với Product |
| `Advertisement` | `Advertisements` | Banner quảng cáo | - |
| `RefreshToken` | `RefreshTokens` | JWT Refresh Token | N-1 với Customer/User |
| `Payment` | `Payments` | Giao dịch thanh toán | N-1 với Order |
| `PhoneBlacklist` | `PhoneBlacklists` | SĐT bị cấm (fraud) | - |

### 3.3 Order Status Enum

```
Pending (0) ──► PendingVerification (4) ──► Confirmed (5) ──► Preparing (6) ──► Shipping (1) ──► Completed (2)
                                 │                                                        │
                                 └──► Cancelled (3) (nếu không xác minh kịp)              │
                                                                                          │
      Bất kỳ trạng thái nào ngoại trừ Completed ──► Cancelled (3)                        │
                                                                                          └──► Cancelled (3)
```

---

## 4. Authentication & Authorization

### 4.1 Cơ chế xác thực kép (Dual Authentication)

```
                    REQUEST
                       │
                       ▼
            ┌─────────────────────┐
            │ AnhCMS.Auth Policy  │
            │ (PolicyScheme)      │
            └────────┬────────────┘
                     │
          ┌──────────┴──────────┐
          ▼                     ▼
   ┌──────────────┐    ┌────────────────┐
   │ /api/*       │    │ /Admin/*       │
   │ Path starts  │    │ Path starts    │
   │ with /api    │    │ NOT /api       │
   └──────┬───────┘    └───────┬────────┘
          ▼                     ▼
   ┌──────────────┐    ┌────────────────┐
   │ JWT Bearer   │    │ Cookie Auth    │
   │ (API client) │    │ (ASP.NET MVC)  │
   └──────┬───────┘    └───────┬────────┘
          │                    │
          ▼                    ▼
   ┌──────────────┐    ┌────────────────┐
   │ Validate:    │    │ Validate:      │
   │ - Issuer    │    │ - Cookie       │
   │ - Audience  │    │ - Session      │
   │ - Lifetime  │    │ - Role claim   │
   │ - Signature │    └────────────────┘
   └──────────────┘
```

### 4.2 JWT Token Flow (API)

```
Khách hàng                    Frontend React                  Backend API
    │                             │                              │
    │  Đăng nhập                  │  POST /api/auth/login        │
    ├────────────────────────────►├─────────────────────────────►│
    │                             │                              ├── Validate credentials
    │                             │                              ├── Generate Access Token (1h)
    │                             │                              ├── Generate Refresh Token (7d)
    │                             │  { accessToken, refreshToken }│
    │                             │◄─────────────────────────────┤
    │                             │                              │
    │  Lưu token vào              │                              │
    │  localStorage               │                              │
    │◄────────────────────────────┤                              │
    │                             │                              │
    │  Gọi API                   │  GET /api/products           │
    │                             ├─────────────────────────────►│
    │                             │  Authorization: Bearer xxx   │
    │                             │                              ├── Validate JWT
    │                             │                              ├── Trả dữ liệu
    │                             │◄─────────────────────────────┤
```

### 4.3 Authorization Policies

```csharp
// Program.cs
options.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin", "Administrator"));

options.AddPolicy("StaffOnly", policy =>
    policy.RequireRole("Admin", "Administrator", "Editor"));
```

### 4.4 Session Validation Middleware

Middleware `SessionValidationMiddleware` chạy sau `UseAuthentication()`, kiểm tra:
- RefreshToken còn hiệu lực không
- Login có bị expired không
- Nếu token hết hạn → trả về 401, frontend tự redirect đến login

---

## 5. Workflow: Mua hàng (Checkout)

```
Sơ đồ luồng mua hàng hoàn chỉnh:

┌──────────┐    ┌─────────┐    ┌──────────┐    ┌──────────┐    ┌────────────┐    ┌───────────┐
│  Duyệt   │    │  Xem    │    │  Giỏ     │    │ Thanh    │    │  Xác nhận  │    │  Hoàn     │
│  Sản     │──► │  Chi    │──► │  Hàng    │──► │ Toán     │──► │  Đơn hàng  │──► │  Tất      │
│  phẩm    │    │  Tiết   │    │          │    │          │    │            │    │           │
└──────────┘    └─────────┘    └──────────┘    └──────────┘    └────────────┘    └───────────┘
                                                                                        │
                                                                                        ▼
                                                                                 ┌──────────────┐
                                                                                 │  Giao hàng   │
                                                                                 │  & Nhận      │
                                                                                 └──────────────┘
```

### 5.1 Chi tiết từng bước

**Bước 1 - Thêm vào giỏ hàng:**
```
User click "Thêm vào giỏ"
    │
    ▼
CartContext.addToCart(product, quantity)
    │
    ├── Kiểm tra stock tồn kho (stockQuantity)
    ├── Nếu quantity > stock → toast cảnh báo + cap quantity
    ├── Nếu đã có trong giỏ → tăng quantity
    ├── Nếu chưa có → thêm mới CartItem
    │
    ▼
localStorage.setItem('cart', JSON.stringify(cartItems))
```

**Bước 2 - Checkout:**
```
User click "Thanh toán" tại /cart
    │
    ▼
/checkout page
    │
    ├── Form chia làm 4 section:
    │   1. Thông tin người mua (fullName, email, phone)
    │   2. Thông tin người nhận (recipientName, recipientPhone, note)
    │   3. Thời gian giao hàng (deliveryDate, deliveryTimeSlot)
    │   4. Phương thức thanh toán (COD / OnlinePayment)
    │
    ├── Phone blacklist check:
    │   PUT /api/orders/check-phone-blacklist
    │   Nếu SĐT nằm trong blacklist → buộc OnlinePayment
    │
    └── Submit: POST /api/orders
```

**Bước 3 - Backend xử lý đơn hàng:**
```
OrderService.Create(CreateOrderDTO)
    │
    ├── 1. Validate dữ liệu đầu vào
    ├── 2. Kiểm tra Stock Lock (StockLockService)
    │      ├── Lock từng sản phẩm với số lượng
    │      ├── Lock tồn tại 15 phút (IMemoryCache + TTL)
    │      └── Nếu lock thất bại → rollback + trả lỗi
    ├── 3. Tạo Order entity (Status = Pending)
    ├── 4. Tạo OrderDetails
    ├── 5. Trừ StockQuantity của Product
    ├── 6. Tạo Payment record
    ├── 7. Gửi email xác nhận (EmailService)
    │
    └── Trả về orderId cho frontend
```

**Bước 4 - Thanh toán:**
```
Nếu COD:
    Order.Status → PendingVerification
    Chờ admin xác nhận

Nếu OnlinePayment:
    Frontend redirect → MomoMock page
    User click "Thanh toán thành công" → POST /api/payment/webhook
    Order.Status → Confirmed
    Payment.Status → Completed
```

---

## 6. Workflow: Quản lý Đơn hàng

### 6.1 Vòng đời đơn hàng (Order Lifecycle)

```
                    ┌──────────────────────────┐
                    │     Pending (0)          │
                    │  (Đơn vừa tạo, chờ xử lý) │
                    └────────────┬─────────────┘
                                 │
                                 ▼
                    ┌──────────────────────────┐
              ┌────►│  PendingVerification (4) │◄────┐
              │    │  (Chờ xác minh - 30 phút)  │     │
              │    └────────────┬───────────────┘     │
              │                 │                      │
              │     ╔══════════╧══════════╗           │
              │     ║  30 phút không      ║           │
              │     ║  xác minh?          ║           │
              │     ╚══════════╤══════════╝           │
              │            YES │         NO            │
              │                 ▼         ▼            │
              │    ┌──────────┐  ┌──────────────┐     │
              │    │Cancelled │  │  Confirmed (5)│─────┤
              │    │  (3)     │  │ (Đã xác nhận) │     │
              │    └──────────┘  └──────┬───────┘     │
              │                         │              │
              │                         ▼              │
              │              ┌──────────────────┐     │
              │              │  Preparing (6)   │     │
              │              │ (Đang chuẩn bị)  │     │
              │              └──────┬───────────┘     │
              │                     │                  │
              │                     ▼                  │
              │              ┌──────────────────┐     │
              │              │  Shipping (1)    │     │
              │              │ (Đang giao hàng) │     │
              │              └──────┬───────────┘     │
              │                     │                  │
              │                     ▼                  │
              │              ┌──────────────────┐     │
              │              │ Completed (2)    │     │
              │              │ (Hoàn thành)     │     │
              │              └──────────────────┘     │
              │                                       │
              └───────────────────────────────────────┘
              (Có thể hủy bất kỳ lúc nào trước Completed)
```

### 6.2 Background Service: Order Expiry

`OrderExpiryBackgroundService` chạy mỗi 60 giây:

```
foreach (order in Orders where Status == PendingVerification)
{
    if (DateTime.Now - order.CreatedAt > 30 phút)
    {
        order.Status = Cancelled;
        // Hoàn lại stock
    }
}
```

### 6.3 Email Notifications

Khi đơn hàng thay đổi trạng thái, `EmailService` gửi email HTML template:

| Sự kiện | Template | Người nhận |
|---------|----------|------------|
| Tạo đơn | Xác nhận đã nhận đơn | Khách hàng |
| Xác nhận | Đơn đã xác nhận, đang chuẩn bị | Khách hàng |
| Hủy đơn | Thông báo hủy + lý do | Khách hàng |
| Hoàn thành | Cảm ơn + đánh giá | Khách hàng |

---

## 7. Workflow: Hủy đơn hàng

```
Khách hàng                     Frontend                        Backend
    │                             │                              │
    │  Xem chi tiết đơn #16      │                              │
    │◄────────────────────────────┤                              │
    │                             │                              │
    │  Bấm "Hủy đơn"             │                              │
    ├────────────────────────────►│                              │
    │                             │                              │
    │  Popup xác nhận:            │                              │
    │  - Chọn lý do hủy           │                              │
    │  - Bấm Xác nhận hủy        │                              │
    ├────────────────────────────►│                              │
    │                             │  PUT /api/orders/16/cancel   │
    │                             ├─────────────────────────────►│
    │                             │                              │
    │                             │   ┌──────────────────────┐   │
    │                             │   │ 1. Validate JWT       │   │
    │                             │   │ 2. Check ownership    │   │
    │                             │   │ 3. Status check       │   │
    │                             │   │ 4. Begin Transaction  │   │
    │                             │   │ 5. Update Order       │   │
    │                             │   │    Status=Cancelled   │   │
    │                             │   │ 6. Restore Stock      │   │
    │                             │   │ 7. Commit Transaction │   │
    │                             │   │ 8. Send Email         │   │
    │                             │   └──────────────────────┘   │
    │                             │                              │
    │                             │  200 OK                      │
    │                             │◄─────────────────────────────┤
    │                             │                              │
    │  Giao diện cập nhật:        │                              │
    │  - Badge "Đã hủy"           │                              │
    │  - Nút Hủy bị vô hiệu hóa   │                              │
    │◄────────────────────────────┤                              │
```

### Business Rules

| Rule | Mô tả |
|------|-------|
| Chỉ hủy được đơn ở trạng thái `Pending`, `PendingVerification`, `Confirmed`, `Preparing` |
| Không hủy được đơn `Shipping` hoặc `Completed` |
| Chỉ chủ đơn mới được hủy (kiểm tra CustomerId từ JWT) |
| Hủy xong tự động hoàn lại StockQuantity |
| Gửi email thông báo hủy đến khách hàng |

---

## 8. Workflow: Giỏ hàng & Yêu thích

### 8.1 Giỏ hàng (Cart)

```
                  CartContext (React Context)
                  ┌─────────────────────────────────────┐
                  │   state: CartItem[]                 │
                  │   persisted in localStorage          │
                  │                                     │
                  │   addToCart(product, qty)            │
                  │   removeFromCart(productId)          │
                  │   updateQuantity(productId, qty)     │
                  │   clearCart()                        │
                  │   cartCount (computed)               │
                  │   cartTotal (computed)               │
                  └─────────────────────────────────────┘
                               │
                               ▼
                  ┌─────────────────────────────────────┐
                  │   CartItem = Product + quantity     │
                  │   Stock validation on add/update     │
                  │   Capped at stockQuantity            │
                  └─────────────────────────────────────┘
```

**Tính năng trên giao diện Cart:**
- Table hiển thị sản phẩm: ảnh, tên, giá, số lượng (tăng/giảm), thành tiền
- Summary sidebar: tạm tính, phí ship (nếu có), tổng cộng
- Nút "Tiến hành thanh toán" → /checkout
- Trust badges: "Cam kết hoa tươi", "Giao miễn phí", "Đổi trả 24h"

### 8.2 Danh sách yêu thích (Wishlist)

```
             WishlistContext (React Context)
             ┌─────────────────────────────────────┐
             │   state: Product[]                  │
             │   persisted in localStorage          │
             │                                     │
             │   toggleFavorite(product)            │
             │   isFavorite(productId): boolean     │
             │   removeFavorite(productId)          │
             │   favoritesCount: number             │
             └─────────────────────────────────────┘
```

Giao diện `/wishlist`: Grid sản phẩm yêu thích với nút "Thêm vào giỏ" và "Xóa".

---

## 9. Workflow: Blog & Tin tức

```
Trang chủ (/):
  ┌─────────────────────────────────────────────────────┐
  │  LatestBlog section: hiển thị 3 bài viết mới nhất   │
  │  Gọi API: GET /api/posts?page=1&pageSize=3          │
  └─────────────────────────────────────────────────────┘

Trang Blog (/blog):
  ┌─────────────────────────────────────────────────────┐
  │  Main: Danh sách bài viết (phân trang)              │
  │  Sidebar: Categories + Recent Posts                 │
  │  Gọi API: GET /api/posts?page=N&pageSize=6         │
  │  Gọi API: GET /api/categories                       │
  └─────────────────────────────────────────────────────┘

Chi tiết (/blog/:id):
  ┌─────────────────────────────────────────────────────┐
  │  Hero image                                         │
  │  Content (HTML, rendered với DOMPurify)             │
  │  Thông tin: author, ngày đăng, category             │
  │  Social share buttons (Facebook, Twitter, Email)     │
  │  Comments section (UI giao diện, placeholder)       │
  │  Shopping section: sản phẩm liên quan trong bài     │
  └─────────────────────────────────────────────────────┘
```

---

## 10. Workflow: Tìm kiếm & Lọc sản phẩm

### 10.1 Trang Shop (/shop)

```
┌──────────────────────────────────────────────────────────┐
│  Sidebar (Filter)                       │ Product Grid   │
│  ┌──────────────────┐                  │ ┌──┐ ┌──┐ ┌──┐│
│  │ Category         │                  │ │P1│ │P2│ │P3││
│  │ ☐ Hoa hồng      │                  │ └──┘ └──┘ └──┘│
│  │ ☐ Hoa lan       │                  │ ┌──┐ ┌──┐ ┌──┐│
│  │ ☐ Hoa cưới      │                  │ │P4│ │P5│ │P6││
│  ├──────────────────┤                  │ └──┘ └──┘ └──┘│
│  │ Price Range      │                  │ ┌──┐ ┌──┐ ┌──┐│
│  │ ○ Dưới 200k     │                  │ │P7│ │P8│ │P9││
│  │ ○ 200k - 500k   │                  │ └──┘ └──┘ └──┘│
│  │ ○ 500k - 1tr    │                  │                │
│  │ ○ Trên 1tr      │                  │ ◄ 1 2 3 ... ► │
│  │ ○ Tùy chọn:     │                  │  (Pagination)  │
│  │  [___] - [___]  │                  └────────────────┘
│  ├──────────────────┤
│  │ Sort             │
│  │ ▼ Mới nhất       │
│  │ ▼ Giá: Thấp-Cao  │
│  │ ▼ Giá: Cao-Thấp  │
│  └──────────────────┘
└──────────────────────────────────────────────────────────┘
```

**API call:** `GET /api/products?page=1&pageSize=9&categoryProductId=2&minPrice=200000&maxPrice=500000`

**Filter logic (ProductService.GetPaged):**
```csharp
public async Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize,
    decimal? minPrice, decimal? maxPrice, int? categoryProductId)
{
    var query = BuildQuery(); // Includes CategoryProduct

    if (categoryProductId.HasValue)
        query = query.Where(p => p.CategoryProductId == categoryProductId.Value);
    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);
    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    query = query.OrderByDescending(p => p.Id);

    var totalCount = await query.CountAsync();
    var items = await query.Skip((page-1)*pageSize).Take(pageSize).ToListAsync();

    return new PagedResult { Items = items.Select(p => p.ToDTO()), TotalCount, Page, PageSize };
}
```

### 10.2 Tìm kiếm (/search?query=...)

```
User nhập từ khóa → Header search bar
    │
    ▼
Gọi API: GET /api/products/search?query=hoa+hồng
    │
    ▼
ProductService.Search(query):
    - Trim + ToLower
    - Tìm trong Name hoặc Sku (Contains)
    - OrderByDescending(Id)
    - Trả về ProductDTO list
```

---

## 11. Workflow: Xác thực & Bảo mật

### 11.1 Đăng nhập (Login/Register)

```
Login:
  Frontend: /login → POST /api/auth/login { username, password }
  Backend:
    ├── Xác thực Username + PasswordHash
    ├── Tạo Access Token (JWT, 1h)
    ├── Tạo Refresh Token (lưu vào DB, 7 ngày)
    ├── Lưu token vào HttpOnly cookie (optional) + response body
    └── Trả về { accessToken, refreshToken, user }
  Frontend:
    ├── Lưu accessToken vào localStorage
    ├── AuthContext cập nhật user state
    ├── Redirect đến trang chủ
    └── Interceptor tự động gắn Bearer token vào mọi request

Register:
  Frontend: /register → POST /api/auth/register
  Body: { fullName, email, phone, address, password }
  Backend:
    ├── Validate unique email
    ├── Hash password (PasswordHasher)
    └── Tạo Customer + Return JWT tokens
```

### 11.2 Refresh Token Flow

```
Access Token hết hạn (401)
    │
    ▼
Axios interceptor catch 401
    │
    ├── Gọi POST /api/auth/refresh { refreshToken }
    ├── Nếu thành công → lưu token mới → retry request gốc
    └── Nếu thất bại → xóa token, emit 'unauthorized', redirect /login
```

### 11.3 Forgot / Reset Password

```
Forgot Password:
  /forgot-password → POST /api/auth/forgot-password { email }
  Backend:
    ├── Tìm Customer theo email
    ├── Tạo ResetToken (GUID ngẫu nhiên)
    ├── Lưu ResetToken + Expiry vào DB
    └── Gửi email chứa link: http://localhost:3000/reset-password?token=xxx

Reset Password:
  /reset-password?token=xxx → POST /api/auth/reset-password { token, newPassword }
  Backend:
    ├── Validate token (tồn tại + chưa hết hạn)
    ├── Hash password mới
    ├── Cập nhật PasswordHash
    └── Xóa token (dùng 1 lần)
```

---

## 12. SignalR Real-time Notifications

### 12.1 Architecture

```
Backend (NotificationService)                    Frontend (useRealtimeUpdates hook)
    │                                                    │
    │  CRUD Event xảy ra                                 │
    │  (Product created, Order updated, ...)             │
    │                                                    │
    │  NotificationService.Send(entityType, action, data)│
    │                                                    │
    │  ┌────────────────┐                               │
    │  │ NotificationHub │                               │
    │  │ .Clients.All    │───── SignalR WebSocket ──────►│  on('ReceiveNotification')
    │  │ .SendAsync(...) │                               │  ├── Update TanStack Query cache
    │  └────────────────┘                               │  ├── Show toast notification
    │                                                    │  └── Refresh UI
```

### 12.2 Events broadcast

| Entity | Actions | Frontend effect |
|--------|---------|-----------------|
| Product | Created, Updated, Deleted | Refresh product list cache |
| Order | StatusChanged | Refresh order detail |
| Post | Created, Updated, Deleted | Refresh blog list |
| Category | Created, Updated, Deleted | Refresh category sidebar |

---

## 13. Fraud Detection & Stock Locking

### 13.1 Fraud Detection

```
Checkout → Kiểm tra SĐT:
    │
    ▼
FraudDetectionService.CheckPhoneBlacklist(phone)
    │
    ├── Tra cứu trong bảng PhoneBlacklists
    ├── Nếu có:
    │   ├── Tính điểm fraud (càng nhiều lần blacklist → điểm càng cao)
    │   └── Buộc OnlinePayment (không cho chọn COD)
    └── Nếu không có → cho phép COD
```

### 13.2 Stock Locking

```
Khi user bắt đầu checkout → StockLockService:

StockLockService class:
    ┌─────────────────────────────────────────────┐
    │ IMemoryCache (in-memory, singleton)          │
    │                                              │
    │ LockKey = $"stock_lock_{productId}"          │
    │ Value = Dictionary<customerId, quantity>     │
    │ TTL = 15 phút                                │
    │                                              │
    │ TryLock(productId, customerId, quantity):     │
    │   ├── product.StockQuantity >= locked + qty  │
    │   └── Nếu OK → cache.Set(key, value, TTL)    │
    │                                              │
    │ ReleaseLock(productId, customerId):           │
    │   └── Xóa cache entry                        │
    │                                              │
    │ Auto-expire sau 15 phút (TTL)               │
    └─────────────────────────────────────────────┘
```

---

## 14. Admin Panel

### 14.1 MVC Controllers (17 controllers)

| Controller | Route | Chức năng |
|-----------|-------|-----------|
| `HomeController` | `/Admin` | Dashboard thống kê |
| `AccountController` | `/Account` | Đăng nhập/đăng xuất Admin |
| `CategoryController` | `/Admin/Categories` | CRUD danh mục Blog |
| `CategoryProductController` | `/Admin/CategoryProducts` | CRUD danh mục Sản phẩm |
| `PostController` | `/Admin/Posts` | CRUD bài viết (CKEditor 5) |
| `ProductController` | `/Admin/Products` | CRUD sản phẩm + upload ảnh |
| `UserController` | `/Admin/Users` | CRUD quản trị viên |
| `CustomerController` | `/Admin/Customers` | Xem danh sách khách hàng |
| `OrderController` | `/Admin/Orders` | Quản lý đơn hàng (duyệt/hủy) |
| `OrderDetailController` | `/Admin/OrderDetails` | Xem chi tiết đơn |
| `AdvertisementController` | `/Admin/Advertisements` | CRUD banner quảng cáo |
| Và 6 controller khác... |

### 14.2 Giao diện Admin

```
┌─────────────────────────────────────────────────────────┐
│  Logo PDA FLOWER    [Tìm kiếm]    👤 Admin    🔔 [Logout] │
├──────────┬──────────────────────────────────────────────┤
│ Sidebar  │  Content Area                                │
│          │                                              │
│ 📊 Tổng  │  ┌──────┐ ┌──────┐ ┌──────┐ ┌──────┐       │
│   quan   │  │ Đơn  │ │ DT   │ │ SP   │ │ KH   │       │
│          │  │ 124  │ │ 15tr │ │ 89   │ │ 56   │       │
│ 📦 SP    │  └──────┘ └──────┘ └──────┘ └──────┘       │
│ 📰 Bài   │                                              │
│   viết   │  Bảng dữ liệu (DataTable)                    │
│ 🛒 Đơn   │  ┌────┬────────┬──────┬──────┬──────────┐  │
│   hàng   │  │ ID │ Tên    │ Giá  │ Kho  │ Hành động│  │
│ 👥 KH    │  ├────┼────────┼──────┼──────┼──────────┤  │
│ 👤 User  │  │ 1  │ Hoa... │ 299k │ 10   │ ✏️ 🗑️   │  │
│ 📢 QC    │  └────┴────────┴──────┴──────┴──────────┘  │
│          │                                              │
│ ⚙️ Cấu   │  [➕ Thêm mới]          Trang: ◄ 1 2 ►      │
│   hình   │                                              │
└──────────┴──────────────────────────────────────────────┘
```

---

## 15. Danh sách API Endpoints

### 15.1 Authentication

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/auth/login` | Đăng nhập (username/password) | No |
| POST | `/api/auth/register` | Đăng ký tài khoản khách hàng | No |
| POST | `/api/auth/refresh` | Refresh JWT token | No |
| GET | `/api/auth/profile` | Lấy thông tin cá nhân | JWT |
| PUT | `/api/auth/profile` | Cập nhật thông tin cá nhân | JWT |
| POST | `/api/auth/forgot-password` | Gửi email reset password | No |
| POST | `/api/auth/reset-password` | Đặt lại mật khẩu với token | No |
| PUT | `/api/auth/change-password` | Đổi mật khẩu | JWT |

### 15.2 Products

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/products` | Danh sách phân trang + lọc | No |
| GET | `/api/products/{id}` | Chi tiết sản phẩm | No |
| GET | `/api/products/search?query=` | Tìm kiếm sản phẩm | No |
| GET | `/api/products/best-sellers` | Sản phẩm bán chạy (top 3) | No |
| GET | `/api/products/recent` | Sản phẩm mới nhất | No |

### 15.3 Orders

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| POST | `/api/orders` | Tạo đơn hàng mới | JWT |
| GET | `/api/orders/my-orders` | Lịch sử đơn hàng | JWT |
| GET | `/api/orders/{id}` | Chi tiết đơn hàng | JWT |
| PUT | `/api/orders/{id}/cancel` | Hủy đơn hàng | JWT |
| PUT | `/api/orders/check-phone-blacklist` | Kiểm tra SĐT có trong blacklist | JWT |

### 15.4 Categories, Posts, Customers, Delivery, Payment, Ads

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| GET | `/api/categories` | Danh mục Blog | No |
| GET | `/api/categories-products` | Danh mục Sản phẩm | No |
| GET | `/api/posts` | Danh sách bài viết (phân trang) | No |
| GET | `/api/posts/{id}` | Chi tiết bài viết | No |
| GET | `/api/posts/search?query=` | Tìm kiếm bài viết | No |
| GET | `/api/customers/{id}` | Thông tin khách hàng | JWT |
| GET | `/api/delivery/districts` | Danh sách quận/huyện | No |
| GET | `/api/delivery/slots?date=&productId=` | Khung giờ giao trống | No |
| POST | `/api/payment/webhook` | Webhook thanh toán | No |
| GET | `/api/advertisements/active` | Banner quảng cáo đang hoạt động | No |
| GET | `/api/orderdetails/{id}` | Chi tiết đơn hàng (order detail) | JWT |

---

<p align="center"><i>&copy; 2026 AnhCMS_Solution - Full-Stack ASP.NET Core &amp; React Architecture Documentation</i></p>
