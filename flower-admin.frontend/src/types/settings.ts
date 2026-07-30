export interface StoreInfoSettings {
  storeName: string
  logo: string
  hotline: string
  email: string
  address: string
  facebook?: string
  zalo?: string
  openHours?: string
  googleMapsEmbedUrl?: string
}

export interface SmtpSettings {
  host: string
  port: number
  username: string
  password: string
  senderName: string
  senderEmail: string
}

export interface VNPaySettings {
  tmnCode: string
  hashSecret: string
  returnUrl: string
  isSandbox: boolean
  enablePayment: boolean
}

export interface ShippingSettings {
  defaultFee: number
  freeShipFrom: number
  maxDistance: number
  deliveryTime: string
}

export interface CloudinarySettings {
  cloudName: string
  apiKey: string
  apiSecret: string
  folder: string
}

export interface OrderSettings {
  autoCancelMinutes: number
  enableCOD: boolean
  enableOnlinePayment: boolean
}

export interface AllSystemSettings {
  store: StoreInfoSettings
  smtp: SmtpSettings
  vnPay: VNPaySettings
  shipping: ShippingSettings
  order: OrderSettings
  cloudinary: CloudinarySettings
}
