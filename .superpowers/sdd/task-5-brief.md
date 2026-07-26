### Task 5: BestSellingProducts — horizontal scroll

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/BestSellingProducts.tsx`

- [ ] **Step 1: Rewrite BestSellingProducts with horizontal scroll**

Replace entire file content with this:

```tsx
import React from 'react';
import { Link } from 'react-router-dom';
import { useBestSellingProducts } from '../../hooks/useProducts';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';

function BestSellingProducts() {
  const { data: products = [], isLoading } = useBestSellingProducts(4);

  if (isLoading) {
    return (
      <section className="mt-stack-lg px-margin-mobile">
        <div className="flex justify-between items-end mb-stack-md">
          <h3 className="font-headline-md text-headline-md text-on-surface">Bán Chạy Nhất</h3>
        </div>
        <div className="flex gap-stack-md overflow-x-auto no-scrollbar pb-4">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="flex-shrink-0 w-64">
              <div className="aspect-[4/5] rounded-xl overflow-hidden mb-base bg-surface-container-high animate-pulse" />
              <div className="h-6 bg-surface-container-high rounded animate-pulse mb-1" />
              <div className="h-4 bg-surface-container-high rounded w-1/3 animate-pulse mx-auto" />
            </div>
          ))}
        </div>
      </section>
    );
  }

  const allProducts = Array.isArray(products) ? products : [];
  const displayProducts = allProducts.slice(0, 6);

  if (displayProducts.length === 0) {
    return null;
  }

  return (
    <section className="mt-stack-lg px-margin-mobile">
      <div className="flex justify-between items-end mb-stack-md">
        <h3 className="font-headline-md text-headline-md text-on-surface">Bán Chạy Nhất</h3>
        <Link to="/shop?sort=best-selling" className="font-label-md text-primary uppercase tracking-widest text-xs no-underline">
          Xem tất cả
        </Link>
      </div>
      <div className="flex gap-stack-md overflow-x-auto no-scrollbar pb-4 -mx-margin-mobile px-margin-mobile">
        {displayProducts.map((product: any) => {
          const imageUrl = getImageUrl(product.imageUrl);
          const displayPrice = product.promotionPrice ?? product.currentPrice ?? product.discountPrice ?? product.price;
          return (
            <Link
              key={product.id}
              to={`/product/${product.id}`}
              className="flex-shrink-0 w-64 group no-underline"
            >
              <div className="relative aspect-[4/5] rounded-xl overflow-hidden mb-base petal-shadow">
                <img
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                  src={imageUrl}
                  alt={product.name}
                  loading="lazy"
                />
              </div>
              <div className="text-center">
                <h4 className="font-headline-sm text-headline-sm text-on-surface">{product.name}</h4>
                <p className="font-label-md text-tertiary">{formatCurrency(displayPrice)}</p>
              </div>
            </Link>
          );
        })}
      </div>
    </section>
  );
}

export default BestSellingProducts;
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/BestSellingProducts.tsx
git commit -m "feat: change BestSellingProducts to horizontal scroll layout"
```
