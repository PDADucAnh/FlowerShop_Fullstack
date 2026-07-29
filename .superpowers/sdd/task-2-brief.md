### Task 2: Frontend â€” Types + API Modules

**Files:**
- Create: `flower-admin.frontend/src/types/order.ts`
- Create: `flower-admin.frontend/src/types/customer.ts`
- Create: `flower-admin.frontend/src/types/contact.ts`
- Create: `flower-admin.frontend/src/api/orders.ts`
- Create: `flower-admin.frontend/src/api/customers.ts`
- Create: `flower-admin.frontend/src/api/contacts.ts`

**Interfaces:**
- Consumes: existing `PagedResponse<T>` from `@/types/api`, existing `apiClient` from `@/api/client`
- Produces: types and API functions consumed by all page tasks below

- [ ] **Step 1: Create type definitions**

Create `flower-admin.frontend/src/types/order.ts`:

```typescript
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
  PartialRefund = 4,
  Expired = 5,
  Cancelled = 6,
  RefundPending = 7,
  PartialRefundPending = 8,
  PartialRefunded = 9,
}

export interface OrderDetailDTO {
  id: number
  orderId: number
  productId: number
  productName?: string
  productImageUrl?: string
  sizeVariant?: string
  quantity: number
  unitPrice: number
  originalPrice: number
  discountAmount: number
  subtotal: number
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
  totalAmount: number
}
```

Create `flower-admin.frontend/src/types/customer.ts`:

```typescript
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
```

Create `flower-admin.frontend/src/types/contact.ts`:

```typescript
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
```

- [ ] **Step 2: Create API modules**

Create `flower-admin.frontend/src/api/orders.ts`:

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { OrderDTO, OrderStatus } from '@/types/order'

export interface OrdersPagedParams {
  page?: number
  pageSize?: number
  status?: string
  search?: string
  dateFrom?: string
  dateTo?: string
}

export const ordersApi = {
  getPaged(params: OrdersPagedParams = {}) {
    return apiClient.get<PaginatedResponse<OrderDTO>>('/api/orders/paged', { params })
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
```

Create `flower-admin.frontend/src/api/customers.ts`:

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { CustomerDTO, UpdateCustomerRequest } from '@/types/customer'
import type { OrderDTO } from '@/types/order'

export interface CustomersPagedParams {
  page?: number
  pageSize?: number
  search?: string
}

export const customersApi = {
  getPaged(params: CustomersPagedParams = {}) {
    return apiClient.get<PaginatedResponse<CustomerDTO>>('/api/customers/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<CustomerDTO>(`/api/customers/${id}`)
  },
  getOrders(id: number, params: { page?: number; pageSize?: number } = {}) {
    return apiClient.get<PaginatedResponse<OrderDTO>>(`/api/customers/${id}/orders`, { params })
  },
  update(id: number, data: UpdateCustomerRequest) {
    return apiClient.put(`/api/customers/${id}`, data)
  },
}
```

Create `flower-admin.frontend/src/api/contacts.ts`:

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { ContactDTO } from '@/types/contact'

export interface ContactsPagedParams {
  page?: number
  pageSize?: number
  isRead?: boolean
}

export const contactsApi = {
  getPaged(params: ContactsPagedParams = {}) {
    return apiClient.get<PaginatedResponse<ContactDTO>>('/api/contacts/paged', { params })
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

- [ ] **Step 3: Verify TypeScript compilation**

Run:

```bash
cd flower-admin.frontend
npx tsc --noEmit
```

Expected: 0 errors.

---

