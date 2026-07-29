export type PromotionType = 'Automatic' | 'Manual'
export type DiscountType = 'Percentage' | 'FixedAmount'

export interface PromotionCampaignDTO {
  id: number
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
  productIds?: number[]
}

export interface CreatePromotionCampaignDTO {
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive?: boolean
  productIds?: number[]
}

export interface UpdatePromotionCampaignDTO {
  id: number
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive?: boolean
  productIds?: number[]
}
