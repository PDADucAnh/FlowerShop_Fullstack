# Nợ Kỹ Thuật — FlowerShop

> Tài liệu này ghi lại các khoản nợ kỹ thuật (technical debt) còn tồn tại.
> Không bao gồm lỗi (bugs) — bugs được ghi trong FIX_PLAN.md.
> Cập nhật: 05/07/2026 (Audit #3)

---

## Kiến Trúc

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| ARCH-01 | Không Repository pattern | Medium | Services gọi trực tiếp IApplicationDbContext | — |
| ARCH-02 | Module coupling cao | Medium | OrderService inject 9 dependencies | — |
| ARCH-03 | Code duplication cancellation | Medium | 3 methods duplicate stock restore logic | — |
| ARCH-04 | Role name inconsistency | Low | 'Admin' vs 'Administrator' gây nhầm lẫn | — |

---

## Backend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| BE-01 | No audit logging | Medium | Không ghi log cho actions quan trọng | — |
| BE-02 | CORS permissive | Medium | AllowAnyHeader + AllowAnyMethod | — |
| BE-03 | No rate limiting global | Medium | Login, ForgotPassword dễ brute-force | — |
| BE-04 | No CSRF protection | Medium | MVC POST actions không anti-forgery | — |
| BE-05 | JWT revoke không blacklist | Low | Token còn hiệu lực 60 phút sau revoke | — |
| BE-06 | Email ParseNotes fragile | Low | Split by '|' dễ gãy với nội dung user | — |

---

## Frontend

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| FE-01 | Không bundle analysis | Low | Không source map / bundle analyzer | — |
| FE-02 | Duplicate CartItem type | Low | types/context.ts và CartContext.tsx | — |
| FE-03 | statusConfig duplication | Low | OrderComponents vs MyOrders | — |
| FE-04 | Type erosion widespread | Medium | useProductsPaged trả về any | 15+ components bị ảnh hưởng |
| FE-05 | Filters không sync URL | Medium | Không share được filtered results | — |
| FE-06 | Dead components | Low | LatestProducts, CategoryMenu không dùng | — |
| FE-07 | DiscountPrice không hiển thị | Low | Card chỉ show item.price | — |

---

## Database

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| DB-01 | Enum lưu dạng int | Low | Khó debug trực tiếp trong DB | — |
| DB-02 | Non-sargable search | Low | LIKE '%term%' không dùng được index | — |
| DB-03 | Missing nav properties | Low | Product.ProductVariants, Order.Payments | — |
| DB-04 | TotalOrders counter manual | Medium | Denormalized counter dễ sai | — |
| DB-05 | DeliverySlot N+1 | Medium | Load all products rồi query từng slot | — |

---

## Testing

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| T-01 | Test coverage thấp | High | 37 tests, nhiều service 0 tests | — |
| T-02 | 0 frontend tests | High | Không Jest, không RTL, không Playwright | — |
| T-03 | Không E2E tests | High | Không Playwright/Cypress | — |
| T-04 | Không integration tests | Medium | Chỉ dùng InMemory DB | — |
| T-05 | Không coverage threshold | Medium | Coverlet không có threshold config | — |

---

## Infrastructure

| ID | Mục | Mức độ | Mô tả | Ghi chú |
|----|-----|--------|-------|---------|
| I-01 | Không CI/CD | High | Không GitHub Actions / Azure DevOps | — |
| I-02 | Không Docker | Medium | Không containerization | — |
| I-03 | Không monitoring | High | Không health checks, OpenTelemetry | — |
| I-04 | LocalDB production risk | High | Không scale, không backup strategy | — |
| I-05 | Auto-migration on startup | High | Destructive migration tự động chạy | — |

---

## Tổng Quan Nợ Kỹ Thuật Còn Lại

| Khu vực | Số lượng | High | Medium | Low |
|---------|----------|------|--------|-----|
| Kiến trúc | 4 | 0 | 3 | 1 |
| Backend | 6 | 0 | 4 | 2 |
| Frontend | 7 | 0 | 2 | 5 |
| Database | 5 | 0 | 2 | 3 |
| Testing | 5 | 3 | 2 | 0 |
| Infrastructure | 5 | 4 | 1 | 0 |
| **Total** | **32** | **7** | **14** | **11** |
