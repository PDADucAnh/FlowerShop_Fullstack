# Task 2 Report — Frontend Types + API Modules

## Status: ✅ Complete

## What was done

Created 6 files under `flower-admin.frontend/`:

| File | Description |
|------|-------------|
| `src/types/order.ts` | `OrderStatus`, `PaymentMethod`, `PaymentStatus` enums + `OrderDTO`, `OrderDetailDTO` interfaces |
| `src/types/customer.ts` | `CustomerDTO`, `UpdateCustomerRequest` interfaces |
| `src/types/contact.ts` | `ContactDTO` interface |
| `src/api/orders.ts` | `ordersApi` — `getPaged`, `getById`, `updateStatus`, `cancelByShop` |
| `src/api/customers.ts` | `customersApi` — `getPaged`, `getById`, `getOrders`, `update` |
| `src/api/contacts.ts` | `contactsApi` — `getPaged`, `getById`, `getUnreadCount`, `markRead`, `delete` |

## TypeScript check

`npx tsc --noEmit` → **0 errors**

## Commit

```
231ba6d feat: add frontend types and API modules for orders, customers, contacts
```

6 files changed, 202 insertions(+)

## Concerns

None.
