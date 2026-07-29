import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { layoutApi } from '@/api/layout'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Save } from 'lucide-react'
import { toast } from 'sonner'

export function LayoutTab() {
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['layout'],
    queryFn: () => layoutApi.getLayout().then((r) => r.data),
  })

  const [headerJson, setHeaderJson] = useState('')
  const [footerJson, setFooterJson] = useState('')

  if (data && !headerJson && !footerJson) {
    if (headerJson === '') setHeaderJson(JSON.stringify(data.header, null, 2))
    if (footerJson === '') setFooterJson(JSON.stringify(data.footer, null, 2))
  }

  const saveHeaderMutation = useMutation({
    mutationFn: (dto: any) => layoutApi.saveHeader(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['layout'] }); toast.success('Đã lưu header') },
    onError: () => toast.error('Không thể lưu header'),
  })

  const saveFooterMutation = useMutation({
    mutationFn: (dto: any) => layoutApi.saveFooter(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['layout'] }); toast.success('Đã lưu footer') },
    onError: () => toast.error('Không thể lưu footer'),
  })

  const handleSaveHeader = () => {
    try {
      const parsed = JSON.parse(headerJson)
      saveHeaderMutation.mutate(parsed)
    } catch {
      toast.error('JSON header không hợp lệ')
    }
  }

  const handleSaveFooter = () => {
    try {
      const parsed = JSON.parse(footerJson)
      saveFooterMutation.mutate(parsed)
    } catch {
      toast.error('JSON footer không hợp lệ')
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải cấu hình giao diện</p></div>

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader><CardTitle className="text-base">Header</CardTitle></CardHeader>
        <CardContent className="space-y-3">
          <textarea
            className="w-full h-64 rounded-md border bg-background p-3 font-mono text-xs"
            value={headerJson}
            onChange={(e) => setHeaderJson(e.target.value)}
          />
          <Button size="sm" onClick={handleSaveHeader} disabled={saveHeaderMutation.isPending}>
            <Save className="mr-1 size-4" />Lưu header
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle className="text-base">Footer</CardTitle></CardHeader>
        <CardContent className="space-y-3">
          <textarea
            className="w-full h-64 rounded-md border bg-background p-3 font-mono text-xs"
            value={footerJson}
            onChange={(e) => setFooterJson(e.target.value)}
          />
          <Button size="sm" onClick={handleSaveFooter} disabled={saveFooterMutation.isPending}>
            <Save className="mr-1 size-4" />Lưu footer
          </Button>
        </CardContent>
      </Card>
    </div>
  )
}
