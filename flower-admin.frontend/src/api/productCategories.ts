import { apiClient } from './client'
import type { ProductCategory, CreateProductCategoryRequest, UpdateProductCategoryRequest } from '@/types/productCategory'

export const productCategoriesApi = {
  getAll() {
    return apiClient.get<ProductCategory[]>('/api/ProductCategories')
  },
  getById(id: number) {
    return apiClient.get<ProductCategory>(`/api/ProductCategories/${id}`)
  },
  create(data: CreateProductCategoryRequest) {
    return apiClient.post<ProductCategory>('/api/ProductCategories', data)
  },
  update(id: number, data: UpdateProductCategoryRequest) {
    return apiClient.put(`/api/ProductCategories/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/ProductCategories/${id}`)
  },
}
