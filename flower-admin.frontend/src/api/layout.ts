import { apiClient } from './client'
import type { HeaderLayoutDTO, FooterColumnDTO, LayoutResponse } from '@/types/layout'

export const layoutApi = {
  getLayout() {
    return apiClient.get<LayoutResponse>('/api/layout')
  },
  saveHeader(dto: HeaderLayoutDTO) {
    return apiClient.put('/api/layout/header', dto)
  },
  saveFooter(dto: FooterColumnDTO[]) {
    return apiClient.put('/api/layout/footer', dto)
  },
}
