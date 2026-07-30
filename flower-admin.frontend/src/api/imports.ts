import { apiClient } from './client'
import type { ImportApiResponse } from '@/types/import'

export const importsApi = {
  upload(formData: FormData) {
    return apiClient.post<ImportApiResponse>('/api/imports/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },

  downloadTemplate() {
    return apiClient.get('/api/imports/template', { responseType: 'blob' })
  },
}
