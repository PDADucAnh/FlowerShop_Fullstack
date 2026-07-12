# Phân Tích Module Promotion - FlowerShop

## 1. Kiến Trúc Hiện Tại

### 1.1. Tổng Quan

```
FlowerShop/
├── Flower.Backend/          # ASP.NET Core 8 Web API + MVC Views
│   ├── Controllers/         # MVC Controllers (CMS Admin)
│   │   └── Api/             # API Controllers (React Frontend)
│   ├── Services/            # Business Logic Layer
│   │   └── Interfaces/      # Service Contracts
│   ├── Models/
│   │   ├── DTOs/            # Data Transfer Objects
│   │   └── *.cs             # Settings models
│   ├── Utils/               # Helpers (Slug, Image, Price, DateTime)
│   ├── Middleware/           # SessionValidationMiddleware
│   └── Hubs/                # SignalR NotificationHub
├── Flower.Data/             # Data Access Layer (EF Core)
│   ├── Entities/            # 22 Entity classes
│   ├── Migrations/          # 12 migration files
│   ├── ApplicationDbContext.cs
│   └── IApplicationDbContext.cs
├── Flower.Tests/            # Unit Tests
├── Flower-shop.frontend/    # React + Vite + TypeScript
│   └── src/
│       ├── api/             # Axios client + QueryClient
│       ├── components/      # Reusable components
│       ├── context/         # Auth, Cart, Wishlist contexts
│       ├── hooks/           # React Query hooks
│       ├── pages/           # Page components
│       ├── services/        # API service calls
│       ├── types/           # TypeScript interfaces
│       └── utils/           # Helpers
└── docs/
    ├── promotions/          # Promotion requirement documents
    └── ...
```

### 1.2. Công Nghệ

| Layer | Technology |
|-------|-----------|
| Frontend | React 19, Vite 8, TypeScript 6, TailwindCSS 3, React Router 7, React Query 5, Axios, SignalR 10 |
| Backend | ASP.NET Core 8 Web API + MVC |
| Database | SQL Server (LocalDB) |
| ORM | Entity Framework Core 8 |
| Auth | JWT Bearer (API) + Cookie (MVC Admin) |
| Real-time | SignalR |
| Payment | VNPay Sandbox |

### 1.3. Authentication & Authorization

- **Dual Auth Scheme**: JWT Bearer cho API `/api/*`, Cookie Authentication cho MVC Admin
- **Policy**: `AdminOnly` (Admin), `StaffOnly` (Admin, Editor)
- **User types**: `User` (admin/editor) + `Customer` (khách hàng)
- **Claims**: NameIdentifier, Name, Role, FullName, AuthType, LoginTime

### 1.4. Dependency Injection Pattern

Tất cả Service đều đăng ký Scoped trong `Program.cs`:
```csharp
builder.Services.AddScoped<IInterface, Implementation>();
```

Không sử dụng Repository pattern riêng. Service truy cập trực tiếp `IApplicationDbContext`.

### 1.5. Service Layer Pattern

Mỗi Service:
- Nhận `IApplicationDbContext` qua DI
- Sử dụng Mapping Extensions (`ToDTO()`, `ToEntity()`, `UpdateEntity()`) để chuyển đổi Entity <-> DTO
- Trả về DTO, không trả về Entity
- Có Interface tương ứng

### 1.6. Controller Pattern

- **MVC Controllers**: Trong `Controllers/` - dùng cho CMS Admin, trả về View
- **API Controllers**: Trong `Controllers/Api/` - dùng cho React Frontend, trả về JSON
- Tất cả API Controller dùng `[Route("api/[controller]")]`
- Tất cả API endpoint có xử lý phân quyền qua `[Authorize]`, `[AllowAnonymous]`

### 1.7. Frontend Pattern

- **API Layer**: `src/api/axiosClient.ts` - Axios instance với interceptor cho JWT và 401 handling
- **Services**: `src/services/*.ts` - Gọi API, trả về typed data
- **Hooks**: `src/hooks/*.ts` - React Query hooks quản lý caching, refetch
- **Context**: Auth, Cart, Wishlist (React Context)
- **Admin pages**: Dùng `AdminLayout` component với sidebar navigation
- **Routing**: React Router v7 với lazy loading (`React.lazy`)

---

## 2. Database Hiện Tại

### 2.1. Entities (22 entities hiện có)

| Entity | Ghi chú |
|--------|---------|
| User | Admin/Editor |
| Customer | Khách hàng |
| Product | Sản phẩm (có `DiscountPrice` field sẵn) |
| ProductVariant | Biến thể sản phẩm |
| CategoryProduct | Danh mục sản phẩm |
| Category | Danh mục bài viết |
| Post | Bài viết blog |
| Order | Đơn hàng (OrderStatus enum 14 giá trị) |
| OrderDetail | Chi tiết đơn hàng |
| Payment | Thanh toán |
| PaymentAttempt | Lần thử thanh toán |
| PaymentMethodDefinition | Định nghĩa phương thức thanh toán |
| CustomerPaymentPreference | Sở thích thanh toán |
| DeliverySlot | Khung giờ giao hàng |
| Notification | Thông báo |
| EmailHistory | Lịch sử email |
| Refund | Hoàn tiền |
| CancellationPolicy | Chính sách hủy |
| Advertisement | Banner quảng cáo |
| PhoneBlacklist | Danh sách đen số điện thoại |
| CustomerAddress | Địa chỉ khách hàng |
| RefreshToken | Refresh token |

### 2.2. Relationships

- Product -> CategoryProduct (N-1)
- Order -> Customer (N-1)
- OrderDetail -> Product (N-1)
- OrderDetail -> Order (N-1)
- ProductVariant -> Product (N-1)
- Payment -> Order (N-1)
- PaymentAttempt -> Payment (N-1)
- Notification -> Customer (N-1)
- Refund -> Order (N-1), Payment (N-1)

### 2.3. Indexes

- `IX_Customers_Phone`, IX trên Email (unique)
- `IX_Orders_Status`, `IX_Orders_Status_OrderDate`, `IX_Orders_OrderDate`
- `IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive`
- `IX_Notifications_CustomerId`, `IX_Notifications_CustomerId_IsRead`
- `IX_CustomerAddresses_CustomerId_IsDefault`
- `IX_PhoneBlacklist_PhoneNumber_IsActive`

### 2.4. Product.DiscountPrice

Product entity **đã có sẵn** field `DiscountPrice` (decimal?). Tuy nhiên field này không được dùng trong luồng xử lý hiện tại - không có logic nào tính toán hoặc cập nhật nó. Đây là field tĩnh, có thể được dùng cho Promotion sau này.

---

## 3. Các File Cần Chỉnh Sửa

### 3.1. Backend - Data Layer (Flower.Data)

| File | Thao tác | Mô tả |
|------|----------|-------|
| `Flower.Data/Entities/PromotionCampaign.cs` | **Tạo mới** | Entity cho chiến dịch khuyến mãi |
| `Flower.Data/Entities/PromotionProduct.cs` | **Tạo mới** | Junction table Promotion <-> Product |
| `Flower.Data/Entities/Coupon.cs` | **Tạo mới** | Entity cho mã giảm giá |
| `Flower.Data/Entities/CouponUsage.cs` | **Tạo mới** | Entity cho lịch sử sử dụng coupon |
| `Flower.Data/ApplicationDbContext.cs` | **Sửa** | Thêm DbSet cho 4 entity mới + Fluent API |
| `Flower.Data/IApplicationDbContext.cs` | **Sửa** | Thêm DbSet properties cho 4 entity mới |

### 3.2. Backend - DTOs (Flower.Backend/Models/DTOs)

| File | Thao tác | Mô tả |
|------|----------|-------|
| `Models/DTOs/PromotionDTOs.cs` | **Tạo mới** | PromotionCampaignDTO, Create/Update DTOs |
| `Models/DTOs/CouponDTOs.cs` | **Tạo mới** | CouponDTO, CouponUsageDTO, Create/Update DTOs |
| `Models/DTOs/ApplyCouponDTOs.cs` | **Tạo mới** | ApplyCouponRequest, ApplyCouponResponse |
| `Models/DTOs/PromotionMappingExtensions.cs` | **Tạo mới** | Mapping cho các entity Promotion mới |
| `Models/DTOs/ProductDTOs.cs` | **Sửa** | Thêm fields: `PromotionPrice`, `PromotionPercent`, `PromotionType`, `HasFlashSale` |
| `Models/DTOs/OrderDTOs.cs` | **Sửa** | Thêm fields: `DiscountAmount`, `CouponCode`, `CouponDiscount`, `PromotionDiscount` <br>Hoặc tạo `OrderPricingDTO` riêng |

### 3.3. Backend - Services & Interfaces

| File | Thao tác | Mô tả |
|------|----------|-------|
| `Services/Interfaces/IPromotionService.cs` | **Tạo mới** | Interface cho Promotion service |
| `Services/PromotionService.cs` | **Tạo mới** | CRUD + auto-activate/expire |
| `Services/Interfaces/ICouponService.cs` | **Tạo mới** | Interface cho Coupon service |
| `Services/CouponService.cs` | **Tạo mới** | CRUD + validate + apply |
| `Services/Interfaces/IPriceCalculationService.cs` | **Tạo mới** | Interface cho Price Calculation |
| `Services/PriceCalculationService.cs` | **Tạo mới** | Tính giá: Original -> FlashSale -> Voucher -> Shipping -> Total |
| `Services/Interfaces/IPromotionScheduler.cs` | **Tạo mới** | Interface cho scheduler |
| `Services/PromotionScheduler.cs` | **Tạo mới** | Background service auto-activate/expire |
| `Services/Interfaces/IOrderService.cs` | **Sửa** | Thêm method `CalculateOrderTotal` hoặc tham số coupon |
| `Services/OrderService.cs` | **Sửa** | Tích hợp PriceCalculationService vào CreateOrder |

### 3.4. Backend - Controllers

| File | Thao tác | Mô tả |
|------|----------|-------|
| `Controllers/Api/PromotionsController.cs` | **Tạo mới** | API CRUD cho Promotion Campaign |
| `Controllers/Api/CouponsController.cs` | **Tạo mới** | API CRUD cho Coupon + validate + apply |
| `Controllers/Api/OrdersController.cs` | **Sửa** | Thêm coupon vào CreateOrder API |
| `Controllers/Api/PaymentController.cs` | **Sửa** | Có thể cần - nếu discount ảnh hưởng thanh toán |
| `Controllers/ProductController.cs` | **Không sửa** | Giữ nguyên |
| `Controllers/Api/ProductsController.cs` | **Sửa** | Thêm promotion info vào response |
| `Controllers/OrderController.cs` | **Sửa** | MVC View: thêm hiển thị promotion |

### 3.5. Backend - Program.cs (DI)

| File | Thao tác | Mô tả |
|------|----------|-------|
| `Program.cs` | **Sửa** | Thêm DI registration cho PromotionService, CouponService, PriceCalculationService, PromotionScheduler (HostedService) |

### 3.6. Frontend - Types

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/types/promotion.ts` | **Tạo mới** | PromotionCampaign, Coupon types |
| `src/types/order.ts` | **Sửa** | Thêm discount/coupon fields |
| `src/types/product.ts` | **Sửa** | Thêm promotion fields |

### 3.7. Frontend - Services

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/services/promotionService.ts` | **Tạo mới** | API calls cho Promotion |
| `src/services/couponService.ts` | **Tạo mới** | API calls cho Coupon |

### 3.8. Frontend - Hooks

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/hooks/usePromotions.ts` | **Tạo mới** | React Query hooks |
| `src/hooks/useCoupons.ts` | **Tạo mới** | React Query hooks |

### 3.9. Frontend - Pages

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/pages/admin/promotions/index.tsx` | **Tạo mới** | Admin Promotion CRUD |
| `src/pages/admin/coupons/index.tsx` | **Tạo mới** | Admin Coupon CRUD |
| `src/pages/admin/promotions/PromotionDashboard.tsx` | **Tạo mới** | Promotion dashboard |
| `src/pages/shop/ProductList.tsx` | **Sửa** | Hiển thị Flash Sale badge + giá khuyến mãi |
| `src/pages/product-detail/index.tsx` | **Sửa** | Hiển thị promotion info |
| `src/pages/checkout/index.tsx` | **Sửa** | Thêm voucher input, apply button, discount display |
| `src/pages/auth/MyOrders.tsx` | **Sửa** | Hiển thị discount/coupon trong lịch sử |
| `src/pages/auth/OrderDetail.tsx` | **Sửa** | Hiển thị chi tiết promotion |

### 3.10. Frontend - Components

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/components/AdminLayout.tsx` | **Sửa** | Thêm menu items: Khuyến mãi, Coupon |

### 3.11. Frontend - App.tsx

| File | Thao tác | Mô tả |
|------|----------|-------|
| `src/App.tsx` | **Sửa** | Thêm routes cho admin promotions, coupons |

---

## 4. Entity Cần Bổ Sung

### 4.1. PromotionCampaign

```csharp
public class PromotionCampaign
{
    public int Id { get; set; }
    public string Name { get; set; }              // Tên chiến dịch
    public string? Description { get; set; }       // Mô tả
    public PromotionType PromotionType { get; set; } // FlashSale, Seasonal
    public DiscountType DiscountType { get; set; }   // Percent, FixedAmount
    public decimal DiscountValue { get; set; }       // Giá trị giảm
    public DateTime StartDate { get; set; }          // Bắt đầu
    public DateTime EndDate { get; set; }            // Kết thúc
    public int Priority { get; set; }                // Độ ưu tiên (cao hơn = ưu tiên hơn)
    public string? BannerImage { get; set; }         // Banner
    public bool IsStackable { get; set; }            // Cho phép stack với voucher
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 4.2. PromotionProduct (Junction Table)

```csharp
public class PromotionProduct
{
    public int Id { get; set; }
    public int PromotionId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual PromotionCampaign? Promotion { get; set; }
    public virtual Product? Product { get; set; }
}
```

### 4.3. Coupon

```csharp
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; }                // Mã giảm giá (UNIQUE)
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }   // Percent, FixedAmount
    public decimal DiscountValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; } // Đơn tối thiểu
    public decimal? MaximumDiscountAmount { get; set; } // Giảm tối đa
    public int? UsageLimit { get; set; }             // Giới hạn lượt
    public int UsedCount { get; set; }              // Đã dùng
    public int? UsagePerCustomer { get; set; }      // Lượt mỗi khách
    public int? CustomerId { get; set; }            // Chỉ định khách (nullable)
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsPublic { get; set; }              // Công khai?
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 4.4. CouponUsage

```csharp
public class CouponUsage
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public int CustomerId { get; set; }
    public int OrderId { get; set; }
    public decimal DiscountAmount { get; set; }     // Số tiền giảm
    public DateTime UsedAt { get; set; }

    // Navigation
    public virtual Coupon? Coupon { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual Order? Order { get; set; }
}
```

### 4.5. Relationship Diagram

```
PromotionCampaign 1──N PromotionProduct N──1 Product
                                           
Coupon 1──N CouponUsage N──1 Order
         └──────────────N──1 Customer
```

### 4.6. Enums

```csharp
public enum PromotionType
{
    FlashSale = 0,
    Seasonal = 1
}

public enum DiscountType
{
    Percent = 0,
    FixedAmount = 1
}
```

---

## 5. API Cần Bổ Sung

### 5.1. Promotion Campaign API

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | `/api/promotions` | StaffOnly | Danh sách tất cả campaign |
| GET | `/api/promotions/{id}` | StaffOnly | Chi tiết campaign |
| POST | `/api/promotions` | StaffOnly | Tạo campaign |
| PUT | `/api/promotions/{id}` | StaffOnly | Sửa campaign |
| DELETE | `/api/promotions/{id}` | AdminOnly | Xóa campaign |
| PATCH | `/api/promotions/{id}/enable` | StaffOnly | Bật campaign |
| PATCH | `/api/promotions/{id}/disable` | StaffOnly | Tắt campaign |
| POST | `/api/promotions/{id}/products` | StaffOnly | Thêm sản phẩm vào campaign |
| DELETE | `/api/promotions/{id}/products/{productId}` | StaffOnly | Xóa sản phẩm khỏi campaign |
| GET | `/api/promotions/active` | AllowAnonymous | Lấy danh sách promotion đang hoạt động |

### 5.2. Coupon API

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| GET | `/api/coupons` | StaffOnly | Danh sách coupon |
| GET | `/api/coupons/{id}` | StaffOnly | Chi tiết coupon |
| POST | `/api/coupons` | StaffOnly | Tạo coupon |
| PUT | `/api/coupons/{id}` | StaffOnly | Sửa coupon |
| DELETE | `/api/coupons/{id}` | AdminOnly | Xóa coupon |
| PATCH | `/api/coupons/{id}/enable` | StaffOnly | Bật coupon |
| PATCH | `/api/coupons/{id}/disable` | StaffOnly | Tắt coupon |
| POST | `/api/coupons/validate` | Customer | Validate coupon (trả về thông tin giảm giá) |
| POST | `/api/coupons/apply` | Customer | Apply coupon vào đơn hàng |

### 5.3. Modified API Endpoints

| Method | Endpoint | Thay đổi |
|--------|----------|----------|
| GET | `/api/products` | Thêm promotion info: `promotionPrice`, `promotionPercent`, `promotionType` |
| GET | `/api/products/{id}` | Thêm promotion info |
| POST | `/api/orders` | Thêm optional `couponCode` field |
| GET | `/api/orders/{id}` | Thêm `discountAmount`, `couponCode`, `couponDiscount` vào response |

### 5.4. Frontend Routes Cần Thêm

| Route | Component | Mô tả |
|-------|-----------|-------|
| `/admin/promotions` | Admin Promotions | Quản lý campaign |
| `/admin/coupons` | Admin Coupons | Quản lý coupon |

**Routes hiện tại cần sửa** (đã có trong AdminLayout nhưng chưa có page):
- `AdminLayout.tsx` line 12: `{ path: '/admin/promotions', label: 'Khuyến mãi', icon: 'local_offer' }` — đã có menu, cần tạo page

---

## 6. Database Cần Bổ Sung

### 6.1. Migration mới

Tạo migration mới (không sửa migration cũ):
```
dotnet ef migrations add AddPromotionModule
```

Migration sẽ tạo 4 bảng:
- `PromotionCampaigns`
- `PromotionProducts`
- `Coupons`
- `CouponUsages`

### 6.2. Constraints & Indexes

```sql
-- Coupon Code UNIQUE
CREATE UNIQUE INDEX IX_Coupons_Code ON Coupons(Code);

-- Coupon code case-insensitive
-- (SQL Server default is case-insensitive, ok)

-- CouponUsage unique per order
CREATE UNIQUE INDEX IX_CouponUsages_OrderId ON CouponUsages(OrderId);

-- CouponUsage index for lookup
CREATE INDEX IX_CouponUsages_CouponId_CustomerId ON CouponUsages(CouponId, CustomerId);

-- Promotion active index
CREATE INDEX IX_PromotionCampaigns_IsActive_StartDate_EndDate 
    ON PromotionCampaigns(IsActive, StartDate, EndDate);

-- PromotionProduct
CREATE INDEX IX_PromotionProducts_PromotionId ON PromotionProducts(PromotionId);
CREATE INDEX IX_PromotionProducts_ProductId ON PromotionProducts(ProductId);
CREATE UNIQUE INDEX IX_PromotionProducts_Unique ON PromotionProducts(PromotionId, ProductId);
```

### 6.3. Fluent API cho ApplicationDbContext

Cần thêm vào `OnModelCreating`:
- Unique index cho `Coupon.Code`
- Unique index cho `PromotionProduct (PromotionId, ProductId)`
- Unique index cho `CouponUsage.OrderId`
- Foreign key relationships với Restrict delete
- Index cho Promotion search queries

---

## 7. Thứ Tự Triển Khai

### Phase 1: Data Layer
1. Tạo 4 Entity files
2. Cập nhật `IApplicationDbContext.cs` (thêm DbSet)
3. Cập nhật `ApplicationDbContext.cs` (thêm DbSet + Fluent API)
4. Chạy migration

### Phase 2: Backend Logic
5. Tạo DTOs + MappingExtensions
6. Tạo IPromotionService + PromotionService
7. Tạo ICouponService + CouponService
8. Tạo IPriceCalculationService + PriceCalculationService
9. Tạo PromotionScheduler (BackgroundService)
10. Đăng ký DI trong Program.cs

### Phase 3: Backend API
11. Tạo PromotionsController
12. Tạo CouponsController
13. Sửa ProductsController (thêm promotion data)
14. Sửa OrdersController (thêm coupon vào create order)

### Phase 4: Frontend
15. Tạo TypeScript types
16. Tạo services (promotionService, couponService)
17. Tạo hooks
18. Tạo admin pages
19. Sửa shop/product-detail pages
20. Sửa checkout page
21. Sửa my-orders pages
22. Cập nhật App.tsx routes

---

## 8. Những Điểm Có Thể Gây Xung Đột

### 8.1. Product.DiscountPrice - Xung đột ngữ nghĩa

- **Hiện tại**: `Product.DiscountPrice` đã tồn tại nhưng không được sử dụng trong business logic
- **Nguy cơ**: Nếu code khác vô tình đọc/ghi field này, sẽ xảy ra conflict
- **Giải pháp**: 
  - Option A: Dùng lại `DiscountPrice` làm Flash Sale price (rủi ro cao)
  - Option B: **Không dùng** `DiscountPrice` cũ. Tính toán promotion price runtime trong `PriceCalculationService`. Giữ `DiscountPrice` như legacy field.

### 8.2. Order Status Flow

- **Hiện tại**: Order có status `Confirmed (5)` khi được xác nhận, lúc này stock mới giảm
- **Promotion**: Cần đảm bảo promotion price được tính trước khi tạo order (trong `CreateOrder`)
- **Coupon Usage**: Cần ghi nhận CouponUsage khi order được tạo, và xử lý hoàn trả khi order bị hủy
- **Rủi ro cao**: Coupon usage không được rollback đúng cách khi order bị hủy

### 8.3. CouponValidation trong Order Flow

- Luồng hiện tại: `CreateOrder` trong `OrderService` nhận `OrderItemInput` list
- Cần thêm: Kiểm tra coupon hợp lệ → tính discount → lưu CouponUsage → tạo order
- Cần rollback CouponUsage nếu create order thất bại

### 8.4. Xung đột với OrderCancellationService

- Khi order bị hủy bởi `OrderCancellationService`, cần:
  - Hoàn trả `CouponUsage` (tăng UsedCount giảm)
  - Chỉ hoàn nếu order status là `PendingPayment` hoặc `Pending`
  - Không hoàn nếu đã `Completed` hoặc `Shipping`
- Cần sửa `OrderCancellationService` để gọi `CouponService.ReleaseCoupon(orderId)`

### 8.5. Price Display ở Frontend

- **Hiện tại**: `CartContext` tính `cartTotal` dựa trên `product.discountPrice || product.price`
- **Promotion**: Cần đổi thành gọi API để lấy giá promotion (không tính client-side)
- **Hiện tại**: Checkout page không có coupon input
- **Thêm mới**: Cần thêm coupon input + apply button + loading state + error handling

### 8.6. Admin Sidebar - Menu đã tồn tại

- **AdminLayout.tsx line 12**: `{ path: '/admin/promotions', label: 'Khuyến mãi', icon: 'local_offer' }`
- Menu đã được thêm nhưng page chưa tồn tại → sẽ dẫn đến 404
- Cần tạo page component tương ứng

### 8.7. Authorization

- Promotion management: `StaffOnly` (Admin + Editor)
- Coupon management: `StaffOnly` (Admin + Editor)
- Coupon usage/history: `AdminOnly`
- Customer apply coupon: `Customer` role (authenticated customer)

### 8.8. Performance

- **Flash Sale**: Cần cache promotion data (MemoryCache) để tránh query DB mỗi request
- **Product list**: JOIN với PromotionProducts có thể ảnh hưởng performance
- Giải pháp: Sử dụng MemoryCache + cache invalidation khi promotion thay đổi

### 8.9. VNPay Integration

- Discount có thể ảnh hưởng số tiền VNPay cần thanh toán
- `VnPaymentRequestModel.Amount` cần là số tiền sau discount
- Cần kiểm tra luồng thanh toán discount có ảnh hưởng gì không

### 8.10. Scheduling

- `PromotionScheduler` cần chạy periodic (có thể dùng `IHostedService` như `OrderExpiryBackgroundService`)
- Kiểm tra tự động kích hoạt/hết hạn promotion mỗi X phút
- Không conflict với `OrderExpiryBackgroundService` hiện có

---

## 9. Tóm Tắt Tác Động

| Module | Files Created | Files Modified | Risk Level |
|--------|-------------|----------------|------------|
| Flower.Data.Entities | 4 | 0 | Thấp |
| ApplicationDbContext | 0 | 2 (class + interface) | Thấp |
| Migrations | 1 | 0 | Thấp |
| Models/DTOs | 4 | 2 (Product, Order) | Trung bình |
| Services | 6 (4 interface + 4 impl) | 2 (IOrderService, OrderService) | Cao |
| Controllers (API) | 2 | 2 (Products, Orders) | Trung bình |
| Controllers (MVC) | 0 | 1 (Order) | Thấp |
| Program.cs | 0 | 1 | Trung bình |
| Frontend Types | 1 | 2 | Thấp |
| Frontend Services | 2 | 0 | Thấp |
| Frontend Pages | 3 | 4 | Trung bình |
| Frontend Components | 0 | 1 (AdminLayout) | Thấp |
| App.tsx | 0 | 1 | Thấp |

**Tổng cộng dự kiến**: ~20 file tạo mới, ~15 file sửa đổi

---

## 10. Ghi Chú Kiến Trúc Quan Trọng

### 10.1. Luồng Price Calculation (bắt buộc backend)

```
OriginalPrice → If has FlashSale → Apply best Promotion → FlashSalePrice
FlashSalePrice → If Coupon valid → Apply Coupon → DiscountedPrice
DiscountedPrice → Add Shipping → TotalPrice
```

### 10.2. Coupon Validation Rules

```
1. IsActive == true
2. StartDate <= now <= EndDate
3. UsageLimit == null || UsedCount < UsageLimit
4. UsagePerCustomer == null || customer usage < UsagePerCustomer
5. MinimumOrderAmount == null || orderTotal >= MinimumOrderAmount
6. If CustomerId specified → must match current customer
7. If DiscountType == Percent → MaximumDiscountAmount applies
```

### 10.3. Promotion Priority Rule

```
Khi nhiều Promotion cùng áp dụng cho 1 Product:
→ Chỉ promotion có Priority cao nhất được áp dụng
→ Không cộng dồn (trừ khi IsStackable)
```

### 10.4. Coupon Rollback Strategy

```
Order Cancelled (PendingPayment): Release coupon usage → usedCount--
Order Cancelled (Confirmed+): Chính sách hoàn tiền áp dụng → coupon NOT released
Order CancelledByCustomer: Coupon NOT released
Order CancelledByShop với Refund: Coupon NOT released
```
