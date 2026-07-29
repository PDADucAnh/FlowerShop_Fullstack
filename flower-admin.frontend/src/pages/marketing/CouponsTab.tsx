import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { couponsApi } from '@/api/coupons'
import { Button } from '@/components/ui/button'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2, ToggleLeft, ToggleRight, Eye } from 'lucide-react'
import { toast } from 'sonner'
import type { CouponDTO } from '@/types/coupon'

export function CouponsTab() {
  const [page, setPage] = useState(1)
  const [editItem, setEditItem] = useState<CouponDTO | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [usagesItem, setUsagesItem] = useState<CouponDTO | null>(null)
  const [usagesOpen, setUsagesOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data, isLoading, error } = useQuery({
    queryKey: ['coupons', page],
    queryFn: () => couponsApi.getPaged(page).then((r) => r.data),
  })

  const { data: usages } = useQuery({
    queryKey: ['coupon-usages', usagesItem?.id],
    queryFn: () => couponsApi.getUsages(usagesItem!.id).then((r) => r.data),
    enabled: !!usagesItem && usagesOpen,
  })

  const createMutation = useMutation({
    mutationFn: (dto: any) => couponsApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); setDialogOpen(false); toast.success('Đã thêm mã giảm giá') },
    onError: () => toast.error('Không thể thêm mã giảm giá'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: any }) => couponsApi.update(id, dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật mã giảm giá') },
    onError: () => toast.error('Không thể cập nhật mã giảm giá'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => couponsApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); toast.success('Đã xóa mã giảm giá') },
    onError: () => toast.error('Không thể xóa mã giảm giá'),
  })

  const toggleMutation = useMutation({
    mutationFn: ({ id, enable }: { id: number; enable: boolean }) => enable ? couponsApi.enable(id) : couponsApi.disable(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['coupons'] }); toast.success('Đã cập nhật trạng thái') },
  })

  const openCreate = () => { setEditItem(null); setDialogOpen(true) }
  const openEdit = (item: CouponDTO) => { setEditItem(item); setDialogOpen(true) }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const form = e.currentTarget
    const formData = new FormData(form)
    const code = formData.get('code') as string
    if (!code) return

    const dto: any = {
      code,
      description: formData.get('description') as string || undefined,
      discountType: formData.get('discountType') as string,
      discountValue: Number(formData.get('discountValue')),
      minimumOrderAmount: Number(formData.get('minimumOrderAmount')) || undefined,
      maximumDiscountAmount: Number(formData.get('maximumDiscountAmount')) || undefined,
      usageLimit: Number(formData.get('usageLimit')) || undefined,
      usagePerCustomer: Number(formData.get('usagePerCustomer')) || undefined,
      isPublic: formData.get('isPublic') === 'on',
    }

    if (editItem) {
      dto.id = editItem.id
      updateMutation.mutate({ id: editItem.id, dto })
    } else {
      createMutation.mutate(dto)
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải mã giảm giá</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button size="sm" />} onClick={openCreate}>
            <Plus className="mr-1 size-4" />Thêm mã giảm giá
          </DialogTrigger>
          <DialogContent className="max-w-lg">
            <DialogHeader><DialogTitle>{editItem ? 'Sửa mã giảm giá' : 'Thêm mã giảm giá'}</DialogTitle></DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-3">
              <input name="code" defaultValue={editItem?.code ?? ''} placeholder="Mã giảm giá" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm font-mono uppercase" />
              <input name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <div className="grid grid-cols-2 gap-3">
                <select name="discountType" defaultValue={editItem?.discountType ?? 'Percentage'} className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm">
                  <option value="Percentage">Phần trăm</option>
                  <option value="FixedAmount">Số tiền cố định</option>
                </select>
                <input name="discountValue" type="number" step="0.01" defaultValue={editItem?.discountValue ?? 0} required placeholder="Giá trị" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <input name="minimumOrderAmount" type="number" step="0.01" defaultValue={editItem?.minimumOrderAmount ?? ''} placeholder="Đơn tối thiểu" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="maximumDiscountAmount" type="number" step="0.01" defaultValue={editItem?.maximumDiscountAmount ?? ''} placeholder="Giảm tối đa" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <input name="usageLimit" type="number" defaultValue={editItem?.usageLimit ?? ''} placeholder="SL sử dụng" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
                <input name="usagePerCustomer" type="number" defaultValue={editItem?.usagePerCustomer ?? ''} placeholder="SL/KH" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              </div>
              <label className="flex items-center gap-2 text-sm">
                <input name="isPublic" type="checkbox" defaultChecked={editItem?.isPublic ?? true} />
                Công khai
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
                  <th className="px-4 py-3 font-medium">Mã</th>
                  <th className="px-4 py-3 font-medium">Loại</th>
                  <th className="px-4 py-3 font-medium">Giá trị</th>
                  <th className="px-4 py-3 font-medium">Đã dùng</th>
                  <th className="px-4 py-3 font-medium">Trạng thái</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3 font-mono text-xs">{item.code}</td>
                    <td className="px-4 py-3 text-muted-foreground">{item.discountType === 'Percentage' ? '%' : 'VNĐ'}</td>
                    <td className="px-4 py-3">{item.discountValue.toLocaleString()}</td>
                    <td className="px-4 py-3 text-muted-foreground">{item.usedCount}{item.usageLimit ? `/${item.usageLimit}` : ''}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>
                        {item.isActive ? 'Kích hoạt' : 'Tắt'}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => { setUsagesItem(item); setUsagesOpen(true) }}><Eye className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => toggleMutation.mutate({ id: item.id, enable: !item.isActive })}>
                        {item.isActive ? <ToggleRight className="size-4" /> : <ToggleLeft className="size-4" />}
                      </Button>
                      <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa mã giảm giá này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có mã giảm giá nào</div>
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

      <Dialog open={usagesOpen} onOpenChange={setUsagesOpen}>
        <DialogContent>
          <DialogHeader><DialogTitle>Lịch sử sử dụng — {usagesItem?.code}</DialogTitle></DialogHeader>
          {usages && usages.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="pb-2 font-medium">Khách hàng</th>
                  <th className="pb-2 font-medium">Đơn hàng</th>
                  <th className="pb-2 font-medium">Giảm</th>
                  <th className="pb-2 font-medium">Ngày</th>
                </tr>
              </thead>
              <tbody>
                {usages.map((u) => (
                  <tr key={u.id} className="border-b last:border-0">
                    <td className="py-2">{u.customerName ?? `#${u.customerId}`}</td>
                    <td className="py-2">#{u.orderId}</td>
                    <td className="py-2">{u.discountAmount.toLocaleString()}₫</td>
                    <td className="py-2 text-muted-foreground text-xs">{new Date(u.usedAt).toLocaleDateString('vi-VN')}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-24 items-center justify-center text-muted-foreground">Chưa có lượt sử dụng</div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  )
}
