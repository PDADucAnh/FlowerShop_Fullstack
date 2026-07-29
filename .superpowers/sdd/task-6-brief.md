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
### Task 6: Frontend — Delete Product Dialog

**Files:**
- Create: `flower-admin.frontend/src/pages/products/components/DeleteProductDialog.tsx`

**Interfaces:**
- Consumes: `productsApi`, `Product`
- Produces: delete confirmation dialog used by ProductsPage

- [ ] **Step 1: Create `DeleteProductDialog.tsx`**

```typescript
import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
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
import type { Product } from '@/types/product'

interface DeleteProductDialogProps {
  product: Product | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onDeleted?: () => void
}

export function DeleteProductDialog({
  product,
  open,
  onOpenChange,
  onDeleted,
}: DeleteProductDialogProps) {
  const queryClient = useQueryClient()

  const deleteMutation = useMutation({
    mutationFn: (id: number) => productsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] })
      toast.success('Đã xóa sản phẩm')
      onOpenChange(false)
      onDeleted?.()
    },
    onError: () => {
      toast.error('Xóa sản phẩm thất bại')
    },
  })

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <div className="flex items-center gap-2">
            <AlertTriangle className="size-5 text-destructive" />
            <DialogTitle>Xóa sản phẩm</DialogTitle>
          </div>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa sản phẩm này? Hành động này không thể hoàn tác.
          </DialogDescription>
        </DialogHeader>

        {product && (
          <div className="flex items-center gap-3 rounded-lg border bg-muted/50 p-3">
            <img
              src={product.images?.[0]?.imageUrl || product.imageUrl || '/placeholder.svg'}
              alt={product.name}
              className="size-12 rounded-md object-cover"
            />
            <div>
              <p className="font-medium">{product.name}</p>
              <p className="text-sm text-muted-foreground">SKU: {product.sku || '—'}</p>
            </div>
          </div>
        )}

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
            onClick={() => product && deleteMutation.mutate(product.id)}
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

- [ ] **Step 2: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 3: Commit**

```bash
git add flower-admin.frontend/src/pages/products/components/DeleteProductDialog.tsx
git commit -m "feat: add delete product confirmation dialog"
```

---

