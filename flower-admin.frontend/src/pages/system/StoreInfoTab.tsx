import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'
import { useState, useEffect } from 'react'
import type { StoreInfoSettings } from '@/types/settings'

export function StoreInfoTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<StoreInfoSettings>({} as StoreInfoSettings)

  useEffect(() => {
    if (allSettings?.store) setForm(allSettings.store)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: StoreInfoSettings) => settingsApi.saveStoreInfo(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu thông tin cửa hàng') },
    onError: () => toast.error('Không thể lưu thông tin'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Thông tin cửa hàng</CardTitle></CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="text-sm font-medium">Tên cửa hàng</label><Input value={form.storeName || ''} onChange={(e) => setForm({ ...form, storeName: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Hotline</label><Input value={form.hotline || ''} onChange={(e) => setForm({ ...form, hotline: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Email</label><Input type="email" value={form.email || ''} onChange={(e) => setForm({ ...form, email: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Địa chỉ</label><Input value={form.address || ''} onChange={(e) => setForm({ ...form, address: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Logo (URL)</label><Input value={form.logo || ''} onChange={(e) => setForm({ ...form, logo: e.target.value })} /></div>
            <div><label className="text-sm font-medium">Facebook</label><Input value={form.facebook || ''} onChange={(e) => setForm({ ...form, facebook: e.target.value })} /></div>
            <div><label className="text-sm font-medium">Zalo</label><Input value={form.zalo || ''} onChange={(e) => setForm({ ...form, zalo: e.target.value })} /></div>
            <div><label className="text-sm font-medium">Giờ mở cửa</label><Input value={form.openHours || ''} onChange={(e) => setForm({ ...form, openHours: e.target.value })} /></div>
          </div>
          <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
        </form>
      </CardContent>
    </Card>
  )
}
