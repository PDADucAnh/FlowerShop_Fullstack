import { apiClient } from './client'
import type { FlashSale, CreateFlashSaleRequest, UpdateFlashSaleRequest } from '@/types/flashSale'

export const flashSalesApi = {
  getAll() {
    return apiClient.get<FlashSale[]>('/api/FlashSales')
  },
  getById(id: number) {
    return apiClient.get<FlashSale>(`/api/FlashSales/${id}`)
  },
  create(data: CreateFlashSaleRequest) {
    return apiClient.post<FlashSale>('/api/FlashSales', data)
  },
  update(id: number, data: UpdateFlashSaleRequest) {
    return apiClient.put(`/api/FlashSales/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/FlashSales/${id}`)
  },
}
