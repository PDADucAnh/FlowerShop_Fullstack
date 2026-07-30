import { cn } from '@/lib/utils'
import { Link, useLocation } from 'react-router-dom'
import {
  LayoutDashboard,
  ShoppingBag,
  Package,
  FolderTree,
  Users,
  MessageSquare,
  FileText,
  Megaphone,
  Settings,
  Upload,
  ShieldCheck,
  Bell,
  type LucideIcon,
} from 'lucide-react'

interface NavItem {
  label: string
  href: string
  icon: LucideIcon
}

const navItems: NavItem[] = [
  { label: 'Tổng quan', href: '/', icon: LayoutDashboard },
  { label: 'Đơn hàng', href: '/orders', icon: ShoppingBag },
  { label: 'Sản phẩm', href: '/products', icon: Package },
  { label: 'Danh mục', href: '/products/categories', icon: FolderTree },
  { label: 'Nhập hàng loạt', href: '/products/import', icon: Upload },
  { label: 'Khách hàng', href: '/customers', icon: Users },
  { label: 'Liên hệ', href: '/contacts', icon: MessageSquare },
  { label: 'Nội dung', href: '/content', icon: FileText },
  { label: 'Nhân viên', href: '/users', icon: ShieldCheck },
  { label: 'Thông báo', href: '/notifications', icon: Bell },
  { label: 'Marketing', href: '/marketing', icon: Megaphone },
  { label: 'Hệ thống', href: '/system', icon: Settings },
]

interface AppSidebarProps {
  collapsed: boolean
}

export function AppSidebar({ collapsed }: AppSidebarProps) {
  const { pathname } = useLocation()

  return (
    <aside
      className={cn(
        'flex flex-col border-r bg-surface transition-all duration-300',
        collapsed ? 'w-16' : 'w-64'
      )}
    >
      <div className="flex h-16 items-center gap-2 border-b px-4">
        <div className="flex size-9 items-center justify-center rounded-lg bg-primary">
          <span className="text-lg font-bold text-on-primary">F</span>
        </div>
        {!collapsed && (
          <span className="text-lg font-semibold text-on-surface">
            Flower Admin
          </span>
        )}
      </div>
      <nav className="flex-1 space-y-1 p-2">
        {navItems.map((item) => {
          const Icon = item.icon
          const active = pathname === item.href
          return (
            <Link
              key={item.href}
              to={item.href}
              className={cn(
                'flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors',
                active
                  ? 'bg-primary/10 text-primary'
                  : 'text-on-surface-variant hover:bg-secondary-container/50 hover:text-on-surface'
              )}
            >
              <Icon className="size-5 shrink-0" />
              {!collapsed && <span>{item.label}</span>}
            </Link>
          )
        })}
      </nav>
    </aside>
  )
}
