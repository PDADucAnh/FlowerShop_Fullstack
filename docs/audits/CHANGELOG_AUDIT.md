# Lịch Sử Audit — FlowerShop

> File này ghi lại tất cả các lần audit, các vấn đề phát hiện và tình trạng khắc phục.

---

## Audit #1 — 05/07/2026

### Thông tin chung
- **Ngày:** 05/07/2026 (Chủ nhật)
- **Auditor:** Principal Software Architect (AI-assisted)
- **Phạm vi:** Full project audit (kiến trúc, backend, frontend, database, API, security, performance, maintainability, documentation)
- **Phương pháp:** Source code analysis, dependency mapping, DB schema review, OWASP Top 10 assessment

### Thống kê

| Hạng mục | Số lượng |
|----------|----------|
| Tổng số file source code | ~55+ (.ts/.tsx frontend) + ~30 (.cs backend) |
| Tổng số vấn đề phát hiện | 28 |
| 🔴 Critical | 5 |
| 🟠 High | 8 |
| 🟡 Medium | 10 |
| 🟢 Low | 5 |

### Chi tiết vấn đề phát hiện

| ID | Severity | Category | File | Mô tả | Status |
|----|----------|----------|------|-------|--------|
| C01 | CRITICAL | Security/Auth | `AuthController.cs:194` | `[AllowAnonymous]` override `[Authorize]` trên ChangePassword | ✅ Đã sửa |
| C02 | CRITICAL | Security/Secrets | `appsettings.json` | Gmail App Password trong source code | ✅ Đã sửa |
| C03 | CRITICAL | Security/Auth | `PaymentController.cs:36` | OTP hardcoded "000000" | ✅ Đã sửa |
| C04 | CRITICAL | Backend/Async | `AuthService.cs:233` | Fire-and-forget Task không await, silent catch | Chưa sửa |
| C05 | CRITICAL | Database/DataLoss | `ApplicationDbContext.cs` | Cascade delete trên tất cả FKs | Chưa sửa |
| H01 | HIGH | Architecture | `PaymentService.cs:116` | Circular DI / Service Locator | Chưa sửa |
| H02 | HIGH | Performance | `MappingExtensions.cs:45` | N+1 query Categories → Posts | Chưa sửa |
| H03 | HIGH | Security | `AuthService.cs:231` | Token reset password trong URL | Chưa sửa |
| H04 | HIGH | Security | `PaymentService.cs` | Webhook không signature validation | Chưa sửa |
| H05 | HIGH | Frontend/SEO | Frontend pages | Không dynamic SEO metadata | Chưa sửa |
| H06 | HIGH | Performance/DB | Database | Thiếu index trên Order.Status | Chưa sửa |
| H07 | HIGH | Performance/DB | Database | Thiếu composite index (Status, OrderDate) | Chưa sửa |
| H08 | HIGH | Testing | Project-wide | 0 frontend tests, only 2 backend test files | Chưa sửa |
| M01 | MEDIUM | Backend/Transactions | `PaymentService.cs:62-123` | ProcessWebhook không transaction | Chưa sửa |
| M02 | MEDIUM | Backend | `SessionValidationMiddleware.cs:34,47` | Double redirect, pipeline vẫn chạy | ✅ Không cần — đã có `return;` |
| M03 | MEDIUM | Backend/Validation | `AuthService.cs:276-308` | UpdateProfile không validate email unique | Chưa sửa |
| M04 | MEDIUM | Backend | Multiple files | DateTime.Now vs DateTime.UtcNow inconsistency | Chưa sửa |
| M05 | MEDIUM | Security | `AuthService.cs:216` | Không rate limiting forgot-password | Chưa sửa |
| M06 | MEDIUM | Frontend/Perf | Frontend images | Missing `loading="lazy"` | ✅ Đã sửa |
| M07 | MEDIUM | Architecture | `OrderExpiryBackgroundService.cs:53` | Service Locator pattern | ✅ Đã sửa |
| M08 | MEDIUM | Documentation | `.env.example` | Stale `REACT_APP_` prefix | ✅ Đã sửa |
| M09 | MEDIUM | Documentation | `README.md` | Stale CRA boilerplate README | Chưa sửa |
| M10 | MEDIUM | Tooling | Project-wide | Không EditorConfig / ESLint | ✅ Đã sửa |
| L01 | LOW | Database | `Product.cs` | Price decimal(18,0) vs DiscountPrice decimal(18,2) | Chưa sửa |
| L02 | LOW | Code Quality | Multiple .cs files | Empty using statements | Chưa sửa |
| L03 | LOW | Code Quality | Multiple services | Silent catch blocks thiếu logging | Chưa sửa |
| L04 | LOW | API Design | `AuthController.cs:207` | 400 thay vì 401 cho lỗi auth | Chưa sửa |
| L05 | LOW | Code Quality | `MappingExtensions.cs` | Mapping thủ công, không AutoMapper | Chưa sửa |

### Điểm số

| Category | Score |
|----------|-------|
| Architecture | 7/10 |
| Backend | 5/10 |
| Frontend | 7/10 |
| Database | 6/10 |
| Performance | 6/10 |
| Security | 3/10 |
| Maintainability | 5/10 |
| Scalability | 5/10 |
| Testing | 3/10 |
| Documentation | 8/10 |
| **Overall** | **5.5/10** |

### Khắc phục đợt 1 (05/07/2026)

| ID | Thay đổi | Files | Ghi chú |
|----|----------|-------|---------|
| C01 | Xóa `[AllowAnonymous]` khỏi `ChangePassword` | `AuthController.cs` | API giờ yêu cầu token |
| C02 | Xóa Username & Password khỏi `appsettings.json` | `appsettings.json`, `EmailService.cs` | Dùng User Secrets; update sender fallback |
| C03 | Random OTP 6-digit, cache 10 phút, gửi email | `OrderService.cs`, `PaymentController.cs`, `IEmailService.cs`, `EmailService.cs` | Thay hardcoded "000000" |

### Khắc phục đợt 2 (05/07/2026)

| ID | Thay đổi | Files | Ghi chú |
|----|----------|-------|---------|
| M06 | Thêm `loading="lazy"` cho 16 images, `loading="eager"` cho 3 images above-fold | 10 files frontend | Cải thiện LCP performance |
| M07 | Đổi `IServiceProvider` → `IServiceScopeFactory` | `OrderExpiryBackgroundService.cs` | Best practice cho BackgroundService |
| M08 | Đổi `REACT_APP_*` → `VITE_*` | `.env.example` | Khớp với Vite convention hiện tại |
| M10 | Thêm `.editorconfig` tại project root | `.editorconfig` | Chuẩn hóa coding style |

### Tổng kết
- Phát hiện **28 vấn đề**, đã khắc phục **7** (3 Critical + 4 Medium), còn **21 vấn đề**
- Security cải thiện từ 3/10 lên ~5/10 sau 3 fixes
- Testing cần cải thiện đáng kể (3/10)
- Documentation là điểm mạnh (8/10)
- Infrastructure (CI/CD, Docker, monitoring) hoàn toàn không có
