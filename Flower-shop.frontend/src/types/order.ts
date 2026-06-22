export type OrderStatus = 'Pending' | 'Shipping' | 'Completed';

export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  customerName?: string;
}

export interface Order {
  id: number;
  customerId: number;
  orderDate: string;
  status: OrderStatus;
  totalAmount: number;
  orderDetails?: OrderDetail[];
}

export interface OrderInput {
  customerId: number;
  items: { productId: number; quantity: number }[];
}
