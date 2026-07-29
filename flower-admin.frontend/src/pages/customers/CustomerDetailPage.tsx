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
