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
### Task 5: Frontend — Product Create/Edit Form + ImageUploader

**Files:**
- Create: `flower-admin.frontend/src/pages/products/ProductFormPage.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ProductForm.tsx`
- Create: `flower-admin.frontend/src/pages/products/components/ImageUploader.tsx`

**Interfaces:**
- Consumes: `productsApi`, `categoriesApi`, `uploadApi`, `Product`, `CategoryProduct`
- Produces: `/products/new` and `/products/:id/edit` pages

- [ ] **Step 1: Create `ImageUploader.tsx`**

```typescript
import { useState, useCallback } from 'react'
import { useDropzone } from 'react-dropzone'
import { uploadApi } from '@/api/upload'
import { Button } from '@/components/ui/button'
import { X, Upload, Loader2 } from 'lucide-react'
import { toast } from 'sonner'
import type { ProductImage } from '@/types/product'

interface ImageItem {
  id: string
  url: string
  isExisting: boolean
  existingId?: number
  uploading?: boolean
}

interface ImageUploaderProps {
  existingImages?: ProductImage[]
  onImagesChange: (urls: string[]) => void
  onDeleteExisting?: (imageId: number) => void
}

export function ImageUploader({ existingImages = [], onImagesChange, onDeleteExisting }: ImageUploaderProps) {
  const [images, setImages] = useState<ImageItem[]>(
    existingImages.map((img) => ({
      id: `existing-${img.id}`,
      url: img.imageUrl,
      isExisting: true,
      existingId: img.id,
    }))
  )

  const onDrop = useCallback(async (acceptedFiles: File[]) => {
    const newUrls: string[] = []

    for (const file of acceptedFiles) {
      const tempId = `uploading-${Date.now()}-${Math.random()}`
      setImages((prev) => [
        ...prev,
        { id: tempId, url: '', isExisting: false, uploading: true },
      ])

      try {
        const { data } = await uploadApi.upload(file)
        newUrls.push(data.url)
        setImages((prev) =>
          prev.map((img) =>
            img.id === tempId
              ? { ...img, url: data.url, uploading: false }
              : img
          )
        )
      } catch {
        toast.error(`Tải ảnh thất bại: ${file.name}`)
        setImages((prev) => prev.filter((img) => img.id !== tempId))
      }
    }

    onImagesChange(newUrls)
  }, [onImagesChange])

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: { 'image/*': ['.png', '.jpg', '.jpeg', '.gif', '.webp'] },
    maxSize: 5 * 1024 * 1024, // 5MB
  })

  const removeImage = (item: ImageItem) => {
    if (item.isExisting && item.existingId && onDeleteExisting) {
      onDeleteExisting(item.existingId)
    }
    setImages((prev) => prev.filter((img) => img.id !== item.id))
  }

  return (
    <div className="space-y-3">
      <div
        {...getRootProps()}
        className={`flex cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-6 transition-colors ${
          isDragActive
            ? 'border-primary bg-primary/5'
            : 'border-muted-foreground/25 hover:border-primary/50'
        }`}
      >
        <input {...getInputProps()} />
        <Upload className="mb-2 size-8 text-muted-foreground" />
        <p className="text-sm text-muted-foreground">
          {isDragActive
            ? 'Thả ảnh vào đây…'
            : 'Kéo thả ảnh vào đây hoặc nhấn để chọn'}
        </p>
        <p className="mt-1 text-xs text-muted-foreground">
          PNG, JPG, WebP tối đa 5MB
        </p>
      </div>

      {images.length > 0 && (
        <div className="grid grid-cols-4 gap-3 sm:grid-cols-6 md:grid-cols-8">
          {images.map((item) => (
            <div key={item.id} className="group relative aspect-square">
              {item.uploading ? (
                <div className="flex h-full items-center justify-center rounded-lg border bg-muted">
                  <Loader2 className="size-5 animate-spin text-muted-foreground" />
                </div>
              ) : (
                <img
                  src={item.url}
                  alt=""
                  className="h-full w-full rounded-lg border object-cover"
                />
              )}
              {!item.uploading && (
                <button
                  type="button"
                  onClick={() => removeImage(item)}
                  className="absolute -right-1.5 -top-1.5 flex size-5 items-center justify-center rounded-full bg-destructive text-destructive-foreground shadow transition-opacity opacity-0 group-hover:opacity-100"
                >
                  <X className="size-3" />
                </button>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
```

- [ ] **Step 2: Install react-dropzone**

```bash
npm install react-dropzone
```

- [ ] **Step 3: Add @types/react-dropzone if needed**

```bash
npm install -D @types/react-dropzone
```
(May not be needed with newer TypeScript — skip if `npm run build` passes without it)

- [ ] **Step 4: Create `ProductForm.tsx`**

```typescript
import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { productsApi } from '@/api/products'
import { categoriesApi } from '@/api/categories'
import { ImageUploader } from './ImageUploader'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { Switch } from '@/components/ui/switch'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { toast } from 'sonner'
import { Loader2, ArrowLeft } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import type { Product, CreateProductRequest } from '@/types/product'

interface ProductFormProps {
  product?: Product | null
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

function generateSku(name: string): string {
  const prefix = name
    .split(' ')
    .map((w) => w[0])
    .join('')
    .toUpperCase()
    .slice(0, 5)
  const timestamp = Date.now().toString().slice(-6)
  return `SP-${prefix}-${timestamp}`
}

export function ProductForm({ product }: ProductFormProps) {
  const navigate = useNavigate()
  const isEditing = !!product
  const [saving, setSaving] = useState(false)
  const [newImageUrls, setNewImageUrls] = useState<string[]>([])

  const [form, setForm] = useState({
    name: '',
    slug: '',
    sku: '',
    price: 0,
    stockQuantity: 0,
    categoryProductId: 0,
    isActive: true,
    description: '',
    flowerMeaning: '',
    origin: '',
    careInstruction: '',
  })

  useEffect(() => {
    if (product) {
      setForm({
        name: product.name,
        slug: product.slug || '',
        sku: product.sku || '',
        price: product.price,
        stockQuantity: product.stockQuantity,
        categoryProductId: product.categoryProductId,
        isActive: product.isActive,
        description: product.description || '',
        flowerMeaning: product.flowerMeaning || '',
        origin: product.origin || '',
        careInstruction: product.careInstruction || '',
      })
    }
  }, [product])

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: () => categoriesApi.getAll().then((r) => r.data),
  })

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      ...prev,
      name,
      slug: isEditing ? prev.slug : generateSlug(name),
      sku: isEditing ? prev.sku : prev.sku || generateSku(name),
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.name || !form.categoryProductId || form.price <= 0) {
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc')
      return
    }

    setSaving(true)
    try {
      const payload: CreateProductRequest = {
        name: form.name,
        slug: form.slug || undefined,
        sku: form.sku || undefined,
        price: form.price,
        stockQuantity: form.stockQuantity,
        categoryProductId: form.categoryProductId,
        isActive: form.isActive,
        description: form.description || undefined,
        flowerMeaning: form.flowerMeaning || undefined,
        origin: form.origin || undefined,
        careInstruction: form.careInstruction || undefined,
        newImages: newImageUrls.length > 0 ? newImageUrls : undefined,
      }

      if (isEditing && product) {
        await productsApi.update(product.id, { ...payload, id: product.id })
        toast.success('Cập nhật sản phẩm thành công')
      } else {
        await productsApi.create(payload)
        toast.success('Thêm sản phẩm thành công')
      }

      navigate('/products')
    } catch {
      toast.error(isEditing ? 'Cập nhật thất bại' : 'Thêm sản phẩm thất bại')
    } finally {
      setSaving(false)
    }
  }

  const handleDeleteExistingImage = async (imageId: number) => {
    if (!product) return
    try {
      await productsApi.deleteImage(product.id, imageId)
    } catch {
      toast.error('Xóa ảnh thất bại')
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => navigate('/products')}>
          <ArrowLeft className="size-5" />
        </Button>
        <h1 className="text-2xl font-semibold">
          {isEditing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm'}
        </h1>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Thông tin cơ bản</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="name">Tên sản phẩm *</Label>
                <Input
                  id="name"
                  value={form.name}
                  onChange={(e) => handleNameChange(e.target.value)}
                  required
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="category">Danh mục *</Label>
                <Select
                  value={String(form.categoryProductId)}
                  onValueChange={(v) =>
                    setForm((prev) => ({ ...prev, categoryProductId: Number(v) }))
                  }
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn danh mục" />
                  </SelectTrigger>
                  <SelectContent>
                    {categories?.map((cat) => (
                      <SelectItem key={cat.id} value={String(cat.id)}>
                        {cat.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-3">
              <div className="space-y-2">
                <Label htmlFor="slug">Slug</Label>
                <Input
                  id="slug"
                  value={form.slug}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, slug: e.target.value }))
                  }
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="sku">SKU</Label>
                <Input
                  id="sku"
                  value={form.sku}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, sku: e.target.value }))
                  }
                />
              </div>
              <div className="flex items-end gap-2">
                <div className="flex-1 space-y-2">
                  <Label htmlFor="isActive">Trạng thái</Label>
                  <div className="flex items-center gap-2 rounded-lg border px-3 py-2">
                    <Switch
                      id="isActive"
                      checked={form.isActive}
                      onCheckedChange={(v) =>
                        setForm((prev) => ({ ...prev, isActive: v }))
                      }
                    />
                    <Label htmlFor="isActive" className="cursor-pointer">
                      {form.isActive ? 'Đang bán' : 'Ngừng bán'}
                    </Label>
                  </div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Giá & Tồn kho</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="price">Giá (VNĐ) *</Label>
              <Input
                id="price"
                type="number"
                min={0}
                value={form.price}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, price: Number(e.target.value) }))
                }
                required
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="stockQuantity">Số lượng tồn</Label>
              <Input
                id="stockQuantity"
                type="number"
                min={0}
                value={form.stockQuantity}
                onChange={(e) =>
                  setForm((prev) => ({
                    ...prev,
                    stockQuantity: Number(e.target.value),
                  }))
                }
              />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Hình ảnh</CardTitle>
          </CardHeader>
          <CardContent>
            <ImageUploader
              existingImages={product?.images || []}
              onImagesChange={setNewImageUrls}
              onDeleteExisting={handleDeleteExistingImage}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Mô tả & Thông tin thêm</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="description">Mô tả</Label>
              <Textarea
                id="description"
                rows={4}
                value={form.description}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, description: e.target.value }))
                }
              />
            </div>
            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="flowerMeaning">Ý nghĩa hoa</Label>
                <Input
                  id="flowerMeaning"
                  value={form.flowerMeaning}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, flowerMeaning: e.target.value }))
                  }
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="origin">Xuất xứ</Label>
                <Input
                  id="origin"
                  value={form.origin}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, origin: e.target.value }))
                  }
                />
              </div>
            </div>
            <div className="space-y-2">
              <Label htmlFor="careInstruction">Hướng dẫn chăm sóc</Label>
              <Textarea
                id="careInstruction"
                rows={3}
                value={form.careInstruction}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, careInstruction: e.target.value }))
                }
              />
            </div>
          </CardContent>
        </Card>

        <div className="flex items-center justify-end gap-3">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate('/products')}
          >
            Hủy
          </Button>
          <Button type="submit" disabled={saving}>
            {saving && <Loader2 className="mr-2 size-4 animate-spin" />}
            {isEditing ? 'Cập nhật' : 'Thêm sản phẩm'}
          </Button>
        </div>
      </form>
    </div>
  )
}
```

- [ ] **Step 5: Add missing shadcn components**

```bash
npx shadcn@canary add textarea switch label -y
```

- [ ] **Step 6: Create `ProductFormPage.tsx`**

```typescript
import { useParams } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { productsApi } from '@/api/products'
import { ProductForm } from './components/ProductForm'
import { Loader2 } from 'lucide-react'

export function ProductFormPage() {
  const { id } = useParams()
  const isEditing = !!id

  const { data: product, isLoading } = useQuery({
    queryKey: ['product', id],
    queryFn: () => productsApi.getById(Number(id)).then((r) => r.data),
    enabled: isEditing,
  })

  if (isEditing && isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  return <ProductForm product={product ?? null} />
}
```

- [ ] **Step 7: Add `useBlocker` for unsaved changes guard (minimal)**

Add a simple `useEffect` with `beforeunload` in `ProductForm.tsx`:
```typescript
// At the top of ProductForm component, add:
useEffect(() => {
  const handler = (e: BeforeUnloadEvent) => {
    e.preventDefault()
    e.returnValue = ''
  }
  window.addEventListener('beforeunload', handler)
  return () => window.removeEventListener('beforeunload', handler)
}, [])
```

- [ ] **Step 8: Verify build**

```bash
npm run build
```
Expected: 0 errors

- [ ] **Step 9: Commit**

```bash
git add flower-admin.frontend/src/pages/products/ProductFormPage.tsx
git add flower-admin.frontend/src/pages/products/components/ProductForm.tsx
git add flower-admin.frontend/src/pages/products/components/ImageUploader.tsx
git commit -m "feat: add product create/edit form with multi-image upload"
```

---

