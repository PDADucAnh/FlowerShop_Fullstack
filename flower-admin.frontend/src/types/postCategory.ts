export interface PostCategory {
  id: number
  name: string
  description?: string
  slug?: string
}

export interface CreatePostCategoryRequest {
  name: string
  description?: string
  slug?: string
}

export interface UpdatePostCategoryRequest extends CreatePostCategoryRequest {
  id: number
}
