import axiosClient from '../api/axiosClient';

const promotionService = {
  getActive: async () => axiosClient.get('/Promotions/active'),
  getBestForProduct: async (productId: number) => axiosClient.get(`/Promotions/product/${productId}`),
  getAll: async () => axiosClient.get('/Promotions'),
  getById: async (id: number) => axiosClient.get(`/Promotions/${id}`),
  create: async (data: any) => axiosClient.post('/Promotions', data),
  update: async (id: number, data: any) => axiosClient.put(`/Promotions/${id}`, data),
  delete: async (id: number) => axiosClient.delete(`/Promotions/${id}`),
  enable: async (id: number) => axiosClient.patch(`/Promotions/${id}/enable`),
  disable: async (id: number) => axiosClient.patch(`/Promotions/${id}/disable`),
  addProduct: async (id: number, productId: number) => axiosClient.post(`/Promotions/${id}/products`, { productId }),
  removeProduct: async (id: number, productId: number) => axiosClient.delete(`/Promotions/${id}/products/${productId}`),
  applyCoupon: async (data: { code: string; customerId: number; orderTotal: number }) => axiosClient.post('/Promotions/apply', data),
};

export default promotionService;
