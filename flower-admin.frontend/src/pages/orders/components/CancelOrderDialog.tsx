import { useState } from 'react'
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
} from '@/components/ui/alert-dialog'
import { Button } from '@/components/ui/button'
import { Textarea } from '@/components/ui/textarea'

interface CancelOrderDialogProps {
  orderId: number
  open: boolean
  onOpenChange: (open: boolean) => void
  onConfirm: (reason: string) => void
  loading?: boolean
}

export function CancelOrderDialog({ orderId, open, onOpenChange, onConfirm, loading }: CancelOrderDialogProps) {
  const [reason, setReason] = useState('')

  const handleConfirm = () => {
    if (!reason.trim()) return
    onConfirm(reason.trim())
  }

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Hủy đơn hàng #{orderId}</AlertDialogTitle>
          <AlertDialogDescription>
            Hành động này sẽ hủy đơn hàng và thông báo cho khách hàng. Vui lòng nhập lý do hủy.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <div className="py-3">
          <Textarea
            placeholder="Nhập lý do hủy đơn…"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            rows={3}
            className="w-full"
          />
        </div>
        <AlertDialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Hủy</Button>
          <Button
            variant="destructive"
            onClick={handleConfirm}
            disabled={!reason.trim() || loading}
          >
            {loading ? 'Đang hủy…' : 'Xác nhận hủy'}
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
