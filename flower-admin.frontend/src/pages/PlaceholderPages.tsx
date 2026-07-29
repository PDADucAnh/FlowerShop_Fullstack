import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

const placeholderPages = [
  { href: '/orders', title: 'Đơn hàng' },
  { href: '/products', title: 'Sản phẩm' },
  { href: '/content', title: 'Nội dung' },
  { href: '/marketing', title: 'Marketing' },
  { href: '/system', title: 'Hệ thống' },
]

export function PlaceholderPage({ title }: { title: string }) {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-semibold text-on-surface">{title}</h1>
      <Card>
        <CardHeader>
          <CardTitle className="text-base">Sắp ra mắt</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-on-surface-variant">
            Trang này sẽ được triển khai trong phiên bản tiếp theo.
          </p>
        </CardContent>
      </Card>
    </div>
  )
}

export function OrdersPage() {
  return <PlaceholderPage title="Đơn hàng" />
}

export function ContentPage() {
  return <PlaceholderPage title="Nội dung" />
}

export function MarketingPage() {
  return <PlaceholderPage title="Marketing" />
}

export function SystemPage() {
  return <PlaceholderPage title="Hệ thống" />
}
