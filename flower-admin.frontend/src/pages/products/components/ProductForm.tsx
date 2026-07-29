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

  useEffect(() => {
    const handler = (e: BeforeUnloadEvent) => {
      e.preventDefault()
      e.returnValue = ''
    }
    window.addEventListener('beforeunload', handler)
    return () => window.removeEventListener('beforeunload', handler)
  }, [])

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
                      onCheckedChange={(v: boolean) =>
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
