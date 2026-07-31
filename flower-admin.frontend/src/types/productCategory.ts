export interface ProductCategory {
  id: number
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface CreateProductCategoryRequest {
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface UpdateProductCategoryRequest {
  id: number
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}
