import { useAuth } from '@/context/AuthContext'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { Button } from '@/components/ui/button'
import { PanelLeftClose, PanelLeft, LogOut, User } from 'lucide-react'

interface AppHeaderProps {
  collapsed: boolean
  onToggle: () => void
}

export function AppHeader({ collapsed, onToggle }: AppHeaderProps) {
  const { user, logout } = useAuth()

  const initials = user?.fullName
    ? user.fullName
        .split(' ')
        .map((n) => n[0])
        .join('')
        .toUpperCase()
        .slice(0, 2)
    : '??'

  return (
    <header className="flex h-16 items-center justify-between border-b bg-surface px-4">
      <Button variant="ghost" size="icon" onClick={onToggle}>
        {collapsed ? (
          <PanelLeft className="size-5" />
        ) : (
          <PanelLeftClose className="size-5" />
        )}
      </Button>
      <DropdownMenu>
        <DropdownMenuTrigger className="flex cursor-pointer items-center gap-2 rounded-lg px-2 py-1.5 outline-none hover:bg-muted">
          <Avatar className="size-8">
            <AvatarFallback className="bg-primary text-xs text-on-primary">
              {initials}
            </AvatarFallback>
          </Avatar>
          <span className="text-sm font-medium text-on-surface">
            {user?.fullName}
          </span>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end" className="w-48">
          <DropdownMenuItem disabled>
            <User className="mr-2 size-4" />
            {user?.email}
          </DropdownMenuItem>
          <DropdownMenuItem onClick={logout}>
            <LogOut className="mr-2 size-4" />
            Đăng xuất
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </header>
  )
}
