import { useState } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { flashSalesApi } from '@/api/flashSales'
import { productCategoriesApi } from '@/api/productCategories'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Checkbox } from '@/components/ui/checkbox'
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter,
} from '@/components/ui/dialog'
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from '@/components/ui/table'
import { Loader2, X } from 'lucide-react'
import { toast } from 'sonner'
import type { FlashSale, FlashSaleProductPreview, FlashSalePreviewRequest } from '@/types/flashSale'

interface PreviewRow {
  productId: number
  productName?: string
  sku?: string
  productImageUrl?: string
  originalPrice: number
  stockQuantity: number
  salePrice: number
  quantity: number
}

interface BulkAddFlashSaleModalProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  flashSale: FlashSale | null
  onSuccess: () => void
}

const formatCurrency = (value: number) => value.toLocaleString('vi-VN') + '₫'

export function BulkAddFlashSaleModal({ open, onOpenChange, flashSale, onSuccess }: BulkAddFlashSaleModalProps) {
  const [selectedCategoryIds, setSelectedCategoryIds] = useState<number[]>([])
  const [minStock, setMinStock] = useState('')
  const [topCount, setTopCount] = useState('10')
  const [discountPercent, setDiscountPercent] = useState('15')
  const [excelFile, setExcelFile] = useState<File | null>(null)
  const [rows, setRows] = useState<PreviewRow[]>([])
  const [previewed, setPreviewed] = useState(false)
  const [loading, setLoading] = useState(false)

  const { data: categories } = useQuery({
    queryKey: ['product-categories'],
    queryFn: () => productCategoriesApi.getAll().then((r) => r.data),
    enabled: open,
  })

  const bulkMutation = useMutation({
    mutationFn: (products: PreviewRow[]) =>
      flashSalesApi.bulkAdd({
        flashSaleId: flashSale!.id,
        products: products.map((p) => ({ productId: p.productId, salePrice: p.salePrice, quantity: p.quantity })),
      }),
    onSuccess: (res) => {
      toast.success(`Đã thêm ${res.data?.added ?? 0} sản phẩm vào Flash Sale`)
      onSuccess()
      handleClose()
    },
    onError: (err: any) => toast.error(err.response?.data?.message || 'Không thể thêm sản phẩm'),
  })

  const toggleCategory = (id: number) => {
    setSelectedCategoryIds((prev) => (prev.includes(id) ? prev.filter((c) => c !== id) : [...prev, id]))
  }

  const buildRequest = (): FlashSalePreviewRequest => ({
    flashSaleId: flashSale!.id,
    defaultDiscountPercent: Number(discountPercent) || 15,
    ...(selectedCategoryIds.length > 0 ? { productCategoryIds: selectedCategoryIds } : {}),
  })

  const mapPreview = (items: FlashSaleProductPreview[]): PreviewRow[] =>
    items.map((p) => ({
      productId: p.productId,
      productName: p.productName,
      sku: p.sku,
      productImageUrl: p.productImageUrl,
      originalPrice: p.originalPrice,
      stockQuantity: p.stockQuantity,
      salePrice: p.suggestedSalePrice,
      quantity: p.quantity,
    }))

  const runPreview = async (fn: () => Promise<FlashSaleProductPreview[]>) => {
    setLoading(true)
    setRows([])
    setPreviewed(false)
    try {
      const items = await fn()
      setRows(mapPreview(items))
      setPreviewed(true)
    } catch (err: any) {
      toast.error(err.response?.data?.message || 'Không thể xem trước sản phẩm')
    } finally {
      setLoading(false)
    }
  }

  const previewByCategory = () =>
    runPreview(() => flashSalesApi.previewByCategory(buildRequest()).then((r) => r.data))

  const previewByBestSeller = () =>
    runPreview(() => {
      const req: FlashSalePreviewRequest = {
        ...buildRequest(),
        minStockQuantity: Number(minStock) || undefined,
        topCount: Number(topCount) || undefined,
      }
      return flashSalesApi.previewByBestSeller(req).then((r) => r.data)
    })

  const previewByExcel = () => {
    if (!excelFile) {
      toast.error('Vui lòng chọn file Excel')
      return
    }
    runPreview(() => flashSalesApi.previewByExcel(flashSale!.id, excelFile, Number(discountPercent) || 15).then((r) => r.data))
  }

  const updateRow = (productId: number, patch: Partial<PreviewRow>) => {
    setRows((prev) => prev.map((r) => (r.productId === productId ? { ...r, ...patch } : r)))
  }

  const removeRow = (productId: number) => {
    setRows((prev) => prev.filter((r) => r.productId !== productId))
  }

  const discountOf = (row: PreviewRow) =>
    row.originalPrice > 0 ? Math.round(((row.originalPrice - row.salePrice) / row.originalPrice) * 100) : 0

  const handleBulkAdd = () => {
    if (rows.length === 0) {
      toast.error('Không có sản phẩm nào để thêm')
      return
    }
    if (rows.some((r) => r.salePrice < 0 || r.quantity < 0)) {
      toast.error('Giá và số lượng không được âm')
      return
    }
    if (rows.some((r) => r.salePrice > r.originalPrice)) {
      toast.error('Giá Flash Sale không được lớn hơn giá gốc')
      return
    }
    bulkMutation.mutate(rows)
  }

  const handleClose = () => {
    setRows([])
    setPreviewed(false)
    setExcelFile(null)
    onOpenChange(false)
  }

  return (
    <Dialog open={open} onOpenChange={(o) => { if (!o) handleClose() }}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>Thêm hàng loạt sản phẩm — {flashSale?.name}</DialogTitle>
        </DialogHeader>

        <Tabs defaultValue="category">
          <TabsList>
            <TabsTrigger value="category">Theo danh mục</TabsTrigger>
            <TabsTrigger value="bestseller">Bán chạy</TabsTrigger>
            <TabsTrigger value="excel">Excel</TabsTrigger>
          </TabsList>

          <TabsContent value="category" className="space-y-4">
            <div className="space-y-2">
              <Label>% giảm giá mặc định</Label>
              <Input
                type="number"
                min={0}
                max={100}
                value={discountPercent}
                onChange={(e) => setDiscountPercent(e.target.value)}
                className="w-32"
              />
            </div>
            <div className="space-y-2">
              <Label>Danh mục sản phẩm</Label>
              <div className="rounded-md border max-h-48 overflow-y-auto p-2 space-y-1">
                {categories && categories.length > 0 ? (
                  categories.map((cat) => (
                    <label key={cat.id} className="flex items-center gap-2 px-1 py-1 text-sm hover:bg-muted rounded cursor-pointer">
                      <Checkbox checked={selectedCategoryIds.includes(cat.id)} onCheckedChange={() => toggleCategory(cat.id)} />
                      <span>{cat.name}</span>
                    </label>
                  ))
                ) : (
                  <p className="px-1 py-2 text-sm text-muted-foreground">Không có danh mục nào</p>
                )}
              </div>
            </div>
            <Button onClick={previewByCategory} disabled={loading || selectedCategoryIds.length === 0}>
              {loading ? <Loader2 className="mr-2 size-4 animate-spin" /> : null}
              Xem trước
            </Button>
          </TabsContent>

          <TabsContent value="bestseller" className="space-y-4">
            <div className="grid grid-cols-3 gap-4">
              <div className="space-y-2">
                <Label>Số lượng tồn kho tối thiểu</Label>
                <Input type="number" min={0} value={minStock} onChange={(e) => setMinStock(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Số lượng top (N)</Label>
                <Input type="number" min={1} value={topCount} onChange={(e) => setTopCount(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>% giảm giá mặc định</Label>
                <Input type="number" min={0} max={100} value={discountPercent} onChange={(e) => setDiscountPercent(e.target.value)} />
              </div>
            </div>
            <Button onClick={previewByBestSeller} disabled={loading}>
              {loading ? <Loader2 className="mr-2 size-4 animate-spin" /> : null}
              Xem trước
            </Button>
          </TabsContent>

          <TabsContent value="excel" className="space-y-4">
            <div className="space-y-2">
              <Label>File Excel (.xlsx)</Label>
              <div className="flex items-center gap-2">
                <Input
                  type="file"
                  accept=".xlsx"
                  onChange={(e) => setExcelFile(e.target.files?.[0] ?? null)}
                />
                <Button variant="outline" size="icon" onClick={() => setExcelFile(null)} type="button">
                  <X className="size-4" />
                </Button>
              </div>
              <p className="text-xs text-muted-foreground">
                Định dạng: cột 1 = SKU, cột 2 = Giá Flash Sale, cột 3 = Số lượng. Hàng 1 là tiêu đề.
              </p>
            </div>
            <Button onClick={previewByExcel} disabled={loading || !excelFile}>
              {loading ? <Loader2 className="mr-2 size-4 animate-spin" /> : null}
              Xem trước
            </Button>
          </TabsContent>
        </Tabs>

        {previewed && (
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <Label className="text-base font-medium">Danh sách sản phẩm ({rows.length})</Label>
              <Button size="sm" variant="outline" onClick={handleBulkAdd} disabled={bulkMutation.isPending}>
                {bulkMutation.isPending ? <Loader2 className="mr-2 size-4 animate-spin" /> : null}
                Thêm vào Flash Sale
              </Button>
            </div>
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Sản phẩm</TableHead>
                    <TableHead>SKU</TableHead>
                    <TableHead className="text-right">Giá gốc</TableHead>
                    <TableHead className="text-right">Tồn kho</TableHead>
                    <TableHead className="text-right">Giá Flash Sale</TableHead>
                    <TableHead className="text-right">Số lượng</TableHead>
                    <TableHead className="text-right">Giảm</TableHead>
                    <TableHead className="w-10"></TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {rows.map((row) => (
                    <TableRow key={row.productId}>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          {row.productImageUrl && (
                            <img src={row.productImageUrl} alt="" className="size-8 rounded object-cover" />
                          )}
                          <span className="font-medium">{row.productName || `#${row.productId}`}</span>
                        </div>
                      </TableCell>
                      <TableCell className="font-mono text-xs">{row.sku || '—'}</TableCell>
                      <TableCell className="text-right font-mono">{formatCurrency(row.originalPrice)}</TableCell>
                      <TableCell className="text-right">{row.stockQuantity}</TableCell>
                      <TableCell className="text-right">
                        <Input
                          type="number"
                          min={0}
                          value={row.salePrice}
                          onChange={(e) => updateRow(row.productId, { salePrice: Number(e.target.value) })}
                          className="w-28 ml-auto text-right h-8"
                        />
                      </TableCell>
                      <TableCell className="text-right">
                        <Input
                          type="number"
                          min={0}
                          value={row.quantity}
                          onChange={(e) => updateRow(row.productId, { quantity: Number(e.target.value) })}
                          className="w-20 ml-auto text-right h-8"
                        />
                      </TableCell>
                      <TableCell className="text-right font-mono text-destructive">
                        {discountOf(row) > 0 ? `-${discountOf(row)}%` : '—'}
                      </TableCell>
                      <TableCell className="text-right">
                        <Button variant="ghost" size="icon" onClick={() => removeRow(row.productId)} type="button">
                          <X className="size-4" />
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
            <DialogFooter>
              <Button onClick={handleBulkAdd} disabled={bulkMutation.isPending}>
                {bulkMutation.isPending ? <Loader2 className="mr-2 size-4 animate-spin" /> : null}
                {bulkMutation.isPending ? 'Đang thêm...' : 'Thêm vào Flash Sale'}
              </Button>
            </DialogFooter>
          </div>
        )}
      </DialogContent>
    </Dialog>
  )
}
