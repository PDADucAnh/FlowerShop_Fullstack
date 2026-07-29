import { useState } from 'react'
import { Outlet, useLocation } from 'react-router-dom'
import { BannersTab } from './content/BannersTab'
import { PostsTab } from './content/PostsTab'
import { PagesTab } from './content/PagesTab'
import { LayoutTab } from './content/LayoutTab'

const tabs = [
  { key: 'banners', label: 'Banner' },
  { key: 'posts', label: 'Bài viết' },
  { key: 'pages', label: 'Trang tĩnh' },
  { key: 'layout', label: 'Giao diện' },
]

export function ContentPage() {
  const [activeTab, setActiveTab] = useState('banners')
  const location = useLocation()

  if (location.pathname !== '/content') {
    return <Outlet />
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold">Nội dung</h1>
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
      {activeTab === 'banners' && <BannersTab />}
      {activeTab === 'posts' && <PostsTab />}
      {activeTab === 'pages' && <PagesTab />}
      {activeTab === 'layout' && <LayoutTab />}
    </div>
  )
}
