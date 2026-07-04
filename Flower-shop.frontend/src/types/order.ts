export type OrderStatus = 'Pending' | 'PendingVerification' | 'Confirmed' | 'Preparing' | 'Shipping' | 'Completed' | 'Cancelled';

export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  productName?: string;
  productImageUrl?: string;
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
}
