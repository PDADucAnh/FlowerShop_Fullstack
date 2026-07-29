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
  [OrderStatus.PendingVerification]: { label: 'Chá» xÃ¡c nháº­n', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  [OrderStatus.Confirmed]: { label: 'ÄÃ£ xÃ¡c nháº­n', className: 'bg-blue-100 text-blue-800 hover:bg-blue-100' },
  [OrderStatus.Preparing]: { label: 'Äang cáº¯m hoa', className: 'bg-blue-100 text-blue-800 hover:bg-blue-100' },
  [OrderStatus.ReadyForDelivery]: { label: 'Sáºµn sÃ ng giao', className: 'bg-teal-100 text-teal-800 hover:bg-teal-100' },
  [OrderStatus.Shipping]: { label: 'Äang giao', className: 'bg-indigo-100 text-indigo-800 hover:bg-indigo-100' },
  [OrderStatus.Completed]: { label: 'ÄÃ£ giao', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  [OrderStatus.Cancelled]: { label: 'ÄÃ£ há»§y', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.CancelledByCustomer]: { label: 'KhÃ¡ch há»§y', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.CancelledByShop]: { label: 'Shop há»§y', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  [OrderStatus.PendingPayment]: { label: 'Chá» thanh toÃ¡n', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  [OrderStatus.Paid]: { label: 'ÄÃ£ thanh toÃ¡n', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  [OrderStatus.RefundPending]: { label: 'Chá» hoÃ n tiá»n', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
  [OrderStatus.Refunded]: { label: 'ÄÃ£ hoÃ n tiá»n', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
  [OrderStatus.Pending]: { label: 'Chá» xá»­ lÃ½', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
}

export function OrderStatusBadge({ status }: { status: OrderStatus }) {
  const config = statusConfig[status] ?? { label: 'KhÃ´ng xÃ¡c Ä‘á»‹nh', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' }
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
  0: { label: 'Chá» TT', className: 'bg-amber-100 text-amber-800 hover:bg-amber-100' },
  1: { label: 'ÄÃ£ TT', className: 'bg-green-100 text-green-800 hover:bg-green-100' },
  2: { label: 'Tháº¥t báº¡i', className: 'bg-red-100 text-red-800 hover:bg-red-100' },
  3: { label: 'ÄÃ£ HT', className: 'bg-gray-100 text-gray-800 hover:bg-gray-100' },
}

export function OrderTable({ orders }: OrderTableProps) {
  const navigate = useNavigate()

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('vi-VN').format(value) + 'â‚«'

  const formatDate = (dateStr: string) => {
    const d = new Date(dateStr)
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>MÃ£ Ä‘Æ¡n</TableHead>
          <TableHead>KhÃ¡ch hÃ ng</TableHead>
          <TableHead>NgÃ y Ä‘áº·t</TableHead>
          <TableHead className="text-right">Tá»•ng tiá»n</TableHead>
          <TableHead>Thanh toÃ¡n</TableHead>
          <TableHead>Tráº¡ng thÃ¡i</TableHead>
          <TableHead className="w-20">Thao tÃ¡c</TableHead>
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
            <TableCell>{order.customerName || 'â€”'}</TableCell>
            <TableCell className="text-muted-foreground">{formatDate(order.orderDate)}</TableCell>
            <TableCell className="text-right font-mono">{formatCurrency(order.finalAmount)}</TableCell>
            <TableCell>
              <div className="flex items-center gap-1.5">
                <Badge variant="outline" className="text-xs">
                  {paymentMethodLabel[order.paymentMethod] ?? 'â€”'}
                </Badge>
                <Badge className={(paymentStatusConfig[order.paymentStatus]?.className ?? '') + ' text-xs'}>
                  {paymentStatusConfig[order.paymentStatus]?.label ?? 'â€”'}
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
  { label: 'Táº¥t cáº£', value: '' },
  { label: 'Chá» xÃ¡c nháº­n', value: 'PendingVerification' },
  { label: 'ÄÃ£ xÃ¡c nháº­n', value: 'Confirmed' },
  { label: 'Äang xá»­ lÃ½', value: 'Preparing,ReadyForDelivery' },
  { label: 'Äang giao', value: 'Shipping' },
  { label: 'ÄÃ£ giao', value: 'Completed' },
  { label: 'ÄÃ£ há»§y', value: 'Cancelled,CancelledByCustomer,CancelledByShop' },
  { label: 'Chá» thanh toÃ¡n', value: 'PendingPayment' },
  { label: 'ÄÃ£ thanh toÃ¡n', value: 'Paid' },
  { label: 'HoÃ n tiá»n', value: 'RefundPending,Refunded' },
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
        <p>KhÃ´ng thá»ƒ táº£i danh sÃ¡ch Ä‘Æ¡n hÃ ng</p>
        <Button variant="outline" onClick={() => window.location.reload()}>Thá»­ láº¡i</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">ÄÆ¡n hÃ ng</h1>
      </div>

      <div className="flex items-center gap-3">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="TÃ¬m kiáº¿m theo tÃªn hoáº·c SÄTâ€¦"
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
            {data ? `${data.totalCount} Ä‘Æ¡n hÃ ng` : ''}
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
                    <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>TrÆ°á»›c</Button>
                    <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>KhÃ´ng cÃ³ Ä‘Æ¡n hÃ ng nÃ o</p>
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

