import { apiClient } from './client'
import type { AllSystemSettings, StoreInfoSettings, SmtpSettings, VNPaySettings, ShippingSettings, OrderSettings, CloudinarySettings } from '@/types/settings'

export const settingsApi = {
  getAll() {
    return apiClient.get<AllSystemSettings>('/api/settings')
  },
  saveStoreInfo(dto: StoreInfoSettings) {
    return apiClient.put('/api/settings/store-info', dto)
  },
  saveSmtp(dto: SmtpSettings) {
    return apiClient.put('/api/settings/smtp', dto)
  },
  saveVnPay(dto: VNPaySettings) {
    return apiClient.put('/api/settings/vnpay', dto)
  },
  saveShipping(dto: ShippingSettings) {
    return apiClient.put('/api/settings/shipping', dto)
  },
  saveOrder(dto: OrderSettings) {
    return apiClient.put('/api/settings/order', dto)
  },
  saveCloudinary(dto: CloudinarySettings) {
    return apiClient.put('/api/settings/cloudinary', dto)
  },
}
