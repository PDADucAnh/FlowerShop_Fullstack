export interface PageDTO {
  id: number
  title: string
  slug?: string
  content: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreatePageDTO {
  title: string
  slug?: string
  content: string
  isActive?: boolean
}

export interface UpdatePageDTO {
  id: number
  title: string
  slug?: string
  content: string
  isActive?: boolean
}
