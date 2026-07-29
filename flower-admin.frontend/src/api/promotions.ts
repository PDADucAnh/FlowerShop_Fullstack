import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PromotionCampaignDTO, CreatePromotionCampaignDTO, UpdatePromotionCampaignDTO } from '@/types/promotion'

export const promotionsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<PromotionCampaignDTO>>('/api/promotions/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<PromotionCampaignDTO>(`/api/promotions/${id}`)
  },
  create(dto: CreatePromotionCampaignDTO) {
    return apiClient.post<PromotionCampaignDTO>('/api/promotions', dto)
  },
  update(id: number, dto: UpdatePromotionCampaignDTO) {
    return apiClient.put(`/api/promotions/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/promotions/${id}`)
  },
  enable(id: number) {
    return apiClient.patch(`/api/promotions/${id}/enable`)
  },
  disable(id: number) {
    return apiClient.patch(`/api/promotions/${id}/disable`)
  },
  addProduct(id: number, productId: number) {
    return apiClient.post(`/api/promotions/${id}/products`, { productId })
  },
  removeProduct(id: number, productId: number) {
    return apiClient.delete(`/api/promotions/${id}/products/${productId}`)
  },
}
