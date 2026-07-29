import { apiClient } from './client'

export const uploadApi = {
  upload(file: File) {
    const formData = new FormData()
    formData.append('file', file)
    return apiClient.post<{ url: string }>('/api/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },
}
