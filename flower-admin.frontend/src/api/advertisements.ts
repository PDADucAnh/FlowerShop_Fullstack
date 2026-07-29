import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { AdvertisementDTO, CreateAdvertisementDTO, UpdateAdvertisementDTO } from '@/types/advertisement'

export const advertisementsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<AdvertisementDTO>>('/api/advertisements/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<AdvertisementDTO>(`/api/advertisements/${id}`)
  },
  create(dto: CreateAdvertisementDTO) {
    return apiClient.post<AdvertisementDTO>('/api/advertisements', dto)
  },
  update(id: number, dto: UpdateAdvertisementDTO) {
    return apiClient.put(`/api/advertisements/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/advertisements/${id}`)
  },
}
