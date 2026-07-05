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
| ~~ARCH-03~~ | ~~Service Locator anti-pattern~~ | — | ✅ Đã sửa: IServiceProvider → IServiceScopeFactory + IOrderCancellationService |
| ~~ARCH-04~~ | ~~Circular dependency tiềm ẩn~~ | — | ✅ Đã sửa: Tách IOrderCancellationService |

---

## Backend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| BE-02 | Không audit logging | Medium | Không ghi log cho các hành động quan trọng (create/update/delete) | — |
| BE-05 | Không CORS restriction | Medium | `AllowAnyHeader` + `AllowAnyMethod` quá permissive | — |
| ~~BE-01~~ | ~~MappingExtensions thủ công~~ | — | ✅ Không cần — mapping type-safe, phù hợp quy mô |
| ~~BE-03~~ | ~~Catch blocks silent~~ | — | ✅ Đã sửa: Thêm ILogger + LogError |
| ~~BE-04~~ | ~~DateTime.Now inconsistency~~ | — | ✅ Đã sửa: Đồng bộ DateTime.UtcNow |

---

## Frontend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| FE-02 | Không bundle analysis | Low | Không source map trong production config? Không bundle analyzer | — |
| ~~FE-01~~ | ~~Không ESLint/Prettier~~ | — | ✅ Đã sửa: Thêm .editorconfig, ESLint config |
| ~~FE-03~~ | ~~Không dynamic SEO~~ | — | ✅ Đã sửa: react-helmet-async + SEO component |
| ~~FE-04~~ | ~~Image optimization~~ | — | ✅ Đã sửa: loading="lazy" |
| ~~FE-05~~ | ~~Stale README.md~~ | — | ✅ Đã sửa: Rewrite cho Vite + React 19 |
| ~~FE-06~~ | ~~Stale .env.example~~ | — | ✅ Đã sửa: REACT_APP_ → VITE_ |

---

## Database

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| DB-01 | Enum lưu dạng int | Low | Khó debug trực tiếp trong DB | Nên dùng string converter |
| ~~DB-02~~ | ~~Price decimal không đồng nhất~~ | — | ✅ Đã sửa: decimal(18,2) đồng bộ |
| ~~DB-03~~ | ~~Cascade Delete on all FKs~~ | — | ✅ Đã sửa: DeleteBehavior.Restrict |

---

## Testing

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| T-02 | 0 frontend tests | High | Không Jest, không React Testing Library, không Playwright | — |
| T-03 | Không E2E tests | High | Không Playwright/Cypress | — |
| T-04 | Không integration tests | Medium | Tests chỉ dùng InMemory DB, không SQL Server thật | — |
| T-05 | Không coverage threshold | Medium | Coverlet installed nhưng không có threshold config | — |
| T-01 | Test coverage thấp | Medium | 37 tests (AuthService, UserService, PaymentService). Các service khác 0 tests | — |

---

## Infrastructure

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| I-01 | Không CI/CD | High | Không GitHub Actions, Jenkins, Azure DevOps | — |
| I-03 | Không monitoring | High | Không health checks, không OpenTelemetry, không Application Insights | — |
| I-05 | LocalDB production risk | High | Dùng SQL Server LocalDB — không scale, không backup strategy | Cần xác nhận target môi trường |
| I-02 | Không Docker | Medium | Không containerization | — |
| ~~I-04~~ | ~~Không .editorconfig~~ | — | ✅ Đã sửa: Thêm .editorconfig tại root |

---

## Tổng Quan Nợ Kỹ Thuật Còn Lại

| Khu vực | Số lượng | High | Medium | Low |
|---------|----------|------|--------|-----|
| Kiến trúc | 2 | 0 | 2 | 0 |
| Backend | 2 | 0 | 2 | 0 |
| Frontend | 1 | 0 | 0 | 1 |
| Database | 1 | 0 | 0 | 1 |
| Testing | 4 | 2 | 2 | 0 |
| Infrastructure | 4 | 3 | 1 | 0 |
| **Total** | **14** | **5** | **7** | **2** |
