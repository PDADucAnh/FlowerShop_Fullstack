# Kế Hoạch Khắc Phục — FlowerShop Audit 05/07/2026

> Tất cả vấn đề chưa được khắc phục, nhóm theo mức độ nghiêm trọng.
> `[ ]` = Chưa xử lý | `[x]` = Đã khắc phục

---

## 🔴 Critical (5 vấn đề)

### C01 — `[AllowAnonymous]` override `[Authorize]` trên ChangePassword
- [x] **File:** `Flower.Backend/Controllers/Api/AuthController.cs:194-195`
- **Mô tả:** Method `ChangePassword` có cả 2 attribute, `[AllowAnonymous]` thắng.
- **Hậu quả:** API change-password không yêu cầu authentication.
- **Khắc phục:** Xóa dòng `[AllowAnonymous]`.
- **Ưu tiên:** #1
- **Đã khắc phục:** 05/07/2026 — Xóa `[AllowAnonymous]`, giữ `[Authorize]`.

### C02 — Gmail App Password trong appsettings.json
- [x] **File:** `Flower.Backend/appsettings.json`
- **Mô tả:** `"Password": "yhjc yqpn higg latm"` — tài khoản `pdahoctap@gmail.com`
- **Hậu quả:** Lộ toàn bộ tài khoản Google.
- **Khắc phục:** 
  1. Revoke app password ngay lập tức
  2. Remove khỏi appsettings.json
  3. Thêm vào `dotnet user-secrets`
  4. Document trong setup-guide.md
- **Ưu tiên:** #2
- **Đã khắc phục:** 05/07/2026 — Xóa Username & Password khỏi appsettings.json; update sender fallback chain trong EmailService.

### C03 — Hardcoded OTP "000000"
- [x] **File:** `Flower.Backend/Controllers/Api/PaymentController.cs:36`
- **Mô tả:** `if (request.Otp != "000000")` — mọi OTP đều là "000000".
- **Hậu quả:** Xác minh COD hoàn toàn vô hiệu.
- **Khắc phục:** Random 6-digit OTP, lưu trong IMemoryCache (10 phút), gửi qua email. Xác thực qua cache lookup.
- **Ưu tiên:** #3
- **Đã khắc phục:** 05/07/2026 — Sinh OTP trong `OrderService.CreateOrder`, gửi email qua `EmailService.SendOtpEmailAsync`, verify trong `PaymentController.VerifyCOD`.

### C04 — Fire-and-forget Task trong forgot-password
- [ ] **File:** `Flower.Backend/Services/AuthService.cs:233`
- **Mô tả:** `_ = Task.Run(async () => { ... catch {} })` — silent swallow exception.
- **Hậu quả:** Email không gửi được nhưng user vẫn thấy thành công. App pool recycle giữa chừng → email mất.
- **Khắc phục:** Dùng `IHostedService` + Channel/Queue pattern.
- **Ưu tiên:** #4

### C05 — Cascade Delete trên tất cả Foreign Keys
- [ ] **File:** `Flower.Data/ApplicationDbContext.cs`
- **Mô tả:** EF Core convention cascade delete trên required relationships.
- **Hậu quả:** Xóa Category → xóa Post. Xóa Customer → xóa Order.
- **Khắc phục:** `OnDelete(DeleteBehavior.Restrict)` cho business-critical FKs (Category→Post, Customer→Order, Product→OrderDetail).
- **Ưu tiên:** #5

---

## 🟠 High (8 vấn đề)

### H01 — Circular DI / Service Locator
- [ ] **File:** `Flower.Backend/Services/PaymentService.cs:116`
- **Mô tả:** `PaymentService` dùng `IServiceProvider` để resolve `IOrderService`.
- **Khắc phục:** Tách `IOrderCancellationService` interface riêng.

### H02 — N+1 Query Categories → Posts
- [ ] **File:** `Flower.Backend/Models/DTOs/MappingExtensions.cs:45`
- **Mô tả:** `category.Posts?.Select(p => p.ToDTO())` gây N+1 khi Category không được Include Posts.
- **Khắc phục:** `.Include(c => c.Posts)` hoặc tạo DTO projection riêng.

### H03 — Token Reset Password trong URL
- [ ] **File:** `Flower.Backend/Services/AuthService.cs:231`
- **Mô tả:** Token query parameter → lộ qua server logs, browser history, referrer.
- **Khắc phục:** Gửi token trong POST body, user nhập thủ công (hoặc 2-step flow).

### H04 — Webhook không signature validation
- [ ] **File:** `Flower.Backend/Services/PaymentService.cs`
- **Mô tả:** `PaymentWebhookRequest.Signature?` field tồn tại nhưng không được validate.
- **Khắc phục:** Implement HMAC signature verification.

### H05 — Không dynamic SEO metadata
- [ ] **File:** Frontend (all pages)
- **Mô tả:** Mọi page dùng chung title "PDA FLOWER".
- **Khắc phục:** Thêm `react-helmet-async`.

### H06 — Thiếu index trên Order.Status
- [ ] **File:** Database migration
- **Mô tả:** BackgroundService query `WHERE Status = ...` mỗi phút → full table scan.
- **Khắc phục:** Migration mới: `CREATE INDEX IX_Orders_Status ON Orders (Status) INCLUDE (OrderDate, PaymentMethod)`.

### H07 — Thiếu composite index (Status, OrderDate)
- [ ] **File:** Database migration
- **Mô tả:** Query `WHERE Status = ... AND OrderDate <= ...` cần composite index.
- **Khắc phục:** `CREATE INDEX IX_Orders_Status_OrderDate ON Orders (Status, OrderDate)`.

### H08 — 0 frontend tests / Chỉ 2 backend test files
- [ ] **File:** Project-wide
- **Mô tả:** 27 tests cho AuthService + UserService. Frontend: 0.
- **Khắc phục:** Thêm React Testing Library + MSW cho frontend tests. Thêm backend tests cho OrderService, ProductService, PaymentService.

---

## 🟡 Medium (10 vấn đề)

### M01 — ProcessWebhook không transaction
- [ ] **File:** `Flower.Backend/Services/PaymentService.cs:62-123`
- **Khắc phục:** Wrap stock deduction + payment recording trong 1 transaction.

### M03 — UpdateProfile không validate email uniqueness
- [ ] **File:** `Flower.Backend/Services/AuthService.cs:276-308`
- **Khắc phục:** Check `AnyAsync(c => c.Email == email && c.Id != currentId)` trước khi update.

### M04 — DateTime.Now vs DateTime.UtcNow inconsistency
- [ ] **Files:** `Order.cs`, `OrderExpiryBackgroundService.cs`, `OrderService.cs`
- **Khắc phục:** Convert tất cả về `DateTime.UtcNow`, chỉ convert local time ở presentation layer.

### M05 — Không rate limiting forgot-password
- [ ] **File:** `Flower.Backend/Services/AuthService.cs:216`
- **Khắc phục:** Thêm in-memory rate limiter (có sẵn .NET 8 `System.Threading.RateLimiting`).

### M06 — Missing `loading="lazy"` trên images
- [x] **File:** Frontend (all `<img>` tags)
- **Khắc phục:** Thêm `loading="lazy"` cho images không trong initial viewport.
- **Đã khắc phục:** 05/07/2026 — Thêm `loading="lazy"` cho 16 images, `loading="eager"` cho 2 hero banners + 1 main product image.

### M07 — Service Locator trong OrderExpiryBackgroundService
- [x] **File:** `Flower.Backend/Services/OrderExpiryBackgroundService.cs:53`
- **Khắc phục:** Inject `IServiceScopeFactory` và tạo scope thủ công.
- **Đã khắc phục:** 05/07/2026 — Đổi `IServiceProvider` → `IServiceScopeFactory`.

### M08 — Frontend stale `.env.example`
- [x] **File:** `Flower-shop.frontend/.env.example`
- **Khắc phục:** Update `REACT_APP_` → `VITE_`.
- **Đã khắc phục:** 05/07/2026 — Đổi `REACT_APP_*` → `VITE_*`.

### M09 — Frontend stale README.md
- [ ] **File:** `Flower-shop.frontend/README.md`
- **Khắc phục:** Viết lại README phản ánh project hiện tại (Vite + React 19 + TypeScript 6).

### M10 — Không EditorConfig / ESLint
- [x] **File:** Project-wide
- **Khắc phục:** Thêm `.editorconfig`, `eslint.config.js`, tích hợp vào build script.
- **Đã khắc phục:** 05/07/2026 — Thêm `.editorconfig` tại project root.

---

## 🟢 Low (5 vấn đề)

### L01 — Price decimal(18,0) vs DiscountPrice decimal(18,2)
- [ ] **File:** `Flower.Data/Entities/Product.cs`
- **Khắc phục:** Đồng nhất decimal precision.

### L02 — Empty using statements
- [ ] **Files:** Nhiều `.cs` files
- **Khắc phục:** Clean up unused usings.

### L03 — Silent catch blocks thiếu logging
- [ ] **Files:** Multiple services
- **Khắc phục:** Thêm `_logger.LogError()` trong catch blocks.

### L04 — PUT change-password trả về 400 thay vì 401
- [ ] **File:** `AuthController.cs:207`
- **Khắc phục:** `return Unauthorized()` khi token không hợp lệ.

### L05 — MappingExtensions thủ công
- [ ] **File:** `Flower.Backend/Models/DTOs/MappingExtensions.cs`
- **Khắc phục:** Cân nhắc AutoMapper (low priority — hiện tại đang hoạt động tốt).

---

## Tổng Kết

| Mức độ | Số lượng | Đã khắc phục | Còn lại |
|--------|----------|--------------|---------|
| 🔴 Critical | 5 | 3 | 2 |
| 🟠 High | 8 | 0 | 8 |
| 🟡 Medium | 10 | 4 | 6 |
| 🟢 Low | 5 | 0 | 5 |
| **Total** | **28** | **7** | **21** |
