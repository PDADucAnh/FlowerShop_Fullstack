# Báo Cáo Audit Toàn Diện — FlowerShop (PDA FLOWER)

**Ngày:** 05/07/2026  
**Auditor:** Principal Software Architect (AI-assisted)  
**Phiên bản dự án:** net8.0 / React 19 / TypeScript 6 / SQL Server LocalDB  
**Loại audit:** Full project audit (kiến trúc, backend, frontend, database, API, security, performance, maintainability, documentation)

---

## Tổng Quan Kiến Trúc

```
Frontend (Vite + React 19 SPA) ──API──▶ Backend (ASP.NET Core 8.0 MVC + API) ──EF Core──▶ SQL Server LocalDB
                                          │
                                     SignalR Hub (real-time notification)
                                          │
                                    BackgroundService (OrderExpiry - auto-cancel orders)
```

**4 projects:**
| Project | Type | Target |
|---------|------|--------|
| `Flower.Data` | Class Library | net8.0 |
| `Flower.Backend` | Web App (MVC + API) | net8.0 |
| `Flower-shop.frontend` | Vite + React SPA | — |
| `Flower.Tests` | xUnit Test | net8.0 |

**Hybrid Auth Scheme:** JWT Bearer cho `/api/*`, Cookie Authentication cho MVC admin pages.  
**Database:** 13 tables, 6 migrations, SQL Server LocalDB.

---

## 1. Kiến Trúc

### Severity: High — Circular Dependency (nghi ngờ cần xác minh)
- **File:** `Flower.Backend/Services/PaymentService.cs:116`
- **Nguyên nhân:** `PaymentService` inject `IServiceProvider` để resolve `IOrderService` (service locator anti-pattern). `OrderService` inject `IPaymentService`. Tạo circular dependency tiềm ẩn.
- **Ảnh hưởng:** Khó maintain, khó test, runtime error nếu vòng lặp không được xử lý đúng.
- **Cách khắc phục:** Tách interface `IOrderCancellationService` riêng, hoặc dùng domain events.

### Severity: Medium — Service Locator Anti-Pattern
- **Files:** `PaymentService.cs:116`, `OrderExpiryBackgroundService.cs:53`
- Sử dụng `IServiceProvider.GetRequiredService<>()` thay vì inject trực tiếp.

### Severity: Medium — Module Coupling Cao
- `OrderService` inject 9 dependencies (ILogger, IHttpContextAccessor, IDeliverySlotService, IPaymentService, IFraudDetectionService, StockLockService, IEmailService, TimeSettings) — vi phạm Single Responsibility Principle.

### Severity: Low — Không Repository Pattern Riêng
- Services gọi trực tiếp `IApplicationDbContext`. Thiếu layer abstraction, khó mock testing (hiện dùng InMemory DB).

---

## 2. Backend (ASP.NET Core)

### Severity: CRITICAL — `[AllowAnonymous]` + `[Authorize]` trên cùng method
- **File:** `AuthController.cs:194-195`
- **Method:** `ChangePassword`
- **Nguyên nhân:** `[AllowAnonymous]` override `[Authorize]`, cho phép gọi API change-password KHÔNG CẦN token.
- **Ảnh hưởng:** Bất kỳ ai biết username có thể đổi password.
- **Cách khắc phục:** Xóa `[AllowAnonymous]`.

### Severity: CRITICAL — Gmail App Password trong appsettings.json
- **File:** `appsettings.json`
- **Giá trị:** `"Username": "pdahoctap@gmail.com"`, `"Password": "yhjc yqpn higg latm"`
- **Ảnh hưởng:** Toàn bộ tài khoản Google bị lộ. Có thể bị lạm dụng gửi spam, truy cập dịch vụ Google khác.
- **Cách khắc phục:** Chuyển vào User Secrets / environment variables / vault.

### Severity: CRITICAL — Hardcoded OTP "000000"
- **File:** `PaymentController.cs:36`
- **Logic:** `if (request.Otp != "000000")`
- **Ảnh hưởng:** Vô hiệu hóa hoàn toàn cơ chế xác minh COD.
- **Cách khắc phục:** Tích hợp OTP thật qua SMS/email.

### Severity: HIGH — Fire-and-forget Task (email trong forgot-password)
- **File:** `AuthService.cs:233`
- **Code:** `_ = Task.Run(async () => { ... })` với catch block rỗng.
- **Ảnh hưởng:**
  - Nếu request kết thúc trước, `HttpContext`/scope DI bị dispose → crash.
  - Exception bị silent swallow.
  - Không có logging cho failure.
- **Cách khắc phục:** Dùng `IHostedService` / background job queue.

### Severity: HIGH — Cascade Delete trên tất cả Foreign Keys
- **File:** `ApplicationDbContext.cs` (theo EF Core convention)
- **Ảnh hưởng:** Xóa `Category` → xóa tất cả `Post`. Xóa `Customer` → xóa tất cả `Order`. Mất dữ liệu business-critical.
- **Cách khắc phục:** Set `OnDelete(DeleteBehavior.Restrict)` hoặc `SetNull` cho tất cả required relationships.

### Severity: HIGH — N+1 Query trong CategoryDTO Mapping
- **File:** `MappingExtensions.cs:45`
- **Code:** `Posts = category.Posts?.Select(p => p.ToDTO()).ToList()`
- **Nguyên nhân:** Category không được `.Include(c => c.Posts)` → mỗi category trigger 1 query riêng.
- **Cách khắc phục:** `.Include()` hoặc DTO projection riêng.

### Severity: HIGH — Token Reset Password trong URL
- **File:** `AuthService.cs:231`
- **Code:** `resetLink = $"{clientUrl}/reset-password?token={rawToken}"`
- **Ảnh hưởng:** Token xuất hiện trong URL → server logs, browser history, referrer header.
- **Cách khắc phục:** Gửi token trong POST body, user nhập thủ công.

### Severity: MEDIUM — ProcessWebhook không transaction
- **File:** `PaymentService.cs:62-123`
- Stock deduction và payment recording không atomic → data inconsistent nếu crash giữa chừng.

### Severity: MEDIUM — SessionValidationMiddleware double redirect
- **File:** `SessionValidationMiddleware.cs:34,47`
- `Redirect()` không dừng pipeline → `_next(context)` vẫn chạy sau khi đã redirect.

### Severity: MEDIUM — UpdateProfile không validate email uniqueness
- **File:** `AuthService.cs:276-308`
- Có thể update email thành email đã tồn tại → unique constraint violation.

### Severity: MEDIUM — DateTime.Now vs DateTime.UtcNow inconsistency
- **Files:** `Order.cs` dùng `DateTime.Now` cho OrderDate, `OrderExpiryBackgroundService.cs` dùng `DateTime.Now`, `RefreshToken.cs` dùng `DateTime.UtcNow`.

### Severity: MEDIUM — AutoCancelUnverifiedOrders dùng DateTime.Now
- **File:** `OrderExpiryBackgroundService.cs:55`, `OrderService.cs:605`
- Server timezone thay đổi sẽ ảnh hưởng đến logic timeout.

### Severity: MEDIUM — Không rate limiting trên forgot-password
- **File:** `AuthService.cs:216` — dễ bị email bombing.

### Severity: LOW — Empty using statements
- Nhiều file có `using System;` nhưng không dùng.

### Severity: LOW — Silent catch blocks
- Một số catch blocks không log exception.

---

## 3. Frontend (React + Vite)

> **Lưu ý:** Project là Vite + React SPA, **không phải Next.js**. `index.html` ở root, routing bằng `react-router-dom` v7.

### Severity: HIGH — Không dynamic SEO metadata
- Không có react-helmet hoặc tương đương.
- Mọi page dùng chung title `<title>PDA FLOWER</title>`.
- Product detail, blog detail không có meta tags riêng.

### Severity: MEDIUM — Missing `loading="lazy"` trên images
- Tất cả images load eager → tăng initial load time.

### Severity: MEDIUM — Stale `.env.example`
- Dùng `REACT_APP_` prefix (CRA style), project dùng Vite (`VITE_` prefix).

### Severity: LOW — Frontend README.md là CRA boilerplate
- Không phản ánh project hiện tại (Vite, React 19, TypeScript 6).

### Tích cực:
- ✅ 20/20 pages lazy-loaded via `React.lazy()`
- ✅ TanStack Query với staleTime 5 phút
- ✅ SignalR real-time notifications
- ✅ Zod schemas cho form validation
- ✅ Error boundary + toast notifications
- ✅ DOMPurify cho HTML sanitization
- ✅ Accessibility attributes (26 aria-* attributes)
- ✅ Reduced motion support

---

## 4. Database

### Severity: MEDIUM — Thiếu index trên Order.Status
- Query auto-cancel mỗi phút: `WHERE Status = PendingVerification OR Status = Pending` → full table scan.

### Severity: MEDIUM — Thiếu composite index (Status, OrderDate)
- Query: `WHERE Status = ... AND OrderDate <= ...`

### Severity: MEDIUM — Enum lưu dạng int không có converter
- Khó debug trực tiếp trong DB.

### Severity: LOW — Price decimal(18,0) vs DiscountPrice decimal(18,2)
- Không đồng nhất precision.

### Tích cực:
- ✅ Unique index trên Customer.Email
- ✅ Unique filtered index trên Product.Sku
- ✅ Covering index `IX_Products_BasePrice` trên Products(Price) INCLUDE (Name, Slug, Sku, ImageUrl)
- ✅ FK indexes trên tất cả foreign key columns

---

## 5. API

### Severity: HIGH — Webhook không signature validation
- **File:** `PaymentService.cs` — `PaymentWebhookRequest.Signature?` được định nghĩa nhưng không validate.

### Severity: MEDIUM — Không consistent response format
- Thiếu standard envelope (`{ data, success, errors }`).

### Severity: LOW — PUT change-password trả về 400 thay vì 401 cho lỗi auth

---

## 6. Security (OWASP Top 10)

| OWASP Category | Issue | Severity |
|----------------|-------|----------|
| A01 Broken Access Control | `[AllowAnonymous]` override `[Authorize]` trên ChangePassword | CRITICAL |
| A02 Cryptographic Failures | Gmail App Password trong source code | CRITICAL |
| A04 Insecure Design | OTP hardcoded, webhook không signature, token trong URL, no rate limiting | HIGH |
| A05 Security Misconfiguration | CORS AllowAnyHeader+AllowAnyMethod, không security headers | MEDIUM |
| A07 Identification & Auth Failures | OTP "000000", fire-and-forget email có thể fail silent | CRITICAL |
| A09 Logging & Monitoring Failures | Không structured logging, không health checks, không audit log | HIGH |

---

## 7. Performance

### Backend
| Issue | Severity |
|-------|----------|
| N+1 Query Categories → Posts | HIGH |
| In-memory stock lock không scale multi-instance | MEDIUM |
| GetTrending query 2 lần OrderDetails riêng biệt | MEDIUM |
| `GetAll` Products không pagination (trả về all) | MEDIUM |
| BackgroundService full table scan mỗi phút | MEDIUM |

### Frontend
| Issue | Severity |
|-------|----------|
| No image lazy loading | MEDIUM |

---

## 8. Maintainability

| Issue | Severity |
|-------|----------|
| Không EditorConfig | MEDIUM |
| Frontend không ESLint/Prettier | MEDIUM |
| Không CI/CD pipeline | MEDIUM |
| Không Docker | MEDIUM |
| Test coverage chỉ 2 files backend (27 tests) | HIGH |
| 0 frontend tests | HIGH |
| Service Locator pattern | MEDIUM |
| MappingExtensions.cs thủ công, không AutoMapper | LOW |

---

## 9. Documentation

### Strengths
- ✅ Architecture overview (1018 dòng)
- ✅ Setup guide chi tiết
- ✅ Workflow docs cho từng tính năng (search, price filter, cancellation, forgot-password, CKEditor, env config)
- ✅ Gmail SMTP setup guide
- ✅ 30+ design documents trong `docs/superpowers/`

### Weaknesses
- ❌ Frontend README.md stale (CRA boilerplate)
- ❌ `.env.example` stale (`REACT_APP_` → `VITE_`)

---

## 10. Những Phần Còn Thiếu

| Mục | Status |
|-----|--------|
| Multilanguage support | SQL seed script tồn tại, migration chưa có |
| Real payment gateway | MoMo mock |
| OTP delivery | Hardcoded |
| CI/CD pipeline | ❌ |
| Docker | ❌ |
| Monitoring / Health checks | ❌ |
| Frontend tests | ❌ (0 tests) |
| E2E tests | ❌ |
| Performance testing (Lighthouse, k6) | ❌ |
| Audit logging | ❌ |

---

## 11. Chấm Điểm

| Category | Score | Ghi chú |
|----------|-------|---------|
| Architecture | 7/10 | Circular dependency tiềm ẩn, module coupling cao |
| Backend | 5/10 | Critical security bugs, fire-and-forget, cascade delete |
| Frontend | 7/10 | Code quality tốt, thiếu SEO, thiếu tests |
| Database | 6/10 | Thiếu index quan trọng, cascade delete rủi ro |
| Performance | 6/10 | N+1, full scan, no lazy loading images |
| **Security** | **3/10** | OTP hardcoded, credentials trong code, webhook không auth |
| Maintainability | 5/10 | Thiếu CI/CD, Docker, EditorConfig, ESLint |
| Scalability | 5/10 | In-memory stock lock, LocalDB |
| Testing | 3/10 | 27 tests, 0 frontend tests |
| Documentation | 8/10 | Phong phú, frontend README stale |
| **Overall** | **5.5/10** | Nền tảng tốt, security là điểm yếu chí mạng |

---

## 12. Roadmap Cải Thiện

### Phase 1 — Critical (sửa ngay)
- [ ] Fix `[AllowAnonymous][Authorize]` trên ChangePassword
- [ ] Remove Gmail credentials → User Secrets
- [ ] Replace hardcoded OTP "000000"
- [ ] Sửa fire-and-forget email pattern
- [ ] Thêm cascade delete protection

### Phase 2 — High (trong tuần)
- [ ] Webhook signature validation
- [ ] Thêm index `IX_Orders_Status`, `IX_Orders_Status_OrderDate`
- [ ] Fix N+1 query Categories
- [ ] Convert fire-and-forget → background job queue
- [ ] Thêm rate limiting forgot-password

### Phase 3 — Medium (trong tháng)
- [ ] react-helmet cho SEO
- [ ] Image lazy loading
- [ ] ESLint + EditorConfig
- [ ] Unit tests cho frontend
- [ ] CI/CD pipeline

### Phase 4 — Low (kế hoạch dài hạn)
- [ ] Docker hóa
- [ ] Monitoring (health checks, OpenTelemetry)
- [ ] Payment gateway thật
- [ ] E2E tests (Playwright)
- [ ] Multi-language support
