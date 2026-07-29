import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'
import { useState, useEffect } from 'react'
import type { OrderSettings } from '@/types/settings'

export function OrderTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<OrderSettings>({} as OrderSettings)

  useEffect(() => {
    if (allSettings?.order) setForm(allSettings.order)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: OrderSettings) => settingsApi.saveOrder(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình đơn hàng') },
    onError: () => toast.error('Không thể lưu cấu hình đơn hàng'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Cấu hình đơn hàng</CardTitle></CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="text-sm font-medium">Tự động hủy sau (phút)</label><Input type="number" value={form.autoCancelMinutes || ''} onChange={(e) => setForm({ ...form, autoCancelMinutes: Number(e.target.value) })} required /></div>
            <div className="flex items-end gap-4">
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={form.enableCOD ?? true} onChange={(e) => setForm({ ...form, enableCOD: e.target.checked })} />
                Cho phép COD
              </label>
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={form.enableOnlinePayment ?? true} onChange={(e) => setForm({ ...form, enableOnlinePayment: e.target.checked })} />
                Cho phép thanh toán online
              </label>
            </div>
          </div>
          <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
        </form>
      </CardContent>
    </Card>
  )
}
