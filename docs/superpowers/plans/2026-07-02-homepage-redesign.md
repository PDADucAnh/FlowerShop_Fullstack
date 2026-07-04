# Homepage and Shared Layouts Redesign Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Redesign the frontend homepage and shared layout components (Header, Footer, Product Cards, Blog Cards) to match the provided premium luxury theme, while keeping all dynamic backend integration, state management, and routing logic intact.

**Architecture:**
- Refactor `cms.frontend/src/components/Header.tsx` to match the style of the template header (shadow, padding, logo design) and add a "Danh mục" dropdown linking to categories.
- Refactor `cms.frontend/src/components/Footer.tsx` to match the style of the template footer.
- Refactor `cms.frontend/src/components/ProductCard.tsx` to support a `variant` prop (`'standard' | 'featured'`), adapting to the two styles used in the template grids.
- Refactor `cms.frontend/src/components/PostCard.tsx` to style blog cards according to the blog section template.
- Update `cms.frontend/src/pages/home/index.tsx` to compose the redesigned `HeroBanner`, `BestSellingProducts` (Bán Chạy Nhất), `ProductGrid` (Tuyệt Tác Ngàn Hoa), and `LatestBlog` (Câu Chuyện & Cảm Hứng Hoa) sections, ensuring all dynamic data flows perfectly.

**Tech Stack:** React, Tailwind CSS, TypeScript

---

### Task 1: Update Shared Layout (Header & Footer)

**Files:**
- Modify: `cms.frontend/src/components/Header.tsx`
- Modify: `cms.frontend/src/components/Footer.tsx`

- [ ] **Step 1: Redesign Header.tsx**
  Update `cms.frontend/src/components/Header.tsx` to:
  * Use classes: `sticky top-0 z-50 shadow-sm bg-surface w-full shadow-[0px_4px_20px_rgba(171,44,93,0.02)]`
  * Add the "Danh mục" dropdown with links:
    * "Hoa sinh nhật" -> `/shop?category=Sinh%20nhật`
    * "Hoa khai trương" -> `/shop?category=Khai%20trương`
    * "Hoa cưới" -> `/shop?category=Hoa%20cưới`
  * Preserve search bar popup, wishlist count, cart count badge, and profile group logic.

- [ ] **Step 2: Redesign Footer.tsx**
  Update `cms.frontend/src/components/Footer.tsx` to match the requested HTML style including the three columns (FlowerShop, Chính Sách, Liên Hệ), and copyright bar.

- [ ] **Step 3: Verify build**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 4: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/components/Header.tsx cms.frontend/src/components/Footer.tsx
  git commit -m "feat: redesign Header and Footer to match the new luxury layout"
  ```

---

### Task 2: Redesign ProductCard and PostCard Components

**Files:**
- Modify: `cms.frontend/src/components/ProductCard.tsx`
- Modify: `cms.frontend/src/components/PostCard.tsx`

- [ ] **Step 1: Add variant support to ProductCard.tsx**
  Modify `ProductCard.tsx` to accept:
  ```typescript
  interface ProductCardProps {
    item: Product;
    variant?: 'standard' | 'featured';
  }
  ```
  If `variant === 'featured'`:
  - Image container: `aspect-[4/5] overflow-hidden rounded-xl petal-shadow mb-4`
  - Title style: `font-headline-sm text-headline-sm text-on-surface`
  - Price: `text-primary font-label-md`
  If `variant === 'standard'`:
  - Image container: `aspect-square overflow-hidden rounded-lg bg-surface-container-low mb-4`
  - Title style: `font-label-md text-on-surface`
  - Price: `text-label-sm mb-2` with color maroon (`color: '#800000'`)
  Keep all click handlers for "Thêm vào giỏ" and "Mua ngay" buttons.

- [ ] **Step 2: Redesign PostCard.tsx**
  Modify `PostCard.tsx` to match the card style of the blog section:
  - Image container: `aspect-[16/9] overflow-hidden rounded-xl petal-shadow mb-4`
  - Category label: `font-label-sm text-label-sm text-primary uppercase tracking-widest mb-2 block`
  - Title: `font-headline-sm text-headline-sm text-on-surface mb-2`
  - Summary: `font-body-md text-body-md text-on-surface-variant mb-4 line-clamp-3`

- [ ] **Step 3: Verify build**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 4: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/components/ProductCard.tsx cms.frontend/src/components/PostCard.tsx
  git commit -m "feat: update ProductCard with variant styles and redesign PostCard"
  ```

---

### Task 3: Refactor Home Page Structure & Components

**Files:**
- Modify: `cms.frontend/src/pages/home/index.tsx`
- Modify: `cms.frontend/src/pages/home/HeroBanner.tsx`
- Modify: `cms.frontend/src/pages/home/BestSellingProducts.tsx`
- Modify: `cms.frontend/src/pages/home/ProductGrid.tsx`
- Modify: `cms.frontend/src/pages/home/LatestBlog.tsx`

- [ ] **Step 1: Update HeroBanner.tsx**
  Modify `HeroBanner.tsx` to fall back to the premium single hero block requested if active advertisement slides are empty. When slides are present, apply the new overlay layout `h-[819px] md:h-[921px]` and text style.

- [ ] **Step 2: Update BestSellingProducts.tsx**
  Modify the title to "Bán Chạy Nhất" and pass `variant="featured"` to the child `<ProductCard />` components.

- [ ] **Step 3: Update ProductGrid.tsx**
  Update title to "Tuyệt Tác Ngàn Hoa", and pass `variant="standard"` to `<ProductCard />` components.

- [ ] **Step 4: Update LatestBlog.tsx**
  Update the layout header to "Câu Chuyện & Cảm Hứng Hoa".

- [ ] **Step 5: Refactor home/index.tsx**
  Remove the `CategoryMenu` (categories are now in the header dropdown and the shop page), and include the `<BestSellingProducts />` component.
  The revised `Home` component will compose:
  ```tsx
  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20">
      <main className="max-w-[1440px] mx-auto">
        <HeroBanner />
        <BestSellingProducts />
        <ProductGrid categoryId={null} />
        <LatestBlog />
      </main>
    </div>
  );
  ```

- [ ] **Step 6: Verify build**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 7: Commit changes**
  Run:
  ```bash
  git add -A cms.frontend/src/pages/home/
  git commit -m "feat: refactor home page layout and component structures to match the template"
  ```
