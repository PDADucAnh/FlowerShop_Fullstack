import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { CouponDTO, CreateCouponDTO, UpdateCouponDTO, CouponUsageDTO } from '@/types/coupon'

export const couponsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<CouponDTO>>('/api/coupons/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<CouponDTO>(`/api/coupons/${id}`)
  },
  create(dto: CreateCouponDTO) {
    return apiClient.post<CouponDTO>('/api/coupons', dto)
  },
  update(id: number, dto: UpdateCouponDTO) {
    return apiClient.put(`/api/coupons/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/coupons/${id}`)
  },
  enable(id: number) {
    return apiClient.patch(`/api/coupons/${id}/enable`)
  },
  disable(id: number) {
    return apiClient.patch(`/api/coupons/${id}/disable`)
  },
  getUsages(id: number) {
    return apiClient.get<CouponUsageDTO[]>(`/api/coupons/${id}/usages`)
  },
}
