import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { productCategoriesApi } from '@/api/productCategories'
import { CategoryTable } from './components/CategoryTable'
import { CategoryDialog } from './components/CategoryDialog'
import { DeleteCategoryDialog } from './components/DeleteCategoryDialog'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Plus, Loader2, ArrowLeft } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import type { ProductCategory } from '@/types/productCategory'

export function CategoriesPage() {
  const navigate = useNavigate()
  const [editTarget, setEditTarget] = useState<ProductCategory | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<ProductCategory | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)

  const { data: categories, isLoading } = useQuery({
    queryKey: ['categories'],
    queryFn: () => productCategoriesApi.getAll().then((r) => r.data),
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
