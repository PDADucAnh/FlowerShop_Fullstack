import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'
import { useState, useEffect } from 'react'
import type { SmtpSettings } from '@/types/settings'

export function SmtpTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<SmtpSettings>({} as SmtpSettings)

  useEffect(() => {
    if (allSettings?.smtp) setForm(allSettings.smtp)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: SmtpSettings) => settingsApi.saveSmtp(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['settings'] }); toast.success('Đã lưu cấu hình SMTP') },
    onError: () => toast.error('Không thể lưu cấu hình SMTP'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cài đặt</p></div>

  return (
    <Card>
      <CardHeader><CardTitle className="text-base">Cấu hình SMTP</CardTitle></CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div><label className="text-sm font-medium">Host</label><Input value={form.host || ''} onChange={(e) => setForm({ ...form, host: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Port</label><Input type="number" value={form.port || ''} onChange={(e) => setForm({ ...form, port: Number(e.target.value) })} required /></div>
            <div><label className="text-sm font-medium">Username</label><Input value={form.username || ''} onChange={(e) => setForm({ ...form, username: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Password</label><Input type="password" value={form.password || ''} onChange={(e) => setForm({ ...form, password: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Tên người gửi</label><Input value={form.senderName || ''} onChange={(e) => setForm({ ...form, senderName: e.target.value })} required /></div>
            <div><label className="text-sm font-medium">Email người gửi</label><Input type="email" value={form.senderEmail || ''} onChange={(e) => setForm({ ...form, senderEmail: e.target.value })} required /></div>
          </div>
          <Button type="submit" disabled={mutation.isPending}><Save className="mr-1 size-4" />Lưu</Button>
        </form>
      </CardContent>
    </Card>
  )
}
