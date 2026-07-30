import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation, useQuery } from '@tanstack/react-query'
import { ordersApi } from '@/api/orders'
import { customersApi } from '@/api/customers'
import { productsApi } from '@/api/products'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Loader2, Plus, Trash2, ArrowLeft } from 'lucide-react'
import { toast } from 'sonner'
import type { CreateOrderRequest, CreateOrderItemRequest } from '@/types/order'
import { PaymentMethod } from '@/types/order'

interface LineItem extends CreateOrderItemRequest {
  productName: string
  key: number
}

export function OrderCreatePage() {
  const navigate = useNavigate()
  const [customerId, setCustomerId] = useState<number | null>(null)
  const [customerSearch, setCustomerSearch] = useState('')
  const [customerOptions, setCustomerOptions] = useState<Array<{ id: number; fullName: string; phone?: string }>>([])
  const [lineItems, setLineItems] = useState<LineItem[]>([])
  const [nextKey, setNextKey] = useState(1)
  const [paymentMethod, setPaymentMethod] = useState(PaymentMethod.COD)
  const [deliveryDate, setDeliveryDate] = useState('')
  const [deliveryTimeSlot, setDeliveryTimeSlot] = useState('')
  const [deliveryDistrict, setDeliveryDistrict] = useState('')
  const [deliveryAddress, setDeliveryAddress] = useState('')
  const [recipientName, setRecipientName] = useState('')
  const [recipientPhone, setRecipientPhone] = useState('')
  const [notes, setNotes] = useState('')
  const [couponCode, setCouponCode] = useState('')
  const [productSearch, setProductSearch] = useState('')
  const [productResults, setProductResults] = useState<Array<{ id: number; name: string; price: number }>>([])

  const { data: customerData } = useQuery({
    queryKey: ['customers-search', customerSearch],
    queryFn: () => customersApi.getPaged({ search: customerSearch, pageSize: 10 }).then((r) => r.data),
    enabled: customerSearch.length >= 2,
  })

  const { data: productData } = useQuery({
    queryKey: ['products-search', productSearch],
    queryFn: () => productsApi.search(productSearch).then((r) => r.data),
    enabled: productSearch.length >= 2,
  })

  const createMutation = useMutation({
    mutationFn: (dto: CreateOrderRequest) => ordersApi.create(dto),
    onSuccess: (res) => {
      toast.success('Tạo đơn hàng thành công')
      navigate(`/orders/${res.data.orderId}`)
    },
    onError: (err: any) => toast.error(err.response?.data?.message || 'Không thể tạo đơn hàng'),
  })

  const addLineItem = () => {
    setLineItems([...lineItems, { productId: 0, productName: '', quantity: 1, unitPrice: 0, key: nextKey }])
    setNextKey(nextKey + 1)
  }

  const updateLineItem = (key: number, field: string, value: any) => {
    setLineItems(lineItems.map((item) =>
      item.key === key ? { ...item, [field]: value } : item
    ))
  }

  const removeLineItem = (key: number) => {
    setLineItems(lineItems.filter((item) => item.key !== key))
  }

  const selectProduct = (key: number, product: { id: number; name: string; price: number }) => {
    updateLineItem(key, 'productId', product.id)
    updateLineItem(key, 'productName', product.name)
    updateLineItem(key, 'unitPrice', product.price)
    setProductSearch('')
    setProductResults([])
  }

  const selectCustomer = (id: number, name: string) => {
    setCustomerId(id)
    setCustomerSearch(name)
    setCustomerOptions([])
  }

  const handleSubmit = () => {
    if (!customerId) { toast.error('Vui lòng chọn khách hàng'); return }
    if (lineItems.length === 0 || lineItems.some((i) => i.productId === 0)) {
      toast.error('Vui lòng thêm ít nhất 1 sản phẩm hợp lệ'); return
    }

    createMutation.mutate({
      customerId,
      notes: notes || undefined,
      items: lineItems.map(({ productId, quantity, unitPrice, sizeVariant }) => ({
        productId, quantity, unitPrice, sizeVariant,
      })),
      paymentMethod,
      deliveryDate: deliveryDate || undefined,
      deliveryTimeSlot: deliveryTimeSlot || undefined,
      deliveryDistrict: deliveryDistrict || undefined,
      deliveryAddress: deliveryAddress || undefined,
      recipientName: recipientName || undefined,
      recipientPhone: recipientPhone || undefined,
      couponCode: couponCode || undefined,
    })
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-3">
        <Button variant="ghost" size="icon" onClick={() => navigate('/orders')}>
          <ArrowLeft className="size-4" />
        </Button>
        <h1 className="text-2xl font-semibold">Tạo đơn hàng</h1>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="space-y-6 lg:col-span-2">
          <Card>
            <CardHeader><CardTitle className="text-base">Sản phẩm</CardTitle></CardHeader>
            <CardContent className="space-y-4">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Sản phẩm</TableHead>
                    <TableHead className="w-20 text-center">SL</TableHead>
                    <TableHead className="w-28 text-right">Đơn giá</TableHead>
                    <TableHead className="w-28 text-right">Thành tiền</TableHead>
                    <TableHead className="w-10"></TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {lineItems.map((item) => (
                    <TableRow key={item.key}>
                      <TableCell>
                        <div className="relative">
                          <Input
                            placeholder="Tìm sản phẩm..."
                            value={item.productName}
                            onChange={(e) => {
                              updateLineItem(item.key, 'productName', e.target.value)
                              setProductSearch(e.target.value)
                            }}
                          />
                          {productSearch.length >= 2 && productData && (
                            <div className="absolute z-10 mt-1 w-full rounded-md border bg-popover shadow-md">
                              {productData.map((p) => (
                                <button
                                  key={p.id}
                                  className="w-full px-3 py-2 text-left text-sm hover:bg-muted"
                                  onClick={() => selectProduct(item.key, p)}
                                >
                                  {p.name} — {p.price.toLocaleString()}₫
                                </button>
                              ))}
                            </div>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <Input
                          type="number" min={1}
                          value={item.quantity}
                          onChange={(e) => updateLineItem(item.key, 'quantity', Number(e.target.value))}
                          className="w-20 text-center"
                        />
                      </TableCell>
                      <TableCell>
                        <Input
                          type="number"
                          value={item.unitPrice}
                          onChange={(e) => updateLineItem(item.key, 'unitPrice', Number(e.target.value))}
                          className="w-28 text-right"
                        />
                      </TableCell>
                      <TableCell className="text-right font-mono">
                        {(item.quantity * item.unitPrice).toLocaleString()}₫
                      </TableCell>
                      <TableCell>
                        <Button variant="ghost" size="icon" onClick={() => removeLineItem(item.key)}>
                          <Trash2 className="size-4 text-destructive" />
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              <Button variant="outline" size="sm" onClick={addLineItem}>
                <Plus className="mr-1 size-4" />Thêm sản phẩm
              </Button>
            </CardContent>
          </Card>

          <Card>
            <CardHeader><CardTitle className="text-base">Thông tin giao hàng</CardTitle></CardHeader>
            <CardContent className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>Ngày giao</Label>
                <Input type="date" value={deliveryDate} onChange={(e) => setDeliveryDate(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Khung giờ</Label>
                <Input value={deliveryTimeSlot} onChange={(e) => setDeliveryTimeSlot(e.target.value)} placeholder="VD: 08:00-12:00" />
              </div>
              <div className="space-y-2">
                <Label>Quận/Huyện</Label>
                <Input value={deliveryDistrict} onChange={(e) => setDeliveryDistrict(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Địa chỉ</Label>
                <Input value={deliveryAddress} onChange={(e) => setDeliveryAddress(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>Người nhận</Label>
                <Input value={recipientName} onChange={(e) => setRecipientName(e.target.value)} />
              </div>
              <div className="space-y-2">
                <Label>SĐT người nhận</Label>
                <Input value={recipientPhone} onChange={(e) => setRecipientPhone(e.target.value)} />
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card>
            <CardHeader><CardTitle className="text-base">Khách hàng</CardTitle></CardHeader>
            <CardContent className="space-y-3">
              <div className="relative">
                <Input
                  placeholder="Tìm khách hàng..."
                  value={customerSearch}
                  onChange={(e) => { setCustomerSearch(e.target.value); if (e.target.value.length < 2) setCustomerOptions([]) }}
                />
                {customerSearch.length >= 2 && customerData && customerData.items.length > 0 && !customerId && (
                  <div className="absolute z-10 mt-1 w-full rounded-md border bg-popover shadow-md max-h-48 overflow-y-auto">
                    {customerData.items.map((c) => (
                      <button
                        key={c.id}
                        className="w-full px-3 py-2 text-left text-sm hover:bg-muted"
                        onClick={() => selectCustomer(c.id, `${c.fullName} (${c.phone || c.email})`)}
                      >
                        {c.fullName} — {c.phone || c.email}
                      </button>
                    ))}
                  </div>
                )}
              </div>
              {customerId && (
                <p className="text-xs text-muted-foreground">Đã chọn: {customerSearch}</p>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader><CardTitle className="text-base">Thanh toán</CardTitle></CardHeader>
            <CardContent className="space-y-3">
              <div className="space-y-2">
                <Label>Phương thức</Label>
                <Select value={String(paymentMethod)} onValueChange={(v) => setPaymentMethod(Number(v) as typeof PaymentMethod.COD)}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="1">COD</SelectItem>
                    <SelectItem value="0">Chuyển khoản</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label>Mã giảm giá</Label>
                <Input value={couponCode} onChange={(e) => setCouponCode(e.target.value)} placeholder="Nhập mã (nếu có)" />
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader><CardTitle className="text-base">Ghi chú</CardTitle></CardHeader>
            <CardContent>
              <Textarea value={notes} onChange={(e) => setNotes(e.target.value)} placeholder="Ghi chú đơn hàng..." />
            </CardContent>
          </Card>

          <div className="space-y-2">
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Tạm tính</span>
              <span className="font-mono">{lineItems.reduce((s, i) => s + i.quantity * i.unitPrice, 0).toLocaleString()}₫</span>
            </div>
            <Button className="w-full" onClick={handleSubmit} disabled={createMutation.isPending}>
              {createMutation.isPending ? (
                <><Loader2 className="mr-2 size-4 animate-spin" />Đang tạo...</>
              ) : 'Tạo đơn hàng'}
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
