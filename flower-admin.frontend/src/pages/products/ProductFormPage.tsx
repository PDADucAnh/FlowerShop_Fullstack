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
