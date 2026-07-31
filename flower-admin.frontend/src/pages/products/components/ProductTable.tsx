import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Pencil, Trash2 } from 'lucide-react'
import type { Product } from '@/types/product'

interface ProductTableProps {
  products: Product[]
  selectedIds: Set<number>
  onSelectedIdsChange: (ids: Set<number>) => void
  onDelete: (product: Product) => void
}

export function ProductTable({ products, selectedIds, onSelectedIdsChange, onDelete }: ProductTableProps) {
  const navigate = useNavigate()

  const allSelected = products.length > 0 && products.every((p) => selectedIds.has(p.id))

  const toggleAll = () => {
    if (allSelected) {
      onSelectedIdsChange(new Set())
    } else {
      onSelectedIdsChange(new Set(products.map((p) => p.id)))
    }
  }

  const toggleOne = (id: number) => {
    const next = new Set(selectedIds)
    if (next.has(id)) {
      next.delete(id)
    } else {
      next.add(id)
    }
    onSelectedIdsChange(next)
  }

  const stockBadge = (qty: number) => {
    if (qty === 0) return <Badge variant="destructive">Hết hàng</Badge>
    if (qty <= 5) return <Badge className="bg-amber-100 text-amber-800 hover:bg-amber-100">{qty}</Badge>
    return <Badge className="bg-green-100 text-green-800 hover:bg-green-100">{qty}</Badge>
  }

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN').format(price) + '₫'

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-10">
            <Checkbox checked={allSelected} onCheckedChange={toggleAll} />
          </TableHead>
          <TableHead className="w-12">Ảnh</TableHead>
          <TableHead>Tên sản phẩm</TableHead>
          <TableHead>SKU</TableHead>
          <TableHead>Danh mục</TableHead>
          <TableHead className="text-right">Giá</TableHead>
          <TableHead className="text-center">Tồn kho</TableHead>
          <TableHead className="text-center">Trạng thái</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {products.map((product) => (
          <TableRow key={product.id}>
            <TableCell>
              <Checkbox
                checked={selectedIds.has(product.id)}
                onCheckedChange={() => toggleOne(product.id)}
              />
            </TableCell>
            <TableCell>
              <img
                src={product.images?.[0]?.imageUrl || product.imageUrl || '/placeholder.svg'}
                alt={product.name}
                className="size-10 rounded-md object-cover"
              />
            </TableCell>
            <TableCell className="font-medium">{product.name}</TableCell>
            <TableCell className="text-muted-foreground">{product.sku || '—'}</TableCell>
            <TableCell>{product.productCategoryName || '—'}</TableCell>
            <TableCell className="text-right font-mono">{formatPrice(product.price)}</TableCell>
            <TableCell className="text-center">{stockBadge(product.stockQuantity)}</TableCell>
            <TableCell className="text-center">
              <Badge variant={product.isActive ? 'default' : 'outline'}>
                {product.isActive ? 'Đang bán' : 'Ngừng bán'}
              </Badge>
            </TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => navigate(`/products/${product.id}/edit`)}
                >
                  <Pencil className="size-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => onDelete(product)}
                >
                  <Trash2 className="size-4 text-destructive" />
                </Button>
              </div>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
