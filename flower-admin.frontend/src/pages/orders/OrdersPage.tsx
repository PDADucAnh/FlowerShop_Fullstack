import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { ordersApi } from '@/api/orders'
import { OrderTable } from './components/OrderTable'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
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
