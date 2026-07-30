import { apiClient } from './client'
import type { AdminNotification, PaginatedNotifications } from '@/types/adminNotification'

export interface AdminNotificationsParams {
  type?: string
  search?: string
  page?: number
  pageSize?: number
}

export const adminNotificationsApi = {
  getAll(params: AdminNotificationsParams = {}) {
    return apiClient.get<PaginatedNotifications>('/api/admin-notifications', { params })
  },
  getLatest(limit = 10) {
    return apiClient.get<AdminNotification[]>('/api/admin-notifications/latest', { params: { limit } })
  },
  getUnreadCount() {
    return apiClient.get<{ count: number }>('/api/admin-notifications/unread-count')
  },
  markAsRead(id: number) {
    return apiClient.put(`/api/admin-notifications/${id}/read`)
  },
  markAllAsRead() {
    return apiClient.put('/api/admin-notifications/read-all')
  },
}
