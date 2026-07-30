import { apiClient } from './client'
import type { OrderDetailDTO } from '@/types/order'

export interface CreateOrderDetailRequest {
  orderId: number
  productId: number
  quantity: number
  unitPrice: number
  sizeVariant?: string
}

export const orderDetailsApi = {
  getAll() {
    return apiClient.get<OrderDetailDTO[]>('/api/orderdetails')
  },
  getByOrderId(orderId: number) {
    return apiClient.get<OrderDetailDTO[]>(`/api/orderdetails/order/${orderId}`)
  },
  getById(id: number) {
    return apiClient.get<OrderDetailDTO>(`/api/orderdetails/${id}`)
  },
  create(dto: CreateOrderDetailRequest) {
    return apiClient.post<OrderDetailDTO>('/api/orderdetails', dto)
  },
  update(id: number, dto: OrderDetailDTO) {
    return apiClient.put(`/api/orderdetails/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/orderdetails/${id}`)
  },
}
