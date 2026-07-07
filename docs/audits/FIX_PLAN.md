# Kế Hoạch Khắc Phục — FlowerShop Audit 05/07/2026 (Audit #3)

> Đã khắc phục: 60/83 (12 Critical ✅, 14 High ✅, 34 Medium ✅)
> Còn lại: 23 Low ⬜
> `[ ]` = Chưa xử lý | `[x]` = Đã khắc phục

---

## 🔴 Critical (12 vấn đề)

### BE-C01 — Payment Webhook bị [Authorize] chặn
- [x] **File:** `Flower.Backend/Controllers/Api/PaymentController.cs:10,27`
- **Mô tả:** `[Authorize]` ở class level chặn webhook từ gateway bên ngoài.
- **Hậu quả:** Webhook thanh toán không bao giờ được xử lý.
- **Khắc phục:** Thêm `[AllowAnonymous]` cho Webhook action.

### BE-C02 — Fire-and-forget Task trong OrderService
- [x] **File:** `Flower.Backend/Services/OrderService.cs:326-337`
- **Mô tả:** `_ = Task.Run(async () => { ... })` gửi email OTP ngoài request scope.
- **Hậu quả:** Email OTP không gửi được, process crash nếu exception unobserved.
- **Khắc phục:** Dùng `IHostedService` + Channel pattern hoặc capture scope.

### BE-C03 — Fire-and-forget Task trong AuthService
- [x] **File:** `Flower.Backend/Services/AuthService.cs:249-259`
- **Mô tả:** Giống BE-C02 cho forgot-password email.
- **Khắc phục:** Giống BE-C02.

### BE-C04 — OTP Master Code "000000"
- [x] **File:** `Flower.Backend/Services/FraudDetectionService.cs:52-64`
- **Mô tả:** OTP '000000' hardcoded bypasses all verification.
- **Hậu quả:** Backdoor cho phép xác nhận COD order trái phép.
- **Khắc phục:** Xoá hardcoded check '000000'.

### BE-C05 — JWT SecretKey trong source code
- [x] **File:** `Flower.Backend/appsettings.json:16`
- **Mô tả:** `"SecretKey": "FlowerShop-SuperSecret-Key..."` trong file committed.
- **Hậu quả:** Bất kỳ ai có repo access có thể forge JWT token admin.
- **Khắc phục:** Chuyển vào environment variables / user secrets.

### BE-C06 — DeliverySlot transaction nesting
- [x] **File:** `Flower.Backend/Services/OrderService.cs:210-218`
- **Mô tả:** `TryLockSlot()` gọi `SaveChangesAsync()` bên trong transaction của `CreateOrder`.
- **Hậu quả:** Slot bị consume vĩnh viễn kể cả khi order thất bại.
- **Khắc phục:** Dùng cùng DbContext, không commit riêng.

### FE-C01 — AuthContext thiếu useMemo
- [x] **File:** `Flower-shop.frontend/src/context/AuthContext.tsx:88`
- **Mô tả:** Context value object được tạo mới mỗi render.
- **Hậu quả:** Tất cả consumer re-render không cần thiết.
- **Khắc phục:** Wrap value object trong `useMemo`.

### FE-C02 — Mobile menu không hoạt động
- [x] **File:** `Flower-shop.frontend/src/components/Header.tsx:158-160`
- **Mô tả:** Hamburger button không có onClick handler.
- **Hậu quả:** Navigation trên mobile bị hỏng hoàn toàn.
- **Khắc phục:** Implement mobile drawer/sheet component.

### FE-C03 — Blog pagination bị ẩn khi filter
- [x] **File:** `Flower-shop.frontend/src/pages/blog/index.tsx:16-18,58`
- **Mô tả:** Pagination ẩn khi chọn category filter.
- **Hậu quả:** Không xem được blog posts page 2+.
- **Khắc phục:** Server-side filtering hoặc luôn show pagination.

### FE-C04 — Sort dropdown không có onChange
- [x] **File:** `Flower-shop.frontend/src/pages/shop/ShopHeader.tsx:19-24`
- **Mô tả:** Dropdown sort không có onChange handler.
- **Hậu quả:** Tính năng sort không hoạt động.
- **Khắc phục:** Thêm onChange + backend sortBy param.

### FE-C05 — JWT không được validate
- [x] **File:** `Flower-shop.frontend/src/context/AuthContext.tsx:15-17`
- **Mô tả:** JWT decode không verify signature, không check expiry.
- **Hậu quả:** Token giả/hết hạn vẫn được chấp nhận.
- **Khắc phục:** Verify signature + exp claim.

### DB-C01 — PriceAdjustment type mismatch
- [x] **File:** `Flower.Data/Migrations/20260705053142_AddRecipientAndProductSnapshot.cs:68`
- **Mô tả:** Entity `decimal(18,0)` vs Migration `decimal(18,2)` vs Designer `decimal(18,0)`.
- **Hậu quả:** EF Core sẽ tạo ALTER migration thừa.
- **Khắc phục:** Đồng bộ tất cả về `decimal(18,2)`.

---

## 🟠 High (14 vấn đề)

### BE-H01 — File upload không validate nội dung
- [x] **Files:** `ProductController.cs:60-73`, `PostController.cs`, `AdvertisementController.cs`
- **Mô tả:** Không check magic bytes, chỉ dựa vào extension.
- **Khắc phục:** Validate magic bytes với SixLabors.ImageSharp.

### BE-H02 — RefreshToken Cookie Secure=false
- [x] **File:** `Flower.Backend/Controllers/AccountController.cs:54`
- **Mô tả:** Cookie gửi qua HTTP không mã hóa.
- **Khắc phục:** Set `Secure = true, SameSite = Lax`.

### BE-H03 — Webhook secret hardcoded
- [x] **File:** `Flower.Backend/appsettings.json:36`
- **Mô tả:** HMAC secret mặc định trong source code.
- **Khắc phục:** Env vars, throw nếu thiếu config.

### BE-H04 — Auto-migration trên startup
- [x] **File:** `Flower.Backend/Program.cs:174`
- **Mô tả:** `MigrateAsync()` chạy mỗi lần khởi động.
- **Khắc phục:** Migration thủ công ở production.

### BE-H05 — AllowedHosts: '*'
- [x] **File:** `Flower.Backend/appsettings.json:38`
- **Mô tả:** Chấp nhận mọi Host header.
- **Khắc phục:** Restrict về domain cụ thể.

### FE-H01 — JWT trong localStorage
- [x] **File:** `Flower-shop.frontend/src/services/tokenService.ts:1-8`
- **Mô tả:** Token lưu trong localStorage, dễ bị XSS đánh cắp.
- **Khắc phục:** Dùng HttpOnly Secure cookie.

### FE-H02 — Homepage fetch tất cả products
- [x] **File:** `Flower-shop.frontend/src/hooks/useProducts.ts:42-48`
- **Mô tả:** `useLatestProducts` gọi API getAll rồi slice client.
- **Khắc phục:** Endpoint `/Products/latest?count=N`.

### FE-H03 — Blog detail fetch tất cả products
- [x] **File:** `Flower-shop.frontend/src/pages/blog-detail/index.tsx:16,46`
- **Mô tả:** Fetch toàn bộ catalog cho 4 recommendations.
- **Khắc phục:** Endpoint `/Products/random?count=4`.

### FE-H04 — PagedResult<any> type erosion
- [x] **File:** `Flower-shop.frontend/src/hooks/useProducts.ts:19,57-62`
- **Mô tả:** 15+ components dùng `any` typed data.
- **Khắc phục:** Type là `PagedResult<Product>`.

### FE-H05 — HeroBanner error không hiển thị
- [x] **File:** `Flower-shop.frontend/src/pages/home/HeroBanner.tsx:16-101`
- **Mô tả:** `error` state set nhưng không render.
- **Khắc phục:** Thêm error render block.

### DB-H01 — UnitPrice decimal(18,0)
- [x] **File:** `Flower.Data/Entities/OrderDetail.cs:17-18`
- **Mô tả:** Mất precision decimal.
- **Khắc phục:** Đổi thành `decimal(18,2)`.

### DB-H02 — Payment.Amount decimal(18,0)
- [x] **File:** `Flower.Data/Entities/Payment.cs:14-15`
- **Mô tả:** Mất precision decimal.
- **Khắc phục:** Đổi thành `decimal(18,2)`.

### DB-H03 — Missing PhoneBlacklist index
- [x] **File:** `Flower.Data/Entities/PhoneBlacklist.cs:12-13`
- **Mô tả:** Full table scan trên hot path.
- **Khắc phục:** Index `(PhoneNumber, IsActive)`.

### DB-H04 — TotalOrders counter sai
- [x] **File:** `Flower.Data/Entities/Customer.cs:30-32`
- **Mô tả:** Chỉ increment cho COD, không cho OnlinePayment.
- **Khắc phục:** Increment cho mọi payment method hoặc bỏ counter.

---

## 🟡 Medium (34 vấn đề)

### Backend (14)
- [x] **BE-M01:** Không validate items list rỗng — Thêm check `items == null || items.Count == 0`
- [x] **BE-M02:** ProcessCODOrder không transaction — Wrap trong transaction
- [x] **BE-M03:** 3 cancellation methods duplicate code — `Cancel()` delegate tới `CancelWithPolicy()`
- [x] **BE-M04:** Date inconsistency Vietnam time vs UtcNow — Dùng `DateTime.UtcNow` cho delivery date check
- [x] **BE-M05:** Rate limiting TOCTOU race condition — Dùng `ConcurrentDictionary.AddOrUpdate` atomic
- [x] **BE-M06:** Delete order không admin check — Thêm `[Authorize(Policy = "AdminOnly")]`
- [x] **BE-M07:** AutoCancelUnverifiedOrders duplicate — Xoá method (đã có trong OrderExpiryBackgroundService)
- [x] **BE-M08:** OrderInputDTO thiếu [Required] — Thêm `[Required]`, `[MinLength]`
- [x] **BE-M09:** SessionValidation DB query mỗi request — Cache token validation 5 phút
- [x] **BE-M10:** OrderCancellationService không logging — Thêm `ILogger<OrderCancellationService>`
- [x] **BE-M11:** Role 'Admin' vs 'Administrator' — Chuẩn hoá về `"Admin"`
- [x] **BE-M12:** PaymentService multiple SaveChangesAsync — Gộp, xoá redundant SaveChangesAsync
- [x] **BE-M13:** Không CSRF protection — Thêm Antiforgery services
- [x] **BE-M14:** Không rate limiting global — Thêm RateLimiter middleware (100 req/min)

### Frontend (14)
- [x] **FE-M01:** CartItem type duplicate — Xoá CartItem khỏi `types/context.ts` (giữ bản trong CartContext)
- [x] **FE-M02:** OrderInput type mismatch — Thêm các field còn thiếu (paymentMethod, deliveryDate...)
- [x] **FE-M03:** paymentMethod validation lỏng — `z.enum(['COD', 'OnlinePayment'])`
- [x] **FE-M04:** Filters không sync URL — Dùng `useSearchParams` sync page/category/price
- [x] **FE-M05:** Phone blur API spam — Debounce 500ms
- [x] **FE-M06:** SEO thiếu OG/Twitter tags — Thêm meta tags
- [x] **FE-M07:** statusConfig duplication — Export `statusStyles` từ OrderComponents, import trong MyOrders
- [x] **FE-M08:** Recipient effect chạy luôn — Chỉ sync khi `recipientIsBuyer` thay đổi
- [x] **FE-M09:** GreetingCard bị mất — Gộp greetingCard vào notes khi gửi order
- [x] **FE-M10:** Form autofill overwrite user input — Dùng `useRef` chạy 1 lần
- [x] **FE-M11:** Sort by ID thay vì createdDate — Sort bằng `createdDate`/`publishedAt`
- [x] **FE-M12:** readOnly conflict với register() — `disabled` thay vì `readOnly`
- [x] **FE-M13:** Orders cache không invalidate products — Thêm `invalidateQueries(['products'])`
- [x] **FE-M14:** MyOrders typed as any — Dùng `Order` type, dùng `statusConfig` cho label

### Database (6)
- [x] **DB-M01:** Missing Customer.Phone index — Thêm index trong OnModelCreating
- [x] **DB-M02:** Missing TokenHash index — Thêm index unique
- [x] **DB-M03:** DeliverySlot cần composite index — `IX_DeliverySlots_ProductId_DeliveryDate_TimeSlot_IsActive`
- [x] **DB-M04:** Missing OrderDate index — `IX_Orders_OrderDate`
- [x] **DB-M05:** Missing ResetToken index — `IX_Customers_ResetToken` (có filter)
- [x] **DB-M06:** Missing ProductVariants nav property — Thêm `ICollection<ProductVariant>? ProductVariants`

---

## 🟢 Low (23 vấn đề)

### Backend (7)
- [ ] **BE-L01:** OrderExpiry scope pattern fragile — `OrderExpiryBackgroundService.cs:48`
- [ ] **BE-L02:** StockLock TTL mismatch — `OrderService.cs:284`
- [ ] **BE-L03:** ToDTO null pattern — `MappingExtensions.cs`
- [ ] **BE-L04:** Random() seed reuse — `SlugHelper.cs:34`
- [ ] **BE-L05:** Unused import RateLimiting — `AuthService.cs:10`
- [ ] **BE-L06:** PUT Cancel nên là POST — `OrdersController.cs:127`
- [ ] **BE-L07:** JWT revoke không blacklist — `AuthService.cs:171`
- [ ] **BE-L08:** Email ParseNotes fragile — `EmailService.cs:425`

### Frontend (11)
- [ ] **FE-L01:** ErrorFallback dùng Bootstrap — `ErrorFallback.tsx:7`
- [ ] **FE-L02:** Social links href="#" — `contact/index.tsx:50`
- [ ] **FE-L03:** Blog buttons không onClick — `blog-detail/index.tsx:99`
- [ ] **FE-L04:** Footer policy links sai — `Footer.tsx:44`
- [ ] **FE-L05:** Delivery estimate sai — `order-confirmation/index.tsx:54`
- [ ] **FE-L06:** DiscountPrice không hiển thị — `ProductCard.tsx:71`
- [ ] **FE-L07:** Video URL hardcoded — `product-detail/index.tsx:29`
- [ ] **FE-L08:** Không max date — `checkout/index.tsx:356`
- [ ] **FE-L09:** getToken() mỗi render — `AuthContext.tsx:88`
- [ ] **FE-L10:** Dead components — `LatestProducts.tsx`
- [ ] **FE-L11:** Keyboard navigation — `ProductCard.tsx:46`
- [ ] **FE-L12:** Currency rounding — `currency.ts:2`

### Database (4)
- [ ] **DB-L01:** Missing Payments nav — `Order.cs`
- [ ] **DB-L02:** Non-sargable search — `Product.cs:15`
- [ ] **DB-L03:** RefreshToken FK không explicit — `ApplicationDbContext.cs`
- [ ] **DB-L04:** Category.Posts null risk — `Category.cs:21`
- [ ] **DB-L05:** DeliverySlot N+1 — `DeliverySlotService.cs:128`

---

## Tổng Kết

| Mức độ | Số lượng | Đã khắc phục | Còn lại |
|--------|----------|--------------|---------|
| 🔴 Critical | 12 | 12 | 0 |
| 🟠 High | 14 | 14 | 0 |
| 🟡 Medium | 34 | 0 | 34 |
| 🟢 Low | 23 | 0 | 23 |
| **Total** | **83** | **26** | **57** |
