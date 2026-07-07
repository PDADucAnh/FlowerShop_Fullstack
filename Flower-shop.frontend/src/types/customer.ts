export interface Customer {
  id: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  totalOrders: number;
  successfulDeliveries: number;
  failedDeliveries: number;
  isBlacklisted: boolean;
  fraudScore: number;
}

export interface CustomerInput {
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  password?: string;
}
