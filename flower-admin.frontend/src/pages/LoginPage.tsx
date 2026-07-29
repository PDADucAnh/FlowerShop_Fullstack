import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { authApi } from '@/api/auth'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { toast } from 'sonner'
import { LogIn } from 'lucide-react'

export function LoginPage() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const { login } = useAuth()
  const navigate = useNavigate()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email || !password) {
      toast.error('Vui lòng nhập email và mật khẩu')
      return
    }
    setLoading(true)
    try {
      const { data } = await authApi.login({ email, password })
      if (data.data) {
        login(data.data.accessToken, data.data.refreshToken, data.data.user)
        toast.success('Đăng nhập thành công')
        navigate('/', { replace: true })
      } else {
        toast.error(data.message || 'Đăng nhập thất bại')
      }
    } catch {
      toast.error('Email hoặc mật khẩu không đúng')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-surface-container-low p-4">
      <Card className="w-full max-w-sm">
        <CardHeader className="space-y-1 text-center">
          <div className="mx-auto mb-2 flex size-12 items-center justify-center rounded-xl bg-primary">
            <LogIn className="size-6 text-on-primary" />
          </div>
          <CardTitle className="text-xl">Đăng nhập</CardTitle>
          <p className="text-sm text-on-surface-variant">
            Quản trị viên Flower Shop
          </p>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <label className="text-sm font-medium text-on-surface">
                Email
              </label>
              <Input
                type="email"
                placeholder="admin@flowershop.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                autoComplete="email"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium text-on-surface">
                Mật khẩu
              </label>
              <Input
                type="password"
                placeholder="••••••••"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                autoComplete="current-password"
              />
            </div>
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? 'Đang đăng nhập…' : 'Đăng nhập'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
