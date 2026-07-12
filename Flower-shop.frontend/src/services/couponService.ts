import axiosClient from '../api/axiosClient';

const couponService = {
  getAll: async () => axiosClient.get('/Coupons'),
  getById: async (id: number) => axiosClient.get(`/Coupons/${id}`),
  create: async (data: any) => axiosClient.post('/Coupons', data),
  update: async (id: number, data: any) => axiosClient.put(`/Coupons/${id}`, data),
  delete: async (id: number) => axiosClient.delete(`/Coupons/${id}`),
  enable: async (id: number) => axiosClient.patch(`/Coupons/${id}/enable`),
  disable: async (id: number) => axiosClient.patch(`/Coupons/${id}/disable`),
  validate: async (data: any) => axiosClient.post('/Coupons/validate', data),
  apply: async (data: any) => axiosClient.post('/Coupons/apply', data),
  getUsages: async (id: number) => axiosClient.get(`/Coupons/${id}/usages`),
};

export default couponService;
