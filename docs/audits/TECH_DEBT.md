# Nợ Kỹ Thuật — FlowerShop

> Tài liệu này ghi lại các khoản nợ kỹ thuật (technical debt) còn tồn tại.
> Không bao gồm lỗi (bugs) — bugs được ghi trong FIX_PLAN.md.
> Cập nhật: 05/07/2026

---

## Kiến Trúc

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| ARCH-01 | Không Repository pattern | Medium | Services gọi trực tiếp `IApplicationDbContext`. Khó mock, khó test unit. | Hiện dùng InMemory DB cho tests |
| ARCH-02 | Module coupling cao | Medium | `OrderService` inject 9 dependencies, vi phạm SRP | — |
| ARCH-03 | Service Locator anti-pattern | Medium | `PaymentService.cs:116`, `OrderExpiryBackgroundService.cs:53` dùng `IServiceProvider.GetRequiredService<>()` | — |
| ARCH-04 | Circular dependency tiềm ẩn | High | `PaymentService` → `IOrderService` → `IPaymentService` | Nghi ngờ cần xác minh |

---

## Backend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| BE-01 | MappingExtensions thủ công | Low | Không AutoMapper, mapping code viết tay dài, dễ quên field | Dễ bảo trì nhưng nhiều code |
| BE-02 | Không audit logging | Medium | Không ghi log cho các hành động quan trọng (create/update/delete) | — |
| BE-03 | Catch blocks silent | Low | Một số catch không log exception | — |
| BE-04 | DateTime.Now inconsistency | Medium | Hỗn hợp `DateTime.Now` và `DateTime.UtcNow` | Cần chuẩn hóa |
| BE-05 | Không CORS restriction | Medium | `AllowAnyHeader` + `AllowAnyMethod` quá permissive | — |

---

## Frontend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| FE-01 | Không ESLint/Prettier | Medium | Không code quality tool cho frontend | — |
| FE-02 | Không bundle analysis | Low | Không source map trong production config? Không bundle analyzer | — |
| FE-03 | Không dynamic SEO | High | Không react-helmet, mọi page chung title | Ảnh hưởng SEO |
| FE-04 | Image optimization | Medium | `<img>` tags không `loading="lazy"`, không `srcSet` | — |
| FE-05 | Stale README.md | Low | CRA boilerplate, không reflect project hiện tại | — |
| FE-06 | Stale .env.example | Low | `REACT_APP_` prefix (CRA) nhưng project dùng Vite | — |

---

## Database

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| DB-01 | Enum lưu dạng int | Low | Khó debug trực tiếp trong DB | Nên dùng string converter |
| DB-02 | Price decimal không đồng nhất | Low | `decimal(18,0)` cho Price, `decimal(18,2)` cho DiscountPrice | — |
| DB-03 | Cascade Delete on all FKs | High | Business-critical data có thể bị xóa hàng loạt | Cần Restrict hoặc SetNull |

---

## Testing

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| T-01 | Chỉ 2 test files | High | AuthService (11 tests) + UserService (16 tests). Các service khác 0 tests | — |
| T-02 | 0 frontend tests | High | Không Jest, không React Testing Library, không Playwright | — |
| T-03 | Không E2E tests | High | Không Playwright/Cypress | — |
| T-04 | Không integration tests | Medium | Tests chỉ dùng InMemory DB, không SQL Server thật | — |
| T-05 | Không coverage threshold | Medium | Coverlet installed nhưng không có threshold config | — |

---

## Infrastructure

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| I-01 | Không CI/CD | High | Không GitHub Actions, Jenkins, Azure DevOps | — |
| I-02 | Không Docker | Medium | Không containerization | — |
| I-03 | Không monitoring | High | Không health checks, không OpenTelemetry, không Application Insights | — |
| I-04 | Không .editorconfig | Medium | Coding style không enforced | — |
| I-05 | LocalDB production risk | High | Dùng SQL Server LocalDB — không scale, không backup strategy | Cần xác nhận target môi trường |

---

## Tổng Quan Nợ Kỹ Thuật

| Khu vực | Số lượng | High | Medium | Low |
|---------|----------|------|--------|-----|
| Kiến trúc | 4 | 1 | 3 | 0 |
| Backend | 5 | 0 | 4 | 1 |
| Frontend | 6 | 1 | 3 | 2 |
| Database | 3 | 1 | 0 | 2 |
| Testing | 5 | 3 | 2 | 0 |
| Infrastructure | 5 | 3 | 2 | 0 |
| **Total** | **28** | **9** | **14** | **5** |
