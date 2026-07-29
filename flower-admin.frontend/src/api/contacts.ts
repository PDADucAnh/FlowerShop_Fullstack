import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { ContactDTO } from '@/types/contact'

export interface ContactsPagedParams {
  page?: number
  pageSize?: number
  isRead?: boolean
}

export const contactsApi = {
  getPaged(params: ContactsPagedParams = {}) {
    return apiClient.get<PaginatedResponse<ContactDTO>>('/api/contacts/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<ContactDTO>(`/api/contacts/${id}`)
  },
  getUnreadCount() {
    return apiClient.get<{ count: number }>('/api/contacts/unread-count')
  },
  markRead(id: number, isRead: boolean) {
    return apiClient.put(`/api/contacts/${id}/read`, { isRead })
  },
  delete(id: number) {
    return apiClient.delete(`/api/contacts/${id}`)
  },
}
