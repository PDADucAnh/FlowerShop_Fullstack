import { apiClient } from './client'
import type { EmailHistory } from '@/types/emailHistory'

export const emailHistoriesApi = {
  getByOrderId(orderId: number) {
    return apiClient.get<EmailHistory[]>(`/api/orders/${orderId}/email-history`)
  },
}
