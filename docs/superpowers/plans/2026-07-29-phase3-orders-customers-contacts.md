# Phase 3: Orders, Customers & Contacts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build Orders management (list + detail + status updates), Customers management (list + detail + edit), and Contact inquiries management (list + detail + mark read/delete) for the admin SPA.

**Architecture:** New backend service methods + controller actions for paginated queries and dedicated status updates. Frontend uses existing patterns: React Query for data fetching, Base UI components, shadcn-style table/badge/button components. All UI text in Vietnamese.

**Tech Stack:** .NET 8 (C#), React 19, @tanstack/react-query, Base UI (Radix-inspired), react-router-dom v7, lucide-react, sonner, axios

---

## File Structure

### New Backend Files
- None — all additions go into existing files

### Modified Backend Files
- `Flower.Backend/Services/Interfaces/IOrderService.cs` — add `UpdateStatus` method
- `Flower.Backend/Services/OrderService.cs` — implement `UpdateStatus`
- `Flower.Backend/Services/Interfaces/IContactService.cs` — add `GetPaged` method
- `Flower.Backend/Services/ContactService.cs` — implement `GetPaged`
- `Flower.Backend/Controllers/Api/OrdersController.cs` — add `GetPaged`, `UpdateStatus` actions
- `Flower.Backend/Controllers/Api/CustomersController.cs` — add `GetPaged`, `GetCustomerOrders` actions; extend `CustomerDTO` with `CreatedAt`
- `Flower.Backend/Controllers/Api/ContactsController.cs` — add `GetPaged` action
- `Flower.Backend/Models/DTOs/CustomerDTOs.cs` — add `CreatedAt` to `CustomerDTO`

### New Frontend Files
- `flower-admin.frontend/src/types/order.ts`
- `flower-admin.frontend/src/types/customer.ts`
- `flower-admin.frontend/src/types/contact.ts`
- `flower-admin.frontend/src/api/orders.ts`
- `flower-admin.frontend/src/api/customers.ts`
- `flower-admin.frontend/src/api/contacts.ts`
- `flower-admin.frontend/src/pages/orders/OrdersPage.tsx`
- `flower-admin.frontend/src/pages/orders/OrderDetailPage.tsx`
- `flower-admin.frontend/src/pages/orders/components/OrderTable.tsx`
- `flower-admin.frontend/src/pages/orders/components/OrderStatusBadge.tsx`
- `flower-admin.frontend/src/pages/orders/components/CancelOrderDialog.tsx`
- `flower-admin.frontend/src/pages/customers/CustomersPage.tsx`
- `flower-admin.frontend/src/pages/customers/CustomerDetailPage.tsx`
- `flower-admin.frontend/src/pages/customers/components/CustomerTable.tsx`
- `flower-admin.frontend/src/pages/customers/components/CustomerEditDialog.tsx`
- `flower-admin.frontend/src/pages/contacts/ContactsPage.tsx`
- `flower-admin.frontend/src/pages/contacts/ContactDetailPage.tsx`
- `flower-admin.frontend/src/pages/contacts/components/ContactTable.tsx`

### Modified Frontend Files
- `flower-admin.frontend/src/components/AppSidebar.tsx` — add Khách hàng, Liên hệ nav items
- `flower-admin.frontend/src/App.tsx` — add routes, import new pages

---

## Global Constraints

- All UI text in Vietnamese (follow existing patterns)
- API responses are raw objects (not wrapped in `ApiResponse<T>`) — frontend reads `response.data` directly
- Use `@tanstack/react-query` for all data fetching
- Use `sonner` (`toast`) for notifications
- Follow existing component patterns: Base UI, shadcn-style components
- No modification to existing MVC controllers or non-API controllers
- No modification to existing entities or non-API services unless explicitly listed
- Route paths: `/orders`, `/orders/:id`, `/customers`, `/customers/:id`, `/contacts`, `/contacts/:id`

---

### Task 1: Backend — New Service Methods + Controller Actions

**Files:**
- Modify: `Flower.Backend/Services/Interfaces/IOrderService.cs`
- Modify: `Flower.Backend/Services/OrderService.cs`
- Modify: `Flower.Backend/Services/Interfaces/IContactService.cs`
- Modify: `Flower.Backend/Services/ContactService.cs`
- Modify: `Flower.Backend/Models/DTOs/CustomerDTOs.cs`
- Modify: `Flower.Backend/Controllers/Api/OrdersController.cs`
- Modify: `Flower.Backend/Controllers/Api/CustomersController.cs`
- Modify: `Flower.Backend/Controllers/Api/ContactsController.cs`

**Interfaces:**
- Consumes: existing `IOrderService`, `IContactService`, `ICustomerService`, existing `PagedResult<T>`, existing `ContactDTO`, `CustomerDTO`, `OrderDTO`
- Produces: `IOrderService.UpdateStatus(int id, OrderStatus newStatus) → bool`, `IContactService.GetPaged(int page, int pageSize, bool? isRead) → PagedResult<ContactDTO>`, new controller actions listed below

- [ ] **Step 1: Add `UpdateStatus` to IOrderService**

Open `Flower.Backend/Services/Interfaces/IOrderService.cs`. Add after `CancelWithReason` line:

```csharp
Task<bool> UpdateStatus(int id, OrderStatus newStatus);
```

- [ ] **Step 2: Implement `UpdateStatus` in OrderService**

Open `Flower.Backend/Services/OrderService.cs`. Add before the `Delete` method:

```csharp
public async Task<bool> UpdateStatus(int id, OrderStatus newStatus)
{
    var order = await _context.Orders.FindAsync(id);
    if (order == null) return false;

    var oldStatus = order.Status;
    order.Status = newStatus;

    try
    {
        await _context.SaveChangesAsync();

        var statusChangedToConfirmed = oldStatus != OrderStatus.Confirmed && order.Status == OrderStatus.Confirmed;
        var statusChangedToCompleted = oldStatus != OrderStatus.Completed && order.Status == OrderStatus.Completed;
        var statusChangedToShipping = oldStatus != OrderStatus.Shipping && order.Status == OrderStatus.Shipping;

        if (statusChangedToConfirmed || statusChangedToCompleted || statusChangedToShipping)
        {
            await _context.Entry(order).Reference(o => o.Customer).LoadAsync();
            await _context.Entry(order).Collection(o => o.OrderDetails).LoadAsync();
            if (order.OrderDetails != null)
            {
                foreach (var detail in order.OrderDetails)
                {
                    await _context.Entry(detail).Reference(d => d.Product).LoadAsync();
                }
            }

            if (order.Customer != null && !string.IsNullOrEmpty(order.Customer.Email))
            {
                if (statusChangedToConfirmed)
                    await _emailService.SendOrderConfirmedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                else if (statusChangedToShipping)
                    await _emailService.SendOrderShippingEmailAsync(order, order.Customer.Email, order.Customer.FullName);
                else if (statusChangedToCompleted)
                    await _emailService.SendOrderCompletedEmailAsync(order, order.Customer.Email, order.Customer.FullName);
            }

            if (order.CustomerId > 0)
            {
                var (notifTitle, notifType, notifIcon) = statusChangedToConfirmed
                    ? ($"Đơn hàng #{order.Id} đã được xác nhận", "OrderConfirmed", "Verified")
                    : statusChangedToShipping
                        ? ($"Đơn hàng #{order.Id} đang được giao", "OrderShipping", "LocalShipping")
                        : ($"Đơn hàng #{order.Id} đã hoàn thành", "OrderCompleted", "CheckCircle");

                await _notificationService.CreateCustomerNotification(
                    customerId: order.CustomerId,
                    title: notifTitle,
                    content: $"Trạng thái đơn hàng #{order.Id} đã được cập nhật.",
                    type: notifType,
                    orderId: order.Id,
                    referenceType: "OrderStatusChanged",
                    icon: notifIcon,
                    priority: "High",
                    navigationUrl: $"/my-orders/{order.Id}"
                );
            }
        }

        if (oldStatus != order.Status && order.CustomerId > 0)
        {
            await _notificationService.NotifyCustomerEvent(order.CustomerId, "OrderChanged", new { orderId = order.Id, status = order.Status.ToString() });
        }

        return true;
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Orders.AnyAsync(e => e.Id == id))
            return false;
        throw;
    }
}
```

- [ ] **Step 3: Add `GetPaged` to IContactService**

Open `Flower.Backend/Services/Interfaces/IContactService.cs`. Add after `GetAll` line:

```csharp
Task<PagedResult<ContactDTO>> GetPaged(int page, int pageSize, bool? isRead = null);
```

- [ ] **Step 4: Implement `GetPaged` in ContactService**

Open `Flower.Backend/Services/ContactService.cs`. Add the method:

```csharp
public async Task<PagedResult<ContactDTO>> GetPaged(int page, int pageSize, bool? isRead = null)
{
    IQueryable<Contact> query = _context.Contacts.OrderByDescending(c => c.CreatedAt);

    if (isRead.HasValue)
        query = query.Where(c => c.IsRead == isRead.Value);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(c => new ContactDTO
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        Subject = c.Subject,
        Message = c.Message,
        IsRead = c.IsRead,
        ReadAt = c.ReadAt,
        CreatedAt = c.CreatedAt
    }).ToList();

    return new PagedResult<ContactDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

- [ ] **Step 5: Add `CreatedAt` to CustomerDTO**

Open `Flower.Backend/Models/DTOs/CustomerDTOs.cs`. Add to `CustomerDTO`:

```csharp
public DateTime CreatedAt { get; set; }
```

- [ ] **Step 6: Add paginated + status endpoint actions to OrdersController**

Open `Flower.Backend/Controllers/Api/OrdersController.cs`. Add after `GetAll`:

```csharp
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? status = null,
    [FromQuery] string? search = null,
    [FromQuery] DateTime? dateFrom = null,
    [FromQuery] DateTime? dateTo = null)
{
    List<OrderStatus>? statuses = null;
    if (!string.IsNullOrEmpty(status))
    {
        var parts = status.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        statuses = new List<OrderStatus>();
        foreach (var part in parts)
        {
            if (Enum.TryParse<OrderStatus>(part, true, out var parsed))
                statuses.Add(parsed);
        }
    }

    var result = await _orderService.GetPaged(page, pageSize, statuses, search, dateFrom, dateTo);
    return Ok(result);
}

[HttpPut("{id}/status")]
public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
{
    var updated = await _orderService.UpdateStatus(id, request.Status);
    if (!updated) return NotFound();
    return NoContent();
}
```

Also add the `UpdateOrderStatusRequest` DTO at the bottom of the file (or in a separate file):

```csharp
public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
```

Now update `IOrderService` to add the extended `GetPaged` overload. Add to the interface:

```csharp
Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null);
```

Then update the `OrderService.GetPaged(int page, int pageSize)` implementation to accept the new parameters. Replace the existing `GetPaged` method:

```csharp
public async Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null)
{
    IQueryable<Order> query = _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
        .Include(o => o.Promotion)
        .Include(o => o.Coupon)
        .OrderByDescending(o => o.OrderDate);

    query = ApplyOwnershipFilter(query);

    if (statuses != null && statuses.Count > 0)
        query = query.Where(o => statuses.Contains(o.Status));

    if (!string.IsNullOrEmpty(search))
        query = query.Where(o =>
            (o.Customer != null && o.Customer.FullName.Contains(search)) ||
            (o.Customer != null && o.Customer.Phone != null && o.Customer.Phone.Contains(search)));

    if (dateFrom.HasValue)
        query = query.Where(o => o.OrderDate >= dateFrom.Value);

    if (dateTo.HasValue)
        query = query.Where(o => o.OrderDate <= dateTo.Value);

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(o => o.ToDTO()).ToList();
    return new PagedResult<OrderDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

- [ ] **Step 7: Add paginated + orders actions to CustomersController**

Open `Flower.Backend/Controllers/Api/CustomersController.cs`. Add after `GetAll`:

```csharp
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null)
{
    var result = await _customerService.GetPaged(page, pageSize, search);
    return Ok(result);
}

[HttpGet("{id}/orders")]
public async Task<IActionResult> GetCustomerOrders(
    int id,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    var result = await _orderService.GetPaged(page, pageSize, customerId: id);
    return Ok(result);
}
```

Now update `ICustomerService` to add the extended `GetPaged` overload. Replace:

```csharp
Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize);
```

With:

```csharp
Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize, string? search = null);
```

Update `CustomerService.GetPaged(int page, int pageSize)` to accept search:

```csharp
public async Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize, string? search = null)
{
    IQueryable<Customer> query = _context.Customers.OrderByDescending(c => c.CreatedAt);

    if (!string.IsNullOrEmpty(search))
    {
        query = query.Where(c =>
            c.FullName.Contains(search) ||
            c.Email.Contains(search) ||
            (c.Phone != null && c.Phone.Contains(search)));
    }

    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var dtos = items.Select(c => new CustomerDTO
    {
        Id = c.Id,
        FullName = c.FullName,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        TotalOrders = c.TotalOrders,
        SuccessfulDeliveries = c.SuccessfulDeliveries,
        FailedDeliveries = c.FailedDeliveries,
        IsBlacklisted = c.IsBlacklisted,
        FraudScore = c.FraudScore,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    }).ToList();

    return new PagedResult<CustomerDTO>
    {
        Items = dtos,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

Also update `IOrderService` to add customerId filter to `GetPaged`:

```csharp
Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null, int? customerId = null);
```

And update the `OrderService.GetPaged` implementation to include the customerId filter, add after the `dateTo` filter block:

```csharp
if (customerId.HasValue)
    query = query.Where(o => o.CustomerId == customerId.Value);
```

- [ ] **Step 8: Add paginated endpoint to ContactsController**

Open `Flower.Backend/Controllers/Api/ContactsController.cs`. Add after the `GetUnreadCount` action:

```csharp
[Authorize(Policy = "StaffOnly")]
[HttpGet("paged")]
public async Task<IActionResult> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] bool? isRead = null)
{
    var result = await _contactService.GetPaged(page, pageSize, isRead);
    return Ok(result);
}
```

- [ ] **Step 9: Build and verify backend**

Run:

```bash
cd Flower.Backend
dotnet build
```

Expected: Build succeeded with 0 errors.

---

### Task 2: Frontend — Types + API Modules

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

### Task 3: Orders List Page

**Files:**
- Create: `flower-admin.frontend/src/pages/orders/components/OrderStatusBadge.tsx`
- Create: `flower-admin.frontend/src/pages/orders/components/OrderTable.tsx`
- Create: `flower-admin.frontend/src/pages/orders/OrdersPage.tsx`

**Interfaces:**
- Consumes: `ordersApi` from `@/api/orders`, `OrderDTO`, `OrderStatus` from `@/types/order`
- Produces: rendered routes for `/orders`

- [ ] **Step 1: Create OrderStatusBadge component**

Create `flower-admin.frontend/src/pages/orders/components/OrderStatusBadge.tsx`:

```tsx
import { Badge } from '@/components/ui/badge'
import { OrderStatus } from '@/types/order'

const statusConfig: Record<number, { label: string; className: string }> = {
  [OrderStatus.PendingVerification]: { label: 'Chờ xác nhận', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  [OrderStatus.Confirmed]: { label: 'Đã xác nhận', className: 'bg-blue-100 text-blue-800 hover:bg-blue-100' },
  [OrderStatus.Preparing]: { label: 'Đang cắm hoa', className: 'bg-blue-100 text-blue-800 hover:bg-blue-100' },
  [OrderStatus.ReadyForDelivery]: { label: 'Sẵn sàng giao', className: 'bg-teal-100 text-teal-800 hover:bg-teal-100' },
  [OrderStatus.Shipping]: { label: 'Đang giao', className: 'bg-indigo-100 text-indigo-800 hover:bg-indigo-100' },
  [OrderStatus.Completed]: { label: 'Đã giao', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  [OrderStatus.Cancelled]: { label: 'Đã hủy', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.CancelledByCustomer]: { label: 'Khách hủy', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.CancelledByShop]: { label: 'Shop hủy', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.PendingPayment]: { label: 'Chờ thanh toán', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  [OrderStatus.Paid]: { label: 'Đã thanh toán', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  [OrderStatus.RefundPending]: { label: 'Chờ hoàn tiền', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
  [OrderStatus.Refunded]: { label: 'Đã hoàn tiền', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
  [OrderStatus.Pending]: { label: 'Chờ xử lý', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
}

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  const config = statusConfig[status] ?? { label: 'Không xác định', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' }
  return <Badge className={config.className}>{config.label}</Badge>
}
```

- [ ] **Step 2: Create OrderTable component**

Create `flower-admin.frontend/src/pages/orders/components/OrderTable.tsx`:

```tsx
import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Eye } from 'lucide-react'
import { OrderStatusBadge } from './OrderStatusBadge'
import type { OrderDTO, PaymentMethod, PaymentStatus } from '@/types/order'

interface OrderTableProps {
  orders: OrderDTO[]
}

const paymentMethodLabel: Record<number, string> = {
  0: 'VNPay',
  1: 'COD',
}

const paymentStatusConfig: Record<number, { label: string; className: string }> = {
  0: { label: 'Chờ TT', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  1: { label: 'Đã TT', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  2: { label: 'Thất bại', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  3: { label: 'Đã HT', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
}

export function OrderTable({ orders }: OrderTableProps) {
  const navigate = useNavigate()

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN').format(value) + '₫'

  const formatDate = (dateStr: string) => {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Mã đơn</TableHead>
          <TableHead>Khách hàng</TableHead>
          <TableHead>Ngày đặt</TableHead>
          <TableHead className="text-right">Tổng tiền</TableHead>
          <TableHead>Thanh toán</TableHead>
          <TableHead>Trạng thái</TableHead>
          <TableHead className="w-20">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {orders.map((order) => (
          <TableRow
            key={order.id}
            className="cursor-pointer"
            onClick={() => navigate(`/orders/${order.id}`)}
          >
            <TableCell className="font-medium">#{order.id}</TableCell>
            <TableCell>{order.customerName || '—'}</TableCell>
            <TableCell className="text-muted-foreground">{formatDate(order.orderDate)}</TableCell>
            <TableCell className="text-right font-mono">{formatCurrency(order.finalAmount)}</TableCell>
            <TableCell>
              <div className="flex items-center gap-1.5">
                <Badge variant="outline" className="text-xs">
                  {paymentMethodLabel[order.paymentMethod] ?? '—'}
                </Badge>
                <Badge className={(paymentStatusConfig[order.paymentStatus]?.className ?? '') + ' text-xs'}>
                  {paymentStatusConfig[order.paymentStatus]?.label ?? '—'}
                </Badge>
              </div>
            </TableCell>
            <TableCell><OrderStatusBadge status={order.status} /></TableCell>
            <TableCell>
              <Button
                variant="ghost"
                size="icon"
                onClick={(e) => { e.stopPropagation(); navigate(`/orders/${order.id}`) }}
              >
                <Eye className="size-4" />
              </Button>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
```

- [ ] **Step 3: Create OrdersPage**

Create `flower-admin.frontend/src/pages/orders/OrdersPage.tsx`:

```tsx
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ordersApi } from '@/api/orders'
import { OrderTable } from './components/OrderTable'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Search, Loader2, AlertCircle } from 'lucide-react'

interface StatusTab {
  label: string
  value: string
}

const statusTabs: StatusTab[] = [
  { label: 'Tất cả', value: '' },
  { label: 'Chờ xác nhận', value: 'PendingVerification' },
  { label: 'Đã xác nhận', value: 'Confirmed' },
  { label: 'Đang xử lý', value: 'Preparing,ReadyForDelivery' },
  { label: 'Đang giao', value: 'Shipping' },
  { label: 'Đã giao', value: 'Completed' },
  { label: 'Đã hủy', value: 'Cancelled,CancelledByCustomer,CancelledByShop' },
  { label: 'Chờ thanh toán', value: 'PendingPayment' },
  { label: 'Đã thanh toán', value: 'Paid' },
  { label: 'Hoàn tiền', value: 'RefundPending,Refunded' },
]

export function OrdersPage() {
  const [page, setPage] = useState(1)
  const [statusFilter, setStatusFilter] = useState('')
  const [search, setSearch] = useState('')
  const pageSize = 20

  const { data, isLoading, error } = useQuery({
    queryKey: ['orders', page, statusFilter, search],
    queryFn: () =>
      ordersApi.getPaged({
        page,
        pageSize,
        status: statusFilter || undefined,
        search: search || undefined,
      }).then((r) => r.data),
  })

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách đơn hàng</p>
        <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Đơn hàng</h1>
      </div>

      <div className="flex items-center gap-3">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm kiếm theo tên hoặc SĐT…"
            className="pl-9"
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1) }}
          />
        </div>
      </div>

      <div className="flex flex-wrap gap-2">
        {statusTabs.map((tab) => (
          <button
            key={tab.value}
            onClick={() => { setStatusFilter(tab.value); setPage(1) }}
            className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
              statusFilter === tab.value
                ? 'bg-primary text-primary-foreground border-primary'
                : 'bg-background text-muted-foreground border-border hover:bg-muted'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} đơn hàng` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <OrderTable orders={data.items} />
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">
                    Trang {data.page} / {data.totalPages}
                  </p>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
                    <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>Không có đơn hàng nào</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 4: Verify TypeScript compilation**

Run:

```bash
cd flower-admin.frontend
npx tsc --noEmit
```

Expected: 0 errors.

---

### Task 4: Order Detail Page

**Files:**
- Create: `flower-admin.frontend/src/pages/orders/components/CancelOrderDialog.tsx`
- Create: `flower-admin.frontend/src/pages/orders/OrderDetailPage.tsx`

**Interfaces:**
- Consumes: `ordersApi` from `@/api/orders`, `OrderDTO`, `OrderStatus` from `@/types/order`

- [ ] **Step 1: Create CancelOrderDialog**

Create `flower-admin.frontend/src/pages/orders/components/CancelOrderDialog.tsx`:

```tsx
import { useState } from 'react'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
} from '@/components/ui/alert-dialog'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'

interface CancelOrderDialogProps {
  orderId: number
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: (reason: string) => void
  loading?: boolean
}

export function CancelOrderDialog({ orderId, open, onOpenChange, onConfirm, loading }: CancelOrderDialogProps) {
  const [reason, setReason] = useState('')

  const handleConfirm = () => {
    if (!reason.trim()) return
    onConfirm(reason.trim())
  }

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Hủy đơn hàng #{orderId}</AlertDialogTitle>
          <AlertDialogDescription>
            Hành động này sẽ hủy đơn hàng và thông báo cho khách hàng. Vui lòng nhập lý do hủy.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <div className="py-3">
          <Textarea
            placeholder="Nhập lý do hủy đơn…"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            rows={3}
            className="w-full"
          />
        </div>
        <AlertDialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button
            variant="destructive"
            onClick={handleConfirm}
            disabled={!reason.trim() || loading}
          >
            {loading ? 'Đang hủy…' : 'Xác nhận hủy'}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
```

- [ ] **Step 2: Create OrderDetailPage**

Create `flower-admin.frontend/src/pages/orders/OrderDetailPage.tsx`:

```tsx
import { useState, useRef } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi } from '@/api/orders'
import { OrderStatusBadge } from './components/OrderStatusBadge'
import { CancelOrderDialog } from './components/CancelOrderDialog'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { ArrowLeft, Printer, Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import { OrderStatus } from '@/types/order'
import type { OrderDTO } from '@/types/order'

const statusOptions: { value: OrderStatus; label: string }[] = [
  { value: OrderStatus.PendingVerification, label: 'Chờ xác nhận' },
  { value: OrderStatus.Confirmed, label: 'Đã xác nhận' },
  { value: OrderStatus.Preparing, label: 'Đang cắm hoa' },
  { value: OrderStatus.ReadyForDelivery, label: 'Sẵn sàng giao' },
  { value: OrderStatus.Shipping, label: 'Đang giao' },
  { value: OrderStatus.Completed, label: 'Đã giao' },
]

const terminalStatuses = [OrderStatus.Cancelled, OrderStatus.CancelledByCustomer, OrderStatus.CancelledByShop, OrderStatus.Completed, OrderStatus.Refunded]

function isTerminal(status: OrderStatus) {
  return terminalStatuses.includes(status)
}

export function OrderDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const printRef = useRef<HTMLDivElement>(null)
  const [selectedStatus, setSelectedStatus] = useState<OrderStatus | null>(null)
  const [cancelOpen, setCancelOpen] = useState(false)

  const orderId = Number(id)

  const { data: order, isLoading, error } = useQuery({
    queryKey: ['order', orderId],
    queryFn: () => ordersApi.getById(orderId).then((r) => r.data),
    enabled: !!orderId,
  })

  const statusMutation = useMutation({
    mutationFn: (newStatus: OrderStatus) => ordersApi.updateStatus(orderId, newStatus),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['order', orderId] })
      queryClient.invalidateQueries({ queryKey: ['orders'] })
      toast.success('Cập nhật trạng thái thành công')
      setSelectedStatus(null)
    },
    onError: () => toast.error('Không thể cập nhật trạng thái'),
  })

  const cancelMutation = useMutation({
    mutationFn: (reason: string) => ordersApi.cancelByShop(orderId, reason),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['order', orderId] })
      queryClient.invalidateQueries({ queryKey: ['orders'] })
      toast.success('Đã hủy đơn hàng')
      setCancelOpen(false)
    },
    onError: () => toast.error('Không thể hủy đơn hàng'),
  })

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN').format(value) + '₫'

  const formatDate = (dateStr?: string) => {
    if (!dateStr) return '—'
    return new Date(dateStr).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
  }

  const handlePrint = () => window.print()

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !order) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không tìm thấy đơn hàng</p>
        <Button variant="outline" onClick={() => navigate('/orders')}>Quay lại</Button>
      </div>
    )
  }

  return (
    <div ref={printRef} className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/orders')}>
            <ArrowLeft className="size-4" />
          </Button>
          <div>
            <h1 className="text-2xl font-semibold">Đơn hàng #{order.id}</h1>
            <p className="text-sm text-muted-foreground">{formatDate(order.orderDate)}</p>
          </div>
          <OrderStatusBadge status={order.status} />
        </div>
        <div className="flex items-center gap-2 print-hidden">
          {!isTerminal(order.status) && (
            <>
              <div className="flex items-center gap-2">
                <Select
                  value={selectedStatus?.toString() ?? ''}
                  onValueChange={(v) => setSelectedStatus(Number(v) as OrderStatus)}
                >
                  <SelectTrigger className="w-44">
                    <SelectValue placeholder="Cập nhật trạng thái" />
                  </SelectTrigger>
                  <SelectContent>
                    {statusOptions.map((opt) => (
                      <SelectItem key={opt.value} value={String(opt.value)}>
                        {opt.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {selectedStatus !== null && (
                  <Button
                    size="sm"
                    onClick={() => statusMutation.mutate(selectedStatus)}
                    disabled={statusMutation.isPending}
                  >
                    {statusMutation.isPending ? 'Đang lưu…' : 'Lưu'}
                  </Button>
                )}
              </div>
              {order.canCancel && (
                <Button variant="destructive" size="sm" onClick={() => setCancelOpen(true)}>
                  Hủy đơn
                </Button>
              )}
            </>
          )}
          <Button variant="outline" size="sm" onClick={handlePrint}>
            <Printer className="mr-1 size-4" />
            In phiếu giao
          </Button>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader><CardTitle className="text-sm">Khách hàng</CardTitle></CardHeader>
          <CardContent className="space-y-1 text-sm">
            <p className="font-medium">{order.customerName || '—'}</p>
            <p className="text-muted-foreground">{order.customerEmail || '—'}</p>
            <p className="text-muted-foreground">{order.customerPhone || '—'}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-sm">Thanh toán</CardTitle></CardHeader>
          <CardContent className="space-y-1 text-sm">
            <p>Phương thức: <span className="font-medium">{order.paymentMethod === 1 ? 'COD' : 'VNPay'}</span></p>
            <p>Trạng thái: <Badge variant={order.paymentStatus === 1 ? 'default' : 'outline'} className="text-xs">
              {order.paymentStatus === 1 ? 'Đã thanh toán' : 'Chưa thanh toán'}
            </Badge></p>
            {order.paymentTransactionId && <p className="text-muted-foreground">GD: {order.paymentTransactionId}</p>}
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-sm">Giao hàng</CardTitle></CardHeader>
          <CardContent className="space-y-1 text-sm">
            <p>Ngày: <span className="font-medium">{formatDate(order.deliveryDate)}</span></p>
            {order.deliveryTimeSlot && <p>Khung giờ: {order.deliveryTimeSlot}</p>}
            {(order.deliveryAddress || order.deliveryDistrict) && <p>Địa chỉ: {[order.deliveryAddress, order.deliveryDistrict].filter(Boolean).join(', ')}</p>}
            <p>Người nhận: {order.recipientName || '—'} {order.recipientPhone ? `(${order.recipientPhone})` : ''}</p>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Sản phẩm</CardTitle></CardHeader>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-10">STT</TableHead>
                <TableHead>Sản phẩm</TableHead>
                <TableHead>Size</TableHead>
                <TableHead className="text-center">SL</TableHead>
                <TableHead className="text-right">Đơn giá</TableHead>
                <TableHead className="text-right">Giảm giá</TableHead>
                <TableHead className="text-right">Thành tiền</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {order.orderDetails?.map((detail, idx) => (
                <TableRow key={detail.id}>
                  <TableCell>{idx + 1}</TableCell>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      {detail.productImageUrl && (
                        <img src={detail.productImageUrl} alt="" className="size-10 rounded object-cover" />
                      )}
                      <span className="font-medium">{detail.productName || '—'}</span>
                    </div>
                  </TableCell>
                  <TableCell>{detail.sizeVariant || '—'}</TableCell>
                  <TableCell className="text-center">{detail.quantity}</TableCell>
                  <TableCell className="text-right font-mono">{formatCurrency(detail.unitPrice)}</TableCell>
                  <TableCell className="text-right font-mono text-destructive">{detail.discountAmount > 0 ? `-${formatCurrency(detail.discountAmount)}` : '—'}</TableCell>
                  <TableCell className="text-right font-mono">{formatCurrency(detail.subtotal)}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <div className="flex justify-end">
        <div className="w-72 space-y-1 text-sm">
          <div className="flex justify-between">
            <span className="text-muted-foreground">Tạm tính</span>
            <span className="font-mono">{formatCurrency(order.originalAmount)}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-muted-foreground">Giảm giá</span>
            <span className="font-mono text-destructive">-{formatCurrency(order.discountAmount)}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-muted-foreground">Phí ship</span>
            <span className="font-mono">{formatCurrency(order.shippingFee)}</span>
          </div>
          <div className="flex justify-between border-t pt-1 font-semibold">
            <span>Tổng cộng</span>
            <span className="font-mono text-lg">{formatCurrency(order.finalAmount)}</span>
          </div>
        </div>
      </div>

      <CancelOrderDialog
        orderId={order.id}
        open={cancelOpen}
        onOpenChange={setCancelOpen}
        onConfirm={(reason) => cancelMutation.mutate(reason)}
        loading={cancelMutation.isPending}
      />

      <style>{`
        @media print {
          .print-hidden { display: none !important; }
          nav, aside, .sidebar { display: none !important; }
        }
      `}</style>
    </div>
  )
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

### Task 5: Customers Pages

**Files:**
- Create: `flower-admin.frontend/src/pages/customers/components/CustomerTable.tsx`
- Create: `flower-admin.frontend/src/pages/customers/components/CustomerEditDialog.tsx`
- Create: `flower-admin.frontend/src/pages/customers/CustomersPage.tsx`
- Create: `flower-admin.frontend/src/pages/customers/CustomerDetailPage.tsx`

**Interfaces:**
- Consumes: `customersApi` from `@/api/customers`, `CustomerDTO`, `UpdateCustomerRequest` from `@/types/customer`

- [ ] **Step 1: Create CustomerTable**

Create `flower-admin.frontend/src/pages/customers/components/CustomerTable.tsx`:

```tsx
import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import type { CustomerDTO } from '@/types/customer'

interface CustomerTableProps {
  customers: CustomerDTO[]
}

export function CustomerTable({ customers }: CustomerTableProps) {
  const navigate = useNavigate()

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Tên khách hàng</TableHead>
          <TableHead>Email</TableHead>
          <TableHead>SĐT</TableHead>
          <TableHead className="text-center">Tổng đơn</TableHead>
          <TableHead className="text-center">Trạng thái</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {customers.map((customer) => (
          <TableRow
            key={customer.id}
            className="cursor-pointer"
            onClick={() => navigate(`/customers/${customer.id}`)}
          >
            <TableCell className="font-medium">{customer.fullName}</TableCell>
            <TableCell className="text-muted-foreground">{customer.email}</TableCell>
            <TableCell>{customer.phone || '—'}</TableCell>
            <TableCell className="text-center">{customer.totalOrders}</TableCell>
            <TableCell className="text-center">
              <span className={`inline-block size-2 rounded-full ${customer.isActive ? 'bg-green-500' : 'bg-gray-300'}`} />
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
```

- [ ] **Step 2: Create CustomerEditDialog**

Create `flower-admin.frontend/src/pages/customers/components/CustomerEditDialog.tsx`:

```tsx
import { useState, useEffect } from 'react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogFooter,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Switch } from '@/components/ui/switch'
import type { CustomerDTO, UpdateCustomerRequest } from '@/types/customer'

interface CustomerEditDialogProps {
  customer: CustomerDTO | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSave: (data: UpdateCustomerRequest) => void
  loading?: boolean
}

export function CustomerEditDialog({ customer, open, onOpenChange, onSave, loading }: CustomerEditDialogProps) {
  const [form, setForm] = useState<UpdateCustomerRequest>({
    id: 0,
    fullName: '',
    email: '',
    phone: '',
    address: '',
    isActive: true,
  })

  useEffect(() => {
    if (customer) {
      setForm({
        id: customer.id,
        fullName: customer.fullName,
        email: customer.email,
        phone: customer.phone || '',
        address: customer.address || '',
        isActive: customer.isActive,
      })
    }
  }, [customer])

  const handleSubmit = () => {
    if (!form.fullName.trim() || !form.email.trim()) return
    onSave(form)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Chỉnh sửa khách hàng</DialogTitle>
        </DialogHeader>
        <div className="space-y-4 py-3">
          <div className="space-y-1">
            <Label>Tên khách hàng</Label>
            <Input value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>Email</Label>
            <Input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>SĐT</Label>
            <Input value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>Địa chỉ</Label>
            <Input value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={form.isActive}
              onCheckedChange={(checked) => setForm({ ...form, isActive: checked })}
            />
            <Label>Đang hoạt động</Label>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button onClick={handleSubmit} disabled={loading}>
            {loading ? 'Đang lưu…' : 'Lưu'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 3: Create CustomersPage**

Create `flower-admin.frontend/src/pages/customers/CustomersPage.tsx`:

```tsx
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { customersApi } from '@/api/customers'
import { CustomerTable } from './components/CustomerTable'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Search, Loader2, AlertCircle } from 'lucide-react'

export function CustomersPage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const pageSize = 20

  const { data, isLoading, error } = useQuery({
    queryKey: ['customers', page, search],
    queryFn: () =>
      customersApi.getPaged({ page, pageSize, search: search || undefined }).then((r) => r.data),
  })

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách khách hàng</p>
        <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Khách hàng</h1>
      </div>

      <div className="relative max-w-sm">
        <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Tìm kiếm theo tên, email, SĐT…"
          className="pl-9"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1) }}
        />
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} khách hàng` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <CustomerTable customers={data.items} />
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">Trang {data.page} / {data.totalPages}</p>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
                    <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>Chưa có khách hàng nào</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 4: Create CustomerDetailPage**

Create `flower-admin.frontend/src/pages/customers/CustomerDetailPage.tsx`:

```tsx
import { useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { customersApi } from '@/api/customers'
import { CustomerEditDialog } from './components/CustomerEditDialog'
import { OrderStatusBadge } from '@/pages/orders/components/OrderStatusBadge'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { ArrowLeft, Pencil, Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import type { UpdateCustomerRequest } from '@/types/customer'

export function CustomerDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [editOpen, setEditOpen] = useState(false)
  const [ordersPage, setOrdersPage] = useState(1)

  const customerId = Number(id)

  const { data: customer, isLoading, error } = useQuery({
    queryKey: ['customer', customerId],
    queryFn: () => customersApi.getById(customerId).then((r) => r.data),
    enabled: !!customerId,
  })

  const { data: orders } = useQuery({
    queryKey: ['customer-orders', customerId, ordersPage],
    queryFn: () => customersApi.getOrders(customerId, { page: ordersPage, pageSize: 10 }).then((r) => r.data),
    enabled: !!customerId,
  })

  const updateMutation = useMutation({
    mutationFn: (data: UpdateCustomerRequest) => customersApi.update(customerId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['customer', customerId] })
      queryClient.invalidateQueries({ queryKey: ['customers'] })
      toast.success('Cập nhật thông tin thành công')
      setEditOpen(false)
    },
    onError: () => toast.error('Không thể cập nhật thông tin'),
  })

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN').format(value) + '₫'

  const formatDate = (dateStr?: string) => {
    if (!dateStr) return '—'
    return new Date(dateStr).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !customer) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không tìm thấy khách hàng</p>
        <Button variant="outline" onClick={() => navigate('/customers')}>Quay lại</Button>
      </div>
    )
  }

  const fraudColor = customer.fraudScore < 20 ? 'text-green-600' : customer.fraudScore < 50 ? 'text-amber-600' : 'text-red-600'

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/customers')}>
            <ArrowLeft className="size-4" />
          </Button>
          <h1 className="text-2xl font-semibold">{customer.fullName}</h1>
          <Badge variant={customer.isActive ? 'default' : 'outline'}>
            {customer.isActive ? 'Đang hoạt động' : 'Ngưng'}
          </Badge>
        </div>
        <Button variant="outline" size="sm" onClick={() => setEditOpen(true)}>
          <Pencil className="mr-1 size-4" />
          Chỉnh sửa
        </Button>
      </div>

      {customer.isBlacklisted && (
        <Card className="border-destructive">
          <CardContent className="py-3 text-destructive text-sm font-medium">
            Khách hàng này nằm trong danh sách đen. Fraud Score: {customer.fraudScore}
          </CardContent>
        </Card>
      )}

      <div className="grid gap-4 md:grid-cols-4">
        {[
          { label: 'Tổng đơn hàng', value: customer.totalOrders },
          { label: 'Giao thành công', value: customer.successfulDeliveries },
          { label: 'Giao thất bại', value: customer.failedDeliveries },
          { label: 'Fraud Score', value: customer.fraudScore, className: fraudColor },
        ].map((stat) => (
          <Card key={stat.label}>
            <CardContent className="py-4 text-center">
              <p className={`text-2xl font-bold ${stat.className ?? ''}`}>{stat.value}</p>
              <p className="text-sm text-muted-foreground">{stat.label}</p>
            </CardContent>
          </Card>
        ))}
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Thông tin khách hàng</CardTitle></CardHeader>
        <CardContent className="grid gap-4 md:grid-cols-2 text-sm">
          <div>
            <p className="text-muted-foreground">Email</p>
            <p>{customer.email}</p>
          </div>
          <div>
            <p className="text-muted-foreground">SĐT</p>
            <p>{customer.phone || '—'}</p>
          </div>
          <div>
            <p className="text-muted-foreground">Địa chỉ</p>
            <p>{customer.address || '—'}</p>
          </div>
          <div>
            <p className="text-muted-foreground">Ngày tham gia</p>
            <p>{formatDate(customer.createdAt)}</p>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle className="text-base">Lịch sử đơn hàng</CardTitle></CardHeader>
        <CardContent className="p-0">
          {orders && orders.items.length > 0 ? (
            <div>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Mã đơn</TableHead>
                    <TableHead>Ngày đặt</TableHead>
                    <TableHead className="text-right">Tổng tiền</TableHead>
                    <TableHead>Trạng thái</TableHead>
                    <TableHead>Thanh toán</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {orders.items.map((o) => (
                    <TableRow key={o.id} className="cursor-pointer" onClick={() => navigate(`/orders/${o.id}`)}>
                      <TableCell className="font-medium">#{o.id}</TableCell>
                      <TableCell className="text-muted-foreground">{formatDate(o.orderDate)}</TableCell>
                      <TableCell className="text-right font-mono">{formatCurrency(o.finalAmount)}</TableCell>
                      <TableCell><OrderStatusBadge status={o.status} /></TableCell>
                      <TableCell>{o.paymentMethod === 1 ? 'COD' : 'VNPay'}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              {(orders.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">Trang {orders.page} / {orders.totalPages}</p>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" disabled={ordersPage <= 1} onClick={() => setOrdersPage((p) => Math.max(1, p - 1))}>Trước</Button>
                    <Button variant="outline" size="sm" disabled={ordersPage >= (orders.totalPages ?? 1)} onClick={() => setOrdersPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground text-sm">
              Khách hàng chưa có đơn hàng nào
            </div>
          )}
        </CardContent>
      </Card>

      <CustomerEditDialog
        customer={customer}
        open={editOpen}
        onOpenChange={setEditOpen}
        onSave={(data) => updateMutation.mutate(data)}
        loading={updateMutation.isPending}
      />
    </div>
  )
}
```

- [ ] **Step 5: Verify TypeScript compilation**

Run:

```bash
cd flower-admin.frontend
npx tsc --noEmit
```

Expected: 0 errors.

---

### Task 6: Contacts Pages

**Files:**
- Create: `flower-admin.frontend/src/pages/contacts/components/ContactTable.tsx`
- Create: `flower-admin.frontend/src/pages/contacts/ContactsPage.tsx`
- Create: `flower-admin.frontend/src/pages/contacts/ContactDetailPage.tsx`

**Interfaces:**
- Consumes: `contactsApi` from `@/api/contacts`, `ContactDTO` from `@/types/contact`

- [ ] **Step 1: Create ContactTable**

Create `flower-admin.frontend/src/pages/contacts/components/ContactTable.tsx`:

```tsx
import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Trash2, Mail, MailOpen } from 'lucide-react'
import type { ContactDTO } from '@/types/contact'

interface ContactTableProps {
  contacts: ContactDTO[]
  onToggleRead: (contact: ContactDTO) => void
  onDelete: (contact: ContactDTO) => void
}

export function ContactTable({ contacts, onToggleRead, onDelete }: ContactTableProps) {
  const navigate = useNavigate()

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('vi-VN', {
      day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
    })
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Người gửi</TableHead>
          <TableHead>Email</TableHead>
          <TableHead>Tiêu đề</TableHead>
          <TableHead>Ngày gửi</TableHead>
          <TableHead>Trạng thái</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {contacts.map((contact) => (
          <TableRow
            key={contact.id}
            className={`cursor-pointer ${!contact.isRead ? 'font-semibold' : ''}`}
            onClick={() => navigate(`/contacts/${contact.id}`)}
          >
            <TableCell>{contact.name}</TableCell>
            <TableCell className="text-muted-foreground">{contact.email}</TableCell>
            <TableCell className="max-w-xs truncate">{contact.subject}</TableCell>
            <TableCell className="text-muted-foreground">{formatDate(contact.createdAt)}</TableCell>
            <TableCell>
              <Badge variant={contact.isRead ? 'outline' : 'default'} className="text-xs">
                {contact.isRead ? 'Đã đọc' : 'Chưa đọc'}
              </Badge>
            </TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => { e.stopPropagation(); onToggleRead(contact) }}
                >
                  {contact.isRead ? <MailOpen className="size-4" /> : <Mail className="size-4" />}
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => { e.stopPropagation(); onDelete(contact) }}
                >
                  <Trash2 className="size-4 text-destructive" />
                </Button>
              </div>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
```

- [ ] **Step 2: Create ContactsPage**

Create `flower-admin.frontend/src/pages/contacts/ContactsPage.tsx`:

```tsx
import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { contactsApi } from '@/api/contacts'
import { ContactTable } from './components/ContactTable'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogAction,
  AlertDialogCancel,
} from '@/components/ui/alert-dialog'
import { Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import type { ContactDTO } from '@/types/contact'

interface FilterTab {
  label: string
  value: boolean | undefined
}

const filterTabs: FilterTab[] = [
  { label: 'Tất cả', value: undefined },
  { label: 'Chưa đọc', value: false },
  { label: 'Đã đọc', value: true },
]

export function ContactsPage() {
  const [page, setPage] = useState(1)
  const [isReadFilter, setIsReadFilter] = useState<boolean | undefined>(undefined)
  const [deleteTarget, setDeleteTarget] = useState<ContactDTO | null>(null)
  const queryClient = useQueryClient()
  const pageSize = 20

  const { data: unreadCount } = useQuery({
    queryKey: ['contacts-unread'],
    queryFn: () => contactsApi.getUnreadCount().then((r) => r.data.count),
  })

  const { data, isLoading, error } = useQuery({
    queryKey: ['contacts', page, isReadFilter],
    queryFn: () =>
      contactsApi.getPaged({ page, pageSize, isRead: isReadFilter }).then((r) => r.data),
  })

  const toggleReadMutation = useMutation({
    mutationFn: ({ id, isRead }: { id: number; isRead: boolean }) => contactsApi.markRead(id, isRead),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => contactsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Đã xóa liên hệ')
      setDeleteTarget(null)
    },
    onError: () => toast.error('Không thể xóa liên hệ'),
  })

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách liên hệ</p>
        <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Liên hệ</h1>
      </div>

      <div className="flex gap-2">
        {filterTabs.map((tab) => (
          <button
            key={tab.label}
            onClick={() => { setIsReadFilter(tab.value); setPage(1) }}
            className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
              isReadFilter === tab.value
                ? 'bg-primary text-primary-foreground border-primary'
                : 'bg-background text-muted-foreground border-border hover:bg-muted'
            }`}
          >
            {tab.label}
            {tab.value === false && unreadCount !== undefined && (
              <span className="ml-1.5 inline-flex items-center justify-center size-5 rounded-full bg-primary text-[11px] font-medium text-primary-foreground">
                {unreadCount}
              </span>
            )}
          </button>
        ))}
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} liên hệ` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <ContactTable
                contacts={data.items}
                onToggleRead={(contact) =>
                  toggleReadMutation.mutate({ id: contact.id, isRead: !contact.isRead })
                }
                onDelete={setDeleteTarget}
              />
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">Trang {data.page} / {data.totalPages}</p>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
                    <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>{isReadFilter === false ? 'Không có liên hệ chưa đọc' : 'Không có liên hệ nào'}</p>
            </div>
          )}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Xóa liên hệ</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc muốn xóa liên hệ từ "{deleteTarget?.name}"? Hành động này không thể hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/80"
              onClick={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)}
            >
              Xóa
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
```

- [ ] **Step 3: Create ContactDetailPage**

Create `flower-admin.frontend/src/pages/contacts/ContactDetailPage.tsx`:

```tsx
import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { contactsApi } from '@/api/contacts'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogAction,
  AlertDialogCancel,
} from '@/components/ui/alert-dialog'
import { ArrowLeft, Trash2, Mail, MailOpen, Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import { useState } from 'react'

export function ContactDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [deleteOpen, setDeleteOpen] = useState(false)

  const contactId = Number(id)

  const { data: contact, isLoading, error } = useQuery({
    queryKey: ['contact', contactId],
    queryFn: () => contactsApi.getById(contactId).then((r) => r.data),
    enabled: !!contactId,
  })

  const toggleReadMutation = useMutation({
    mutationFn: (isRead: boolean) => contactsApi.markRead(contactId, isRead),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contact', contactId] })
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Cập nhật trạng thái thành công')
    },
  })

  const deleteMutation = useMutation({
    mutationFn: () => contactsApi.delete(contactId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Đã xóa liên hệ')
      navigate('/contacts')
    },
    onError: () => toast.error('Không thể xóa liên hệ'),
  })

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('vi-VN', {
      day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
    })
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !contact) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không tìm thấy liên hệ</p>
        <Button variant="outline" onClick={() => navigate('/contacts')}>Quay lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/contacts')}>
            <ArrowLeft className="size-4" />
          </Button>
          <h1 className="text-2xl font-semibold">{contact.name}</h1>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => toggleReadMutation.mutate(!contact.isRead)}
          >
            {contact.isRead ? <MailOpen className="mr-1 size-4" /> : <Mail className="mr-1 size-4" />}
            {contact.isRead ? 'Đánh dấu chưa đọc' : 'Đánh dấu đã đọc'}
          </Button>
          <Button variant="destructive" size="sm" onClick={() => setDeleteOpen(true)}>
            <Trash2 className="mr-1 size-4" />
            Xóa
          </Button>
        </div>
      </div>

      <div className="flex items-center gap-3 text-sm text-muted-foreground">
        <span>{contact.email}</span>
        {contact.phone && <span>· {contact.phone}</span>}
        <span>· {formatDate(contact.createdAt)}</span>
        <Badge variant={contact.isRead ? 'outline' : 'default'} className="text-xs">
          {contact.isRead ? 'Đã đọc' : 'Chưa đọc'}
        </Badge>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{contact.subject}</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="whitespace-pre-wrap text-sm leading-relaxed">{contact.message}</p>
        </CardContent>
      </Card>

      <Button variant="link" onClick={() => navigate('/contacts')}>
        <ArrowLeft className="mr-1 size-4" />
        Quay lại danh sách
      </Button>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Xóa liên hệ</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc muốn xóa liên hệ này? Hành động này không thể hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/80"
              onClick={() => deleteMutation.mutate()}
            >
              Xóa
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
```

- [ ] **Step 4: Verify TypeScript compilation**

Run:

```bash
cd flower-admin.frontend
npx tsc --noEmit
```

Expected: 0 errors.

---

### Task 7: Sidebar + Routing + Build Verification

**Files:**
- Modify: `flower-admin.frontend/src/components/AppSidebar.tsx`
- Modify: `flower-admin.frontend/src/App.tsx`

**Interfaces:**
- Consumes: all pages from Tasks 3-6, all API modules from Task 2

- [ ] **Step 1: Update AppSidebar**

Open `flower-admin.frontend/src/components/AppSidebar.tsx`. Add `Users` and `MessageSquare` to the lucide-react imports:

```typescript
import {
  LayoutDashboard,
  ShoppingBag,
  Package,
  FolderTree,
  Users,
  MessageSquare,
  FileText,
  Megaphone,
  Settings,
  type LucideIcon,
} from 'lucide-react'
```

Add two new nav items after the "Danh mục" entry:

```typescript
const navItems: NavItem[] = [
  { label: 'Tổng quan', href: '/', icon: LayoutDashboard },
  { label: 'Đơn hàng', href: '/orders', icon: ShoppingBag },
  { label: 'Sản phẩm', href: '/products', icon: Package },
  { label: 'Danh mục', href: '/products/categories', icon: FolderTree },
  { label: 'Khách hàng', href: '/customers', icon: Users },
  { label: 'Liên hệ', href: '/contacts', icon: MessageSquare },
  { label: 'Nội dung', href: '/content', icon: FileText },
  { label: 'Marketing', href: '/marketing', icon: Megaphone },
  { label: 'Hệ thống', href: '/system', icon: Settings },
]
```

- [ ] **Step 2: Update App.tsx routing**

Open `flower-admin.frontend/src/App.tsx`. Replace the import of `OrdersPage` from `PlaceholderPages` with imports from the new pages. Add new route entries:

```tsx
import { OrdersPage } from '@/pages/orders/OrdersPage'
import { OrderDetailPage } from '@/pages/orders/OrderDetailPage'
import { CustomersPage } from '@/pages/customers/CustomersPage'
import { CustomerDetailPage } from '@/pages/customers/CustomerDetailPage'
import { ContactsPage } from '@/pages/contacts/ContactsPage'
import { ContactDetailPage } from '@/pages/contacts/ContactDetailPage'
```

Remove `OrdersPage` from the PlaceholderPages import (only keep `ContentPage`, `MarketingPage`, `SystemPage`):

```tsx
import {
  ContentPage,
  MarketingPage,
  SystemPage,
} from '@/pages/PlaceholderPages'
```

Add new routes under the `AppShell` element, before the `content` route:

```tsx
<Route path="orders" element={<OrdersPage />} />
<Route path="orders/:id" element={<OrderDetailPage />} />
<Route path="customers" element={<CustomersPage />} />
<Route path="customers/:id" element={<CustomerDetailPage />} />
<Route path="contacts" element={<ContactsPage />} />
<Route path="contacts/:id" element={<ContactDetailPage />} />
```

- [ ] **Step 3: Build and verify**

First, verify backend compiles:

```bash
cd Flower.Backend
dotnet build
```

Expected: Build succeeded with 0 errors.

Then, verify frontend compiles:

```bash
cd flower-admin.frontend
npx tsc --noEmit
```

Expected: 0 errors.

Then, verify the full application runs:

```bash
cd flower-admin.frontend
npm run dev
```

Expected: dev server starts, navigate to `/orders`, `/customers`, `/contacts` and see the new pages rendering.

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "feat: phase 3 orders, customers, contacts management
- Add order list with status filter tabs, detail page, status update
- Add customer list, detail page with stats and order history
- Add contact list with read/unread filter, detail page
- Add backend paginated endpoints and dedicated status update API
- Update sidebar with Khách hàng and Liên hệ nav items"
```
