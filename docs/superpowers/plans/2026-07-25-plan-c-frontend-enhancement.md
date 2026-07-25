# Plan C: Frontend Enhancement Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Fix coupon service dead code, add discount filter/sort, create Flash Sale landing page.

**Architecture:** Three independent changes — one services fix, one shop filter enhancement, one new page.

**Tech Stack:** React 18, TypeScript, Tailwind CSS, React Router v6

## Global Constraints

- Use existing services, types, and hooks
- Vietnamese labels in UI
- Follow existing patterns (file naming, exports, component structure)
- No new npm packages

---

### Task 1: Fix couponService Dead Code in Checkout

**Files:**
- Modify: `Flower-shop.frontend/src/pages/checkout/index.tsx`

**Interfaces:**
- Consumes: `couponService` from `../../services/couponService`, specifically `couponService.apply()`

- [ ] **Step 1: Add import for couponService**

At top of file, add after line 11 (`import axiosClient...`):

```tsx
import couponService from '../../services/couponService';
```

- [ ] **Step 2: Replace axiosClient call with couponService.apply()**

Replace lines 168-172:

From:
```tsx
      const res: any = await axiosClient.post('/Promotions/apply', {
        code: couponCode.trim(),
        customerId: user?.id || 0,
        orderTotal: subtotal
      });
```

To:
```tsx
      const res: any = await couponService.apply({
        code: couponCode.trim(),
        customerId: user?.id || 0,
        orderTotal: subtotal
      });
```

- [ ] **Step 3: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors. (The `axiosClient` import is still used elsewhere in the file, so don't remove it.)

---

### Task 2: Add Shop Discount Filter and Sort

**Files:**
- Modify: `Flower-shop.frontend/src/pages/shop/ShopSidebar.tsx`
- Modify: `Flower-shop.frontend/src/pages/shop/ShopHeader.tsx`

**Interfaces:**
- ShopSidebar: Add `onPromotionFilterChange` prop, `activePromotionOnly` prop
- ShopHeader: Add `"discount_desc"` sort option

- [ ] **Step 1: Update ShopSidebar props and add checkbox**

Update the interface and add the filter checkbox. Add after the price range section (after line 132 `</div>`) and before the closing `</div>` (line 133):

```tsx
// Inside the component, before return
interface ShopSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeCategoryId: number | null;
  onPriceChange: (min: number | null, max: number | null) => void;
  activePricePreset: string | null;
  setActivePricePreset: (preset: string | null) => void;
  // Add these two:
  onPromotionFilterChange: (value: boolean) => void;
  activePromotionOnly: boolean;
}
```

Find the `const ShopSidebar = ({` destructuring and update to include the new props.

Add the promotion filter section after the price range div (after line 132, before the closing `</div>` on line 133):

```tsx
<div className="flex flex-col gap-stack-sm">
  <h3 className="font-label-md text-label-md text-on-surface uppercase tracking-widest">Khuyến mãi</h3>
  <label className="flex items-center gap-3 cursor-pointer group">
    <input
      type="checkbox"
      checked={activePromotionOnly}
      onChange={(e) => onPromotionFilterChange(e.target.checked)}
      className="border-outline-variant text-primary focus:ring-primary/20 w-4 h-4 rounded transition-colors"
    />
    <span className={`font-body-md text-body-md transition-colors ${activePromotionOnly ? 'text-primary font-semibold' : 'text-on-surface-variant group-hover:text-primary'}`}>
      Đang khuyến mãi
    </span>
  </label>
</div>
```

- [ ] **Step 2: Update ShopHeader sort options**

Add new option to the `<select>` (between line 28 and 29, after `Hàng mới`):

```tsx
<option value="discount_desc">Giảm nhiều nhất</option>
```

- [ ] **Step 3: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.

---

### Task 3: Create Flash Sale Landing Page

**Files:**
- Create: `Flower-shop.frontend/src/pages/flash-sale/index.tsx`
- Modify: `Flower-shop.frontend/src/App.tsx`

**Interfaces:**
- Consumes: `GET /api/FlashSales/active` (already exists in `FlashSalesController.cs`)
- Consumes: `CountdownTimer` component (Plan B Task 2)
- Consumes: `ProductCard` component

- [ ] **Step 1: Create flash sale page**

```tsx
import React, { useEffect, useState } from 'react';
import axiosClient from '../../api/axiosClient';
import ProductCard from '../../components/ProductCard';
import CountdownTimer from '../../components/CountdownTimer';
import SEO from '../../components/SEO';
import type { Product } from '../../types/product';

interface FlashSaleItem {
  productId: number;
  productName: string;
  productImageUrl: string;
  originalPrice: number;
  salePrice: number;
  discountPercent: number;
  promotionName: string;
  promotionEndTime: string;
}

const FlashSalePage: React.FC = () => {
  const [items, setItems] = useState<FlashSaleItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    axiosClient.get('/api/FlashSales/active')
      .then((res: any) => {
        setItems(Array.isArray(res) ? res : res.data ?? []);
        setLoading(false);
      })
      .catch(() => {
        setError('Không thể tải danh sách Flash Sale.');
        setLoading(false);
      });
  }, []);

  // Derive the earliest end time across all items for the global countdown
  const globalEndTime = items.length > 0
    ? items.reduce((earliest, item) =>
        item.promotionEndTime < earliest ? item.promotionEndTime : earliest,
        items[0].promotionEndTime
      )
    : null;

  // Convert FlashSaleItem to Product shape for ProductCard
  const products: Product[] = items.map(item => ({
    id: item.productId,
    name: item.productName,
    price: item.originalPrice,
    imageUrl: item.productImageUrl,
    promotionPrice: item.salePrice,
    promotionPercent: item.discountPercent,
    hasFlashSale: true,
    isFlashSale: true,
    promotionEndTime: item.promotionEndTime,
    stockQuantity: 999,
    description: '',
    discountPrice: undefined,
    currentPrice: undefined,
    discountPercent: undefined,
    slug: '',
    sku: '',
    categoryId: 0,
    categoryName: '',
    weight: 0,
    length: 0,
    width: 0,
    height: 0,
    isActive: true,
    isFeatured: false,
    isNew: false,
    sizePrice: undefined,
    trendingBadge: undefined,
    promotionType: 'FlashSale',
    promotionName: item.promotionName,
    createdAt: '',
    updatedAt: '',
  }));

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Flash Sale" description="Các chương trình Flash Sale hot nhất" />
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        {/* Hero Section */}
        <div className="relative bg-gradient-to-r from-red-600 to-red-500 rounded-2xl p-8 md:p-12 mb-stack-lg overflow-hidden">
          <div className="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/2" />
          <div className="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/2" />
          <div className="relative z-10">
            <div className="flex items-center gap-3 mb-4">
              <span className="material-symbols-outlined text-4xl text-yellow-300">bolt</span>
              <h1 className="font-display-lg text-display-lg text-white">Flash Sale</h1>
            </div>
            <p className="font-body-lg text-body-lg text-white/80 max-w-xl mb-6">
              Săn hoa đẹp giá tốt — số lượng có hạn, nhanh tay bạn nhé!
            </p>
            {globalEndTime && (
              <div className="inline-flex items-center gap-2 bg-white/20 backdrop-blur rounded-lg px-4 py-3">
                <span className="text-white/80 font-label-sm uppercase tracking-wider">Kết thúc trong:</span>
                <CountdownTimer endTime={globalEndTime} className="text-white text-lg" />
              </div>
            )}
          </div>
        </div>

        {/* Content */}
        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin inline-block w-8 h-8 border-2 border-primary border-t-transparent rounded-full" />
            <p className="mt-4 text-on-surface-variant">Đang tải...</p>
          </div>
        ) : error ? (
          <div className="text-center py-12">
            <span className="material-symbols-outlined text-4xl text-error mb-2 block">error</span>
            <p className="text-on-surface-variant">{error}</p>
          </div>
        ) : items.length === 0 ? (
          <div className="text-center py-12">
            <span className="material-symbols-outlined text-4xl text-outline mb-2 block">local_fire_department</span>
            <h2 className="font-headline-sm text-headline-sm text-on-surface mb-2">Hiện không có Flash Sale nào</h2>
            <p className="text-on-surface-variant">Theo dõi để không bỏ lỡ chương trình tiếp theo!</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-gutter">
            {products.map(product => (
              <ProductCard key={product.id} item={product} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
};

export default FlashSalePage;
```

- [ ] **Step 2: Add route to App.tsx**

In `Flower-shop.frontend/src/App.tsx`, add import at top:

```tsx
import FlashSale from './pages/flash-sale';
```

Add route after line 121 (wishlist route):

```tsx
<Route path="/flash-sale" element={<FlashSale />} />
```

- [ ] **Step 3: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.
