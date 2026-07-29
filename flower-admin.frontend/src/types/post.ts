export interface PostDTO {
  id: number
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl: string
  createdDate: string
  categoryId: number
  categoryName?: string
}

export interface CreatePostDTO {
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl?: string
  categoryId: number
}

export interface UpdatePostDTO {
  id: number
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl?: string
  categoryId: number
}
