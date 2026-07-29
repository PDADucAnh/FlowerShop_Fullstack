import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { postsApi } from '@/api/posts'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent } from '@/components/ui/card'
import { Search, Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'

export function PostsTab() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['posts', page, search],
    queryFn: () => postsApi.getPaged({ page, pageSize: 10, search: search || undefined }).then((r) => r.data),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => postsApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã xóa bài viết') },
    onError: () => toast.error('Không thể xóa bài viết'),
  })

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải bài viết</p></div>

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between gap-4">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input placeholder="Tìm kiếm bài viết…" className="pl-9" value={search} onChange={(e) => { setSearch(e.target.value); setPage(1) }} />
        </div>
        <Button size="sm" onClick={() => navigate('/content/posts/new')}><Plus className="mr-1 size-4" />Thêm bài viết</Button>
      </div>

      <Card>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="px-4 py-3 font-medium">Tiêu đề</th>
                  <th className="px-4 py-3 font-medium">Danh mục</th>
                  <th className="px-4 py-3 font-medium">Ngày tạo</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3">{item.title}</td>
                    <td className="px-4 py-3 text-muted-foreground">{item.categoryName}</td>
                    <td className="px-4 py-3 text-muted-foreground">{new Date(item.createdDate).toLocaleDateString('vi-VN')}</td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => navigate(`/content/posts/${item.id}/edit`)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa bài viết này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có bài viết nào</div>
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
