export interface Product {
  id: number
  sku?: string
  name: string
  description?: string
  slug?: string
  price: number
  stockQuantity: number
  imageUrl?: string
  images: ProductImage[]
  categoryProductId: number
  categoryProductName?: string
  isActive: boolean
  flowerMeaning?: string
  origin?: string
  careInstruction?: string
  viewCount: number
  createdAt?: string
}

export interface ProductImage {
  id: number
  imageUrl: string
  sortOrder: number
}

export interface CreateProductRequest {
  name: string
  slug?: string
  sku?: string
  description?: string
  price: number
  stockQuantity: number
  categoryProductId: number
  isActive?: boolean
  flowerMeaning?: string
  origin?: string
  careInstruction?: string
  newImages?: string[]
}

export interface UpdateProductRequest extends CreateProductRequest {
  id: number
}

export interface PagedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
