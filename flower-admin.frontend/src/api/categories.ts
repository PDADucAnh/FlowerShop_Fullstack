import { apiClient } from './client'
import type { CategoryProduct, CreateCategoryRequest, UpdateCategoryRequest } from '@/types/category'

export const categoriesApi = {
  getAll() {
    return apiClient.get<CategoryProduct[]>('/api/CategoriesProducts')
  },

  getById(id: number) {
    return apiClient.get<CategoryProduct>(`/api/CategoriesProducts/${id}`)
  },

  create(data: CreateCategoryRequest) {
    return apiClient.post<CategoryProduct>('/api/CategoriesProducts', data)
  },

  update(id: number, data: UpdateCategoryRequest) {
    return apiClient.put(`/api/CategoriesProducts/${id}`, data)
  },

  delete(id: number) {
    return apiClient.delete(`/api/CategoriesProducts/${id}`)
  },
}
