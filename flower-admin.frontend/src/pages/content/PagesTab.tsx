import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { pagesApi } from '@/api/pages'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

export function PagesTab() {
  const [page, setPage] = useState(1)
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['pages', page],
    queryFn: () => pagesApi.getPaged(page).then((r) => r.data),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => pagesApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã xóa trang') },
    onError: () => toast.error('Không thể xóa trang'),
  })

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải trang</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Button size="sm" onClick={() => navigate('/content/pages/new')}><Plus className="mr-1 size-4" />Thêm trang</Button>
      </div>

      <Card>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="px-4 py-3 font-medium">Tiêu đề</th>
                  <th className="px-4 py-3 font-medium">Slug</th>
                  <th className="px-4 py-3 font-medium">Trạng thái</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3">{item.title}</td>
                    <td className="px-4 py-3 text-muted-foreground">/{item.slug}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                        {item.isActive ? 'Hiển thị' : 'Ẩn'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => navigate(`/content/pages/${item.id}/edit`)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa trang này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có trang nào</div>
          )}
        </CardContent>
      </Card>

      {data && (data.totalPages ?? 0) > 1 && (
        <div className="flex items-center justify-between text-sm">
          <span className="text-muted-foreground">Trang {data.page} / {data.totalPages}</span>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
            <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
          </div>
        </div>
      )}
    </div>
  )
}
