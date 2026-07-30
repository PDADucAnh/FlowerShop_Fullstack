import { useState, useEffect } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import type { CloudinarySettings } from '@/types/settings'

export function CloudinaryTab() {
  const queryClient = useQueryClient()
  const { data: allSettings, isLoading, error } = useQuery({
    queryKey: ['settings'],
    queryFn: () => settingsApi.getAll().then((r) => r.data),
  })

  const [form, setForm] = useState<CloudinarySettings>({
    cloudName: '', apiKey: '', apiSecret: '', folder: 'flowershop_products',
  })

  useEffect(() => {
    if (allSettings?.cloudinary) setForm(allSettings.cloudinary)
  }, [allSettings])

  const mutation = useMutation({
    mutationFn: (dto: CloudinarySettings) => settingsApi.saveCloudinary(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['settings'] })
      toast.success('Đã lưu cấu hình Cloudinary')
    },
    onError: () => toast.error('Không thể lưu cấu hình Cloudinary'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cấu hình</p></div>

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Card>
        <CardContent className="space-y-4 pt-6">
          <div className="space-y-2">
            <Label>Cloud Name</Label>
            <Input value={form.cloudName} onChange={(e) => setForm({ ...form, cloudName: e.target.value })} required placeholder="your-cloud-name" />
          </div>
          <div className="space-y-2">
            <Label>API Key</Label>
            <Input value={form.apiKey} onChange={(e) => setForm({ ...form, apiKey: e.target.value })} required placeholder="123456789012345" />
          </div>
          <div className="space-y-2">
            <Label>API Secret</Label>
            <Input type="password" value={form.apiSecret} onChange={(e) => setForm({ ...form, apiSecret: e.target.value })} required />
          </div>
          <div className="space-y-2">
            <Label>Upload Folder</Label>
            <Input value={form.folder} onChange={(e) => setForm({ ...form, folder: e.target.value })} placeholder="flowershop_products" />
            <p className="text-xs text-muted-foreground">Thư mục lưu ảnh trên Cloudinary</p>
          </div>
        </CardContent>
      </Card>
      <div className="flex justify-end">
        <Button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? <><Loader2 className="mr-2 size-4 animate-spin" />Đang lưu...</> : 'Lưu cấu hình'}
        </Button>
      </div>
    </form>
  )
}
