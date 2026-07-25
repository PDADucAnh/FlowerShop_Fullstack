# Discount / Promotion Feature Completion Design

**Date:** 2026-07-25
**Status:** Draft

## 1. Goal

Complete all missing/incomplete pieces of the discount/promotion/coupon/flash sale feature across backend (ASP.NET Core 8 MVC) and frontend (React).

## 2. Architecture Overview

The feature is split into three independent work groups that can be built in parallel:

```
Group A (Backend Admin MVC)
  └─ FlashSaleController (MVC) + 3 Views + Sidebar Link

Group B (Frontend Display Fixes)
  ├─ Wishlist promotion price
  ├─ CountdownTimer component
  ├─ Cart savings breakdown
  ├─ Cart promotion badges
  └─ Order confirmation details

Group C (Frontend Enhancement)
  ├─ couponService dead code cleanup
  ├─ Shop filter/sort by discount
  └─ Flash Sale landing page
```

Each group is independent — no shared state or sequential dependencies.

---

## 3. Group A — FlashSale Admin MVC

### Files
| Action | File |
|--------|------|
| Create | `Flower.Backend/Controllers/FlashSaleController.cs` |
| Create | `Flower.Backend/Views/FlashSale/Index.cshtml` |
| Create | `Flower.Backend/Views/FlashSale/Create.cshtml` |
| Create | `Flower.Backend/Views/FlashSale/Edit.cshtml` |
| Modify | `Flower.Backend/Views/Shared/_LayoutAdmin.cshtml` |

### Controller Design
- `[Authorize(Policy = "StaffOnly")]` at class level
- `Index()` — GET list, calls `IFlashSaleService.GetAll()`
- `Create()` — GET form + POST. Only Admin
- `Edit(int id)` — GET form + POST. Only Admin
- `Delete(int id)` — POST. Only Admin
- `ToggleActive(int id)` — POST. Only Admin
- Injects `IFlashSaleService` + `INotificationService`
- Notify entity change via `_notificationService.NotifyEntityChanged("FlashSale")`

### View Design
- Follow exact pattern of `Views/Promotion/` (same Tailwind design tokens, same table structure, same status badges)
- Index: table with columns "Tên", "Ngày bắt đầu", "Ngày kết thúc", "Trạng thái", "Thao tác"
- Create/Edit: form with Name, Description, StartDate, EndDate, IsActive
- Product selection via CSV text input (same pattern as Promotion)

### Sidebar
- Insert link after "Khuyến mãi" in `_LayoutAdmin.cshtml`, in the "Tiếp thị" section
- Icon: `local_fire_department`
- Text: "Flash Sale"
- Active detection: `controller == "FlashSale"`

---

## 4. Group B — Frontend Display Fixes

### 4.1 Wishlist Promotion Price

**File:** `Flower-shop.frontend/src/pages/wishlist/index.tsx`

Change price display from:
```tsx
product.discountPrice ?? product.price
```
to:
```tsx
product.promotionPrice ?? product.discountPrice ?? product.price
```

Also add flash sale badge and promotion badge matching `ProductCard.tsx` pattern.

### 4.2 CountdownTimer Component

**File:** `Flower-shop.frontend/src/components/CountdownTimer.tsx`

New component:
```
Props: endTime: string (ISO)
State: { days, hours, minutes, seconds }
- setInterval every 1s
- Display: "Kết thúc trong 2g 30m 15s"
- When expired: "Đã kết thúc"
- Cleanup interval on unmount
```

Consumed by:
- `ProductCard.tsx` — when `hasFlashSale || isFlashSale || promotionType === 'FlashSale'`
- `product-detail/index.tsx` — below the flash sale badge
- Flash Sale page (Group C)

### 4.3 Cart Savings Breakdown

**File:** `Flower-shop.frontend/src/pages/cart/index.tsx`

Add between header and the cart table / sidebar:
- Compute `originalTotal` = sum of `item.price * item.quantity` for all items
- Compute `savings` = `originalTotal - cartTotal`
- If `savings > 0`, show green banner/card: "Bạn tiết kiệm {formatCurrency(savings)} nhờ khuyến mãi"

### 4.4 Cart Promotion Badges

**File:** `Flower-shop.frontend/src/pages/cart/CartTable.tsx`

In the product info column (next to product name):
- If `item.hasFlashSale || item.isFlashSale || item.promotionType === 'FlashSale'`: red badge "Flash Sale"
- Else if `item.promotionPercent`: badge "KM -{promotionPercent}%"
- If `item.promotionPrice`: show discounted price in red with strikethrough original

### 4.5 Order Confirmation Details

**File:** `Flower-shop.frontend/src/pages/order-confirmation/index.tsx`

Expand from just "order ID + status" to:
- Order ID
- Order status with colored badge
- Payment method
- Price breakdown card:
  - Tạm tính: {originalAmount}
  - Giảm giá: -{discountAmount} (nếu có coupon, hiển thị mã)
  - Phí vận chuyển: {shippingFee}
  - Tổng cộng: {finalAmount}

Fetch order data via API (likely from route param or context). Use existing types from `order.ts`.

---

## 5. Group C — Frontend Enhancement

### 5.1 couponService Dead Code Cleanup

**File:** `Flower-shop.frontend/src/pages/checkout/index.tsx`

Replace:
```tsx
const res = await axiosClient.post('/Promotions/apply', { ... });
```
with:
```tsx
import { couponService } from '../../services/couponService';
const res = await couponService.apply({ code, customerId, orderTotal });
```

This ensures consistent API call patterns and makes the `couponService.apply()` method live.

### 5.2 Shop Filter/Sort by Discount

**Files:**
- `Flower-shop.frontend/src/pages/shop/ShopSidebar.tsx`
- `Flower-shop.frontend/src/pages/shop/ShopHeader.tsx`

**ShopSidebar:** Add checkbox "Đang khuyến mãi" — when checked, filters products to only those with `promotionPrice != null || discountPrice != null || hasFlashSale`.

**ShopHeader:** Add sort option "Giảm nhiều nhất" — sorts by `(promotionPercent ?? discountPercent ?? 0)` descending.

### 5.3 Flash Sale Landing Page

**Files:**
- `Flower-shop.frontend/src/pages/flash-sale/index.tsx`
- `Flower-shop.frontend/src/App.tsx` (add route)

Page layout:
- Hero banner with countdown timer (using CountdownTimer from Group B)
- Grid of flash sale products using ProductCard
- Calls `GET /api/FlashSales/active` on mount
- Route: `/flash-sale`

---

## 6. Data Flow

No new backend endpoints needed — all required APIs already exist:
- `GET /api/FlashSales/active` → flash sale landing page
- `POST /api/Coupons/apply` → coupon validation (checkout currently uses `/Promotions/apply` incorrectly)
- `GET /api/Promotions/product/{id}` → product detail page

---

## 7. Implementation Order

Groups can be done in any order. Recommended:

```
Group A → Group B → Group C
```

Reasoning:
- Group A is pure backend (isolated, low risk)
- Group B is frontend display fixes (localized changes, immediate UX benefit)
- Group C builds on Group B (Flash Sale page reuses CountdownTimer from B)

---

## 8. What NOT to Do (YAGNI)

- Do NOT persist coupon state in localStorage — complexity not justified
- Do NOT create React Query hooks for promotions — direct service calls suffice
- Do NOT make size price deltas configurable — hardcoded business rule
- Do NOT deprecate old API endpoints — backward compatibility
- Do NOT add promotion stacking UI — `isStackable` field exists but no product needs it yet

---

## 9. Testing

- Build verification after each commit (`dotnet build`)
- Existing test suite should still pass (`dotnet test`)
- Frontend TypeScript compilation check (`npm run build` or `tsc --noEmit`)
