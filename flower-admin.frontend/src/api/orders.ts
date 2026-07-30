import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { OrderDTO, OrderStatus, CreateOrderRequest } from '@/types/order'

export interface OrdersPagedParams {
  page?: number
  pageSize?: number
  status?: string
  search?: string
  dateFrom?: string
  dateTo?: string
}

export const ordersApi = {
  getPaged(params: OrdersPagedParams = {}) {
    return apiClient.get<PaginatedResponse<OrderDTO>>('/api/orders/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<OrderDTO>(`/api/orders/${id}`)
  },
  updateStatus(id: number, status: OrderStatus) {
    return apiClient.put(`/api/orders/${id}/status`, { status })
  },
  cancelByShop(id: number, reason: string) {
    return apiClient.put(`/api/orders/${id}/cancel-by-shop`, { reason })
  },
  confirmCod(id: number) {
    return apiClient.put<{ message: string }>(`/api/orders/${id}/confirm-cod`)
  },
  processRefund(id: number) {
    return apiClient.post<{ message: string }>(`/api/orders/${id}/process-refund`)
  },
  create(data: CreateOrderRequest) {
    return apiClient.post<{ message: string; orderId: number }>('/api/orders', data)
  },
}
