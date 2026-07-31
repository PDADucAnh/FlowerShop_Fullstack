import { useMutation, useQueryClient } from '@tanstack/react-query'
import { productCategoriesApi } from '@/api/productCategories'
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
import type { ProductCategory } from '@/types/productCategory'

interface DeleteCategoryDialogProps {
  category: ProductCategory | null
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
    mutationFn: (id: number) => productCategoriesApi.delete(id),
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
