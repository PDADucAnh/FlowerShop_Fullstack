# Task 3: Orders List Page — Report

## Status: ✅ Completed

## Files Created (3)

| File | Lines |
|------|-------|
| `flower-admin.frontend/src/pages/orders/components/OrderStatusBadge.tsx` | 24 |
| `flower-admin.frontend/src/pages/orders/components/OrderTable.tsx` | 94 |
| `flower-admin.frontend/src/pages/orders/OrdersPage.tsx` | 126 |

## Components

- **OrderStatusBadge** — Renders a `<Badge>` with Vietnamese label + color for each `OrderStatus` enum value (14 statuses supported).
- **OrderTable** — Data table with columns: mã đơn, khách hàng, ngày đặt, tổng tiền, thanh toán (method + status), trạng thái đơn, thao tác (view button). Row click navigates to order detail.
- **OrdersPage** — Main page with search input, status filter tabs (10 groups), paginated card listing. Uses `@tanstack/react-query` for data fetching. Handles loading, error, and empty states.

## Verification

- `npx tsc --noEmit` → **0 errors**

## Commit

```
7c5bda7 feat: add orders list page with status filter tabs
```

## Concerns

- Routing to `/orders/:id` is not yet configured (Task 7 handles router setup).
- `PaymentMethod.OnlinePayment = 0` is displayed as "VNPay" in the table (matches existing convention).
