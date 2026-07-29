import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Package, ShoppingBag, Users, DollarSign } from 'lucide-react'

const stats = [
  { label: 'Tổng đơn hàng', value: '—', icon: ShoppingBag, color: 'text-blue-600' },
  { label: 'Tổng sản phẩm', value: '—', icon: Package, color: 'text-green-600' },
  { label: 'Khách hàng', value: '—', icon: Users, color: 'text-purple-600' },
  { label: 'Doanh thu', value: '—', icon: DollarSign, color: 'text-orange-600' },
]

export function DashboardPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold text-on-surface">Tổng quan</h1>
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat) => {
          const Icon = stat.icon
          return (
            <Card key={stat.label}>
              <CardHeader className="flex flex-row items-center justify-between pb-2">
                <CardTitle className="text-sm font-medium">
                  {stat.label}
                </CardTitle>
                <Icon className={`size-5 ${stat.color}`} />
              </CardHeader>
              <CardContent>
                <p className="text-3xl font-bold text-on-surface">
                  {stat.value}
                </p>
              </CardContent>
            </Card>
          )
        })}
      </div>
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Hoạt động gần đây</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-on-surface-variant">
            Dữ liệu sẽ xuất hiện sau khi bạn thêm sản phẩm và nhận đơn hàng.
          </p>
        </CardContent>
      </Card>
    </div>
  )
}
