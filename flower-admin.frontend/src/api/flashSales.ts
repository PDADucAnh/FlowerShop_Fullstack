import { apiClient } from './client'
import type { FlashSale, CreateFlashSaleRequest, UpdateFlashSaleRequest, FlashSalePreviewRequest, FlashSaleProductPreview, BulkAddFlashSaleRequest } from '@/types/flashSale'

export const flashSalesApi = {
  getAll() {
    return apiClient.get<FlashSale[]>('/api/FlashSales')
  },
  getById(id: number) {
    return apiClient.get<FlashSale>(`/api/FlashSales/${id}`)
  },
  create(data: CreateFlashSaleRequest) {
    return apiClient.post<FlashSale>('/api/FlashSales', data)
  },
  update(id: number, data: UpdateFlashSaleRequest) {
    return apiClient.put(`/api/FlashSales/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/FlashSales/${id}`)
  },
  previewByCategory(data: FlashSalePreviewRequest) {
    return apiClient.post<FlashSaleProductPreview[]>('/api/FlashSales/preview/category', data)
  },
  previewByBestSeller(data: FlashSalePreviewRequest) {
    return apiClient.post<FlashSaleProductPreview[]>('/api/FlashSales/preview/bestseller', data)
  },
  previewByExcel(flashSaleId: number, file: File, defaultDiscountPercent?: number) {
    const formData = new FormData()
    formData.append('flashSaleId', String(flashSaleId))
    if (defaultDiscountPercent !== undefined) {
      formData.append('defaultDiscountPercent', String(defaultDiscountPercent))
    }
    formData.append('file', file)
    return apiClient.post<FlashSaleProductPreview[]>('/api/FlashSales/preview/excel', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
  },
  bulkAdd(data: BulkAddFlashSaleRequest) {
    return apiClient.post<{ added: number }>('/api/FlashSales/bulk-add', data)
  },
}
