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
        const { data } = await uploadApi.upload(file, 'flower-shop/products')
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
    maxSize: 5 * 1024 * 1024,
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
