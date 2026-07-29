import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { CustomerDTO, UpdateCustomerRequest } from '@/types/customer'
import type { OrderDTO } from '@/types/order'

export interface CustomersPagedParams {
  page?: number
  pageSize?: number
  search?: string
}

export const customersApi = {
  getPaged(params: CustomersPagedParams = {}) {
    return apiClient.get<PaginatedResponse<CustomerDTO>>('/api/customers/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<CustomerDTO>(`/api/customers/${id}`)
  },
  getOrders(id: number, params: { page?: number; pageSize?: number } = {}) {
    return apiClient.get<PaginatedResponse<OrderDTO>>(`/api/customers/${id}/orders`, { params })
  },
  update(id: number, data: UpdateCustomerRequest) {
    return apiClient.put(`/api/customers/${id}`, data)
  },
}
