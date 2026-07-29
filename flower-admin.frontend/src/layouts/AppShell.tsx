import { useState } from 'react'
import { Outlet } from 'react-router-dom'
import { AppSidebar } from '@/components/AppSidebar'
import { AppHeader } from '@/components/AppHeader'

export function AppShell() {
  const [collapsed, setCollapsed] = useState(false)

  return (
    <div className="flex h-screen overflow-hidden">
      <AppSidebar collapsed={collapsed} />
      <div className="flex flex-1 flex-col overflow-hidden">
        <AppHeader
          collapsed={collapsed}
          onToggle={() => setCollapsed((prev) => !prev)}
        />
        <main className="flex-1 overflow-y-auto bg-surface-container-low p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
