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
