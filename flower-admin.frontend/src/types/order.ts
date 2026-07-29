export const OrderStatus = {
  Pending: 0,
  Shipping: 1,
  Completed: 2,
  Cancelled: 3,
  PendingVerification: 4,
  Confirmed: 5,
  Preparing: 6,
  PendingPayment: 7,
  Paid: 8,
  ReadyForDelivery: 9,
  Refunded: 10,
  CancelledByCustomer: 11,
  CancelledByShop: 12,
  RefundPending: 13,
} as const
export type OrderStatus = (typeof OrderStatus)[keyof typeof OrderStatus]

export const PaymentMethod = {
  OnlinePayment: 0,
  COD: 1,
} as const
export type PaymentMethod = (typeof PaymentMethod)[keyof typeof PaymentMethod]

export const PaymentStatus = {
  Pending: 0,
  Completed: 1,
  Failed: 2,
  Refunded: 3,
  PartialRefund: 4,
  Expired: 5,
  Cancelled: 6,
  RefundPending: 7,
  PartialRefundPending: 8,
  PartialRefunded: 9,
} as const
export type PaymentStatus = (typeof PaymentStatus)[keyof typeof PaymentStatus]

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
