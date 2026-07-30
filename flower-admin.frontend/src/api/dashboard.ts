import { apiClient } from './client'
import type { DashboardSummary } from '@/types/dashboard'

export const dashboardApi = {
  getSummary() {
    return apiClient.get<DashboardSummary>('/api/dashboard/summary')
  },
}
