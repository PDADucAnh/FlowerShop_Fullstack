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
          <TableHead className="w-12">Ảnh</TableHead>
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
            <TableCell>
              {cat.imageUrl ? (
                <img
                  src={cat.imageUrl}
                  alt={cat.name}
                  className="size-9 rounded-md border object-cover"
                />
              ) : (
                <div className="size-9 rounded-md border border-dashed" />
              )}
            </TableCell>
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
