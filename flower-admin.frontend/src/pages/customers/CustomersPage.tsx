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
