export type OrderStatus = 'Pending' | 'PendingVerification' | 'Confirmed' | 'Preparing' | 'Shipping' | 'Completed' | 'Cancelled' | 'CancelledByCustomer' | 'CancelledByShop' | 'PendingPayment' | 'Paid' | 'ReadyForDelivery' | 'RefundPending' | 'Refunded';

export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  productName?: string;
  productImageUrl?: string;
  sizeVariant?: string;
  quantity: number;
  unitPrice: number;
  customerName?: string;
}

export interface Order {
  id: number;
  customerId: number;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  orderDate: string;
  status: OrderStatus;
  notes?: string;
  recipientName?: string;
  recipientPhone?: string;
  orderDetails?: OrderDetail[];
  paymentMethod: number | string;
  paymentStatus: number | string;
  paymentTransactionId?: string;
  paymentPaidAt?: string;
  deliveryDate?: string;
  deliveryTimeSlot?: string;
  deliveryDistrict?: string;
  deliveryAddress?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  cancelledBy?: string;
  cancellationFee: number;
  isVerified: boolean;
  refundAmount: number;
  refundRequestedAt?: string;
  refundCompletedAt?: string;
  canCancel: boolean;
  promotionId?: number;
  promotionName?: string;
  couponId?: number;
  couponCode?: string;
  originalAmount: number;
  discountAmount: number;
  finalAmount: number;
  shippingFee: number;
}

export interface OrderInput {
  customerId: number;
  notes?: string;
  items: { productId: number; quantity: number; unitPrice: number }[];
  paymentMethod?: string;
  deliveryDate?: string;
  deliveryTimeSlot?: string;
  deliveryAddress?: string;
  recipientName?: string;
  recipientPhone?: string;
  couponCode?: string;
}
