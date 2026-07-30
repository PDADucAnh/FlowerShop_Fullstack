export interface EmailHistory {
  id: number
  customerId?: number
  orderId?: number
  emailType: string
  recipient: string
  subject?: string
  status: string
  sentAt?: string
  createdAt: string
}
