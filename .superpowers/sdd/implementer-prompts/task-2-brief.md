# Task 2: Frontend — Types + API Modules

## Global Constraints
- All UI text in Vietnamese (for the types, just English identifiers is fine — API modules use English function names).
- Follow existing patterns: files export an object with methods (e.g. `export const advertisementsApi = { ... }`).
- PaginatedResponse<T> already exists at `@/types/api.ts`.
- apiClient already exists at `@/api/client.ts`.

**Files to Create (in `flower-admin.frontend/src/`):**
- `src/types/advertisement.ts`
- `src/types/page.ts`
- `src/types/post.ts`
- `src/types/layout.ts`
- `src/types/settings.ts`
- `src/types/promotion.ts`
- `src/types/coupon.ts`
- `src/api/advertisements.ts`
- `src/api/pages.ts`
- `src/api/layout.ts`
- `src/api/settings.ts`
- `src/api/posts.ts`
- `src/api/promotions.ts`
- `src/api/coupons.ts`

## Step 1: Create `src/types/advertisement.ts`

```typescript
export interface AdvertisementDTO {
  id: number
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive: boolean
  createdAt: string
}

export interface CreateAdvertisementDTO {
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive?: boolean
}

export interface UpdateAdvertisementDTO {
  id: number
  title: string
  subtitle?: string
  imageUrl?: string
  linkUrl?: string
  sortOrder: number
  isActive?: boolean
}
```

## Step 2: Create `src/types/page.ts`

```typescript
export interface PageDTO {
  id: number
  title: string
  slug?: string
  content: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreatePageDTO {
  title: string
  slug?: string
  content: string
  isActive?: boolean
}

export interface UpdatePageDTO {
  id: number
  title: string
  slug?: string
  content: string
  isActive?: boolean
}
```

## Step 3: Create `src/types/post.ts`

```typescript
export interface PostDTO {
  id: number
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl: string
  createdDate: string
  categoryId: number
  categoryName?: string
}

export interface CreatePostDTO {
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl?: string
  categoryId: number
}

export interface UpdatePostDTO {
  id: number
  title: string
  content: string
  summary?: string
  slug?: string
  imageUrl?: string
  categoryId: number
}
```

## Step 4: Create `src/types/layout.ts`

```typescript
export interface TopBarDTO {
  isActive: boolean
  text?: string
  url?: string
}

export interface ZonesDTO {
  left: string[]
  center: string[]
  right: string[]
}

export interface CtaButtonDTO {
  isActive: boolean
  text?: string
  url?: string
  variant?: string
}

export interface HotlineConfigDTO {
  useDefault: boolean
  customText?: string
}

export interface SearchConfigDTO {
  mode: string
}

export interface MenuItemDTO {
  id: string
  label: string
  url: string
  isExternal?: boolean
  children?: MenuItemDTO[]
}

export interface FooterLinkDTO {
  id: string
  label: string
  type: string
  pageId?: number
  url?: string
}

export interface FooterColumnDTO {
  title: string
  align: string
  sortOrder: number
  type: string
  isActive: boolean
  links: FooterLinkDTO[]
}

export interface HeaderLayoutDTO {
  topBar: TopBarDTO
  zones: ZonesDTO
  ctaButton: CtaButtonDTO
  hotline: HotlineConfigDTO
  search: SearchConfigDTO
  menuItems: MenuItemDTO[]
}

export interface LayoutResponse {
  header: HeaderLayoutDTO
  footer: FooterColumnDTO[]
  storeInfo: StoreInfoSettings
}

import type { StoreInfoSettings } from './settings'
export type { StoreInfoSettings }
```

## Step 5: Create `src/types/settings.ts`

```typescript
export interface StoreInfoSettings {
  storeName: string
  logo: string
  hotline: string
  email: string
  address: string
  facebook?: string
  zalo?: string
  openHours?: string
  googleMapsEmbedUrl?: string
}

export interface SmtpSettings {
  host: string
  port: number
  username: string
  password: string
  senderName: string
  senderEmail: string
}

export interface VNPaySettings {
  tmnCode: string
  hashSecret: string
  returnUrl: string
  isSandbox: boolean
  enablePayment: boolean
}

export interface ShippingSettings {
  defaultFee: number
  freeShipFrom: number
  maxDistance: number
  deliveryTime: string
}

export interface OrderSettings {
  autoCancelMinutes: number
  enableCOD: boolean
  enableOnlinePayment: boolean
}

export interface AllSystemSettings {
  store: StoreInfoSettings
  smtp: SmtpSettings
  vnPay: VNPaySettings
  shipping: ShippingSettings
  order: OrderSettings
}
```

## Step 6: Create `src/types/promotion.ts`

```typescript
export type PromotionType = 'Automatic' | 'Manual'
export type DiscountType = 'Percentage' | 'FixedAmount'

export interface PromotionCampaignDTO {
  id: number
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
  productIds?: number[]
}

export interface CreatePromotionCampaignDTO {
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive?: boolean
  productIds?: number[]
}

export interface UpdatePromotionCampaignDTO {
  id: number
  name: string
  description?: string
  promotionType: PromotionType
  discountType: DiscountType
  discountValue: number
  startDate: string
  endDate: string
  priority: number
  bannerImage?: string
  isStackable: boolean
  isActive?: boolean
  productIds?: number[]
}
```

## Step 7: Create `src/types/coupon.ts`

```typescript
export type DiscountType = 'Percentage' | 'FixedAmount'

export interface CouponDTO {
  id: number
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usedCount: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic: boolean
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

export interface CreateCouponDTO {
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic?: boolean
  isActive?: boolean
}

export interface UpdateCouponDTO {
  id: number
  code: string
  description?: string
  discountType: DiscountType
  discountValue: number
  minimumOrderAmount?: number
  maximumDiscountAmount?: number
  usageLimit?: number
  usagePerCustomer?: number
  customerId?: number
  startDate?: string
  endDate?: string
  isPublic?: boolean
  isActive?: boolean
}

export interface CouponUsageDTO {
  id: number
  couponId: number
  customerId: number
  orderId: number
  discountAmount: number
  usedAt: string
  couponCode?: string
  customerName?: string
}
```

## Step 8: Create `src/api/advertisements.ts`

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { AdvertisementDTO, CreateAdvertisementDTO, UpdateAdvertisementDTO } from '@/types/advertisement'

export const advertisementsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<AdvertisementDTO>>('/api/advertisements/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<AdvertisementDTO>(`/api/advertisements/${id}`)
  },
  create(dto: CreateAdvertisementDTO) {
    return apiClient.post<AdvertisementDTO>('/api/advertisements', dto)
  },
  update(id: number, dto: UpdateAdvertisementDTO) {
    return apiClient.put(`/api/advertisements/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/advertisements/${id}`)
  },
}
```

## Step 9: Create `src/api/pages.ts`

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PageDTO, CreatePageDTO, UpdatePageDTO } from '@/types/page'

export const pagesApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<PageDTO>>('/api/pages/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<PageDTO>(`/api/pages/${id}`)
  },
  getBySlug(slug: string) {
    return apiClient.get<PageDTO>(`/api/pages/slug/${slug}`)
  },
  create(dto: CreatePageDTO) {
    return apiClient.post<PageDTO>('/api/pages', dto)
  },
  update(id: number, dto: UpdatePageDTO) {
    return apiClient.put(`/api/pages/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/pages/${id}`)
  },
}
```

## Step 10: Create `src/api/layout.ts`

```typescript
import { apiClient } from './client'
import type { HeaderLayoutDTO, FooterColumnDTO, LayoutResponse } from '@/types/layout'

export const layoutApi = {
  getLayout() {
    return apiClient.get<LayoutResponse>('/api/layout')
  },
  saveHeader(dto: HeaderLayoutDTO) {
    return apiClient.put('/api/layout/header', dto)
  },
  saveFooter(dto: FooterColumnDTO[]) {
    return apiClient.put('/api/layout/footer', dto)
  },
}
```

## Step 11: Create `src/api/settings.ts`

```typescript
import { apiClient } from './client'
import type { AllSystemSettings, StoreInfoSettings, SmtpSettings, VNPaySettings, ShippingSettings, OrderSettings } from '@/types/settings'

export const settingsApi = {
  getAll() {
    return apiClient.get<AllSystemSettings>('/api/settings')
  },
  saveStoreInfo(dto: StoreInfoSettings) {
    return apiClient.put('/api/settings/store-info', dto)
  },
  saveSmtp(dto: SmtpSettings) {
    return apiClient.put('/api/settings/smtp', dto)
  },
  saveVnPay(dto: VNPaySettings) {
    return apiClient.put('/api/settings/vnpay', dto)
  },
  saveShipping(dto: ShippingSettings) {
    return apiClient.put('/api/settings/shipping', dto)
  },
  saveOrder(dto: OrderSettings) {
    return apiClient.put('/api/settings/order', dto)
  },
}
```

## Step 12: Create `src/api/posts.ts`

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PostDTO, CreatePostDTO, UpdatePostDTO } from '@/types/post'

export interface PostsPagedParams {
  page?: number
  pageSize?: number
  search?: string
}

export const postsApi = {
  getPaged(params: PostsPagedParams = {}) {
    return apiClient.get<PaginatedResponse<PostDTO>>('/api/posts/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<PostDTO>(`/api/posts/${id}`)
  },
  create(dto: CreatePostDTO) {
    return apiClient.post<PostDTO>('/api/posts', dto)
  },
  update(id: number, dto: UpdatePostDTO) {
    return apiClient.put(`/api/posts/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/posts/${id}`)
  },
}
```

## Step 13: Create `src/api/promotions.ts`

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PromotionCampaignDTO } from '@/types/promotion'
import type { CreatePromotionCampaignDTO, UpdatePromotionCampaignDTO } from '@/types/promotion'

export const promotionsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<PromotionCampaignDTO>>('/api/promotions/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<PromotionCampaignDTO>(`/api/promotions/${id}`)
  },
  create(dto: CreatePromotionCampaignDTO) {
    return apiClient.post<PromotionCampaignDTO>('/api/promotions', dto)
  },
  update(id: number, dto: UpdatePromotionCampaignDTO) {
    return apiClient.put(`/api/promotions/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/promotions/${id}`)
  },
  enable(id: number) {
    return apiClient.patch(`/api/promotions/${id}/enable`)
  },
  disable(id: number) {
    return apiClient.patch(`/api/promotions/${id}/disable`)
  },
  addProduct(id: number, productId: number) {
    return apiClient.post(`/api/promotions/${id}/products`, { productId })
  },
  removeProduct(id: number, productId: number) {
    return apiClient.delete(`/api/promotions/${id}/products/${productId}`)
  },
}
```

## Step 14: Create `src/api/coupons.ts`

```typescript
import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { CouponDTO, CreateCouponDTO, UpdateCouponDTO, CouponUsageDTO } from '@/types/coupon'

export const couponsApi = {
  getPaged(page = 1, pageSize = 10) {
    return apiClient.get<PaginatedResponse<CouponDTO>>('/api/coupons/paged', { params: { page, pageSize } })
  },
  getById(id: number) {
    return apiClient.get<CouponDTO>(`/api/coupons/${id}`)
  },
  create(dto: CreateCouponDTO) {
    return apiClient.post<CouponDTO>('/api/coupons', dto)
  },
  update(id: number, dto: UpdateCouponDTO) {
    return apiClient.put(`/api/coupons/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/coupons/${id}`)
  },
  enable(id: number) {
    return apiClient.patch(`/api/coupons/${id}/enable`)
  },
  disable(id: number) {
    return apiClient.patch(`/api/coupons/${id}/disable`)
  },
  getUsages(id: number) {
    return apiClient.get<CouponUsageDTO[]>(`/api/coupons/${id}/usages`)
  },
}
```

## Step 15: Build and verify compilation

Go to `flower-admin.frontend/` and run `npm run build`. Expected: Build succeeds with 0 errors.

## Step 16: Commit

Stage all 14 new files and commit with message `feat(frontend): types + API modules for Phase 4`.
