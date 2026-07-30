import { useNavigate } from 'react-router-dom'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import type { CustomerDTO } from '@/types/customer'

interface CustomerTableProps {
  customers: CustomerDTO[]
}

export function CustomerTable({ customers }: CustomerTableProps) {
  const navigate = useNavigate()

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-12">Avatar</TableHead>
          <TableHead>Tên khách hàng</TableHead>
          <TableHead>Email</TableHead>
          <TableHead>SĐT</TableHead>
          <TableHead className="text-center">Tổng đơn</TableHead>
          <TableHead className="text-center">Trạng thái</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {customers.map((customer) => (
          <TableRow
            key={customer.id}
            className="cursor-pointer"
            onClick={() => navigate(`/customers/${customer.id}`)}
          >
            <TableCell>
              {customer.avatar ? (
                <img src={customer.avatar} alt="" className="size-9 rounded-full border object-cover" />
              ) : (
                <div className="flex size-9 items-center justify-center rounded-full border border-dashed text-xs text-muted-foreground">
                  {customer.fullName.charAt(0).toUpperCase()}
                </div>
              )}
            </TableCell>
            <TableCell className="font-medium">{customer.fullName}</TableCell>
            <TableCell className="text-muted-foreground">{customer.email}</TableCell>
            <TableCell>{customer.phone || '—'}</TableCell>
            <TableCell className="text-center">{customer.totalOrders}</TableCell>
            <TableCell className="text-center">
              <span className={`inline-block size-2 rounded-full ${customer.isActive ? 'bg-green-500' : 'bg-gray-300'}`} />
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}
