import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'
import { useState, useEffect } from 'react'
import type { ShippingSettings } from '@/types/settings'

export function ShippingTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<ShippingSettings>({} as ShippingSettings)

  useEffect(() => {
    if (allSettings?.shipping) setForm(allSettings.shipping)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: ShippingSettings) => settingsApi.saveShipping(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình vận chuyển') },
    onError: () => toast.error('Không thể lưu cấu hình vận chuyển'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Cấu hình vận chuyển</CardTitle></CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="text-sm font-medium">Phí mặc định (₫)</label><Input type="number" value={form.defaultFee || ''} onChange={(e) => setForm({ ...form, defaultFee: Number(e.target.value) })} required /></div>
            <div><label className="text-sm font-medium">Miễn phí từ (₫)</label><Input type="number" value={form.freeShipFrom || ''} onChange={(e) => setForm({ ...form, freeShipFrom: Number(e.target.value) })} required /></div>
            <div><label className="text-sm font-medium">Khoảng cách tối đa (km)</label><Input type="number" step="0.1" value={form.maxDistance || ''} onChange={(e) => setForm({ ...form, maxDistance: Number(e.target.value) })} required /></div>
            <div><label className="text-sm font-medium">Thời gian giao</label><Input value={form.deliveryTime || ''} onChange={(e) => setForm({ ...form, deliveryTime: e.target.value })} /></div>
          </div>
          <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
        </form>
      </CardContent>
    </Card>
  )
}
