import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

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
