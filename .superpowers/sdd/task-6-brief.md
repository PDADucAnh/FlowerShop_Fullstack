### Task 6: ProductGrid — 2-column mobile grid

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/ProductGrid.tsx`

- [ ] **Step 1: Update grid classes and section title**

Change the grid classes from:
```tsx
<div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter mb-xl">
```
To:
```tsx
<div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter mb-xl">
```

Change the section title area to:
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

- [ ] **Step 3: Commit**
```bash
git add Flower-shop.frontend/src/pages/home/ProductGrid.tsx
git commit -m "feat: change ProductGrid to 2-column mobile layout"
```
