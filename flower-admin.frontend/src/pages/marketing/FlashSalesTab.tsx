import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { flashSalesApi } from '@/api/flashSales'
import { productsApi } from '@/api/products'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { Switch } from '@/components/ui/switch'
import { Badge } from '@/components/ui/badge'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from '@/components/ui/table'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2, X } from 'lucide-react'
import { toast } from 'sonner'
import type { FlashSale, CreateFlashSaleRequest, UpdateFlashSaleRequest, CreateFlashSaleProductRequest } from '@/types/flashSale'

const now = () => new Date()

function getStatus(item: FlashSale): { label: string; variant: 'default' | 'secondary' | 'outline' | 'destructive' } {
  const start = new Date(item.startDate)
  const end = new Date(item.endDate)
  const current = now()
  if (!item.isActive) return { label: 'Đã ẩn', variant: 'outline' }
  if (current < start) return { label: 'Sắp diễn ra', variant: 'secondary' }
  if (current >= start && current <= end) return { label: 'Đang diễn ra', variant: 'default' }
  return { label: 'Đã kết thúc', variant: 'destructive' }
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  })
}

interface ProductEntry {
  productId: number
  productName: string
  originalPrice: number
  salePrice: number
}

export function FlashSalesTab() {
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editItem, setEditItem] = useState<FlashSale | null>(null)
  const [deleteConfirm, setDeleteConfirm] = useState<FlashSale | null>(null)
  const queryClient = useQueryClient()

  const { data: items, isLoading, error } = useQuery({
    queryKey: ['flash-sales'],
    queryFn: () => flashSalesApi.getAll().then((r) => r.data),
  })

  const [formName, setFormName] = useState('')
  const [formDesc, setFormDesc] = useState('')
  const [formStart, setFormStart] = useState('')
  const [formEnd, setFormEnd] = useState('')
  const [formActive, setFormActive] = useState(true)
  const [formProducts, setFormProducts] = useState<ProductEntry[]>([])
  const [productSearch, setProductSearch] = useState('')

  const { data: searchResults } = useQuery({
    queryKey: ['products-search', productSearch],
    queryFn: () => productsApi.search(productSearch).then((r) => r.data),
    enabled: productSearch.length >= 2,
  })

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['flash-sales'] })

  const createMutation = useMutation({
    mutationFn: (dto: CreateFlashSaleRequest) => flashSalesApi.create(dto),
    onSuccess: () => { invalidate(); setDialogOpen(false); toast.success('Đã tạo Flash Sale') },
    onError: (err: any) => toast.error(err.response?.data?.message || 'Không thể tạo Flash Sale'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpdateFlashSaleRequest }) => flashSalesApi.update(id, dto),
    onSuccess: () => { invalidate(); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật Flash Sale') },
    onError: (err: any) => toast.error(err.response?.data?.message || 'Không thể cập nhật Flash Sale'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => flashSalesApi.delete(id),
    onSuccess: () => { invalidate(); setDeleteConfirm(null); toast.success('Đã xóa Flash Sale') },
    onError: () => toast.error('Không thể xóa Flash Sale'),
  })

  const openCreate = () => {
    setEditItem(null)
    setFormName(''); setFormDesc(''); setFormStart(''); setFormEnd('')
    setFormActive(true); setFormProducts([]); setDialogOpen(true)
  }

  const openEdit = (item: FlashSale) => {
    setEditItem(item)
    setFormName(item.name)
    setFormDesc(item.description || '')
    setFormStart(toDatetimeLocal(item.startDate))
    setFormEnd(toDatetimeLocal(item.endDate))
    setFormActive(item.isActive)
    setFormProducts(
      (item.products || []).map((p) => ({
        productId: p.productId,
        productName: p.productName || '',
        originalPrice: p.originalPrice,
        salePrice: p.salePrice,
      }))
    )
    setDialogOpen(true)
  }

  const toDatetimeLocal = (d: string) => {
    const date = new Date(d)
    const offset = date.getTimezoneOffset()
    const local = new Date(date.getTime() - offset * 60000)
    return local.toISOString().slice(0, 16)
  }

  const addProduct = (p: { id: number; name: string; price: number }) => {
    if (formProducts.some((fp) => fp.productId === p.id)) return
    setFormProducts([...formProducts, { productId: p.id, productName: p.name, originalPrice: p.price, salePrice: p.price }])
    setProductSearch('')
  }

  const removeProduct = (productId: number) => {
    setFormProducts(formProducts.filter((fp) => fp.productId !== productId))
  }

  const updateSalePrice = (productId: number, salePrice: number) => {
    setFormProducts(formProducts.map((fp) => fp.productId === productId ? { ...fp, salePrice } : fp))
  }

  const handleSubmit = () => {
    if (!formName || !formStart || !formEnd) { toast.error('Vui lòng điền đầy đủ thông tin'); return }
    if (formProducts.length === 0) { toast.error('Vui lòng thêm ít nhất 1 sản phẩm'); return }
    if (new Date(formStart) >= new Date(formEnd)) { toast.error('Ngày kết thúc phải sau ngày bắt đầu'); return }

    const payload = {
      name: formName,
      description: formDesc || undefined,
      startDate: new Date(formStart).toISOString(),
      endDate: new Date(formEnd).toISOString(),
      isActive: formActive,
      products: formProducts.map((fp) => ({ productId: fp.productId, salePrice: fp.salePrice })),
    }

    if (editItem) {
      updateMutation.mutate({ id: editItem.id, dto: { ...payload, id: editItem.id } })
    } else {
      createMutation.mutate(payload)
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải Flash Sale</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger asChild onClick={openCreate}>
            <Button size="sm"><Plus className="mr-1 size-4" />Tạo Flash Sale</Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader><DialogTitle>{editItem ? 'Sửa Flash Sale' : 'Tạo Flash Sale'}</DialogTitle></DialogHeader>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label>Tên chương trình</Label>
                <Input value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="VD: Flash Sale 8/3" required />
              </div>
              <div className="space-y-2">
                <Label>Mô tả</Label>
                <Textarea value={formDesc} onChange={(e) => setFormDesc(e.target.value)} placeholder="Mô tả (tùy chọn)" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>Bắt đầu</Label>
                  <Input type="datetime-local" value={formStart} onChange={(e) => setFormStart(e.target.value)} required />
                </div>
                <div className="space-y-2">
                  <Label>Kết thúc</Label>
                  <Input type="datetime-local" value={formEnd} onChange={(e) => setFormEnd(e.target.value)} required />
                </div>
              </div>
              <div className="flex items-center gap-2">
                <Switch checked={formActive} onCheckedChange={setFormActive} />
                <Label className="cursor-pointer">Kích hoạt</Label>
              </div>

              <div className="space-y-3">
                <Label>Sản phẩm tham gia</Label>
                <div className="relative">
                  <Input
                    placeholder="Tìm sản phẩm để thêm..."
                    value={productSearch}
                    onChange={(e) => setProductSearch(e.target.value)}
                  />
                  {productSearch.length >= 2 && searchResults && searchResults.length > 0 && (
                    <div className="absolute z-10 mt-1 w-full rounded-md border bg-popover shadow-md max-h-48 overflow-y-auto">
                      {searchResults.map((p) => (
                        <button
                          key={p.id}
                          className="w-full px-3 py-2 text-left text-sm hover:bg-muted flex justify-between"
                          onClick={() => addProduct(p)}
                          type="button"
                        >
                          <span>{p.name}</span>
                          <span className="font-mono text-muted-foreground">{p.price.toLocaleString()}₫</span>
                        </button>
                      ))}
                    </div>
                  )}
                </div>

                {formProducts.length > 0 && (
                  <div className="rounded-md border">
                    <table className="w-full text-sm">
                      <thead>
                        <tr className="border-b text-left text-muted-foreground">
                          <th className="px-3 py-2 font-medium">Sản phẩm</th>
                          <th className="px-3 py-2 font-medium text-right">Giá gốc</th>
                          <th className="px-3 py-2 font-medium text-right">Giá Flash Sale</th>
                          <th className="px-3 py-2 font-medium text-right">Giảm</th>
                          <th className="w-10"></th>
                        </tr>
                      </thead>
                      <tbody>
                        {formProducts.map((fp) => {
                          const discount = fp.originalPrice > 0
                            ? Math.round((fp.originalPrice - fp.salePrice) / fp.originalPrice * 100)
                            : 0
                          return (
                            <tr key={fp.productId} className="border-b last:border-0">
                              <td className="px-3 py-2">{fp.productName}</td>
                              <td className="px-3 py-2 text-right font-mono">{fp.originalPrice.toLocaleString()}₫</td>
                              <td className="px-3 py-2 text-right">
                                <Input
                                  type="number"
                                  value={fp.salePrice}
                                  onChange={(e) => updateSalePrice(fp.productId, Number(e.target.value))}
                                  className="w-28 ml-auto text-right h-8"
                                />
                              </td>
                              <td className="px-3 py-2 text-right font-mono text-destructive">{discount > 0 ? `-${discount}%` : '—'}</td>
                              <td className="px-3 py-2">
                                <Button variant="ghost" size="icon" onClick={() => removeProduct(fp.productId)} type="button">
                                  <X className="size-4" />
                                </Button>
                              </td>
                            </tr>
                          )
                        })}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>

              <DialogFooter>
                <DialogClose asChild>
                  <Button variant="outline" type="button">Hủy</Button>
                </DialogClose>
                <Button onClick={handleSubmit} disabled={createMutation.isPending || updateMutation.isPending}>
                  {createMutation.isPending || updateMutation.isPending ? (
                    <><Loader2 className="mr-2 size-4 animate-spin" />Đang lưu...</>
                  ) : (editItem ? 'Cập nhật' : 'Tạo')}
                </Button>
              </DialogFooter>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardContent className="p-0">
          {items && items.length > 0 ? (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Tên chương trình</TableHead>
                  <TableHead>Thời gian</TableHead>
                  <TableHead className="text-center">Số SP</TableHead>
                  <TableHead>Trạng thái</TableHead>
                  <TableHead className="text-right">Thao tác</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {items.map((item) => {
                  const status = getStatus(item)
                  return (
                    <TableRow key={item.id}>
                      <TableCell>
                        <div className="font-medium">{item.name}</div>
                        {item.description && (
                          <div className="text-xs text-muted-foreground truncate max-w-xs">{item.description}</div>
                        )}
                      </TableCell>
                      <TableCell className="text-sm">
                        <div>{formatDate(item.startDate)}</div>
                        <div className="text-muted-foreground">→ {formatDate(item.endDate)}</div>
                      </TableCell>
                      <TableCell className="text-center">{item.products?.length || 0}</TableCell>
                      <TableCell><Badge variant={status.variant}>{status.label}</Badge></TableCell>
                      <TableCell className="text-right">
                        <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                        <Button variant="ghost" size="icon" onClick={() => setDeleteConfirm(item)}><Trash2 className="size-4 text-destructive" /></Button>
                      </TableCell>
                    </TableRow>
                  )
                })}
              </TableBody>
            </Table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có Flash Sale nào</div>
          )}
        </CardContent>
      </Card>

      <Dialog open={!!deleteConfirm} onOpenChange={(open) => { if (!open) setDeleteConfirm(null) }}>
        <DialogContent>
          <DialogHeader><DialogTitle>Xác nhận xóa</DialogTitle></DialogHeader>
          <p>Bạn có chắc muốn xóa Flash Sale <strong>{deleteConfirm?.name}</strong>?</p>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteConfirm(null)}>Hủy</Button>
            <Button
              variant="destructive"
              onClick={() => deleteConfirm && deleteMutation.mutate(deleteConfirm.id)}
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? <><Loader2 className="mr-2 size-4 animate-spin" />Đang xóa...</> : 'Xóa'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
