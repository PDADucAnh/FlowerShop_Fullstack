export interface CustomerDTO {
  id: number
  fullName: string
  email: string
  phone?: string
  address?: string
  avatar?: string
  totalOrders: number
  successfulDeliveries: number
  failedDeliveries: number
  isBlacklisted: boolean
  fraudScore: number
  isActive: boolean
  createdAt: string
}

export interface UpdateCustomerRequest {
  id: number
  fullName: string
  email: string
  phone?: string
  address?: string
  isActive: boolean
}
