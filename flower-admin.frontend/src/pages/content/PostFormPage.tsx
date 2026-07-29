import { useState, useEffect } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { postsApi } from '@/api/posts'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, ArrowLeft, Save } from 'lucide-react'
import { toast } from 'sonner'
import type { CreatePostDTO, UpdatePostDTO } from '@/types/post'

export function PostFormPage() {
  const { id } = useParams()
  const isEdit = !!id
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const { data: post, isLoading } = useQuery({
    queryKey: ['post', id],
    queryFn: () => postsApi.getById(Number(id)).then((r) => r.data),
    enabled: isEdit,
  })

  const [title, setTitle] = useState('')
  const [content, setContent] = useState('')
  const [summary, setSummary] = useState('')
  const [slug, setSlug] = useState('')
  const [imageUrl, setImageUrl] = useState('')
  const [categoryId, setCategoryId] = useState(0)

  useEffect(() => {
    if (post) {
      setTitle(post.title)
      setContent(post.content)
      setSummary(post.summary ?? '')
      setSlug(post.slug ?? '')
      setImageUrl(post.imageUrl)
      setCategoryId(post.categoryId)
    }
  }, [post])

  const createMutation = useMutation({
    mutationFn: (dto: CreatePostDTO) => postsApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã tạo bài viết'); navigate('/content') },
    onError: () => toast.error('Không thể tạo bài viết'),
  })

  const updateMutation = useMutation({
    mutationFn: (dto: UpdatePostDTO) => postsApi.update(Number(id), dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['posts'] }); toast.success('Đã cập nhật bài viết'); navigate('/content') },
    onError: () => toast.error('Không thể cập nhật bài viết'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!title || !content || !categoryId) { toast.error('Vui lòng điền đầy đủ thông tin'); return }

    if (isEdit) {
      updateMutation.mutate({ id: Number(id), title, content, summary: summary || undefined, slug: slug || undefined, imageUrl: imageUrl || undefined, categoryId })
    } else {
      createMutation.mutate({ title, content, summary: summary || undefined, slug: slug || undefined, imageUrl: imageUrl || undefined, categoryId })
    }
  }

  if (isEdit && isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={() => navigate('/content')}><ArrowLeft className="size-5" /></Button>
        <h1 className="text-2xl font-semibold">{isEdit ? 'Sửa bài viết' : 'Thêm bài viết'}</h1>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Thông tin bài viết</CardTitle></CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="text-sm font-medium">Tiêu đề *</label>
              <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
            <div>
              <label className="text-sm font-medium">Tóm tắt</label>
              <Input value={summary} onChange={(e) => setSummary(e.target.value)} />
            </div>
            <div>
              <label className="text-sm font-medium">Slug</label>
              <Input value={slug} onChange={(e) => setSlug(e.target.value)} placeholder="tu-khoa-tieng-viet" />
            </div>
            <div>
              <label className="text-sm font-medium">URL hình ảnh</label>
              <Input value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
            </div>
            <div>
              <label className="text-sm font-medium">ID danh mục *</label>
              <Input type="number" value={categoryId || ''} onChange={(e) => setCategoryId(Number(e.target.value))} required />
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
            <div className="flex gap-3">
              <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>
                <Save className="mr-1 size-4" />{isEdit ? 'Cập nhật' : 'Tạo bài viết'}
              </Button>
              <Button variant="outline" type="button" onClick={() => navigate('/content')}>Hủy</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
