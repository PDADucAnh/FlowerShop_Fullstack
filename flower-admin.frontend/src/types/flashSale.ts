export interface FlashSaleProduct {
  id: number
  flashSaleId: number
  productId: number
  productName?: string
  productImageUrl?: string
  originalPrice: number
  salePrice: number
  discountPercent: number
}

export interface FlashSale {
  id: number
  name: string
  description?: string
  startDate: string
  endDate: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
  products?: FlashSaleProduct[]
}

export interface CreateFlashSaleProductRequest {
  productId: number
  salePrice: number
}

export interface CreateFlashSaleRequest {
  name: string
  description?: string
  startDate: string
  endDate: string
  isActive: boolean
  products: CreateFlashSaleProductRequest[]
}

export interface UpdateFlashSaleRequest {
  id: number
  name?: string
  description?: string
  startDate?: string
  endDate?: string
  isActive?: boolean
  products?: CreateFlashSaleProductRequest[]
}

export interface FlashSalePreviewRequest {
  flashSaleId: number
  productCategoryIds?: number[]
  minStockQuantity?: number
  topCount?: number
  defaultDiscountPercent?: number
}

export interface FlashSaleProductPreview {
  productId: number
  sku?: string
  productName?: string
  productImageUrl?: string
  originalPrice: number
  stockQuantity: number
  suggestedSalePrice: number
  quantity: number
  discountPercent: number
}

export interface BulkAddFlashSaleProductRequest {
  productId: number
  salePrice: number
  quantity: number
}

export interface BulkAddFlashSaleRequest {
  flashSaleId: number
  products: BulkAddFlashSaleProductRequest[]
}
