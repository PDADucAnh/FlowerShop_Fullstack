export interface CategoryProduct {
  id: number
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}

export interface UpdateCategoryRequest {
  id: number
  name: string
  description?: string
  slug?: string
  imageUrl?: string
}
