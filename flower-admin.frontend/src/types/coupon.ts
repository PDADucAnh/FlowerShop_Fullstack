export type DiscountType = 'Percentage' | 'FixedAmount'

export interface CouponDTO {
  id: number
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usedCount: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateCouponDTO {
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic?: boolean
  isActive?: boolean
}

export interface UpdateCouponDTO {
  id: number
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic?: boolean
  isActive?: boolean
}

export interface CouponUsageDTO {
  id: number
  couponId: number
  customerId: number
  orderId: number
  discountAmount: number
  usedAt: string
  couponCode?: string
  customerName?: string
}
