# Migrate Partial Features Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Complete migration of 4 partially-implemented features from Backend MVC Views to React Frontend.

**Architecture:** Backend exposes REST APIs (already built for most features); frontend adds missing pages/tabs consuming these APIs following existing patterns (shadcn/ui, TanStack Query, sonner toast).

**Tech Stack:** React 18, TypeScript, Vite, shadcn/ui, Tailwind CSS, TanStack Query, React Router v6, sonner, lucide-react

## Global Constraints

- Follow existing code patterns in `flower-admin.frontend/` (React Router lazy routes, `@/` path aliases, shadcn/ui components)
- All API calls use `apiClient` from `@/api/client.ts` (axios instance with auto-refresh)
- Use `sonner` toast for notifications (already in project)
- Use TanStack Query for server state management
- Backend changes go in `Flower.Backend/`, frontend changes in `flower-admin.frontend/src/`
- No Entity/DbContext changes needed (all APIs already use existing data)

---
## File Structure

### Task 1 — Backend: Add Cloudinary API Endpoint
- **Modify:** `Flower.Backend/Controllers/Api/SettingsApiController.cs`

### Task 2 — Post Category Management
- **Create:** `flower-admin.frontend/src/types/postCategory.ts`
- **Create:** `flower-admin.frontend/src/api/postCategories.ts`
- **Create:** `flower-admin.frontend/src/pages/content/PostCategoriesTab.tsx`
- **Modify:** `flower-admin.frontend/src/pages/ContentPage.tsx` (add tab)
- **Modify:** `flower-admin.frontend/src/components/AppSidebar.tsx` (add nav item)

### Task 3 — Order Create Page
- **Create:** `flower-admin.frontend/src/pages/orders/OrderCreatePage.tsx`
- **Modify:** `flower-admin.frontend/src/api/orders.ts` (add `create`)
- **Modify:** `flower-admin.frontend/src/types/order.ts` (add `CreateOrderRequest`)
- **Modify:** `flower-admin.frontend/src/App.tsx` (add route)

### Task 4 — OrderDetail CRUD
- **Create:** `flower-admin.frontend/src/api/orderDetails.ts`
- **Modify:** `flower-admin.frontend/src/pages/orders/OrderDetailPage.tsx` (add edit inline)

### Task 5 — Cloudinary Settings Tab
- **Create:** `flower-admin.frontend/src/pages/system/CloudinaryTab.tsx`
- **Modify:** `flower-admin.frontend/src/types/settings.ts` (add `CloudinarySettings`)
- **Modify:** `flower-admin.frontend/src/api/settings.ts` (add `saveCloudinary`)
- **Modify:** `flower-admin.frontend/src/pages/SystemSettingsPage.tsx` (add tab)

---

### Task 1: Backend — Add Cloudinary Save Endpoint

**Files:**
- Modify: `Flower.Backend/Controllers/Api/SettingsApiController.cs`

**Interfaces:**
- Consumes: `ISystemSettingService.SaveSetting("Cloudinary", dto, username)` — already exists
- Produces: `PUT /api/settings/cloudinary` endpoint

- [ ] **Step 1: Add SaveCloudinary endpoint to SettingsApiController**

Add before the closing brace of `SettingsApiController`:

```csharp
[Authorize(Policy = "StaffOnly")]
[HttpPut("cloudinary")]
public async Task<IActionResult> SaveCloudinary([FromBody] CloudinarySettings dto)
{
    var username = User.Identity?.Name ?? "System";
    await _settingService.SaveSetting("Cloudinary", dto, username);
    return NoContent();
}
```

- [ ] **Step 2: Build and verify**

```bash
cd Flower.Backend && dotnet build
```
Expected: Build succeeded, 0 errors.

---

### Task 2: Post Category Management

**Files:**
- Create: `flower-admin.frontend/src/types/postCategory.ts`
- Create: `flower-admin.frontend/src/api/postCategories.ts`
- Create: `flower-admin.frontend/src/pages/content/PostCategoriesTab.tsx`
- Modify: `flower-admin.frontend/src/pages/ContentPage.tsx`
- Modify: `flower-admin.frontend/src/components/AppSidebar.tsx`

**Interfaces:**
- Consumes: `GET /api/categories`, `POST /api/categories`, `PUT /api/categories/{id}`, `DELETE /api/categories/{id}`
- Produces: `PostCategoriesTab` component with list/create/edit/delete

- [ ] **Step 1: Create types/postCategory.ts**

```typescript
export interface PostCategory {
  id: number
  name: string
  description?: string
  slug?: string
}

export interface CreatePostCategoryRequest {
  name: string
  description?: string
  slug?: string
}

export interface UpdatePostCategoryRequest extends CreatePostCategoryRequest {
  id: number
}
```

- [ ] **Step 2: Create api/postCategories.ts**

```typescript
import { apiClient } from './client'
import type { PostCategory, CreatePostCategoryRequest, UpdatePostCategoryRequest } from '@/types/postCategory'

export const postCategoriesApi = {
  getAll() {
    return apiClient.get<PostCategory[]>('/api/categories')
  },
  getById(id: number) {
    return apiClient.get<PostCategory>(`/api/categories/${id}`)
  },
  create(data: CreatePostCategoryRequest) {
    return apiClient.post<PostCategory>('/api/categories', data)
  },
  update(id: number, data: UpdatePostCategoryRequest) {
    return apiClient.put(`/api/categories/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/categories/${id}`)
  },
}
```

- [ ] **Step 3: Create pages/content/PostCategoriesTab.tsx**

```typescript
import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { postCategoriesApi } from '@/api/postCategories'
import { Button } from '@/components/ui/button'
import {
  Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger,
  DialogFooter, DialogClose,
} from '@/components/ui/dialog'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2, AlertCircle, Plus, Pencil, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import type { PostCategory } from '@/types/postCategory'

export function PostCategoriesTab() {
  const [editItem, setEditItem] = useState<PostCategory | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const queryClient = useQueryClient()

  const { data: categories, isLoading, error } = useQuery({
    queryKey: ['post-categories'],
    queryFn: () => postCategoriesApi.getAll().then((r) => r.data),
  })

  const createMutation = useMutation({
    mutationFn: (dto: CreatePostCategoryRequest) => postCategoriesApi.create(dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); setDialogOpen(false); toast.success('Đã thêm danh mục') },
    onError: () => toast.error('Không thể thêm danh mục'),
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, dto }: { id: number; dto: UpdatePostCategoryRequest }) => postCategoriesApi.update(id, dto),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); setDialogOpen(false); setEditItem(null); toast.success('Đã cập nhật danh mục') },
    onError: () => toast.error('Không thể cập nhật danh mục'),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => postCategoriesApi.delete(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['post-categories'] }); toast.success('Đã xóa danh mục') },
    onError: () => toast.error('Không thể xóa danh mục'),
  })

  const openCreate = () => { setEditItem(null); setDialogOpen(true) }
  const openEdit = (item: PostCategory) => { setEditItem(item); setDialogOpen(true) }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const form = e.currentTarget
    const formData = new FormData(form)
    const name = formData.get('name') as string
    if (!name) return

    const payload = {
      name,
      description: formData.get('description') as string || undefined,
      slug: formData.get('slug') as string || undefined,
    }

    if (editItem) {
      updateMutation.mutate({ id: editItem.id, dto: { ...payload, id: editItem.id } })
    } else {
      createMutation.mutate(payload)
    }
  }

  if (isLoading) return <div className="flex h-48 items-center justify-center"><Loader2 className="size-6 animate-spin" /></div>
  if (error) return <div className="flex h-48 items-center justify-center text-destructive gap-2"><AlertCircle className="size-5" /><p>Không thể tải danh mục bài viết</p></div>

  return (
    <div className="space-y-4">
      <div className="flex justify-end">
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogTrigger render={<Button size="sm" />} onClick={openCreate}>
            <Plus className="mr-1 size-4" />Thêm danh mục
          </DialogTrigger>
          <DialogContent>
            <DialogHeader><DialogTitle>{editItem ? 'Sửa danh mục' : 'Thêm danh mục'}</DialogTitle></DialogHeader>
            <form onSubmit={handleSubmit} className="space-y-4">
              <input name="name" defaultValue={editItem?.name ?? ''} placeholder="Tên danh mục" required className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <input name="slug" defaultValue={editItem?.slug ?? ''} placeholder="Slug (tùy chọn)" className="flex h-9 w-full rounded-md border bg-background px-3 py-1 text-sm" />
              <textarea name="description" defaultValue={editItem?.description ?? ''} placeholder="Mô tả (tùy chọn)" className="flex min-h-20 w-full rounded-md border bg-background px-3 py-2 text-sm" />
              <DialogFooter>
                <DialogClose render={<Button variant="outline" type="button" />}>Hủy</DialogClose>
                <Button type="submit">{editItem ? 'Cập nhật' : 'Thêm'}</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </div>

      <Card>
        <CardContent className="p-0">
          {categories && categories.length > 0 ? (
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b text-left text-muted-foreground">
                  <th className="px-4 py-3 font-medium">Tên</th>
                  <th className="px-4 py-3 font-medium">Slug</th>
                  <th className="px-4 py-3 font-medium">Mô tả</th>
                  <th className="px-4 py-3 font-medium text-right">Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {categories.map((item) => (
                  <tr key={item.id} className="border-b last:border-0">
                    <td className="px-4 py-3 font-medium">{item.name}</td>
                    <td className="px-4 py-3 text-muted-foreground font-mono text-xs">{item.slug || '—'}</td>
                    <td className="px-4 py-3 text-muted-foreground max-w-xs truncate">{item.description || '—'}</td>
                    <td className="px-4 py-3 text-right">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(item)}><Pencil className="size-4" /></Button>
                      <Button variant="ghost" size="icon" onClick={() => { if (confirm('Xóa danh mục này?')) deleteMutation.mutate(item.id) }}><Trash2 className="size-4 text-destructive" /></Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="flex h-32 items-center justify-center text-muted-foreground">Chưa có danh mục nào</div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
```

- [ ] **Step 4: Add tab to ContentPage.tsx**

Add import:
```typescript
import { PostCategoriesTab } from './content/PostCategoriesTab'
```

Add tab entry:
```typescript
const tabs = [
  { key: 'banners', label: 'Banner' },
  { key: 'posts', label: 'Bài viết' },
  { key: 'pages', label: 'Trang tĩnh' },
  { key: 'layout', label: 'Giao diện' },
  { key: 'categories', label: 'Danh mục bài viết' },
]
```

Add render condition:
```typescript
{activeTab === 'categories' && <PostCategoriesTab />}
```

- [ ] **Step 5: Add sidebar nav item for Post Categories under Content**

Nothing needed — it's a tab inside ContentPage, so the existing "Nội dung" nav item covers it.

---

### Task 3: Order Create Page

**Files:**
- Create: `flower-admin.frontend/src/pages/orders/OrderCreatePage.tsx`
- Modify: `flower-admin.frontend/src/api/orders.ts`
- Modify: `flower-admin.frontend/src/types/order.ts`
- Modify: `flower-admin.frontend/src/App.tsx`

**Interfaces:**
- Consumes: `POST /api/orders` with `OrderInputDTO`, `GET /api/customers`, `GET /api/Products`, `GET /api/settings/checkout`
- Produces: Order create form page at `/orders/new`

- [ ] **Step 1: Add createOrder request type to types/order.ts**

Add before the `OrderDetailDTO` interface:
```typescript
export interface CreateOrderItemRequest {
  productId: number
  quantity: number
  unitPrice: number
  sizeVariant?: string
}

export interface CreateOrderRequest {
  customerId: number
  notes?: string
  items: CreateOrderItemRequest[]
  paymentMethod: PaymentMethod
  deliveryDate?: string
  deliveryTimeSlot?: string
  deliveryDistrict?: string
  deliveryAddress?: string
  recipientName?: string
  recipientPhone?: string
  couponCode?: string
}
```

- [ ] **Step 2: Add create method to api/orders.ts**

```typescript
  create(data: CreateOrderRequest) {
    return apiClient.post<{ message: string; orderId: number }>('/api/orders', data)
  },
```

Also add the import:
```typescript
import type { OrderDTO, OrderStatus, CreateOrderRequest } from '@/types/order'
```

- [ ] **Step 3: Create pages/orders/OrderCreatePage.tsx**

Full page with:
- Customer search/select (search customer by name/phone from existing API)
- Product line items table (add products, set quantity, unit price)
- Delivery info (date, time slot, district, address, recipient)
- Payment method (COD / Online)
- Notes, coupon code
- Submit button → calls `POST /api/orders` → navigates to order detail

```typescript
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
import { Loader2, Plus, Trash2, Search, ArrowLeft } from 'lucide-react'
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
    queryFn: () => productsApi.search(productSearch),
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
                <Select value={String(paymentMethod)} onValueChange={(v) => setPaymentMethod(Number(v))}>
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
```

- [ ] **Step 4: Add route to App.tsx**

Add import:
```typescript
import { OrderCreatePage } from '@/pages/orders/OrderCreatePage'
```

Add route after `/orders`:
```typescript
<Route path="orders/new" element={<OrderCreatePage />} />
```

---

### Task 4: OrderDetail CRUD

**Files:**
- Create: `flower-admin.frontend/src/api/orderDetails.ts`
- Modify: `flower-admin.frontend/src/pages/orders/OrderDetailPage.tsx`

**Interfaces:**
- Consumes: `POST /api/orderdetails`, `PUT /api/orderdetails/{id}`, `DELETE /api/orderdetails/{id}`
- Produces: Edit/Delete buttons on order detail line items

- [ ] **Step 1: Create api/orderDetails.ts**

```typescript
import { apiClient } from './client'
import type { OrderDetailDTO } from '@/types/order'

export const orderDetailsApi = {
  getAll() {
    return apiClient.get<OrderDetailDTO[]>('/api/orderdetails')
  },
  getByOrderId(orderId: number) {
    return apiClient.get<OrderDetailDTO[]>(`/api/orderdetails/order/${orderId}`)
  },
  getById(id: number) {
    return apiClient.get<OrderDetailDTO>(`/api/orderdetails/${id}`)
  },
  create(dto: Omit<OrderDetailDTO, 'id' | 'productName' | 'productImageUrl' | 'subtotal' | 'originalPrice' | 'discountAmount'>) {
    return apiClient.post<OrderDetailDTO>('/api/orderdetails', dto)
  },
  update(id: number, dto: OrderDetailDTO) {
    return apiClient.put(`/api/orderdetails/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/orderdetails/${id}`)
  },
}
```

- [ ] **Step 2: Add inline edit/delete to OrderDetailPage.tsx**

In `OrderDetailPage.tsx`, add the following:

Add import:
```typescript
import { orderDetailsApi } from '@/api/orderDetails'
```

Add mutation after existing mutations (around line 80):
```typescript
  const deleteDetailMutation = useMutation({
    mutationFn: (detailId: number) => orderDetailsApi.delete(detailId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['order', orderId] })
      toast.success('Đã xóa sản phẩm khỏi đơn hàng')
    },
    onError: () => toast.error('Không thể xóa sản phẩm'),
  })
```

Add edit state:
```typescript
  const [editingDetail, setEditingDetail] = useState<OrderDetailDTO | null>(null)
```

Add edit/delete buttons to the product table (inside the `TableRow` map, around line 230, after `detail.subtotal` cell):
```typescript
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-1 print-hidden">
                      {order.status === OrderStatus.PendingVerification && (
                        <Button variant="ghost" size="icon" onClick={() => deleteDetailMutation.mutate(detail.id)}>
                          <Trash2 className="size-4 text-destructive" />
                        </Button>
                      )}
                    </div>
                  </TableCell>
```

Also add `Trash2` to the import from lucide-react if not already.

---

### Task 5: Cloudinary Settings Tab

**Files:**
- Create: `flower-admin.frontend/src/pages/system/CloudinaryTab.tsx`
- Modify: `flower-admin.frontend/src/types/settings.ts`
- Modify: `flower-admin.frontend/src/api/settings.ts`
- Modify: `flower-admin.frontend/src/pages/SystemSettingsPage.tsx`

**Interfaces:**
- Consumes: `GET /api/settings` (includes Cloudinary via `AllSystemSettings`), `PUT /api/settings/cloudinary`
- Produces: Cloudinary settings tab component

- [ ] **Step 1: Add CloudinarySettings to types/settings.ts**

```typescript
export interface CloudinarySettings {
  cloudName: string
  apiKey: string
  apiSecret: string
  folder: string
}
```

Also update `AllSystemSettings`:
```typescript
export interface AllSystemSettings {
  store: StoreInfoSettings
  smtp: SmtpSettings
  vnPay: VNPaySettings
  shipping: ShippingSettings
  order: OrderSettings
  cloudinary: CloudinarySettings
}
```

- [ ] **Step 2: Add saveCloudinary to api/settings.ts**

Add import:
```typescript
import type { ..., CloudinarySettings } from '@/types/settings'
```

Add method:
```typescript
  saveCloudinary(dto: CloudinarySettings) {
    return apiClient.put('/api/settings/cloudinary', dto)
  },
```

- [ ] **Step 3: Create pages/system/CloudinaryTab.tsx**

```typescript
import { useState, useEffect } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent } from '@/components/ui/card'
import { Loader2 } from 'lucide-react'
import { toast } from 'sonner'
import type { CloudinarySettings } from '@/types/settings'

interface Props {
  data: CloudinarySettings | undefined
}

export function CloudinaryTab({ data }: Props) {
  const queryClient = useQueryClient()
  const [form, setForm] = useState<CloudinarySettings>({
    cloudName: '', apiKey: '', apiSecret: '', folder: 'flowershop_products',
  })

  useEffect(() => {
    if (data) setForm(data)
  }, [data])

  const mutation = useMutation({
    mutationFn: (dto: CloudinarySettings) => settingsApi.saveCloudinary(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['settings'] })
      toast.success('Đã lưu cấu hình Cloudinary')
    },
    onError: () => toast.error('Không thể lưu cấu hình Cloudinary'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    mutation.mutate(form)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <Card>
        <CardContent className="space-y-4 pt-6">
          <div className="space-y-2">
            <Label>Cloud Name</Label>
            <Input value={form.cloudName} onChange={(e) => setForm({ ...form, cloudName: e.target.value })} required placeholder="your-cloud-name" />
          </div>
          <div className="space-y-2">
            <Label>API Key</Label>
            <Input value={form.apiKey} onChange={(e) => setForm({ ...form, apiKey: e.target.value })} required placeholder="123456789012345" />
          </div>
          <div className="space-y-2">
            <Label>API Secret</Label>
            <Input type="password" value={form.apiSecret} onChange={(e) => setForm({ ...form, apiSecret: e.target.value })} required />
          </div>
          <div className="space-y-2">
            <Label>Upload Folder</Label>
            <Input value={form.folder} onChange={(e) => setForm({ ...form, folder: e.target.value })} placeholder="flowershop_products" />
            <p className="text-xs text-muted-foreground">Thư mục lưu ảnh trên Cloudinary</p>
          </div>
        </CardContent>
      </Card>
      <div className="flex justify-end">
        <Button type="submit" disabled={mutation.isPending}>
          {mutation.isPending ? <><Loader2 className="mr-2 size-4 animate-spin" />Đang lưu...</> : 'Lưu cấu hình'}
        </Button>
      </div>
    </form>
  )
}
```

- [ ] **Step 4: Add tab to SystemSettingsPage.tsx**

Add import:
```typescript
import { CloudinaryTab } from './system/CloudinaryTab'
```

Add tab entry:
```typescript
const tabs = [
  { key: 'store', label: 'Cửa hàng' },
  { key: 'smtp', label: 'SMTP' },
  { key: 'vnpay', label: 'VNPay' },
  { key: 'shipping', label: 'Vận chuyển' },
  { key: 'order', label: 'Đơn hàng' },
  { key: 'cloudinary', label: 'Cloudinary' },
]
```

Update the component to fetch settings data and pass to tabs. Since `SystemSettingsPage.tsx` currently doesn't fetch settings, add a query and pass data to `CloudinaryTab`:

```typescript
import { useQuery } from '@tanstack/react-query'
import { settingsApi } from '@/api/settings'

// Inside component:
const { data: settings } = useQuery({
  queryKey: ['settings'],
  queryFn: () => settingsApi.getAll().then((r) => r.data),
})
```

Add render condition:
```typescript
{activeTab === 'cloudinary' && <CloudinaryTab data={settings?.cloudinary} />}
```
