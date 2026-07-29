import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { promotionsApi } from '@/api/promotions'
import { Button } from '@/components/ui/button'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2, ToggleLeft, ToggleRight } from 'lucide-react'
import { toast } from 'sonner'
import type { PromotionCampaignDTO } from '@/types/promotion'

export function PromotionsTab() {
  const [page, setPage] = useState(1)
  const [editItem, setEditItem] = useState<PromotionCampaignDTO | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['promotions', page],
    queryFn: () => promotionsApi.getPaged(page).then((r) => r.data),
  })

  const createMutation = useMutation({
    mutationFn: (dto: any) => promotionsApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); setDialogOpen(false); toast.success('Đã thêm khuyến mãi') },
    onError: () => toast.error('Không thể thêm khuyến mãi'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: any }) => promotionsApi.update(id, dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật khuyến mãi') },
    onError: () => toast.error('Không thể cập nhật khuyến mãi'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => promotionsApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); toast.success('Đã xóa khuyến mãi') },
    onError: () => toast.error('Không thể xóa khuyến mãi'),
  })

  const toggleMutation = useMutation({
    mutationFn: ({ id, enable }: { id: number; enable: boolean }) => enable ? promotionsApi.enable(id) : promotionsApi.disable(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['promotions'] }); toast.success('Đã cập nhật trạng thái') },
  })

  const openCreate = () => { setEditItem(null); setDialogOpen(true) }
  const openEdit = (item: PromotionCampaignDTO) => { setEditItem(item); setDialogOpen(true) }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const form = e.currentTarget
    const formData = new FormData(form)
    const name = formData.get('name') as string
    if (!name) return

    const dto: any = {
      name,
      description: formData.get('description') as string || undefined,
      discountType: formData.get('discountType') as string,
      discountValue: Number(formData.get('discountValue')),
      startDate: formData.get('startDate') as string,
      endDate: formData.get('endDate') as string,
      priority: Number(formData.get('priority')) || 0,
      isStackable: formData.get('isStackable') === 'on',
    }

    if (editItem) {
      dto.id = editItem.id
      updateMutation.mutate({ id: editItem.id, dto })
    } else {
      createMutation.mutate(dto)
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải khuyến mãi</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button size="sm" />} onClick={openCreate}>
            <Plus className="mr-1 size-4" />Thêm khuyến mãi
          </DialogTrigger>
          <DialogContent className="max-w-lg">
            <DialogHeader><DialogTitle>{editItem ? 'Sửa khuyến mãi' : 'Thêm khuyến mãi'}</DialogTitle></DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-3">
              <input name="name" defaultValue={editItem?.name ?? ''} placeholder="Tên khuyến mãi" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <div className="grid grid-cols-2 gap-3">
                <select name="discountType" defaultValue={editItem?.discountType ?? 'Percentage'} className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm">
                  <option value="Percentage">Phần trăm</option>
                  <option value="FixedAmount">Số tiền cố định</option>
                </select>
                <input name="discountValue" type="number" step="0.01" defaultValue={editItem?.discountValue ?? 0} required placeholder="Giá trị" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <input name="startDate" type="datetime-local" defaultValue={editItem?.startDate ? editItem.startDate.substring(0, 16) : ''} required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="endDate" type="datetime-local" defaultValue={editItem?.endDate ? editItem.endDate.substring(0, 16) : ''} required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <input name="priority" type="number" defaultValue={editItem?.priority ?? 0} placeholder="Ưu tiên" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <label className="flex items-center gap-2 text-sm">
                  <input name="isStackable" type="checkbox" defaultChecked={editItem?.isStackable ?? false} />
                  Cộng dồn
                </label>
              </div>
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
                  <th className="px-4 py-3 font-medium">Tên</th>
                  <th className="px-4 py-3 font-medium">Loại giảm</th>
                  <th className="px-4 py-3 font-medium">Giá trị</th>
                  <th className="px-4 py-3 font-medium">Ngày</th>
                  <th className="px-4 py-3 font-medium">Trạng thái</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3">{item.name}</td>
                    <td className="px-4 py-3 text-muted-foreground">{item.discountType === 'Percentage' ? '%' : 'VNĐ'}</td>
                    <td className="px-4 py-3">{item.discountValue.toLocaleString()}</td>
                    <td className="px-4 py-3 text-muted-foreground text-xs">
                      {new Date(item.startDate).toLocaleDateString('vi-VN')} - {new Date(item.endDate).toLocaleDateString('vi-VN')}
                    </td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                        {item.isActive ? 'Kích hoạt' : 'Tắt'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => toggleMutation.mutate({ id: item.id, enable: !item.isActive })}>
                        {item.isActive ? <ToggleRight className="size-4" /> : <ToggleLeft className="size-4" />}
                      </Button>
                      <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa khuyến mãi này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có khuyến mãi nào</div>
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
