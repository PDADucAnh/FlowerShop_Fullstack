import { useState, useEffect, useCallback, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { useDropzone } from 'react-dropzone'
import { productsApi } from '@/api/products'
import { productCategoriesApi } from '@/api/productCategories'
import { uploadApi } from '@/api/upload'
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
import { toast } from 'sonner'
import { Loader2, ArrowLeft, X, CloudUpload, ImagePlus, ChevronDown, ChevronUp, Plus, Save, Trash2 } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import type { Product, CreateProductRequest } from '@/types/product'

interface ProductFormProps {
  product?: Product | null
}

interface ImageItem {
  id: string
  url: string
  isExisting: boolean
  existingId?: number
  uploading?: boolean
}

interface VariantDraft {
  id?: number
  name: string
  price: number
  sku: string
  isDefault: boolean
  saving?: boolean
}

function removeDiacritics(str: string): string {
  return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
}

function generateSlug(name: string): string {
  return removeDiacritics(name)
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
  const slugManuallyEdited = useRef(false)
  const [saving, setSaving] = useState(false)
  const [showExtra, setShowExtra] = useState(false)
  const [mainImage, setMainImage] = useState<string>(product?.imageUrl || '')
  const [mainImageUploading, setMainImageUploading] = useState(false)

  const [form, setForm] = useState({
    name: '',
    slug: '',
    sku: '',
    price: 0,
    stockQuantity: 0,
    productCategoryId: 0,
    isActive: true,
    description: '',
    flowerMeaning: '',
    origin: '',
    careInstruction: '',
  })

  useEffect(() => {
    slugManuallyEdited.current = false
    if (product) {
      setForm({
        name: product.name,
        slug: product.slug || '',
        sku: product.sku || '',
        price: product.price,
        stockQuantity: product.stockQuantity,
        productCategoryId: product.productCategoryId,
        isActive: product.isActive,
        description: product.description || '',
        flowerMeaning: product.flowerMeaning || '',
        origin: product.origin || '',
        careInstruction: product.careInstruction || '',
      })
      setMainImage(product.imageUrl || '')
      setGalleryImages(
        (product.images || []).map((img) => ({
          id: `existing-${img.id}`,
          url: img.imageUrl,
          isExisting: true,
          existingId: img.id,
        }))
      )
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
    queryFn: () => productCategoriesApi.getAll().then((r) => r.data),
  })

  const handleNameChange = (name: string) => {
    setForm((prev) => ({
      ...prev,
      name,
      slug: !slugManuallyEdited.current ? generateSlug(name) : prev.slug,
      sku: isEditing ? prev.sku : prev.sku || generateSku(name),
    }))
  }

  const onMainImageDrop = useCallback(async (acceptedFiles: File[]) => {
    const file = acceptedFiles[0]
    if (!file) return
    setMainImageUploading(true)
    try {
      const { data } = await uploadApi.upload(file, 'flower-shop/products')
      setMainImage(data.url)
      toast.success('Tải ảnh chính thành công')
    } catch {
      toast.error('Tải ảnh chính thất bại')
    } finally {
      setMainImageUploading(false)
    }
  }, [])

  const {
    getRootProps: getMainRootProps,
    getInputProps: getMainInputProps,
    isDragActive: isMainDragActive,
  } = useDropzone({
    onDrop: onMainImageDrop,
    accept: { 'image/*': ['.png', '.jpg', '.jpeg', '.gif', '.webp'] },
    maxSize: 5 * 1024 * 1024,
    multiple: false,
  })

  // Gallery images
  const [galleryImages, setGalleryImages] = useState<ImageItem[]>([])

  // Variants (sizes)
  const [variants, setVariants] = useState<VariantDraft[]>([])

  useEffect(() => {
    if (product) {
      setVariants(
        (product.variants || []).map((v) => ({
          id: v.id,
          name: v.name,
          price: v.price,
          sku: v.sku || '',
          isDefault: v.isDefault,
        }))
      )
    } else {
      setVariants([])
    }
  }, [product])

  const onGalleryDrop = useCallback(async (acceptedFiles: File[]) => {
    for (const file of acceptedFiles) {
      const tempId = `gallery-${Date.now()}-${Math.random()}`
      setGalleryImages((prev) => [
        ...prev,
        { id: tempId, url: '', isExisting: false, uploading: true },
      ])
      try {
        const { data } = await uploadApi.upload(file, 'flower-shop/products')
        setGalleryImages((prev) =>
          prev.map((img) =>
            img.id === tempId
              ? { ...img, url: data.url, uploading: false }
              : img
          )
        )
      } catch {
        toast.error(`Tải ảnh thất bại: ${file.name}`)
        setGalleryImages((prev) => prev.filter((img) => img.id !== tempId))
      }
    }
  }, [])

  const {
    getRootProps: getGalleryRootProps,
    getInputProps: getGalleryInputProps,
    isDragActive: isGalleryDragActive,
  } = useDropzone({
    onDrop: onGalleryDrop,
    accept: { 'image/*': ['.png', '.jpg', '.jpeg', '.gif', '.webp'] },
    maxSize: 5 * 1024 * 1024,
  })

  const removeGalleryImage = (item: ImageItem) => {
    if (item.isExisting && item.existingId && product) {
      productsApi.deleteImage(product.id, item.existingId).catch(() => {
        toast.error('Xóa ảnh thất bại')
      })
    }
    setGalleryImages((prev) => prev.filter((img) => img.id !== item.id))
  }

  const updateVariant = (index: number, patch: Partial<VariantDraft>) => {
    setVariants((prev) => prev.map((v, i) => (i === index ? { ...v, ...patch } : v)))
  }

  const addVariantRow = () => {
    setVariants((prev) => [
      ...prev,
      { name: '', price: 0, sku: '', isDefault: prev.length === 0 },
    ])
  }

  const removeVariantRow = (index: number) => {
    setVariants((prev) => prev.filter((_, i) => i !== index))
  }

  const saveVariant = async (index: number) => {
    const variant = variants[index]
    if (!product) return
    if (!variant.name.trim() || variant.price < 0) {
      toast.error('Tên size và giá biến thể không hợp lệ')
      return
    }

    setVariants((prev) =>
      prev.map((v, i) => (i === index ? { ...v, saving: true } : v))
    )

    const payload = {
      name: variant.name.trim(),
      price: variant.price,
      sku: variant.sku.trim() || undefined,
      isDefault: variant.isDefault,
    }

    try {
      if (variant.id) {
        await productsApi.updateVariant(product.id, variant.id, { ...payload, id: variant.id })
        toast.success('Cập nhật biến thể thành công')
      } else {
        const { data } = await productsApi.addVariant(product.id, payload)
        setVariants((prev) =>
          prev.map((v, i) => (i === index ? { ...v, id: data.id, saving: false } : v))
        )
        toast.success('Thêm biến thể thành công')
        return
      }
    } catch {
      toast.error(variant.id ? 'Cập nhật biến thể thất bại' : 'Thêm biến thể thất bại')
    } finally {
      setVariants((prev) =>
        prev.map((v, i) => (i === index ? { ...v, saving: false } : v))
      )
    }
  }

  const deleteVariant = async (index: number) => {
    const variant = variants[index]
    if (!product) return

    if (variant.id) {
      try {
        await productsApi.deleteVariant(product.id, variant.id)
        toast.success('Xóa biến thể thành công')
      } catch {
        toast.error('Xóa biến thể thất bại')
        return
      }
    }
    removeVariantRow(index)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.name || !form.productCategoryId || form.price <= 0) {
      toast.error('Vui lòng điền đầy đủ thông tin bắt buộc')
      return
    }

    setSaving(true)
    try {
      const newUrls = galleryImages
        .filter((img) => !img.isExisting)
        .map((img) => img.url)
        .filter(Boolean)

      const payload: CreateProductRequest = {
        name: form.name,
        slug: form.slug || undefined,
        sku: form.sku || undefined,
        price: form.price,
        stockQuantity: form.stockQuantity,
        productCategoryId: form.productCategoryId,
        imageUrl: mainImage || undefined,
        isActive: form.isActive,
        description: form.description || undefined,
        flowerMeaning: form.flowerMeaning || undefined,
        origin: form.origin || undefined,
        careInstruction: form.careInstruction || undefined,
        newImages: newUrls.length > 0 ? newUrls : undefined,
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

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-headline-md text-on-surface mb-1">
            {isEditing ? 'Chỉnh sửa sản phẩm' : 'Thêm sản phẩm'}
          </h2>
          <p className="text-body-md text-on-surface-variant">
            {isEditing ? 'Cập nhật thông tin sản phẩm' : 'Thêm sản phẩm mới vào cửa hàng'}
          </p>
        </div>
        <Button
          variant="outline"
          onClick={() => navigate('/products')}
          className="flex items-center gap-2"
        >
          <ArrowLeft className="size-4" />
          Quay lại
        </Button>
      </div>

      <div className="rounded-xl border bg-white p-6 md:p-8 shadow-[0_4px_20px_rgba(171,44,93,0.02)]">
        <form onSubmit={handleSubmit}>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Left column */}
            <div className="space-y-5">
              <div>
                <Label className="text-on-surface-variant mb-1.5 block">Tên sản phẩm</Label>
                <Input
                  value={form.name}
                  onChange={(e) => handleNameChange(e.target.value)}
                  placeholder="Nhập tên sản phẩm..."
                  className="bg-surface-container-low border-input"
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">SKU</Label>
                  <Input
                    value={form.sku}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, sku: e.target.value }))
                    }
                    placeholder="PN-001"
                    className="bg-surface-container-low border-input"
                  />
                </div>
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">Slug</Label>
                  <Input
                    value={form.slug}
                    onChange={(e) => {
                      slugManuallyEdited.current = true
                      setForm((prev) => ({ ...prev, slug: e.target.value }))
                    }}
                    placeholder={generateSlug(form.name) || 'Tự động tạo nếu để trống'}
                    className="bg-surface-container-low border-input"
                  />
                </div>
              </div>

              <div>
                <Label className="text-on-surface-variant mb-1.5 block">Danh mục</Label>
                <Select
                  value={String(form.productCategoryId)}
                  onValueChange={(v) =>
                    setForm((prev) => ({ ...prev, productCategoryId: Number(v) }))
                  }
                >
                  <SelectTrigger className="bg-surface-container-low">
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

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">Giá (VNĐ)</Label>
                  <Input
                    type="number"
                    min={0}
                    value={form.price || ''}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, price: Number(e.target.value) }))
                    }
                    placeholder="0"
                    className="bg-surface-container-low border-input"
                    required
                  />
                </div>
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">Số lượng tồn kho</Label>
                  <Input
                    type="number"
                    min={0}
                    value={form.stockQuantity}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, stockQuantity: Number(e.target.value) }))
                    }
                    placeholder="0"
                    className="bg-surface-container-low border-input"
                  />
                </div>
              </div>

              <div>
                <Label className="text-on-surface-variant mb-1.5 block">Mô tả</Label>
                <Textarea
                  value={form.description}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, description: e.target.value }))
                  }
                  placeholder="Nhập mô tả chi tiết sản phẩm..."
                  rows={5}
                  className="bg-surface-container-low border-input resize-none"
                />
              </div>
            </div>

            {/* Right column */}
            <div className="space-y-6">
              <div>
                <Label className="text-on-surface-variant mb-1.5 block">Hình ảnh chính</Label>
                {mainImage ? (
                  <div className="relative rounded-xl overflow-hidden border border-input bg-surface-container-low group">
                    <img
                      src={mainImage}
                      alt=""
                      className="w-full h-48 object-cover"
                    />
                    <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-center justify-center">
                      <div className="opacity-0 group-hover:opacity-100 transition-opacity flex gap-2">
                        <Button
                          type="button"
                          variant="secondary"
                          size="sm"
                          onClick={() => setMainImage('')}
                        >
                          <X className="size-4 mr-1" />
                          Xóa
                        </Button>
                      </div>
                    </div>
                  </div>
                ) : (
                  <div
                    {...getMainRootProps()}
                    className={`border-2 border-dashed rounded-xl p-8 flex flex-col items-center justify-center bg-surface-container-low cursor-pointer transition-colors ${
                      isMainDragActive
                        ? 'border-primary bg-primary/5'
                        : 'border-outline-variant/30 hover:border-primary/50 hover:bg-primary/5'
                    }`}
                  >
                    <input {...getMainInputProps()} />
                    {mainImageUploading ? (
                      <Loader2 className="size-10 animate-spin text-primary mb-2" />
                    ) : (
                      <CloudUpload className="size-10 text-primary mb-2" />
                    )}
                    <p className="text-sm text-on-surface-variant">
                      {mainImageUploading
                        ? 'Đang tải...'
                        : isMainDragActive
                          ? 'Thả ảnh vào đây'
                          : 'Kéo thả hoặc click để tải ảnh lên'}
                    </p>
                    <p className="text-xs text-on-surface-variant/60 mt-1">
                      PNG, JPG (Tối đa 5MB)
                    </p>
                  </div>
                )}
              </div>

              <div>
                <div className="flex items-center justify-between mb-1.5">
                  <Label className="text-on-surface-variant">Ảnh phụ (Gallery)</Label>
                </div>
                <div className="flex flex-wrap gap-3">
                  {galleryImages.map((item) => (
                    <div key={item.id} className="relative w-[calc(33.333%-12px)] aspect-square rounded-lg overflow-hidden border border-input group">
                      {item.uploading ? (
                        <div className="flex items-center justify-center h-full bg-muted">
                          <Loader2 className="size-5 animate-spin text-muted-foreground" />
                        </div>
                      ) : (
                        <img
                          src={item.url}
                          alt=""
                          className="w-full h-full object-cover"
                        />
                      )}
                      {!item.uploading && (
                        <button
                          type="button"
                          onClick={() => removeGalleryImage(item)}
                          className="absolute -right-1.5 -top-1.5 flex size-5 items-center justify-center rounded-full bg-destructive text-destructive-foreground shadow opacity-0 group-hover:opacity-100 transition-opacity"
                        >
                          <X className="size-3" />
                        </button>
                      )}
                    </div>
                  ))}
                  <div
                    {...getGalleryRootProps()}
                    className={`w-[calc(33.333%-12px)] aspect-square border-2 border-dashed rounded-lg flex flex-col items-center justify-center bg-surface-container-low cursor-pointer transition-colors ${
                      isGalleryDragActive
                        ? 'border-primary bg-primary/5'
                        : 'border-outline-variant/30 hover:border-primary/50 hover:bg-primary/5'
                    }`}
                  >
                    <input {...getGalleryInputProps()} />
                    <ImagePlus className="size-6 text-on-surface-variant group-hover:text-primary transition-colors" />
                    <span className="text-xs text-on-surface-variant mt-1">Thêm ảnh</span>
                  </div>
                </div>
              </div>

              <div className="flex items-center gap-3 pt-2">
                <Label className="text-on-surface-variant mb-0 cursor-pointer">Đang bán</Label>
                <Switch
                  checked={form.isActive}
                  onCheckedChange={(v: boolean) =>
                    setForm((prev) => ({ ...prev, isActive: v }))
                  }
                />
              </div>
            </div>
          </div>

          {/* Extra info collapsible */}
          <div className="mt-6 pt-4 border-t border-border">
            <button
              type="button"
              onClick={() => setShowExtra(!showExtra)}
              className="flex items-center gap-2 text-sm text-on-surface-variant hover:text-primary transition-colors"
            >
              {showExtra ? <ChevronUp className="size-4" /> : <ChevronDown className="size-4" />}
              Thông tin thêm
            </button>
            {showExtra && (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mt-4">
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">Ý nghĩa hoa</Label>
                  <Input
                    value={form.flowerMeaning}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, flowerMeaning: e.target.value }))
                    }
                    placeholder="Ví dụ: Tượng trưng cho tình yêu vĩnh cửu"
                    className="bg-surface-container-low border-input"
                  />
                </div>
                <div>
                  <Label className="text-on-surface-variant mb-1.5 block">Xuất xứ</Label>
                  <Input
                    value={form.origin}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, origin: e.target.value }))
                    }
                    placeholder="Ví dụ: Đà Lạt, Việt Nam"
                    className="bg-surface-container-low border-input"
                  />
                </div>
                <div className="md:col-span-2">
                  <Label className="text-on-surface-variant mb-1.5 block">Hướng dẫn chăm sóc</Label>
                  <Textarea
                    value={form.careInstruction}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, careInstruction: e.target.value }))
                    }
                    placeholder="Nhập hướng dẫn chăm sóc hoa..."
                    rows={3}
                    className="bg-surface-container-low border-input resize-none"
                  />
                </div>
              </div>
            )}
          </div>

          {/* Variants (sizes) */}
          <div className="mt-6 pt-4 border-t border-border">
            <div className="flex items-center justify-between mb-3">
              <div>
                <p className="text-sm font-medium text-on-surface">Biến thể (Size / Giá)</p>
                <p className="text-xs text-on-surface-variant">
                  Các size kèm giá và mã SKU riêng cho sản phẩm
                </p>
              </div>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={addVariantRow}
                className="flex items-center gap-1"
              >
                <Plus className="size-4" />
                Thêm biến thể
              </Button>
            </div>

            {variants.length === 0 ? (
              <p className="text-sm text-on-surface-variant/70 py-2">
                Chưa có biến thể nào. Nhấn "Thêm biến thể" để tạo size riêng.
              </p>
            ) : (
              <div className="space-y-3">
                {variants.map((variant, index) => (
                  <div
                    key={variant.id ?? `new-${index}`}
                    className="grid grid-cols-12 gap-3 items-center rounded-lg border border-input bg-surface-container-low p-3"
                  >
                    <div className="col-span-4">
                      <Label className="text-on-surface-variant mb-1 block text-xs">Tên size</Label>
                      <Input
                        value={variant.name}
                        onChange={(e) => updateVariant(index, { name: e.target.value })}
                        placeholder="VD: Nhỏ, Vừa, Lớn"
                        className="bg-white border-input"
                      />
                    </div>
                    <div className="col-span-3">
                      <Label className="text-on-surface-variant mb-1 block text-xs">Giá (VNĐ)</Label>
                      <Input
                        type="number"
                        min={0}
                        value={variant.price || ''}
                        onChange={(e) => updateVariant(index, { price: Number(e.target.value) })}
                        placeholder="0"
                        className="bg-white border-input"
                      />
                    </div>
                    <div className="col-span-3">
                      <Label className="text-on-surface-variant mb-1 block text-xs">SKU</Label>
                      <Input
                        value={variant.sku}
                        onChange={(e) => updateVariant(index, { sku: e.target.value })}
                        placeholder="VD: SP-01-L"
                        className="bg-white border-input"
                      />
                    </div>
                    <div className="col-span-2 flex items-center justify-end gap-1">
                      <Switch
                        checked={variant.isDefault}
                        onCheckedChange={(v: boolean) => updateVariant(index, { isDefault: v })}
                        aria-label="Biến thể mặc định"
                      />
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() => saveVariant(index)}
                        disabled={variant.saving}
                        title={variant.id ? 'Lưu thay đổi' : 'Thêm'}
                        className="flex items-center gap-1"
                      >
                        {variant.saving ? (
                          <Loader2 className="size-4 animate-spin" />
                        ) : (
                          <Save className="size-4" />
                        )}
                      </Button>
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => deleteVariant(index)}
                        title="Xóa biến thể"
                        className="text-destructive"
                      >
                        <Trash2 className="size-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Footer */}
          <div className="flex items-center justify-end gap-4 mt-8 pt-6 border-t border-border">
            <Button
              type="button"
              variant="outline"
              onClick={() => navigate('/products')}
            >
              Hủy
            </Button>
            <Button
              type="submit"
              disabled={saving}
              className="shadow-[0_4px_20px_rgba(171,44,93,0.2)] hover:shadow-[0_6px_25px_rgba(171,44,93,0.3)] transition-shadow"
            >
              {saving && <Loader2 className="mr-2 size-4 animate-spin" />}
              {isEditing ? 'Cập nhật sản phẩm' : 'Thêm sản phẩm'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  )
}
