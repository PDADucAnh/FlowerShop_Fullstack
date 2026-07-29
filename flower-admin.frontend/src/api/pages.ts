import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PageDTO, CreatePageDTO, UpdatePageDTO } from '@/types/page'

export const pagesApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<PageDTO>>('/api/pages/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<PageDTO>(`/api/pages/${id}`)
  },
  getBySlug(slug: string) {
    return apiClient.get<PageDTO>(`/api/pages/slug/${slug}`)
  },
  create(dto: CreatePageDTO) {
    return apiClient.post<PageDTO>('/api/pages', dto)
  },
  update(id: number, dto: UpdatePageDTO) {
    return apiClient.put(`/api/pages/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/pages/${id}`)
  },
}
