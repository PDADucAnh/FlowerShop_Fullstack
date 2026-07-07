export type OrderStatus = 'Pending' | 'PendingVerification' | 'Confirmed' | 'Preparing' | 'Shipping' | 'Completed' | 'Cancelled' | 'PendingPayment' | 'Paid' | 'ReadyForDelivery' | 'Refunded';

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
  paymentMethod: number;
  paymentStatus: number;
  paymentTransactionId?: string;
  paymentPaidAt?: string;
  deliveryDate?: string;
  deliveryTimeSlot?: string;
  deliveryDistrict?: string;
  deliveryAddress?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  isVerified: boolean;
  refundAmount: number;
  canCancel: boolean;
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
}
