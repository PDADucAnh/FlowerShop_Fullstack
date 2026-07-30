export interface AdminNotification {
  id: number
  title: string
  message: string
  type: string
  referenceId?: string
  referenceType?: string
  userId?: number
  icon?: string
  priority?: string
  isRead: boolean
  readAt?: string
  createdAt: string
  createdBy?: string
  navigationUrl?: string
  metadata?: string
}

export interface PaginatedNotifications {
  items: AdminNotification[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
