import { useQuery } from '@tanstack/react-query'
import { dashboardApi } from '@/api/dashboard'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Package, ShoppingBag, Users, DollarSign, Loader2, AlertCircle, Bell, TrendingUp, Star } from 'lucide-react'
import { format } from 'date-fns'
import { vi } from 'date-fns/locale'

function formatCurrency(value: number) {
  return new Intl.NumberFormat('vi-VN').format(value) + '₫'
}

export function DashboardPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['dashboard-summary'],
    queryFn: () => dashboardApi.getSummary().then((r) => r.data),
  })

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !data) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải dữ liệu tổng quan</p>
      </div>
    )
  }

  const stats = [
    { label: 'Đơn mới', value: data.orders.new.toString(), icon: ShoppingBag, color: 'text-blue-600' },
    { label: 'Sản phẩm', value: data.products.active.toString(), icon: Package, color: 'text-green-600' },
    { label: 'Khách hàng', value: data.customers.total.toString(), icon: Users, color: 'text-purple-600' },
    { label: 'Doanh thu tháng', value: formatCurrency(data.revenue.month), icon: DollarSign, color: 'text-orange-600' },
  ]

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Tổng quan</h1>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat) => {
          const Icon = stat.icon
          return (
            <Card key={stat.label}>
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium">{stat.label}</CardTitle>
                <Icon className={`size-5 ${stat.color}`} />
              </CardHeader>
              <CardContent>
                <p className={`text-3xl font-bold ${stat.color}`}>{stat.value}</p>
              </CardContent>
            </Card>
          )
        })}
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader><CardTitle className="text-sm flex items-center gap-2"><TrendingUp className="size-4" /> Đơn hàng</CardTitle></CardHeader>
          <CardContent className="space-y-2 text-sm">
            <div className="flex justify-between"><span className="text-muted-foreground">Chờ xác nhận</span><span>{data.orders.pendingConfirmation}</span></div>
            <div className="flex justify-between"><span className="text-muted-foreground">Đang chuẩn bị</span><span>{data.orders.preparing}</span></div>
            <div className="flex justify-between"><span className="text-muted-foreground">Đang giao</span><span>{data.orders.delivering}</span></div>
            <div className="flex justify-between"><span className="text-muted-foreground">Đã giao</span><span className="font-semibold text-green-600">{data.orders.completed}</span></div>
            <div className="flex justify-between"><span className="text-muted-foreground">Đã hủy</span><span className="text-destructive">{data.orders.cancelled}</span></div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-sm flex items-center gap-2"><Star className="size-4" /> Sản phẩm bán chạy</CardTitle></CardHeader>
          <CardContent className="space-y-2">
            {data.topProducts.length > 0 ? data.topProducts.slice(0, 5).map((p) => (
              <div key={p.id} className="flex items-center justify-between text-sm">
                <span className="truncate max-w-[160px]">{p.name || '—'}</span>
                <span className="font-mono text-muted-foreground">{p.totalSold} cái</span>
              </div>
            )) : (
              <p className="text-sm text-muted-foreground">Chưa có dữ liệu</p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-sm flex items-center gap-2"><Bell className="size-4" /> Thông báo gần đây</CardTitle></CardHeader>
          <CardContent className="space-y-2">
            {data.notifications.length > 0 ? data.notifications.slice(0, 5).map((n) => (
              <div key={n.id} className="flex items-start gap-2 text-sm">
                <div className={`mt-1 size-2 shrink-0 rounded-full ${n.isRead ? 'bg-muted' : 'bg-primary'}`} />
                <div className="min-w-0 flex-1">
                  <p className="truncate font-medium">{n.title || '—'}</p>
                  <p className="text-xs text-muted-foreground">
                    {format(new Date(n.createdAt), 'dd/MM HH:mm', { locale: vi })}
                  </p>
                </div>
              </div>
            )) : (
              <p className="text-sm text-muted-foreground">Chưa có thông báo</p>
            )}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-sm flex items-center gap-2"><Users className="size-4" /> Khách hàng thân thiết</CardTitle></CardHeader>
        <CardContent>
          {data.topCustomers.length > 0 ? (
            <div className="space-y-2">
              {data.topCustomers.slice(0, 5).map((c, i) => (
                <div key={c.id} className="flex items-center justify-between text-sm">
                  <div className="flex items-center gap-2">
                    <span className="text-muted-foreground">#{i + 1}</span>
                    <span>{c.fullName || '—'}</span>
                  </div>
                  <div className="flex gap-4">
                    <span className="text-muted-foreground">{c.totalOrders} đơn</span>
                    <span className="font-mono w-24 text-right">{formatCurrency(c.totalSpent)}</span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">Chưa có dữ liệu</p>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
