import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { pagesApi } from '@/api/pages'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, ArrowLeft, Save } from 'lucide-react'
import { toast } from 'sonner'
import type { CreatePageDTO, UpdatePageDTO } from '@/types/page'

export function PageFormPage() {
  const { id } = useParams()
  const isEdit = !!id
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const { data: page, isLoading } = useQuery({
    queryKey: ['page', id],
    queryFn: () => pagesApi.getById(Number(id)).then((r) => r.data),
    enabled: isEdit,
  })

  const [title, setTitle] = useState('')
  const [slug, setSlug] = useState('')
  const [content, setContent] = useState('')
  const [isActive, setIsActive] = useState(true)

  useEffect(() => {
    if (page) {
      setTitle(page.title)
      setSlug(page.slug ?? '')
      setContent(page.content)
      setIsActive(page.isActive)
    }
  }, [page])

  const createMutation = useMutation({
    mutationFn: (dto: CreatePageDTO) => pagesApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã tạo trang'); navigate('/content') },
    onError: () => toast.error('Không thể tạo trang'),
  })

  const updateMutation = useMutation({
    mutationFn: (dto: UpdatePageDTO) => pagesApi.update(Number(id), dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['pages'] }); toast.success('Đã cập nhật trang'); navigate('/content') },
    onError: () => toast.error('Không thể cập nhật trang'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!title || !content) { toast.error('Vui lòng điền đầy đủ thông tin'); return }

    if (isEdit) {
      updateMutation.mutate({ id: Number(id), title, slug: slug || undefined, content, isActive })
    } else {
      createMutation.mutate({ title, slug: slug || undefined, content, isActive })
    }
  }

  if (isEdit && isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={() => navigate('/content')}><ArrowLeft className="size-5" /></Button>
        <h1 className="text-2xl font-semibold">{isEdit ? 'Sửa trang' : 'Thêm trang'}</h1>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Thông tin trang</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="text-sm font-medium">Tiêu đề *</label>
              <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
            <div>
              <label className="text-sm font-medium">Slug</label>
              <Input value={slug} onChange={(e) => setSlug(e.target.value)} placeholder="gioi-thieu" />
            </div>
            <div>
              <label className="text-sm font-medium">Nội dung *</label>
              <textarea
                className="w-full min-h-[300px] rounded-md border bg-background p-3 text-sm"
                value={content}
                onChange={(e) => setContent(e.target.value)}
                required
              />
            </div>
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} />
              Hiển thị
            </label>
            <div className="flex gap-3">
              <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                <Save className="mr-1 size-4" />{isEdit ? 'Cập nhật' : 'Tạo trang'}
              </Button>
              <Button variant="outline" type="button" onClick={() => navigate('/content')}>Hủy</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
