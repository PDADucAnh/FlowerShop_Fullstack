export interface ContactDTO {
  id: number
  name: string
  email: string
  phone?: string
  subject: string
  message: string
  isRead: boolean
  readAt?: string
  createdAt: string
}
