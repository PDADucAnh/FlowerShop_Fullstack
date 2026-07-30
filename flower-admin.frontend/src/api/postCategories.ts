import { apiClient } from './client'
import type { PostCategory, CreatePostCategoryRequest, UpdatePostCategoryRequest } from '@/types/postCategory'

export const postCategoriesApi = {
  getAll() {
    return apiClient.get<PostCategory[]>('/api/categories')
  },
  getById(id: number) {
    return apiClient.get<PostCategory>(`/api/categories/${id}`)
  },
  create(data: CreatePostCategoryRequest) {
    return apiClient.post<PostCategory>('/api/categories', data)
  },
  update(id: number, data: UpdatePostCategoryRequest) {
    return apiClient.put(`/api/categories/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/categories/${id}`)
  },
}
