export interface AdvertisementDTO {
  id: number
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
}

export interface CreateAdvertisementDTO {
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive?: boolean
}

export interface UpdateAdvertisementDTO {
  id: number
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive?: boolean
}
