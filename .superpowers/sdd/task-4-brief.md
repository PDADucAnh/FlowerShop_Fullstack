# Phase 2: Products & Categories Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build Products & Categories management UI (list, create, edit, delete with multi-image upload) in the admin SPA.

**Architecture:** Backend adds `ProductImage` entity + image upload/association endpoints; frontend adds DataTable listing, create/edit forms with multi-image upload, and inline category CRUD dialogs.

**Tech Stack:** ASP.NET Core 8, EF Core + PostgreSQL, Cloudinary (via `PhotoService`), React 19 + Vite + Tailwind v4 + shadcn/ui (Base UI) + React Query + React Router v7.

## Global Constraints

- All UI text in Vietnamese
- API responses are raw data (not wrapped in `ApiResponse<T>`) — frontend reads `response.data` directly from axios
- Image upload via `POST /api/Upload` (returns `{ url: string }`), not direct browser-to-Cloudinary
- `CreateProductDTO` / `UpdateProductDTO` extended with: `IsActive`, `FlowerMeaning`, `Origin`, `CareInstruction`, `NewImages`
- `ProductDTO` gains `List<ProductImageDTO> Images`
- Backend creates product + `ProductImage` records in a single transaction (`NewImages` batch)
- Existing MVC controllers (`ProductController`, `CategoryProductController`) NOT modified — only API controllers changed
- Follow existing patterns: `ICategoryProductService` / `CategoryProductService`, `IProductService` / `ProductService`


---
### Task 4: Frontend — Products DataTable Page

**Files:**
- Create: `flower-admin.frontend/src/pages/products/ProductsPage.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ProductTable.tsx`

**Interfaces:**
- Consumes: `productsApi`, `categoriesApi`, `Product`, `CategoryProduct`, `PagedResponse`
- Produces: `/products` route content (replace placeholder)

- [ ] **Step 1: Create `ProductTable.tsx`**

```typescript
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
import { Pencil, Trash2 } from 'lucide-react'
import type { Product } from '@/types/product'

interface ProductTableProps {
  products: Product[]
  onDelete: (product: Product) => void
}

export function ProductTable({ products, onDelete }: ProductTableProps) {
  const navigate = useNavigate()

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
              <img
                src={product.images?.[0]?.imageUrl || product.imageUrl || '/placeholder.svg'}
                alt={product.name}
                className="size-10 rounded-md object-cover"
              />
            </TableCell>
            <TableCell className="font-medium">{product.name}</TableCell>
            <TableCell className="text-muted-foreground">{product.sku || '—'}</TableCell>
            <TableCell>{product.categoryProductName || '—'}</TableCell>
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
```

- [ ] **Step 2: Create `ProductsPage.tsx`**

```typescript
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import { categoriesApi } from '@/api/categories'
import { ProductTable } from './components/ProductTable'
import { DeleteProductDialog } from './components/DeleteProductDialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Search, Loader2, AlertCircle } from 'lucide-react'
import type { Product } from '@/types/product'

export function ProductsPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState<string>('all')
  const [deleteTarget, setDeleteTarget] = useState<Product | null>(null)
  const pageSize = 20

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  const { data, isLoading, error } = useQuery({
    queryKey: ['products', page, categoryFilter],
    queryFn: () =>
      productsApi.getPaged({
        page,
        pageSize,
        categoryProductId: categoryFilter === 'all' ? null : Number(categoryFilter),
      }).then((r) => r.data),
  })

  const handleSearch = () => {
    if (!search.trim()) return
    navigate(`/products?search=${encodeURIComponent(search)}`)
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách sản phẩm</p>
        <Button variant="outline" onClick={() => window.location.reload()}>
          Thử lại
        </Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Sản phẩm</h1>
        <Button onClick={() => navigate('/products/new')}>
          <Plus className="mr-2 size-4" />
          Thêm sản phẩm
        </Button>
      </div>

      <div className="flex items-center gap-3">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm kiếm sản phẩm…"
            className="pl-9"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
          />
        </div>
        <Select value={categoryFilter} onValueChange={(v) => { setCategoryFilter(v); setPage(1) }}>
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Tất cả danh mục" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Tất cả danh mục</SelectItem>
            {categories?.map((cat) => (
              <SelectItem key={cat.id} value={String(cat.id)}>
                {cat.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} sản phẩm` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <ProductTable
                products={data.items}
                onDelete={setDeleteTarget}
              />
              {/* Pagination */}
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">
                    Trang {data.page} / {data.totalPages}
                  </p>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="sm"
                      disabled={page <= 1}
                      onClick={() => setPage((p) => Math.max(1, p - 1))}
                    >
                      Trước
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      disabled={page >= (data.totalPages ?? 1)}
                      onClick={() => setPage((p) => p + 1)}
                    >
                      Sau
                    </Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>Chưa có sản phẩm nào</p>
              <Button variant="link" onClick={() => navigate('/products/new')}>
                Thêm sản phẩm đầu tiên
              </Button>
            </div>
          )}
        </CardContent>
      </Card>

      <DeleteProductDialog
        product={deleteTarget}
        open={!!deleteTarget}
        onOpenChange={(open) => !open && setDeleteTarget(null)}
        onDeleted={() => {
          setDeleteTarget(null)
          // Refetch will happen automatically via query invalidation in the dialog
        }}
      />
    </div>
  )
}
```

- [ ] **Step 3: Create the Select shadcn component** (needed by ProductsPage)

```bash
npx shadcn@canary add select -y
```

- [ ] **Step 4: Verify build**

```bash
npm run build
```
Expected: 0 errors (the DeleteProductDialog import will error since it doesn't exist yet — we'll handle this in Task 6; temporarily comment out the import or use a minimal placeholder)

- [ ] **Step 5: Commit**

```bash
git add flower-admin.frontend/src/pages/products/
git add flower-admin.frontend/src/components/ui/select.tsx
git commit -m "feat: add products list page with DataTable"
```

---

