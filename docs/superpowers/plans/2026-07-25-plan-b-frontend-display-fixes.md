# Plan B: Frontend Display Fixes Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Fix frontend price display across wishlist, cart, and order confirmation; add CountdownTimer component and promotion badges.

**Architecture:** Each change is localized to a single component. No new API calls needed — all required data already exists on the objects.

**Tech Stack:** React 18, TypeScript, Tailwind CSS

## Global Constraints

- Use existing `formatCurrency` from `../../utils/currency`
- Use existing `getImageUrl` from `../../utils/apiUtils`
- Follow existing component patterns (class names, spacing, design tokens)
- Vietnamese labels in UI
- No new npm packages
- All types inferred from existing `Product` / `CartItem` types

---

### Task 1: Fix Wishlist Price Display

**Files:**
- Modify: `Flower-shop.frontend/src/pages/wishlist/index.tsx:78`

**Interface:**
- Consumes: `product.promotionPrice`, `product.discountPrice`, `product.price` (all exist on Product type)

- [ ] **Step 1: Update price display to include promotionPrice**

Change line 78 from:
```tsx
<p className="text-primary font-bold">{formatCurrency(product.discountPrice || product.price)}</p>
```
to:
```tsx
<p className="text-primary font-bold">
  {formatCurrency(product.promotionPrice ?? product.discountPrice ?? product.price)}
  {(product.promotionPrice || product.discountPrice) && (
    <span className="text-on-surface-variant line-through text-xs ml-2">{formatCurrency(product.price)}</span>
  )}
</p>
```

Also add promotion badge after the image link (after line 64 `</Link>`):

```tsx
{product.promotionPrice && (
  <div className="absolute top-2 left-2 bg-primary text-on-primary px-2.5 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-lg z-10">
    KM {product.promotionPercent ? `-${product.promotionPercent}%` : ''}
  </div>
)}
```

- [ ] **Step 2: Verify TypeScript compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No type errors.

---

### Task 2: Create CountdownTimer Component

**Files:**
- Create: `Flower-shop.frontend/src/components/CountdownTimer.tsx`

**Interface:**
- Props: `endTime: string` (ISO date string)
- Produces: Reusable countdown component used by ProductCard, product detail, flash sale page

- [ ] **Step 1: Create CountdownTimer component**

```tsx
import React, { useState, useEffect } from 'react';

interface CountdownTimerProps {
  endTime: string;
  className?: string;
}

const CountdownTimer: React.FC<CountdownTimerProps> = ({ endTime, className = '' }) => {
  const [timeLeft, setTimeLeft] = useState<{ days: number; hours: number; minutes: number; seconds: number } | null>(null);
  const [expired, setExpired] = useState(false);

  useEffect(() => {
    const calculate = () => {
      const now = new Date();
      const end = new Date(endTime);
      const diff = end.getTime() - now.getTime();

      if (diff <= 0) {
        setExpired(true);
        setTimeLeft(null);
        return;
      }

      const days = Math.floor(diff / (1000 * 60 * 60 * 24));
      const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((diff % (1000 * 60)) / 1000);

      setTimeLeft({ days, hours, minutes, seconds });
      setExpired(false);
    };

    calculate();
    const interval = setInterval(calculate, 1000);
    return () => clearInterval(interval);
  }, [endTime]);

  if (expired) {
    return <span className={`text-on-surface-variant font-label-sm ${className}`}>Đã kết thúc</span>;
  }

  if (!timeLeft) {
    return <span className={`text-on-surface-variant font-label-sm ${className}`}>Đang tải...</span>;
  }

  const parts: string[] = [];
  if (timeLeft.days > 0) parts.push(`${timeLeft.days} ngày`);
  if (timeLeft.hours > 0 || timeLeft.days > 0) parts.push(`${timeLeft.hours}g`);
  parts.push(`${timeLeft.minutes}m ${timeLeft.seconds}s`);

  return (
    <span className={`text-error font-label-md font-bold ${className}`}>
      Kết thúc trong {parts.join(' ')}
    </span>
  );
};

export default CountdownTimer;
```

- [ ] **Step 2: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.

---

### Task 3: Add CountdownTimer to ProductCard

**Files:**
- Modify: `Flower-shop.frontend/src/components/ProductCard.tsx`

**Interfaces:**
- Consumes: `CountdownTimer` component (Task 2), `item.promotionEndTime` / `item.flashSaleEndTime`

- [ ] **Step 1: Determine which flash sale end time field to use**

Check what field name the API returns. Common names: `promotionEndTime`, `flashSaleEndTime`, `endTime`. Add after `const percent = ...` line:

```tsx
const flashSaleEndTime = (item as any).promotionEndTime ?? (item as any).flashSaleEndTime ?? (item as any).endTime;
```

Wait — don't add this yet. First, check what the Product type actually has for end time.

Check via grep:
```bash
cd Flower-shop.frontend && grep -rn "EndTime\|endTime\|flashSaleEnd\|promotionEnd" src/types/ --include="*.ts"
```

If no field exists, we skip adding the timer to ProductCard (no reliable data source). Instead, only add it to the Flash Sale page (Plan C) which fetches from `GET /api/FlashSales/active` that returns `promotionEndTime`.

- [ ] **Step 1: Check Product type for end time field**

```bash
grep -rn "EndTime\|endTime\|EndDate\|endDate" Flower-shop.frontend/src/types/ --include="*.ts"
```

If no end time field exists on Product type, skip this task and only add CountdownTimer to Flash Sale page (Plan C Task 3).

---

### Task 4: Add Cart Savings Breakdown

**Files:**
- Modify: `Flower-shop.frontend/src/pages/cart/index.tsx`

**Interfaces:**
- Consumes: `cartItems` and `cartTotal` from `useCart()`

- [ ] **Step 1: Add savings calculation and display**

After the title section (after line 41), before the checkout layout div:

```tsx
{(() => {
  const originalTotal = cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  const savings = originalTotal - cartTotal;
  return savings > 0 ? (
    <div className="mb-stack-lg p-4 bg-green-50 border border-green-200 rounded-lg flex items-center gap-3">
      <span className="material-symbols-outlined text-green-600">redeem</span>
      <div>
        <p className="font-label-md text-green-800">
          Bạn tiết kiệm <strong>{formatCurrency(savings)}</strong> nhờ khuyến mãi
        </p>
        <p className="text-sm text-green-600">
          Tổng gốc: {formatCurrency(originalTotal)} | Tổng sau giảm: {formatCurrency(cartTotal)}
        </p>
      </div>
    </div>
  ) : null;
})()}
```

Insert it right before line 43 (the checkout layout div opening).

- [ ] **Step 2: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.

---

### Task 5: Add Cart Promotion Badges

**Files:**
- Modify: `Flower-shop.frontend/src/pages/cart/CartTable.tsx`

**Interfaces:**
- Consumes: `CartItem` which extends `Product` and has `promotionPrice`, `discountPrice`, `hasFlashSale`, `promotionPercent`, `promotionType`

- [ ] **Step 1: Add promotion badge next to product name**

In the product info column, after the `<h3>` (line 72), add:

```tsx
{item.hasFlashSale ? (
  <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-red-100 text-red-700 rounded text-[10px] font-bold uppercase tracking-wider">
    <span className="material-symbols-outlined text-[12px]">bolt</span>
    Flash Sale
  </span>
) : item.promotionPrice ? (
  <span className="inline-flex items-center px-2 py-0.5 bg-primary/10 text-primary rounded text-[10px] font-bold uppercase tracking-wider">
    KM {item.promotionPercent ? `-${item.promotionPercent}%` : ''}
  </span>
) : null}
```

Insert this after line 72 (after `{item.name}</h3>`).

Also update the price display (line 87) to show strikethrough original price when discounted. Replace line 87:

From:
```tsx
{formatCurrency(item.promotionPrice ?? item.discountPrice ?? item.price)}
```

To:
```tsx
{item.promotionPrice || item.discountPrice ? (
  <>
    <span className="text-error font-bold">{formatCurrency(item.promotionPrice ?? item.discountPrice)}</span>
    <br /><span className="line-through text-on-surface-variant text-xs">{formatCurrency(item.price)}</span>
  </>
) : formatCurrency(item.price)}
```

- [ ] **Step 2: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.

---

### Task 6: Enhance Order Confirmation with Price Details

**Files:**
- Modify: `Flower-shop.frontend/src/pages/order-confirmation/index.tsx`

**Interfaces:**
- Consumes: `order` object from `useOrderDetail(orderId)` hook — need to check its type

- [ ] **Step 1: Check order type for price fields**

```bash
grep -n "interface\|type" Flower-shop.frontend/src/types/order.ts
```

Read the order type to understand available price fields.

- [ ] **Step 2: Add price breakdown card**

After the "Mã đơn hàng" card (around line 121) and before the description section (line 123), add:

```tsx
{order && (
  <div className="border border-outline-variant bg-surface-container-low rounded-lg p-6 max-w-md mx-auto mb-8 space-y-3 relative z-10 text-left">
    <span className="text-[10px] uppercase tracking-widest text-secondary block font-bold mb-3">Chi tiết đơn hàng</span>
    
    <div className="space-y-2">
      <div className="flex justify-between text-sm">
        <span className="text-on-surface-variant">Phương thức thanh toán</span>
        <span className="font-medium">{order.paymentMethod === 0 || order.paymentMethod === 'OnlinePayment' || order.paymentMethod === 'VNPay' ? 'VNPay' : 'COD'}</span>
      </div>
      
      {(order as any).originalAmount != null && (
        <div className="flex justify-between text-sm">
          <span className="text-on-surface-variant">Tạm tính</span>
          <span>{formatCurrency((order as any).originalAmount)}</span>
        </div>
      )}
      
      {(order as any).discountAmount > 0 && (
        <div className="flex justify-between text-sm text-green-600">
          <span>Giảm giá {(order as any).couponCode ? `(${(order as any).couponCode})` : ''}</span>
          <span>-{formatCurrency((order as any).discountAmount)}</span>
        </div>
      )}
      
      <div className="flex justify-between text-sm">
        <span className="text-on-surface-variant">Phí vận chuyển</span>
        <span className="text-primary">Miễn phí</span>
      </div>
      
      <div className="border-t border-outline-variant/30 pt-2 flex justify-between font-bold">
        <span>Tổng cộng</span>
        <span className="text-primary">{formatCurrency((order as any).finalAmount ?? order.totalAmount ?? 0)}</span>
      </div>
    </div>
  </div>
)}
```

Insert it after the order ID card (after `</div>` on line 121).

- [ ] **Step 3: Verify compilation**

```bash
cd Flower-shop.frontend && npx tsc --noEmit --pretty 2>&1 | head -20
```

Expected: No errors.
