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
### Task 7: Frontend — Categories CRUD Page

**Files:**
- Create: `flower-admin.frontend/src/pages/categories/CategoriesPage.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/CategoryTable.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/CategoryDialog.tsx`
- Create: `flower-admin.frontend/src/pages/categories/components/DeleteCategoryDialog.tsx`

**Interfaces:**
- Consumes: `categoriesApi`, `CategoryProduct`
- Produces: `/products/categories` page with inline CRUD

- [ ] **Step 1: Create `CategoryTable.tsx`**

```typescript
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Button } from '@/components/ui/button'
import { Pencil, Trash2 } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface CategoryTableProps {
  categories: CategoryProduct[]
  onEdit: (category: CategoryProduct) => void
  onDelete: (category: CategoryProduct) => void
}

export function CategoryTable({ categories, onEdit, onDelete }: CategoryTableProps) {
  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-16">ID</TableHead>
          <TableHead>Tên danh mục</TableHead>
          <TableHead>Mô tả</TableHead>
          <TableHead>Slug</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {categories.map((cat) => (
          <TableRow key={cat.id}>
            <TableCell className="text-muted-foreground">{cat.id}</TableCell>
            <TableCell className="font-medium">{cat.name}</TableCell>
            <TableCell className="text-muted-foreground max-w-xs truncate">
              {cat.description || '—'}
            </TableCell>
            <TableCell className="text-muted-foreground">{cat.slug || '—'}</TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button variant="ghost" size="icon" onClick={() => onEdit(cat)}>
                  <Pencil className="size-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={() => onDelete(cat)}>
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

- [ ] **Step 2: Create `CategoryDialog.tsx`**

```typescript
import { useState, useEffect } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { toast } from 'sonner'
import { Loader2 } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface CategoryDialogProps {
  category: CategoryProduct | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

function generateSlug(name: string): string {
  return name
    .toLowerCase()
    .replace(/đ/g, 'd')
    .replace(/[^a-z0-9\s-]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim()
}

export function CategoryDialog({ category, open, onOpenChange }: CategoryDialogProps) {
  const queryClient = useQueryClient()
  const isEditing = !!category
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [slug, setSlug] = useState('')

  useEffect(() => {
    if (category) {
      setName(category.name)
      setDescription(category.description || '')
      setSlug(category.slug || '')
    } else {
      setName('')
      setDescription('')
      setSlug('')
    }
  }, [category, open])

  const mutation = useMutation({
    mutationFn: () => {
      const payload = { name, description, slug }
      return isEditing
        ? categoriesApi.update(category!.id, { ...payload, id: category!.id })
        : categoriesApi.create(payload)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      toast.success(isEditing ? 'Cập nhật danh mục thành công' : 'Thêm danh mục thành công')
      onOpenChange(false)
    },
    onError: () => {
      toast.error(isEditing ? 'Cập nhật thất bại' : 'Thêm danh mục thất bại')
    },
  })

  const handleNameChange = (value: string) => {
    setName(value)
    if (!isEditing && !slug) {
      setSlug(generateSlug(value))
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? 'Chỉnh sửa danh mục' : 'Thêm danh mục'}
          </DialogTitle>
        </DialogHeader>
        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="cat-name">Tên danh mục *</Label>
            <Input
              id="cat-name"
              value={name}
              onChange={(e) => handleNameChange(e.target.value)}
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="cat-slug">Slug</Label>
            <Input
              id="cat-slug"
              value={slug}
              onChange={(e) => setSlug(e.target.value)}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="cat-desc">Mô tả</Label>
            <Textarea
              id="cat-desc"
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Hủy
          </Button>
          <Button
            onClick={() => mutation.mutate()}
            disabled={!name || mutation.isPending}
          >
            {mutation.isPending && <Loader2 className="mr-2 size-4 animate-spin" />}
            {isEditing ? 'Cập nhật' : 'Thêm'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 3: Create `DeleteCategoryDialog.tsx`**

```typescript
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { toast } from 'sonner'
import { Loader2, AlertTriangle } from 'lucide-react'
import type { CategoryProduct } from '@/types/category'

interface DeleteCategoryDialogProps {
  category: CategoryProduct | null
  open: boolean
  onOpenChange: (open: boolean) => void
}

export function DeleteCategoryDialog({
  category,
  open,
  onOpenChange,
}: DeleteCategoryDialogProps) {
  const queryClient = useQueryClient()

  const deleteMutation = useMutation({
    mutationFn: (id: number) => categoriesApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      toast.success('Đã xóa danh mục')
      onOpenChange(false)
    },
    onError: () => {
      toast.error('Xóa danh mục thất bại. Có thể danh mục đang chứa sản phẩm.')
    },
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <div className="flex items-center gap-2">
            <AlertTriangle className="size-5 text-destructive" />
            <DialogTitle>Xóa danh mục</DialogTitle>
          </div>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa danh mục "{category?.name}"?
            {category && (
              <span className="mt-2 block text-destructive">
                Lưu ý: Các sản phẩm thuộc danh mục này sẽ bị ảnh hưởng nếu danh mục đang được sử dụng.
              </span>
            )}
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={deleteMutation.isPending}
          >
            Hủy
          </Button>
          <Button
            variant="destructive"
            onClick={() => category && deleteMutation.mutate(category.id)}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending && (
              <Loader2 className="mr-2 size-4 animate-spin" />
            )}
            Xóa
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
```

- [ ] **Step 4: Create `CategoriesPage.tsx`**

```typescript
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { categoriesApi } from '@/api/categories'
import { CategoryTable } from './components/CategoryTable'
import { CategoryDialog } from './components/CategoryDialog'
import { DeleteCategoryDialog } from './components/DeleteCategoryDialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Loader2, ArrowLeft } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import type { CategoryProduct } from '@/types/category'

export function CategoriesPage() {
  const navigate = useNavigate()
  const [editTarget, setEditTarget] = useState<CategoryProduct | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<CategoryProduct | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)

  const { data: categories, isLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/products')}>
            <ArrowLeft className="size-5" />
          </Button>
          <h1 className="text-2xl font-semibold">Danh mục sản phẩm</h1>
        </div>
        <Button onClick={() => { setEditTarget(null); setDialogOpen(true) }}>
          <Plus className="mr-2 size-4" />
          Thêm danh mục
        </Button>
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {categories ? `${categories.length} danh mục` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="flex h-48 items-center justify-center">
              <Loader2 className="size-8 animate-spin text-muted-foreground" />
            </div>
          ) : categories && categories.length > 0 ? (
            <CategoryTable
              categories={categories}
              onEdit={(cat) => { setEditTarget(cat); setDialogOpen(true) }}
              onDelete={setDeleteTarget}
            />
          ) : (
            <div className="flex h-48 items-center justify-center text-muted-foreground">
              Chưa có danh mục nào
            </div>
          )}
        </CardContent>
      </Card>

      <CategoryDialog
        category={editTarget}
        open={dialogOpen}
        onOpenChange={(open) => { setDialogOpen(open); if (!open) setEditTarget(null) }}
      />

      <DeleteCategoryDialog
        category={deleteTarget}
        open={!!deleteTarget}
        onOpenChange={(open) => { if (!open) setDeleteTarget(null) }}
      />
    </div>
  )
}
```

- [ ] **Step 5: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 6: Commit**

```bash
git add flower-admin.frontend/src/pages/categories/
git commit -m "feat: add categories CRUD page with inline dialogs"
```

---

