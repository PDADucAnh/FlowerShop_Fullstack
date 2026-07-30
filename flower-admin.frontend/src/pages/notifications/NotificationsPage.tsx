import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminNotificationsApi } from '@/api/adminNotifications'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent } from '@/components/ui/card'
import { Search, Loader2, AlertCircle, Check, CheckCheck, Bell } from 'lucide-react'
import { toast } from 'sonner'
import type { AdminNotification } from '@/types/adminNotification'

const typeFilters = ['Tất cả', 'Order', 'Payment', 'Promotion', 'Review', 'System']

const typeColors: Record<string, string> = {
  Order: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
  Payment: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
  Promotion: 'bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200',
  Review: 'bg-amber-100 text-amber-800 dark:bg-amber-900 dark:text-amber-200',
  System: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  })
}

export function NotificationsPage() {
  const [type, setType] = useState('')
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['admin-notifications', type, search, page],
    queryFn: () => adminNotificationsApi.getAll({ type: type || undefined, search: search || undefined, page, pageSize: 10 }).then((r) => r.data),
  })

  const markMutation = useMutation({
    mutationFn: (id: number) => adminNotificationsApi.markAsRead(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['admin-notifications'] }),
  })

  const markAllMutation = useMutation({
    mutationFn: () => adminNotificationsApi.markAllAsRead(),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-notifications'] })
      toast.success('Đã đánh dấu tất cả đã đọc')
    },
    onError: () => toast.error('Không thể đánh dấu tất cả'),
  })

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    setPage(1)
  }

  if (isLoading) return <div className="flex h-64 items-center justify-center"><Loader2 className="size-8 animate-spin text-muted-foreground" /></div>
  if (error) return (
    <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
      <AlertCircle className="size-8" /><p>Không thể tải thông báo</p>
      <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
    </div>
  )

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-semibold">Trung tâm thông báo</h1>
          <p className="text-sm text-muted-foreground">Xem và quản lý các thông báo từ hệ thống</p>
        </div>
        <Button variant="outline" size="sm" onClick={() => markAllMutation.mutate()} disabled={markAllMutation.isPending}>
          <CheckCheck className="mr-1 size-4" />
          {markAllMutation.isPending ? 'Đang xử lý...' : 'Đánh dấu tất cả đã đọc'}
        </Button>
      </div>

      <Card>
        <CardContent className="p-4">
          <div className="flex flex-col md:flex-row gap-4">
            <form onSubmit={handleSearch} className="relative flex-1">
              <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                placeholder="Tìm kiếm thông báo..."
                className="pl-9"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
              />
            </form>
            <div className="flex flex-wrap gap-1">
              {typeFilters.map((t) => {
                const active = t === 'Tất cả' ? !type : type === t
                return (
                  <button
                    key={t}
                    onClick={() => { setType(t === 'Tất cả' ? '' : t); setPage(1) }}
                    className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
                      active
                        ? 'bg-primary text-primary-foreground border-primary'
                        : 'bg-background text-muted-foreground border-border hover:bg-muted'
                    }`}
                  >
                    {t}
                  </button>
                )
              })}
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardContent className="p-0 divide-y">
          {data && data.items.length > 0 ? (
            data.items.map((notification) => {
              const typeColor = typeColors[notification.type] || typeColors.System
              return (
                <div
                  key={notification.id}
                  className={`flex items-start gap-4 p-5 transition-colors ${
                    notification.isRead ? 'opacity-75' : 'border-l-4 border-l-primary bg-primary/5'
                  }`}
                >
                  <div className="flex-1 space-y-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <Badge variant="outline" className={`text-[10px] uppercase font-bold tracking-wider ${typeColor}`}>
                        {notification.type}
                      </Badge>
                      <span className="text-xs text-muted-foreground">{formatDate(notification.createdAt)}</span>
                    </div>
                    <h4 className="font-medium">{notification.title}</h4>
                    <p className="text-sm text-muted-foreground">{notification.message}</p>
                  </div>
                  <div className="flex items-center gap-2 shrink-0">
                    {!notification.isRead && (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-xs gap-1"
                        onClick={() => markMutation.mutate(notification.id)}
                        disabled={markMutation.isPending}
                      >
                        <Check className="size-3" />
                        Đã đọc
                      </Button>
                    )}
                  </div>
                </div>
              )
            })
          ) : (
            <div className="flex flex-col items-center justify-center py-12 text-muted-foreground">
              <Bell className="size-8 mb-2 opacity-40" />
              <p>Không tìm thấy thông báo nào</p>
            </div>
          )}
        </CardContent>
      </Card>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">
            Trang {data.page} / {data.totalPages} (Tổng số {data.totalCount} thông báo)
          </span>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage(page - 1)}>
              Trước
            </Button>
            <Button variant="outline" size="sm" disabled={page >= data.totalPages} onClick={() => setPage(page + 1)}>
              Sau
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
