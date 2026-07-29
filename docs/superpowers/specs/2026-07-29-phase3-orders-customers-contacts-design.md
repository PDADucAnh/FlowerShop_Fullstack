# Phase 3 Spec: Orders, Customers & Contacts (Admin SPA)

**Date:** 2026-07-29
**Project:** FlowerShop Admin SPA
**Status:** Draft

---

## 1. Overview

Phase 3 builds the Orders, Customers, and Contact inquiries management UI for the admin SPA. It covers:

- Orders list with status filter tabs, search, pagination
- Order detail page with product line items, status updating, cancellation, print invoice
- Customers list with search and pagination
- Customer detail page with purchase stats, order history, edit dialog
- Contacts list with read/unread filter tabs, unread count badge
- Contact detail page with full message view, mark read/unread, delete
- Backend additions: paginated endpoints for orders/customers/contacts, dedicated status update endpoint

---

## 2. Backend Changes

### 2.1 Existing Backend (unchanged, used as-is)

The following controllers and services already exist and require no modification:

| Controller | Key Methods |
|-----------|-------------|
| `OrdersController` | `GET /api/orders/{id}`, `PUT /api/orders/{id}/cancel-by-shop` (cancel with reason), `PUT /api/orders/{id}/cancel` (customer cancel) |
| `OrderDetailsController` | `GET /api/order-details/order/{orderId}` |
| `CustomersController` | `GET /api/customers/{id}`, `PUT /api/customers/{id}`, `DELETE /api/customers/{id}` |
| `ContactsController` | `GET /api/contacts/{id}`, `PUT /api/contacts/{id}/read`, `DELETE /api/contacts/{id}`, `GET /api/contacts/unread-count` |

| Service | Key Methods |
|---------|-------------|
| `IOrderService` | `GetDetail`, `CancelByCustomer`, `CancelByShop`, `CancelWithReason`, `IsPhoneBlacklisted` |
| `ICustomerService` | `GetById`, `Update`, `Delete` |
| `IContactService` | `GetById`, `MarkRead`, `Delete`, `GetUnreadCount` |

### 2.2 New API Endpoints

#### Orders

**`GET /api/orders/paged`**

Paginated orders with filtering and search.

```
Query params:
  page        int (default 1)
  pageSize    int (default 20)
  status      string? (optional, single status or comma-separated for grouped filters, e.g. "Cancelled,CancelledByCustomer,CancelledByShop")
  search      string? (optional, searches Customer name or Phone)
  dateFrom    DateTime? (optional)
  dateTo      DateTime? (optional)
```

Returns `PagedResult<OrderDTO>`. The `status` param is parsed on the backend: if it contains commas, split into multiple statuses for the filter query.

New action on `OrdersController`. Backed by a new `IOrderService.GetPaged(GetPagedOrdersQuery query)` overload that includes filtering and search logic.

**`PUT /api/orders/{id}/status`**

Dedicated status update endpoint (no need to send full `UpdateOrderDTO`).

```json
// Request body
{
  "status": "Confirmed"  // OrderStatus enum value as string
}
```

Returns `204 No Content` on success, `404` if order not found.

New action on `OrdersController`. Backed by a new `IOrderService.UpdateStatus(int id, OrderStatus newStatus) → bool` method. Implementation:
- Loads the order from DB
- Records `oldStatus` before change
- Sets `order.Status = newStatus`
- Calls `SaveChangesAsync`
- Triggers email notifications for status transitions to Confirmed / Shipping / Completed (same side-effect logic as existing `Update()`)
- Returns `true` on success

No separate print endpoint needed. The frontend uses data from `GET /api/orders/{id}` and renders a print-optimized layout via `window.print()` with `@media print` CSS to hide sidebar/navigation.

#### Customers

**`GET /api/customers/paged`**

Paginated customers with search.

```
Query params:
  page        int (default 1)
  pageSize    int (default 20)
  search      string? (optional, searches FullName, Email, Phone)
```

Returns `PagedResult<CustomerDTO>`.

New action on `CustomersController`. Backed by a new `ICustomerService.GetPaged(GetPagedCustomersQuery query)` overload.

**Note:** `CustomerDTO` gains a `CreatedAt` field (exists on entity but missing from DTO):

```csharp
// Add to CustomerDTO:
public DateTime CreatedAt { get; set; }
```

**`GET /api/customers/{id}/orders`**

Returns the customer's order history.

```
Query params:
  page        int (default 1)
  pageSize    int (default 10)
```

Returns `PagedResult<OrderDTO>` filtered by `CustomerId == id`.

New action on `CustomersController`. Backed by `IOrderService.GetPaged` with customer filter.

#### Contacts

**`GET /api/contacts/paged`**

Paginated contacts with read/unread filter.

```
Query params:
  page        int (default 1)
  pageSize    int (default 20)
  isRead      bool? (optional, if omitted returns all)
```

Returns `PagedResult<ContactDTO>`.

New action on `ContactsController`. Backed by a new `IContactService.GetPaged(GetPagedContactsQuery query)` method. The existing service only has `GetAll()` (no pagination).

---

## 3. Frontend Pages

### 3.1 Orders List (`/orders`)

Replace the current placeholder `OrdersPage`.

**Filter tabs (horizontal pills):**

| Tab | Filter Value(s) |
|-----|----------------|
| Tất cả | (none) |
| Chờ xác nhận | `PendingVerification` |
| Đã xác nhận | `Confirmed` |
| Đang xử lý | `Preparing` + `ReadyForDelivery` |
| Đang giao | `Shipping` |
| Đã giao | `Completed` |
| Đã hủy | `Cancelled` + `CancelledByCustomer` + `CancelledByShop` |
| Chờ thanh toán | `PendingPayment` |
| Đã thanh toán | `Paid` |
| Hoàn tiền | `RefundPending` + `Refunded` |

Active tab highlighted with primary color. Clicking a tab sets `?status=` query param and resets page to 1.

**Search bar:** Text input with search icon, debounced 400ms, searches by customer name or phone.

**DataTable columns:**

| Column | Source | Notes |
|--------|--------|-------|
| Mã đơn | `order.id` | Prefixed as `#ID`, linked to detail page |
| Khách hàng | `order.customerName` | — |
| Ngày đặt | `order.orderDate` | Formatted as `dd/MM/yyyy HH:mm` |
| Tổng tiền | `order.finalAmount` | Formatted with commas, `₫` suffix |
| Thanh toán | `order.paymentMethod` + `order.paymentStatus` | Two badges side by side |
| Trạng thái | `order.status` | Colored status badge |
| Thao tác | — | "Xem" button → `/orders/:id` |

**Status badge colors:**

| Status | Color |
|--------|-------|
| Chờ xác nhận / Chờ thanh toán | `warning` (amber) |
| Đã xác nhận / Đang xử lý | `info` (blue) |
| Sẵn sàng giao | `info` (teal) |
| Đang giao | `primary` (indigo) |
| Đã giao | `success` (green) |
| Đã hủy (any) | `error` (red) |
| Hoàn tiền / Chờ hoàn tiền | `secondary` (gray) |

**Row click:** Entire row clickable → navigate to `/orders/:id`.

**Pagination:** Server-side, 20 per page.

**States:** Loading skeleton, empty state ("Không có đơn hàng nào"), error state with retry.

### 3.2 Order Detail (`/orders/:id`)

**Header section:**
- Left: Mã đơn (`#ID`) + Order date + Status badge (large)
- Right: "Cập nhật trạng thái" dropdown + "In phiếu giao" button + "Hủy đơn" button

**Info cards (3-column grid on wide screens, stacked on mobile):**

1. **Khách hàng**
   - Tên, Email, SĐT, Địa chỉ

2. **Thanh toán**
   - Phương thức: COD / VNPay
   - Trạng thái thanh toán badge
   - Mã giao dịch (nếu có)
   - Tạm tính → Giảm giá → Phí ship → Tổng cộng

3. **Giao hàng**
   - Ngày giao, Khung giờ
   - Địa chỉ giao (Đường, Quận/Huyện)
   - Người nhận, SĐT người nhận

**Order details table:**
| STT | Sản phẩm (ảnh + tên) | Size | SL | Đơn giá | Giảm giá | Thành tiền |

**Totals section:**
```
Tạm tính:        xxx ₫
Giảm giá:       -xxx ₫
Phí ship:        xxx ₫
───────────────
Tổng cộng:      xxx ₫
```

**Status update:**
- Dropdown select listing valid next statuses
- "Lưu" button → calls `PUT /api/orders/{id}/status` with `{ status }`
- Success toast + refetch order data (invalidate `['order', id]`)
- Loading state on the button during save
- If order is in a terminal state (Cancelled, Completed, Refunded), dropdown is hidden

**Cancel order:**
- "Hủy đơn" button calls `PUT /api/orders/{id}/cancel-by-shop`
- Opens confirmation dialog with:
  - Warning text
  - Lý do hủy textarea (required)
  - Confirm: "Xác nhận hủy" (red), Cancel: "Hủy"
- On success: update status, success toast, refetch

**Print Invoice ("In phiếu giao"):**
- Button with Printer icon in the header
- Uses existing `OrderDTO` data + `window.print()` with `@media print` CSS
- Print layout: clean sheet with shop header, order info, customer info, line items table, totals, and signature lines
- Navigation, sidebar, and action buttons hidden via `display: none` in print CSS

**States:** Loading skeleton, not found ("Không tìm thấy đơn hàng"), error state.

### 3.3 Customers List (`/customers`)

**Search bar:** Text input searching by Tên, Email, SĐT (debounced 400ms).

**DataTable columns:**

| Column | Source | Notes |
|--------|--------|-------|
| Tên khách hàng | `customer.fullName` | Linked to detail page |
| Email | `customer.email` | — |
| SĐT | `customer.phone` | — |
| Tổng đơn | `customer.totalOrders` | — |
| Trạng thái | `customer.isActive` | Green dot = Active, Gray dot = Inactive |

**Row click:** Entire row clickable → `/customers/:id`.

**Pagination:** Server-side, 20 per page.

**States:** Loading skeleton, empty state ("Chưa có khách hàng nào"), error state.

### 3.4 Customer Detail (`/customers/:id`)

**Profile card:**
- Avatar placeholder (first letter of name)
- Tên, Email, SĐT, Địa chỉ
- Ngày tham gia (`createdAt`)
- Trạng thái (Đang hoạt động / Ngưng) badge

**Stats row (4 stat cards):**
| Stat | Source |
|------|--------|
| Tổng đơn hàng | `customer.totalOrders` |
| Giao thành công | `customer.successfulDeliveries` |
| Giao thất bại | `customer.failedDeliveries` |
| Fraud Score | `customer.fraudScore` (with color: green < 20, amber 20-50, red > 50) |

**Blacklist indicator:** If `customer.isBlacklisted`, show a prominent red warning banner with fraud score.

**Order history table:**
- Fetched from `GET /api/customers/{id}/orders`
- Columns: Mã đơn (linked → `/orders/:id`), Ngày đặt, Tổng tiền, Trạng thái, Thanh toán
- Paginated (10 per page, server-side)

**Edit button:**
- Opens dialog with form fields: Tên, Email, SĐT, Địa chỉ, Trạng thái (active/inactive)
- Calls `PUT /api/customers/{id}`
- On success: update profile card, success toast

**States:** Loading skeleton, not found, error.

### 3.5 Contacts List (`/contacts`)

**Filter tabs:**

| Tab | `isRead` filter | Notes |
|-----|----------------|-------|
| Tất cả | (none) | Default |
| Chưa đọc | `false` | Shows unread count badge next to label |
| Đã đọc | `true` | — |

Unread count fetched from `GET /api/contacts/unread-count`.

**DataTable columns:**

| Column | Source | Notes |
|--------|--------|-------|
| Người gửi | `contact.name` | Bold if unread |
| Email | `contact.email` | — |
| Tiêu đề | `contact.subject` | Bold if unread, truncated to 60 chars |
| Ngày gửi | `contact.createdAt` | `dd/MM/yyyy HH:mm` |
| Trạng thái | `contact.isRead` | Badge: Chưa đọc (blue) / Đã đọc (gray) |
| Thao tác | — | Mark read/unread toggle icon + Delete icon |

**Unread row styling:** Entire row has semibold font weight when `isRead === false`.

**Mark read/unread:** Click icon toggles `PUT /api/contacts/{id}/read` with `{ isRead: !current }`. Updates row immediately.

**Delete:** Click trash icon → confirmation dialog → `DELETE /api/contacts/{id}` → remove row.

**Row click → detail:** Click row (not on action icons) navigates to `/contacts/:id`.

**Pagination:** Server-side, 20 per page.

**States:** Loading skeleton, empty state with contextual message ("Không có liên hệ nào" / "Không có liên hệ chưa đọc"), error state.

### 3.6 Contact Detail (`/contacts/:id`)

**Header:**
- Người gửi · Email · SĐT (if present)
- Ngày gửi · Trạng thái read/unread badge

**Message card:**
- Subject as card title
- Full message content (preserving line breaks)

**Actions bar:**
- "Đánh dấu đã đọc" / "Đánh dấu chưa đọc" toggle button
- "Xoá" button (red) with confirmation dialog
- "Quay lại" link → `/contacts`

**States:** Loading skeleton, not found, error.

---

## 4. Frontend Implementation Details

### 4.1 File Structure

```
src/
├── types/
│   ├── order.ts              # NEW
│   ├── customer.ts           # NEW
│   └── contact.ts            # NEW
├── api/
│   ├── orders.ts             # NEW
│   ├── customers.ts          # NEW
│   └── contacts.ts           # NEW
├── pages/
│   ├── orders/
│   │   ├── OrdersPage.tsx           # NEW
│   │   ├── OrderDetailPage.tsx      # NEW
│   │   └── components/
│   │       ├── OrderTable.tsx        # NEW
│   │       ├── OrderStatusBadge.tsx  # NEW
│   │       ├── OrderStatusDialog.tsx # NEW
│   │       └── CancelOrderDialog.tsx # NEW
│   ├── customers/
│   │   ├── CustomersPage.tsx             # NEW
│   │   ├── CustomerDetailPage.tsx        # NEW
│   │   └── components/
│   │       ├── CustomerTable.tsx          # NEW
│   │       └── CustomerEditDialog.tsx     # NEW
│   └── contacts/
│       ├── ContactsPage.tsx            # NEW
│       ├── ContactDetailPage.tsx       # NEW
│       └── components/
│           └── ContactTable.tsx         # NEW
├── components/
│   └── AppSidebar.tsx            # MODIFIED: add Khách hàng, Liên hệ
└── App.tsx                       # MODIFIED: add routes, remove OrdersPage from placeholder
```

### 4.2 Type Definitions

```typescript
// types/order.ts
export enum OrderStatus {
  Pending = 0,
  Shipping = 1,
  Completed = 2,
  Cancelled = 3,
  PendingVerification = 4,
  Confirmed = 5,
  Preparing = 6,
  PendingPayment = 7,
  Paid = 8,
  ReadyForDelivery = 9,
  Refunded = 10,
  CancelledByCustomer = 11,
  CancelledByShop = 12,
  RefundPending = 13,
}

export enum PaymentMethod {
  OnlinePayment = 0,
  COD = 1,
}

export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
  Refunded = 3,
  // ... other values as needed
}

export interface OrderDTO {
  id: number
  orderDate: string
  customerId: number
  customerName?: string
  customerEmail?: string
  customerPhone?: string
  status: OrderStatus
  statusDisplay: string
  notes?: string
  orderDetails?: OrderDetailDTO[]
  paymentMethod: PaymentMethod
  paymentStatus: PaymentStatus
  paymentTransactionId?: string
  paymentPaidAt?: string
  deliveryDate?: string
  deliveryTimeSlot?: string
  deliveryDistrict?: string
  deliveryAddress?: string
  recipientName?: string
  recipientPhone?: string
  cancelledAt?: string
  cancellationReason?: string
  cancelledBy?: string
  cancellationFee: number
  isVerified: boolean
  refundAmount: number
  refundRequestedAt?: string
  refundCompletedAt?: string
  promotionId?: number
  promotionName?: string
  couponId?: number
  couponCode?: string
  originalAmount: number
  discountAmount: number
  finalAmount: number
  shippingFee: number
  canCancel: boolean
}

export interface OrderDetailDTO {
  id: number
  orderId: number
  productId: number
  productName?: string
  productImageUrl?: string
  sizeVariant?: string
  customerName?: string
  quantity: number
  unitPrice: number
  originalPrice: number
  discountAmount: number
  subtotal: number
}

// types/customer.ts
export interface CustomerDTO {
  id: number
  fullName: string
  email: string
  phone?: string
  address?: string
  totalOrders: number
  successfulDeliveries: number
  failedDeliveries: number
  isBlacklisted: boolean
  fraudScore: number
  isActive: boolean
  createdAt: string
}

export interface UpdateCustomerRequest {
  id: number
  fullName: string
  email: string
  phone?: string
  address?: string
  isActive: boolean
}

// types/contact.ts
export interface ContactDTO {
  id: number
  name: string
  email: string
  phone?: string
  subject: string
  message: string
  isRead: boolean
  readAt?: string
  createdAt: string
}

export interface MarkReadRequest {
  isRead: boolean
}
```

### 4.3 API Functions

```typescript
// api/orders.ts
export const ordersApi = {
  getPaged(params: {
    page?: number; pageSize?: number;
    status?: OrderStatus | null;
    search?: string;
    dateFrom?: string; dateTo?: string;
  }) {
    return apiClient.get<PagedResponse<OrderDTO>>('/api/orders/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<OrderDTO>(`/api/orders/${id}`)
  },
  updateStatus(id: number, status: OrderStatus) {
    return apiClient.put(`/api/orders/${id}/status`, { status })
  },
  cancelByShop(id: number, reason: string) {
    return apiClient.put(`/api/orders/${id}/cancel-by-shop`, { reason })
  },
}

// api/customers.ts
export const customersApi = {
  getPaged(params: { page?: number; pageSize?: number; search?: string }) {
    return apiClient.get<PagedResponse<CustomerDTO>>('/api/customers/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<CustomerDTO>(`/api/customers/${id}`)
  },
  getOrders(id: number, params: { page?: number; pageSize?: number }) {
    return apiClient.get<PagedResponse<OrderDTO>>(`/api/customers/${id}/orders`, { params })
  },
  update(id: number, data: UpdateCustomerRequest) {
    return apiClient.put(`/api/customers/${id}`, data)
  },
}

// api/contacts.ts
export const contactsApi = {
  getPaged(params: { page?: number; pageSize?: number; isRead?: boolean }) {
    return apiClient.get<PagedResponse<ContactDTO>>('/api/contacts/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<ContactDTO>(`/api/contacts/${id}`)
  },
  getUnreadCount() {
    return apiClient.get<{ count: number }>('/api/contacts/unread-count')
  },
  markRead(id: number, isRead: boolean) {
    return apiClient.put(`/api/contacts/${id}/read`, { isRead })
  },
  delete(id: number) {
    return apiClient.delete(`/api/contacts/${id}`)
  },
}
```

### 4.4 Data Fetching Strategy

All pages use `@tanstack/react-query`:

- **List pages:** `useQuery` with filter/pagination params as query key. `keepPreviousData` for smooth pagination.
- **Detail pages:** `useQuery(['entity', id])` for single item.
- **Mutations:** `useMutation` with `onSuccess` invalidating relevant queries.
  - Status update → invalidate `['orders']` + `['order', id]`
  - Cancel order → invalidate `['orders']` + `['order', id]`
  - Mark read → invalidate `['contacts']` + `['contact', id]` + `['contacts-unread']`
  - Delete contact → invalidate `['contacts']`
  - Edit customer → invalidate `['customers']` + `['customer', id]`

### 4.5 Routing Updates

```tsx
// App.tsx — new imports
import { OrdersPage } from '@/pages/orders/OrdersPage'
import { OrderDetailPage } from '@/pages/orders/OrderDetailPage'
import { CustomersPage } from '@/pages/customers/CustomersPage'
import { CustomerDetailPage } from '@/pages/customers/CustomerDetailPage'
import { ContactsPage } from '@/pages/contacts/ContactsPage'
import { ContactDetailPage } from '@/pages/contacts/ContactDetailPage'

// New routes under AppShell
<Route path="orders" element={<OrdersPage />} />
<Route path="orders/:id" element={<OrderDetailPage />} />
<Route path="customers" element={<CustomersPage />} />
<Route path="customers/:id" element={<CustomerDetailPage />} />
<Route path="contacts" element={<ContactsPage />} />
<Route path="contacts/:id" element={<ContactDetailPage />} />

// Remove OrdersPage from PlaceholderPages import
```

### 4.6 Sidebar Updates

```typescript
// components/AppSidebar.tsx — add items
{ label: 'Khách hàng', href: '/customers', icon: Users },
{ label: 'Liên hệ', href: '/contacts', icon: MessageSquare },
```

New icon imports: `Users` from `lucide-react` for Khách hàng, `MessageSquare` for Liên hệ.

### 4.7 Component Patterns

All new components follow Phase 2 conventions:

- **DataTables:** Use existing Base UI `Table` component pattern. Server-side pagination with `<Pagination>`.
- **Dialogs:** Use existing Base UI `AlertDialog`, `Dialog` components.
- **Status badges:** `OrderStatusBadge` component renders colored badge + Vietnamese label.
- **Customer stats:** Row of `<Card>` components for the 4 stat cards.
- **Print view:** A `PrintInvoice` component or CSS class that hides nav/sidebar when printing.

---

## 5. Migration Plan

1. **Backend Task 1:** Add `OrderService.UpdateStatus()` + `IContactService.GetPaged()` + new controller actions for paginated endpoints
2. **Frontend Task 1:** Types (`order.ts`, `customer.ts`, `contact.ts`) + API modules (`orders.ts`, `customers.ts`, `contacts.ts`)
3. **Frontend Task 2:** Orders list page with status filter tabs, search, DataTable, pagination
4. **Frontend Task 3:** Order detail page with status update dialog + cancel order + print invoice
5. **Frontend Task 4:** Customers list page with search and pagination
6. **Frontend Task 5:** Customer detail page with stats, order history, edit dialog
7. **Frontend Task 6:** Contacts list page + contact detail page with read/unread actions
8. **Frontend Task 7:** Sidebar updates + routing updates + build verification

---

## 6. Out of Scope

- Order creation from admin (manual order entry)
- Order assignment to staff
- Fraud detection workflow UI (existing backend logic is untouched)
- Refund processing UI
- Dashboard stats / charts (Phase 4)
- Notifications / activity log
- Customer address management (existing `CustomerAddress` entity)
- Import/export orders or customers
