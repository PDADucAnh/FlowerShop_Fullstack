import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Trash2, Mail, MailOpen } from 'lucide-react'
import type { ContactDTO } from '@/types/contact'

interface ContactTableProps {
  contacts: ContactDTO[]
  onToggleRead: (contact: ContactDTO) => void
  onDelete: (contact: ContactDTO) => void
}

export function ContactTable({ contacts, onToggleRead, onDelete }: ContactTableProps) {
  const navigate = useNavigate()

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('vi-VN', {
      day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
    })
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Người gửi</TableHead>
          <TableHead>Email</TableHead>
          <TableHead>Tiêu đề</TableHead>
          <TableHead>Ngày gửi</TableHead>
          <TableHead>Trạng thái</TableHead>
          <TableHead className="w-24">Thao tác</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {contacts.map((contact) => (
          <TableRow
            key={contact.id}
            className={`cursor-pointer ${!contact.isRead ? 'font-semibold' : ''}`}
            onClick={() => navigate(`/contacts/${contact.id}`)}
          >
            <TableCell>{contact.name}</TableCell>
            <TableCell className="text-muted-foreground">{contact.email}</TableCell>
            <TableCell className="max-w-xs truncate">{contact.subject}</TableCell>
            <TableCell className="text-muted-foreground">{formatDate(contact.createdAt)}</TableCell>
            <TableCell>
              <Badge variant={contact.isRead ? 'outline' : 'default'} className="text-xs">
                {contact.isRead ? 'Đã đọc' : 'Chưa đọc'}
              </Badge>
            </TableCell>
            <TableCell>
              <div className="flex items-center gap-1">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => { e.stopPropagation(); onToggleRead(contact) }}
                >
                  {contact.isRead ? <MailOpen className="size-4" /> : <Mail className="size-4" />}
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => { e.stopPropagation(); onDelete(contact) }}
                >
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
