import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { advertisementsApi } from '@/api/advertisements'
import { Button } from '@/components/ui/button'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import type { AdvertisementDTO, CreateAdvertisementDTO, UpdateAdvertisementDTO } from '@/types/advertisement'

export function BannersTab() {
  const [page, setPage] = useState(1)
  const [editItem, setEditItem] = useState<AdvertisementDTO | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['advertisements', page],
    queryFn: () => advertisementsApi.getPaged(page).then((r) => r.data),
  })

  const createMutation = useMutation({
    mutationFn: (dto: CreateAdvertisementDTO) => advertisementsApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); setDialogOpen(false); toast.success('Đã thêm banner') },
    onError: () => toast.error('Không thể thêm banner'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpdateAdvertisementDTO }) => advertisementsApi.update(id, dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật banner') },
    onError: () => toast.error('Không thể cập nhật banner'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => advertisementsApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['advertisements'] }); toast.success('Đã xóa banner') },
    onError: () => toast.error('Không thể xóa banner'),
  })

  const openCreate = () => { setEditItem(null); setDialogOpen(true) }
  const openEdit = (item: AdvertisementDTO) => { setEditItem(item); setDialogOpen(true) }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const form = e.currentTarget
    const formData = new FormData(form)
    const title = formData.get('title') as string
    if (!title) return

    if (editItem) {
      updateMutation.mutate({
        id: editItem.id,
        dto: {
          id: editItem.id,
          title,
          subtitle: formData.get('subtitle') as string || undefined,
          imageUrl: formData.get('imageUrl') as string || undefined,
          linkUrl: formData.get('linkUrl') as string || undefined,
          sortOrder: Number(formData.get('sortOrder')) || 0,
          isActive: formData.get('isActive') === 'on',
        },
      })
    } else {
      createMutation.mutate({
        title,
        subtitle: formData.get('subtitle') as string || undefined,
        imageUrl: formData.get('imageUrl') as string || undefined,
        linkUrl: formData.get('linkUrl') as string || undefined,
        sortOrder: Number(formData.get('sortOrder')) || 0,
      })
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải banner</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button size="sm" />} onClick={openCreate}>
            <Plus className="mr-1 size-4" />Thêm banner
          </DialogTrigger>
          <DialogContent>
            <DialogHeader><DialogTitle>{editItem ? 'Sửa banner' : 'Thêm banner'}</DialogTitle></DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <input name="title" defaultValue={editItem?.title ?? ''} placeholder="Tiêu đề" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="subtitle" defaultValue={editItem?.subtitle ?? ''} placeholder="Phụ đề (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="imageUrl" defaultValue={editItem?.imageUrl ?? ''} placeholder="URL hình ảnh" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="linkUrl" defaultValue={editItem?.linkUrl ?? ''} placeholder="URL liên kết (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="sortOrder" type="number" defaultValue={editItem?.sortOrder ?? 0} placeholder="Thứ tự" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <label className="flex items-center gap-2 text-sm">
                <input name="isActive" type="checkbox" defaultChecked={editItem?.isActive ?? true} />
                Kích hoạt
              </label>
              <DialogFooter>
                <DialogClose render={<Button variant="outline" type="button" />}>Hủy</DialogClose>
                <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="px-4 py-3 font-medium">Tiêu đề</th>
                  <th className="px-4 py-3 font-medium">Thứ tự</th>
                  <th className="px-4 py-3 font-medium">Trạng thái</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3">{item.title}</td>
                    <td className="px-4 py-3">{item.sortOrder}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                        {item.isActive ? 'Hoạt động' : 'Ẩn'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa banner này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có banner nào</div>
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
