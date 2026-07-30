import { useState } from 'react'
import { StoreInfoTab } from './system/StoreInfoTab'
import { SmtpTab } from './system/SmtpTab'
import { VnPayTab } from './system/VnPayTab'
import { ShippingTab } from './system/ShippingTab'
import { OrderTab } from './system/OrderTab'
import { CloudinaryTab } from './system/CloudinaryTab'

const tabs = [
  { key: 'store', label: 'Cửa hàng' },
  { key: 'smtp', label: 'SMTP' },
  { key: 'vnpay', label: 'VNPay' },
  { key: 'shipping', label: 'Vận chuyển' },
  { key: 'order', label: 'Đơn hàng' },
  { key: 'cloudinary', label: 'Cloudinary' },
]

export function SystemSettingsPage() {
  const [activeTab, setActiveTab] = useState('store')

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Cài đặt hệ thống</h1>
      <div className="flex flex-wrap gap-2">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
              activeTab === tab.key
                ? 'bg-primary text-primary-foreground border-primary'
                : 'bg-background text-muted-foreground border-border hover:bg-muted'
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>
      {activeTab === 'store' && <StoreInfoTab />}
      {activeTab === 'smtp' && <SmtpTab />}
      {activeTab === 'vnpay' && <VnPayTab />}
      {activeTab === 'shipping' && <ShippingTab />}
      {activeTab === 'order' && <OrderTab />}
      {activeTab === 'cloudinary' && <CloudinaryTab />}
    </div>
  )
}
