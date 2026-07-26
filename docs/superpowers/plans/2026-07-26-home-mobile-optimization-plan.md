# Home Page Mobile Optimization — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Optimize the React frontend Home page for mobile following the Floraison design reference

**Architecture:** Modify existing components in-place (drawer nav extracted from Header); add CSS utilities; no new API calls, hooks, or data flow changes

**Tech Stack:** React 19, TypeScript, Tailwind CSS 3, Vite

## Global Constraints

- No backend/API changes; no new dependencies
- Must preserve all existing functionality (carousel, pagination, auth, cart)
- All changes within `Flower-shop.frontend/src/`
- `npm run build` must pass with zero TypeScript errors
- Color palette, font tokens, spacing tokens stay unchanged (already match the reference)

---

### Task 1: CSS utilities for drawer and scroll

**Files:**
- Modify: `Flower-shop.frontend/src/assets/css/index.css` (append at end before `@media` block)

- [ ] **Step 1: Add .no-scrollbar, .drawer-overlay, .drawer-content CSS**

Append to `index.css` (before the `@media (prefers-reduced-motion)` block at line 96):

```css
.no-scrollbar::-webkit-scrollbar { display: none; }
.no-scrollbar { -ms-overflow-style: none; scrollbar-width: none; }

.drawer-overlay {
    background-color: rgba(0, 0, 0, 0.3);
    backdrop-filter: blur(4px);
    opacity: 0;
    pointer-events: none;
    transition: opacity 0.3s ease;
}
.drawer-overlay.active {
    opacity: 1;
    pointer-events: auto;
}
.drawer-content {
    transform: translateX(-100%);
    transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
.drawer-content.active {
    transform: translateX(0);
}
```

- [ ] **Step 2: Verify build still works**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```
Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/assets/css/index.css
git commit -m "feat: add drawer and no-scrollbar CSS utilities"
```

---

### Task 2: Create DrawerNav component

**Files:**
- Create: `Flower-shop.frontend/src/components/DrawerNav.tsx`

- [ ] **Step 1: Write the DrawerNav component**

```tsx
import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import settingsService from '../services/settingsService';

interface DrawerNavProps {
  isOpen: boolean;
  onClose: () => void;
}

interface MenuItem {
  label: string;
  path: string;
  icon: string;
}

const menuItems: MenuItem[] = [
  { label: 'Trang chủ', path: '/', icon: 'home' },
  { label: 'Cửa hàng', path: '/shop', icon: 'local_florist' },
  { label: 'Bộ sưu tập', path: '/shop', icon: 'auto_awesome' },
  { label: 'Tin tức', path: '/blog', icon: 'auto_stories' },
  { label: 'Giới thiệu', path: '/about', icon: 'favorite' },
  { label: 'Liên hệ', path: '/contact', icon: 'mail' },
];

const DrawerNav: React.FC<DrawerNavProps> = ({ isOpen, onClose }) => {
  const location = useLocation();
  const [storeName, setStoreName] = React.useState('Floraison Boutique');

  React.useEffect(() => {
    settingsService.getStoreInfo().then((res: any) => {
      if (res?.storeName) setStoreName(res.storeName);
    }).catch(() => {});
  }, []);

  React.useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768 && isOpen) {
        onClose();
      }
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [isOpen, onClose]);

  React.useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }
    return () => { document.body.style.overflow = ''; };
  }, [isOpen]);

  const isActive = (path: string) => {
    if (path === '/') return location.pathname === '/';
    return location.pathname.startsWith(path);
  };

  return (
    <>
      <div
        className={`fixed inset-0 z-[100] drawer-overlay ${isOpen ? 'active' : ''}`}
        onClick={onClose}
      />
      <aside
        className={`fixed top-0 left-0 z-[101] bg-surface h-full w-80 rounded-r-xl shadow-xl flex flex-col py-stack-md drawer-content ${isOpen ? 'active' : ''}`}
      >
        <div className="px-margin-mobile mb-stack-lg">
          <h2 className="font-display-lg-mobile text-display-lg-mobile text-primary">
            {storeName}
          </h2>
        </div>
        <nav className="flex-1 flex flex-col gap-1">
          {menuItems.map((item) => {
            const active = isActive(item.path);
            return (
              <Link
                key={item.path}
                to={item.path}
                onClick={onClose}
                className={`flex items-center gap-4 py-3 px-6 mx-2 my-1 rounded-full transition-colors duration-200 no-underline ${
                  active
                    ? 'bg-secondary-container text-on-secondary-container'
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <span className="material-symbols-outlined">{item.icon}</span>
                <span className="font-body-lg text-body-lg">{item.label}</span>
              </Link>
            );
          })}
        </nav>
      </aside>
    </>
  );
};

export default DrawerNav;
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/components/DrawerNav.tsx
git commit -m "feat: create DrawerNav slide-in component"
```

---

### Task 3: Simplify Header — use DrawerNav for mobile

**Files:**
- Modify: `Flower-shop.frontend/src/components/Header.tsx`

- [ ] **Step 1: Add DrawerNav import**

After line 9 (`import layoutService...`), add:
```tsx
import DrawerNav from './DrawerNav';
```

- [ ] **Step 2: Replace mobile nav list with DrawerNav**

Replace lines 319-333 (the `mobileNavOpen` div and its contents):
```tsx
{/* Old code to delete (lines 319-333):
<div className={`${mobileNavOpen ? '' : 'hidden'} md:hidden fixed ...`}>
  {layout?.menuItems ...
</div>
*/}
```
With:
```tsx
<DrawerNav isOpen={mobileNavOpen} onClose={() => setMobileNavOpen(false)} />
```

Also remove `renderMobileMenuItem` function (lines 142-186) since it's no longer used.

Also update the hamburger button onClick (line 401) — keep as is since it toggles `mobileNavOpen`.

- [ ] **Step 3: Remove renderMobileMenuItem function**

Delete lines 142-186 (the entire `renderMobileMenuItem` function) and the import of `MenuItem` type if it's only used there. Check that `MenuItem` type is still imported for the `renderMenuItem` function — it's used at line 66-72.

Actually keep the import because `renderMenuItem` uses `MenuItem`. Just remove the renderMobileMenuItem function body.

- [ ] **Step 4: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 5: Commit**

```bash
git add Flower-shop.frontend/src/components/Header.tsx
git commit -m "feat: replace mobile nav with DrawerNav in Header"
```

---

### Task 4: Polish HeroBanner

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/HeroBanner.tsx`

- [ ] **Step 1: Update fallback hero text and button**

In the empty state (lines 78-112), update:
- Line 100-101: Update subtitle text
- Line 104: Add `active:scale-95` class to the CTA button

```tsx
{/* Line 96 — change title */}
<h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-stack-md leading-tight">
  Artistry in Every Bloom
</h1>
{/* Line 99-101 — change subtitle */}
<p className="font-body-lg text-body-lg text-on-surface-variant mb-lg max-w-xl mx-auto">
  Tinh hoa nghệ thuật cắm hoa — những thiết kế hoa độc đáo, sang trọng dành cho bạn.
</p>
{/* Line 104 — add active:scale-95 to button */}
<Link
  to="/shop"
  className="bg-primary text-on-primary px-8 py-4 font-label-sm text-label-sm uppercase tracking-widest border border-primary text-decoration-none btn-luxury btn-primary-luxury inline-block active:scale-95"
>
  Khám phá ngay
</Link>
```

Also update the tag badge text from "Lãng Mạn Đương Đại" to "Floraison Boutique" (line 94).

Also update the carousel slide badge at line 147 from "Lãng Mạn Đương Đại" to "Floraison Boutique".

Also update button text at line 162 from "Mua ngay" to "Khám phá ngay" and add `active:scale-95`.

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/HeroBanner.tsx
git commit -m "feat: update HeroBanner text and button polish"
```

---

### Task 5: BestSellingProducts — horizontal scroll

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/BestSellingProducts.tsx`

- [ ] **Step 1: Rewrite BestSellingProducts with horizontal scroll**

Replace entire file content:

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

---

### Task 6: ProductGrid — 2-column mobile grid

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/ProductGrid.tsx`

- [ ] **Step 1: Update grid classes and section title**

Change the grid classes on line 35 from:
```tsx
<div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter mb-xl">
```
To:
```tsx
<div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter mb-xl">
```

Change the section title area (lines 29-33):
```tsx
<section className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full border-t border-outline-variant/30">
  <div className="text-center mb-xl">
    <h2 className="font-display-lg text-headline-md uppercase tracking-tight mb-sm">Bộ Sưu Tập</h2>
    <p className="text-secondary font-body-md max-w-xl mx-auto">Tinh hoa nghệ thuật cắm hoa</p>
    <div className="w-12 h-0.5 bg-primary mx-auto mt-md"></div>
  </div>
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/ProductGrid.tsx
git commit -m "feat: change ProductGrid to 2-column mobile layout"
```

---

### Task 7: LatestBlog — stacked vertical layout

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/LatestBlog.tsx`

- [ ] **Step 1: Rewrite LatestBlog with stacked cards**

Replace entire file content:

```tsx
import React from 'react';
import { Link } from 'react-router-dom';
import { usePosts } from '../../hooks/usePosts';
import { getImageUrl } from '../../utils/apiUtils';

function LatestBlog() {
  const { data: posts = [], isLoading } = usePosts();

  if (isLoading) return null;

  const topThreePosts = [...posts]
    .sort((a: any, b: any) => new Date(b.createdDate || b.publishedAt || 0).getTime() - new Date(a.createdDate || a.publishedAt || 0).getTime())
    .slice(0, 3);

  if (topThreePosts.length === 0) return null;

  return (
    <section className="mt-stack-lg px-margin-mobile pb-stack-lg">
      <h3 className="font-headline-md text-headline-md text-on-surface mb-stack-md text-center">
        Câu Chuyện & Cảm Hứng
      </h3>
      <div className="space-y-stack-md max-w-container-max mx-auto">
        {topThreePosts.map((post: any) => {
          const imageUrl = getImageUrl(post.imageUrl || post.thumbnailUrl);
          return (
            <Link
              key={post.id}
              to={`/blog/${post.id}`}
              className="block bg-surface-container-low rounded-xl overflow-hidden petal-shadow no-underline group"
            >
              <div className="h-48 overflow-hidden">
                <img
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                  src={imageUrl}
                  alt={post.title}
                  loading="lazy"
                />
              </div>
              <div className="p-stack-sm">
                <span className="text-primary font-label-sm text-label-sm tracking-widest uppercase mb-1 block">
                  {post.category || 'Inspiration'}
                </span>
                <h4 className="font-headline-sm text-headline-sm text-on-surface mb-2 leading-tight">
                  {post.title}
                </h4>
                <p className="font-body-md text-body-md text-on-surface-variant line-clamp-2">
                  {post.shortDescription || post.excerpt || ''}
                </p>
              </div>
            </Link>
          );
        })}
      </div>
    </section>
  );
}

export default LatestBlog;
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/LatestBlog.tsx
git commit -m "feat: change LatestBlog to stacked vertical cards"
```

---

### Task 8: Home index — scroll reveal animations

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/index.tsx`

- [ ] **Step 1: Update Home with scroll-reveal wrappers**

Replace entire file content:

```tsx
import React from 'react';
import HeroBanner from './HeroBanner';
import BestSellingProducts from './BestSellingProducts';
import ProductGrid from './ProductGrid';
import LatestBlog from './LatestBlog';
import SEO from '../../components/SEO';
import { useScrollReveal } from '../../hooks/useScrollReveal';

function ScrollSection({ children }: { children: React.ReactNode }) {
  const { ref, isVisible } = useScrollReveal<HTMLDivElement>();
  return (
    <div
      ref={ref}
      className={`transition-all duration-700 ${
        isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-4'
      }`}
    >
      {children}
    </div>
  );
}

function Home() {
  return (
    <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen flex flex-col pt-20">
      <SEO title="Trang chủ" description="Cửa hàng hoa tươi PDA Flower - Hoa tươi mỗi ngày" />
      <main className="flex-grow flex flex-col">
        <HeroBanner />
        <ScrollSection><BestSellingProducts /></ScrollSection>
        <ScrollSection><ProductGrid categoryId={null} /></ScrollSection>
        <ScrollSection><LatestBlog /></ScrollSection>
      </main>
    </div>
  );
}

export default Home;
```

- [ ] **Step 2: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 3: Full build check**

```bash
cd Flower-shop.frontend && npx vite build
```

Expected: Build succeeds with no errors.

- [ ] **Step 4: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/index.tsx
git commit -m "feat: add scroll-reveal animations to home sections"
```

---

### Task 9: Final build and lint verification

**Files:** (no changes — verification only)

- [ ] **Step 1: Run full build**

```bash
cd Flower-shop.frontend && npx tsc --noEmit && npx vite build
```

Expected: Zero TypeScript errors, build succeeds.

- [ ] **Step 2: Verify no import warnings or unused variables**

Check console output for any warnings. Fix if found.

- [ ] **Step 3: Commit any remaining fixes**

```bash
git add -A
git commit -m "chore: final build verification"
```
