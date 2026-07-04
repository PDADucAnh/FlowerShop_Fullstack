import axiosClient from '../api/axiosClient';
import type { Order } from '../types/order';

const orderService = {
  submitOrder: async (orderData: any) => {
    try {
      const data: any = await axiosClient.post('/Orders', orderData);
      return data;
    } catch (error) {
      console.error('Error submitting order:', error);
      throw error;
    }
  },

  getMyOrders: async (): Promise<Order[]> => {
    const data: Order[] = await axiosClient.get('/Orders');
    return data;
  },

  getOrderById: async (id: number): Promise<Order> => {
    const data: Order = await axiosClient.get(`/Orders/${id}`);
    return data;
  },

  cancelOrder: async (id: number): Promise<void> => {
    await axiosClient.put(`/Orders/${id}/cancel`);
  }
};

export default orderService;
