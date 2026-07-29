import { useState } from 'react'
import { PromotionsTab } from './marketing/PromotionsTab'
import { CouponsTab } from './marketing/CouponsTab'

const tabs = [
  { key: 'promotions', label: 'Khuyến mãi' },
  { key: 'coupons', label: 'Mã giảm giá' },
]

export function MarketingPage() {
  const [activeTab, setActiveTab] = useState('promotions')

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Tiếp thị</h1>
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
      {activeTab === 'promotions' && <PromotionsTab />}
      {activeTab === 'coupons' && <CouponsTab />}
    </div>
  )
}
