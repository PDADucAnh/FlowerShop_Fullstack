import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { usersApi } from '@/api/users'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Badge } from '@/components/ui/badge'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import {
  Select, SelectContent, SelectItem, SelectTrigger, SelectValue,
} from '@/components/ui/select'
import {
  Table, TableBody, TableCell, TableHead, TableHeader, TableRow,
} from '@/components/ui/table'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import type { User, CreateUserRequest, UpdateUserRequest } from '@/types/user'

const roleOptions = [
  { value: 'Admin', label: 'Admin' },
  { value: 'Staff', label: 'Nhân viên' },
]

function RoleBadge({ role }: { role: string }) {
  const variant = role === 'Admin' ? 'default' : 'secondary'
  return <Badge variant={variant}>{role === 'Admin' ? 'Admin' : 'Staff'}</Badge>
}

export function UsersPage() {
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editItem, setEditItem] = useState<User | null>(null)
  const [deleteConfirm, setDeleteConfirm] = useState<User | null>(null)
  const queryClient = useQueryClient()

  const { data: users, isLoading, error } = useQuery({
    queryKey: ['users'],
    queryFn: () => usersApi.getAll().then((r) => r.data),
  })

  const [formUsername, setFormUsername] = useState('')
  const [formPassword, setFormPassword] = useState('')
  const [formFullName, setFormFullName] = useState('')
  const [formEmail, setFormEmail] = useState('')
  const [formPhone, setFormPhone] = useState('')
  const [formAddress, setFormAddress] = useState('')
  const [formRole, setFormRole] = useState('Staff')

  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['users'] })

  const createMutation = useMutation({
    mutationFn: (dto: CreateUserRequest) => usersApi.create(dto),
    onSuccess: () => { invalidate(); setDialogOpen(false); toast.success('Đã tạo nhân viên') },
    onError: () => toast.error('Không thể tạo nhân viên'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpdateUserRequest }) => usersApi.update(id, dto),
    onSuccess: () => { invalidate(); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật nhân viên') },
    onError: () => toast.error('Không thể cập nhật nhân viên'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => usersApi.delete(id),
    onSuccess: () => { invalidate(); setDeleteConfirm(null); toast.success('Đã xóa nhân viên') },
    onError: () => toast.error('Không thể xóa nhân viên'),
  })

  const openCreate = () => {
    setEditItem(null)
    setFormUsername(''); setFormPassword(''); setFormFullName('')
    setFormEmail(''); setFormPhone(''); setFormAddress('')
    setFormRole('Staff')
    setDialogOpen(true)
  }

  const openEdit = (item: User) => {
    setEditItem(item)
    setFormUsername(item.username)
    setFormPassword('')
    setFormFullName(item.fullName)
    setFormEmail(item.email || '')
    setFormPhone(item.phone || '')
    setFormAddress(item.address || '')
    setFormRole(item.role)
    setDialogOpen(true)
  }

  const handleSubmit = () => {
    if (!formUsername || !formFullName) { toast.error('Vui lòng điền đầy đủ thông tin'); return }
    if (!editItem && !formPassword) { toast.error('Vui lòng nhập mật khẩu'); return }

    if (editItem) {
      updateMutation.mutate({
        id: editItem.id,
        dto: {
          id: editItem.id,
          username: formUsername,
          password: formPassword || undefined,
          fullName: formFullName,
          email: formEmail || undefined,
          phone: formPhone || undefined,
          address: formAddress || undefined,
          role: formRole,
        },
      })
    } else {
      createMutation.mutate({
        username: formUsername,
        password: formPassword,
        fullName: formFullName,
        role: formRole,
      })
    }
  }

  if (isLoading) return <div className="flex h-64 items-center justify-center"><Loader2 className="size-8 animate-spin text-muted-foreground" /></div>
  if (error) return (
    <div className="flex h-64 flex-col items-center justify-center gap-2 text-destructive">
      <AlertCircle className="size-8" /><p>Không thể tải danh sách nhân viên</p>
      <Button variant="outline" onClick={() => window.location.reload()}>Thử lại</Button>
    </div>
  )

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold">Quản lý nhân viên</h1>
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger asChild onClick={openCreate}>
            <Button size="sm"><Plus className="mr-1 size-4" />Thêm nhân viên</Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader><DialogTitle>{editItem ? 'Sửa nhân viên' : 'Thêm nhân viên'}</DialogTitle></DialogHeader>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label>Tên đăng nhập</Label>
                <Input value={formUsername} onChange={(e) => setFormUsername(e.target.value)} placeholder="username" required />
              </div>
              <div className="space-y-2">
                <Label>Mật khẩu {editItem && '(để trống nếu không đổi)'}</Label>
                <Input type="password" value={formPassword} onChange={(e) => setFormPassword(e.target.value)} placeholder={editItem ? '••••••' : 'Mật khẩu'} required={!editItem} />
              </div>
              <div className="space-y-2">
                <Label>Họ tên</Label>
                <Input value={formFullName} onChange={(e) => setFormFullName(e.target.value)} placeholder="Nguyễn Văn A" required />
              </div>
              <div className="space-y-2">
                <Label>Email</Label>
                <Input type="email" value={formEmail} onChange={(e) => setFormEmail(e.target.value)} placeholder="email@example.com" />
              </div>
              <div className="space-y-2">
                <Label>Số điện thoại</Label>
                <Input value={formPhone} onChange={(e) => setFormPhone(e.target.value)} placeholder="0123456789" />
              </div>
              <div className="space-y-2">
                <Label>Địa chỉ</Label>
                <Input value={formAddress} onChange={(e) => setFormAddress(e.target.value)} placeholder="Địa chỉ" />
              </div>
              <div className="space-y-2">
                <Label>Vai trò</Label>
                <Select value={formRole} onValueChange={(v) => v !== null && setFormRole(v)}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    {roleOptions.map((opt) => (
                      <SelectItem key={opt.value} value={opt.value}>{opt.label}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <DialogFooter>
                <DialogClose render={<Button variant="outline" />}>Hủy</DialogClose>
                <Button onClick={handleSubmit} disabled={createMutation.isPending || updateMutation.isPending}>
                  {createMutation.isPending || updateMutation.isPending ? (
                    <><Loader2 className="mr-2 size-4 animate-spin" />Đang lưu...</>
                  ) : (editItem ? 'Cập nhật' : 'Thêm')}
                </Button>
              </DialogFooter>
            </div>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardHeader><CardTitle className="text-base">Tất cả nhân viên ({users?.length || 0})</CardTitle></CardHeader>
        <CardContent className="p-0">
          {users && users.length > 0 ? (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Tên đăng nhập</TableHead>
                  <TableHead>Họ tên</TableHead>
                  <TableHead>Email</TableHead>
                  <TableHead>SĐT</TableHead>
                  <TableHead>Vai trò</TableHead>
                  <TableHead className="text-right">Thao tác</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {users.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell><span className="font-mono text-sm">{user.username}</span></TableCell>
                    <TableCell className="font-medium">{user.fullName}</TableCell>
                    <TableCell className="text-muted-foreground">{user.email || '—'}</TableCell>
                    <TableCell className="text-muted-foreground">{user.phone || '—'}</TableCell>
                    <TableCell><RoleBadge role={user.role} /></TableCell>
                    <TableCell className="text-right">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(user)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => setDeleteConfirm(user)}><Trash2 className="size-4 text-destructive" /></Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có nhân viên nào</div>
          )}
        </CardContent>
      </Card>

      <Dialog open={!!deleteConfirm} onOpenChange={(open) => { if (!open) setDeleteConfirm(null) }}>
        <DialogContent>
          <DialogHeader><DialogTitle>Xác nhận xóa</DialogTitle></DialogHeader>
          <p>Bạn có chắc muốn xóa nhân viên <strong>{deleteConfirm?.fullName}</strong> ({deleteConfirm?.username})?</p>
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteConfirm(null)}>Hủy</Button>
            <Button variant="destructive" onClick={() => deleteConfirm && deleteMutation.mutate(deleteConfirm.id)} disabled={deleteMutation.isPending}>
              {deleteMutation.isPending ? <><Loader2 className="mr-2 size-4 animate-spin" />Đang xóa...</> : 'Xóa'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
