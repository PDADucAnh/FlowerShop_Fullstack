import { useState, useEffect } from 'react'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogFooter,
  DialogTitle,
} from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Switch } from '@/components/ui/switch'
import type { CustomerDTO, UpdateCustomerRequest } from '@/types/customer'

interface CustomerEditDialogProps {
  customer: CustomerDTO | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSave: (data: UpdateCustomerRequest) => void
  loading?: boolean
}

export function CustomerEditDialog({ customer, open, onOpenChange, onSave, loading }: CustomerEditDialogProps) {
  const [form, setForm] = useState<UpdateCustomerRequest>({
    id: 0,
    fullName: '',
    email: '',
    phone: '',
    address: '',
    isActive: true,
  })

  useEffect(() => {
    if (customer) {
      setForm({
        id: customer.id,
        fullName: customer.fullName,
        email: customer.email,
        phone: customer.phone || '',
        address: customer.address || '',
        isActive: customer.isActive,
      })
    }
  }, [customer])

  const handleSubmit = () => {
    if (!form.fullName.trim() || !form.email.trim()) return
    onSave(form)
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Chỉnh sửa khách hàng</DialogTitle>
        </DialogHeader>
        <div className="space-y-4 py-3">
          <div className="space-y-1">
            <Label>Tên khách hàng</Label>
            <Input value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>Email</Label>
            <Input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>SĐT</Label>
            <Input value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          </div>
          <div className="space-y-1">
            <Label>Địa chỉ</Label>
            <Input value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
          </div>
          <div className="flex items-center gap-2">
            <Switch
              checked={form.isActive}
              onCheckedChange={(checked) => setForm({ ...form, isActive: checked })}
            />
            <Label>Đang hoạt động</Label>
          </div>
        </div>
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button onClick={handleSubmit} disabled={loading}>
            {loading ? 'Đang lưu…' : 'Lưu'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
