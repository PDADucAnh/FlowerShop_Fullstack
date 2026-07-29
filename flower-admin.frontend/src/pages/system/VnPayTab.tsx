import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'
import { useState, useEffect } from 'react'
import type { VNPaySettings } from '@/types/settings'

export function VnPayTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<VNPaySettings>({} as VNPaySettings)

  useEffect(() => {
    if (allSettings?.vnPay) setForm(allSettings.vnPay)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: VNPaySettings) => settingsApi.saveVnPay(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình VNPay') },
    onError: () => toast.error('Không thể lưu cấu hình VNPay'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Cấu hình VNPay</CardTitle></CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="text-sm font-medium">TmnCode</label><Input value={form.tmnCode || ''} onChange={(e) => setForm({ ...form, tmnCode: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">HashSecret</label><Input type="password" value={form.hashSecret || ''} onChange={(e) => setForm({ ...form, hashSecret: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">ReturnUrl</label><Input value={form.returnUrl || ''} onChange={(e) => setForm({ ...form, returnUrl: e.target.value })} required /></div>
            <div className="flex items-end gap-4">
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={form.isSandbox ?? true} onChange={(e) => setForm({ ...form, isSandbox: e.target.checked })} />
                Sandbox
              </label>
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={form.enablePayment ?? true} onChange={(e) => setForm({ ...form, enablePayment: e.target.checked })} />
                Bật thanh toán
              </label>
            </div>
          </div>
          <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
        </form>
      </CardContent>
    </Card>
  )
}
