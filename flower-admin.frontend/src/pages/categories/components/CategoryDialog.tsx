import { useState, useEffect, useRef, type ChangeEvent } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { productCategoriesApi } from '@/api/productCategories'
import { uploadApi } from '@/api/upload'
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
import { Loader2, Upload, X } from 'lucide-react'
import type { ProductCategory } from '@/types/productCategory'

interface CategoryDialogProps {
  category: ProductCategory | null
  open: boolean
  onOpenChange: (open: boolean) => void
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

export function CategoryDialog({ category, open, onOpenChange }: CategoryDialogProps) {
  const queryClient = useQueryClient()
  const isEditing = !!category
  const slugManuallyEdited = useRef(false)
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [slug, setSlug] = useState('')
  const [imageUrl, setImageUrl] = useState('')
  const [uploading, setUploading] = useState(false)

  useEffect(() => {
    if (open) {
      slugManuallyEdited.current = false
    }
    if (category) {
      setName(category.name)
      setDescription(category.description || '')
      setSlug(category.slug || '')
      setImageUrl(category.imageUrl || '')
    } else {
      setName('')
      setDescription('')
      setSlug('')
      setImageUrl('')
    }
  }, [category, open])

  const mutation = useMutation({
    mutationFn: () => {
      const payload = { name, description, slug: slug || generateSlug(name), imageUrl: imageUrl || undefined }
      return isEditing
        ? productCategoriesApi.update(category!.id, { ...payload, id: category!.id })
        : productCategoriesApi.create(payload)
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
    if (!isEditing && !slugManuallyEdited.current) {
      setSlug(generateSlug(value))
    }
  }

  const handleImageUpload = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file) return
    setUploading(true)
    try {
      const { data } = await uploadApi.upload(file, 'flower-shop/categories')
      setImageUrl(data.url)
      toast.success('Đã tải ảnh lên')
    } catch {
      toast.error('Tải ảnh thất bại')
    } finally {
      setUploading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
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
              onChange={(e) => {
                slugManuallyEdited.current = true
                setSlug(e.target.value)
              }}
              placeholder={!isEditing ? generateSlug(name) || 'Tự động tạo nếu để trống' : ''}
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
          <div className="space-y-2">
            <Label>Ảnh đại diện</Label>
            <div className="flex items-start gap-4">
              {imageUrl ? (
                <div className="relative size-20 shrink-0 overflow-hidden rounded-lg border">
                  <img
                    src={imageUrl}
                    alt="Preview"
                    className="size-full object-cover"
                  />
                  <button
                    type="button"
                    onClick={() => setImageUrl('')}
                    className="absolute right-0.5 top-0.5 rounded-full bg-background/80 p-0.5 text-muted-foreground hover:text-destructive"
                  >
                    <X className="size-3" />
                  </button>
                </div>
              ) : (
                <div className="flex size-20 shrink-0 items-center justify-center rounded-lg border border-dashed text-muted-foreground">
                  <Upload className="size-6" />
                </div>
              )}
              <div className="flex-1">
                <Input
                  type="file"
                  accept="image/*"
                  onChange={handleImageUpload}
                  disabled={uploading}
                />
                {uploading && <p className="mt-1 text-xs text-muted-foreground">Đang tải ảnh lên...</p>}
              </div>
            </div>
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
