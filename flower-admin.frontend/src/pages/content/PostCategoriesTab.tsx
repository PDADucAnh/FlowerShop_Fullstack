import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { postCategoriesApi } from '@/api/postCategories'
import { Button } from '@/components/ui/button'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import type { PostCategory, CreatePostCategoryRequest, UpdatePostCategoryRequest } from '@/types/postCategory'

export function PostCategoriesTab() {
  const [editItem, setEditItem] = useState<PostCategory | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data: categories, isLoading, error } = useQuery({
    queryKey: ['post-categories'],
    queryFn: () => postCategoriesApi.getAll().then((r) => r.data),
  })

  const createMutation = useMutation({
    mutationFn: (dto: CreatePostCategoryRequest) => postCategoriesApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); setDialogOpen(false); toast.success('Đã thêm danh mục') },
    onError: () => toast.error('Không thể thêm danh mục'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpdatePostCategoryRequest }) => postCategoriesApi.update(id, dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật danh mục') },
    onError: () => toast.error('Không thể cập nhật danh mục'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => postCategoriesApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); toast.success('Đã xóa danh mục') },
    onError: () => toast.error('Không thể xóa danh mục'),
  })

  const openCreate = () => { setEditItem(null); setDialogOpen(true) }
  const openEdit = (item: PostCategory) => { setEditItem(item); setDialogOpen(true) }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const form = e.currentTarget
    const formData = new FormData(form)
    const name = formData.get('name') as string
    if (!name) return

    const payload = {
      name,
      description: (formData.get('description') as string) || undefined,
      slug: (formData.get('slug') as string) || undefined,
    }

    if (editItem) {
      updateMutation.mutate({ id: editItem.id, dto: { ...payload, id: editItem.id } })
    } else {
      createMutation.mutate(payload)
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải danh mục bài viết</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger asChild onClick={openCreate}>
            <Button size="sm">
              <Plus className="mr-1 size-4" />Thêm danh mục
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader><DialogTitle>{editItem ? 'Sửa danh mục' : 'Thêm danh mục'}</DialogTitle></DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <input name="name" defaultValue={editItem?.name ?? ''} placeholder="Tên danh mục" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="slug" defaultValue={editItem?.slug ?? ''} placeholder="Slug (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <textarea name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả (tùy chọn)" className="flex min-h-20 w-full rounded-md border bg-background px-3 py-2 text-sm" />
              <DialogFooter>
                <DialogClose asChild>
                  <Button variant="outline" type="button">Hủy</Button>
                </DialogClose>
                <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardContent className="p-0">
          {categories && categories.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="px-4 py-3 font-medium">Tên</th>
                  <th className="px-4 py-3 font-medium">Slug</th>
                  <th className="px-4 py-3 font-medium">Mô tả</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {categories.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3 font-medium">{item.name}</td>
                    <td className="px-4 py-3 text-muted-foreground font-mono text-xs">{item.slug || '—'}</td>
                    <td className="px-4 py-3 text-muted-foreground max-w-xs truncate">{item.description || '—'}</td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa danh mục này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có danh mục nào</div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
