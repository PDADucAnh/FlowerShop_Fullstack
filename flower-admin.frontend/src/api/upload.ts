import { apiClient } from './client'

export const uploadApi = {
  upload(file: File, folder?: string) {
    const formData = new FormData()
    formData.append('file', file)
    if (folder) formData.append('folder', folder)
    return apiClient.post<{ url: string }>('/api/Upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },
}
