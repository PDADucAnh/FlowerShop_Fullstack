import { apiClient } from './client'
import type { Product, CreateProductRequest, UpdateProductRequest, PagedResponse } from '@/types/product'

export interface ProductListParams {
  page?: number
  pageSize?: number
  categoryProductId?: number | null
  minPrice?: number | null
  maxPrice?: number | null
  includeInactive?: boolean
}

export const productsApi = {
  getPaged(params: ProductListParams = {}) {
    return apiClient.get<PagedResponse<Product>>('/api/Products/paged', { params })
  },

  getById(id: number) {
    return apiClient.get<Product>(`/api/Products/${id}`)
  },

  search(query: string) {
    return apiClient.get<Product[]>('/api/Products/search', { params: { query } })
  },

  create(data: CreateProductRequest) {
    return apiClient.post<Product>('/api/Products', data)
  },

  update(id: number, data: UpdateProductRequest) {
    return apiClient.put(`/api/Products/${id}`, data)
  },

  delete(id: number) {
    return apiClient.delete(`/api/Products/${id}`)
  },

  bulkDelete(productIds: number[]) {
    return apiClient.post('/api/Products/bulk-delete', { productIds })
  },

  getImages(productId: number) {
    return apiClient.get(`/api/Products/${productId}/images`)
  },

  addImage(productId: number, imageUrl: string) {
    return apiClient.post(`/api/Products/${productId}/images`, { imageUrl })
  },

  deleteImage(productId: number, imageId: number) {
    return apiClient.delete(`/api/Products/${productId}/images/${imageId}`)
  },
}
