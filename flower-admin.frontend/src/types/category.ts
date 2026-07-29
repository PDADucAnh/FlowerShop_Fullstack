export interface CategoryProduct {
  id: number
  name: string
  description?: string
  slug?: string
}

export interface CreateCategoryRequest {
  name: string
  description?: string
  slug?: string
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  id: number
}
