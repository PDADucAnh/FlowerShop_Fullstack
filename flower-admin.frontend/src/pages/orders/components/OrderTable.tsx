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
import type { OrderDTO } from '@/types/order'

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
