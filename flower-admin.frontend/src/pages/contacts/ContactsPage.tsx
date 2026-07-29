import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { contactsApi } from '@/api/contacts'
import { ContactTable } from './components/ContactTable'
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
import { Loader2, AlertCircle } from 'lucide-react'
import { toast } from 'sonner'
import type { ContactDTO } from '@/types/contact'

interface FilterTab {
  label: string
  value: boolean | undefined
}

const filterTabs: FilterTab[] = [
  { label: 'Tất cả', value: undefined },
  { label: 'Chưa đọc', value: false },
  { label: 'Đã đọc', value: true },
]

export function ContactsPage() {
  const [page, setPage] = useState(1)
  const [isReadFilter, setIsReadFilter] = useState<boolean | undefined>(undefined)
  const [deleteTarget, setDeleteTarget] = useState<ContactDTO | null>(null)
  const queryClient = useQueryClient()
  const pageSize = 20

  const { data: unreadCount } = useQuery({
    queryKey: ['contacts-unread'],
    queryFn: () => contactsApi.getUnreadCount().then((r) => r.data.count),
  })

  const { data, isLoading, error } = useQuery({
    queryKey: ['contacts', page, isReadFilter],
    queryFn: () =>
      contactsApi.getPaged({ page, pageSize, isRead: isReadFilter }).then((r) => r.data),
  })

  const toggleReadMutation = useMutation({
    mutationFn: ({ id, isRead }: { id: number; isRead: boolean }) => contactsApi.markRead(id, isRead),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => contactsApi.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contacts'] })
      queryClient.invalidateQueries({ queryKey: ['contacts-unread'] })
      toast.success('Đã xóa liên hệ')
      setDeleteTarget(null)
    },
    onError: () => toast.error('Không thể xóa liên hệ'),
  })

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="size-8 animate-spin text-muted-foreground" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
        <AlertCircle className="size-8" />
        <p>Không thể tải danh sách liên hệ</p>
        <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Liên hệ</h1>
      </div>

      <div className="flex gap-2">
        {filterTabs.map((tab) => (
          <button
            key={tab.label}
            onClick={() => { setIsReadFilter(tab.value); setPage(1) }}
            className={`px-3 py-1.5 text-sm rounded-full border transition-colors ${
              isReadFilter === tab.value
                ? 'bg-primary text-primary-foreground border-primary'
                : 'bg-background text-muted-foreground border-border hover:bg-muted'
            }`}
          >
            {tab.label}
            {tab.value === false && unreadCount !== undefined && (
              <span className="ml-1.5 inline-flex items-center justify-center size-5 rounded-full bg-primary text-[11px] font-medium text-primary-foreground">
                {unreadCount}
              </span>
            )}
          </button>
        ))}
      </div>

      <Card>
        <CardHeader className="pb-3">
          <CardTitle className="text-base">
            {data ? `${data.totalCount} liên hệ` : ''}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          {data && data.items.length > 0 ? (
            <div>
              <ContactTable
                contacts={data.items}
                onToggleRead={(contact) =>
                  toggleReadMutation.mutate({ id: contact.id, isRead: !contact.isRead })
                }
                onDelete={setDeleteTarget}
              />
              {(data.totalPages ?? 0) > 1 && (
                <div className="flex items-center justify-between border-t px-4 py-3">
                  <p className="text-sm text-muted-foreground">Trang {data.page} / {data.totalPages}</p>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" disabled={page <= 1} onClick={() => setPage((p) => Math.max(1, p - 1))}>Trước</Button>
                    <Button variant="outline" size="sm" disabled={page >= (data.totalPages ?? 1)} onClick={() => setPage((p) => p + 1)}>Sau</Button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <div className="flex h-48 flex-col items-center justify-center text-muted-foreground">
              <p>{isReadFilter === false ? 'Không có liên hệ chưa đọc' : 'Không có liên hệ nào'}</p>
            </div>
          )}
        </CardContent>
      </Card>

      <AlertDialog open={!!deleteTarget} onOpenChange={(open) => !open && setDeleteTarget(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Xóa liên hệ</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc muốn xóa liên hệ từ "{deleteTarget?.name}"? Hành động này không thể hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/80"
              onClick={() => deleteTarget && deleteMutation.mutate(deleteTarget.id)}
            >
              Xóa
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}
