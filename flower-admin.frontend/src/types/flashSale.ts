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
