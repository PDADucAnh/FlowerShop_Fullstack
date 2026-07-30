export interface DashboardRevenue {
  today: number
  week: number
  month: number
  year: number
}

export interface DashboardOrder {
  new: number
  pendingConfirmation: number
  preparing: number
  arranging: number
  readyForDelivery: number
  delivering: number
  completed: number
  cancelled: number
}

export interface DashboardCustomer {
  total: number
  new: number
  active: number
  locked: number
}

export interface DashboardProductStats {
  total: number
  active: number
  outOfStock: number
  discontinued: number
}

export interface DashboardPaymentStats {
  vnPay: number
  transfer: number
  cash: number
  pending: number
  failed: number
  refunded: number
}

export interface DashboardInventory {
  inStock: number
  lowStock: number
  outOfStock: number
}

export interface DashboardReview {
  averageRating: number
  totalReviews: number
  latestReviews: number
}

export interface DashboardBanner {
  active: number
  expired: number
}

export interface DashboardNotification {
  id: number
  title?: string
  content?: string
  type?: string
  createdAt: string
  isRead: boolean
}

export interface TopProduct {
  id: number
  name?: string
  imageUrl?: string
  totalSold: number
  totalRevenue: number
}

export interface TopCustomer {
  id: number
  fullName?: string
  email?: string
  totalOrders: number
  totalSpent: number
}

export interface DashboardSummary {
  revenue: DashboardRevenue
  orders: DashboardOrder
  customers: DashboardCustomer
  products: DashboardProductStats
  payments: DashboardPaymentStats
  inventory: DashboardInventory
  reviews: DashboardReview
  banners: DashboardBanner
  notifications: DashboardNotification[]
  topProducts: TopProduct[]
  topCustomers: TopCustomer[]
  activePromotions: number
  totalDiscountGiven: number
  promotionUsageCount: number
}
