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
