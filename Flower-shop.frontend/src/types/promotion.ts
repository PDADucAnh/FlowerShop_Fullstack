export type PromotionType = 'FlashSale' | 'Seasonal';
export type DiscountType = 'Percent' | 'FixedAmount';

export interface PromotionCampaign {
  id: number;
  name: string;
  description?: string;
  promotionType: PromotionType;
  discountType: DiscountType;
  discountValue: number;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
  priority: number;
  isStackable: boolean;
  productIds: number[];
  createdAt: string;
  updatedAt?: string;
}

export interface CreatePromotionCampaignInput {
  name: string;
  description?: string;
  promotionType: PromotionType;
  discountType: DiscountType;
  discountValue: number;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
  priority?: number;
  isStackable?: boolean;
  productIds?: number[];
}

export interface UpdatePromotionCampaignInput {
  id: number;
  name?: string;
  description?: string;
  promotionType?: PromotionType;
  discountType?: DiscountType;
  discountValue?: number;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
  priority?: number;
  isStackable?: boolean;
  productIds?: number[];
}

export interface CalculatedPrice {
  productId: number;
  originalPrice: number;
  promotionPrice?: number;
  promotionPercent?: number;
  promotionType?: string;
  hasFlashSale: boolean;
  discountAmount?: number;
}

export interface ApplyCouponRequest {
  code: string;
  customerId: number;
  orderTotal: number;
}

export interface ApplyCouponResponse {
  isValid: boolean;
  message?: string;
  discountAmount: number;
  finalTotal: number;
  coupon?: Coupon;
}

export interface Coupon {
  id: number;
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minimumOrderAmount?: number;
  maximumDiscountAmount?: number;
  usageLimit?: number;
  usedCount: number;
  usagePerCustomer?: number;
  customerId?: number;
  startDate?: string;
  endDate?: string;
  isPublic: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCouponInput {
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minimumOrderAmount?: number;
  maximumDiscountAmount?: number;
  usageLimit?: number;
  usagePerCustomer?: number;
  customerId?: number;
  startDate?: string;
  endDate?: string;
  isPublic?: boolean;
  isActive?: boolean;
}

export interface UpdateCouponInput {
  id: number;
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minimumOrderAmount?: number;
  maximumDiscountAmount?: number;
  usageLimit?: number;
  usagePerCustomer?: number;
  customerId?: number;
  startDate?: string;
  endDate?: string;
  isPublic?: boolean;
  isActive?: boolean;
}

export interface CouponUsage {
  id: number;
  couponId: number;
  customerId: number;
  orderId: number;
  discountAmount: number;
  usedAt: string;
  couponCode?: string;
  customerName?: string;
}
