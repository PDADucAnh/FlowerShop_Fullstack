import { useParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { contactsApi } from '@/api/contacts'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogAction,
  AlertDialogCancel,
} from '@/components/ui/alert-dialog'
import { ArrowLeft, Trash2, Mail, MailOpen, Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import { useState } from 'react'

export function ContactDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [deleteOpen, setDeleteOpen] = useState(false)

  const contactId = Number(id)

  const { data: contact, isLoading, error } = useQuery({
    queryKey: ['contact', contactId],
    queryFn: () => contactsApi.getById(contactId).then((r) => r.data),
    enabled: !!contactId,
  })

  const toggleReadMutation = useMutation({
    mutationFn: (isRead: boolean) => contactsApi.markRead(contactId, isRead),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contact', contactId] })
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Cập nhật trạng thái thành công')
    },
  })

  const deleteMutation = useMutation({
    mutationFn: () => contactsApi.delete(contactId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Đã xóa liên hệ')
      navigate('/contacts')
    },
    onError: () => toast.error('Không thể xóa liên hệ'),
  })

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('vi-VN', {
      day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
    })
  }

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error || !contact) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không tìm thấy liên hệ</p>
        <Button variant="outline" onClick={() => navigate('/contacts')}>Quay lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <Button variant="ghost" size="icon" onClick={() => navigate('/contacts')}>
            <ArrowLeft className="size-4" />
          </Button>
          <h1 className="text-2xl font-semibold">{contact.name}</h1>
        </div>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => toggleReadMutation.mutate(!contact.isRead)}
          >
            {contact.isRead ? <MailOpen className="mr-1 size-4" /> : <Mail className="mr-1 size-4" />}
            {contact.isRead ? 'Đánh dấu chưa đọc' : 'Đánh dấu đã đọc'}
          </Button>
          <Button variant="destructive" size="sm" onClick={() => setDeleteOpen(true)}>
            <Trash2 className="mr-1 size-4" />
            Xóa
          </Button>
        </div>
      </div>

      <div className="flex items-center gap-3 text-sm text-muted-foreground">
        <span>{contact.email}</span>
        {contact.phone && <span>· {contact.phone}</span>}
        <span>· {formatDate(contact.createdAt)}</span>
        <Badge variant={contact.isRead ? 'outline' : 'default'} className="text-xs">
          {contact.isRead ? 'Đã đọc' : 'Chưa đọc'}
        </Badge>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{contact.subject}</CardTitle>
        </CardHeader>
        <CardContent>
          <p className="whitespace-pre-wrap text-sm leading-relaxed">{contact.message}</p>
        </CardContent>
      </Card>

      <Button variant="link" onClick={() => navigate('/contacts')}>
        <ArrowLeft className="mr-1 size-4" />
        Quay lại danh sách
      </Button>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Xóa liên hệ</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc muốn xóa liên hệ này? Hành động này không thể hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/80"
              onClick={() => deleteMutation.mutate()}
            >
              Xóa
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
