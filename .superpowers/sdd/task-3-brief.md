### Task 3: Simplify Header — use DrawerNav for mobile

**Files:**
- Modify: `Flower-shop.frontend/src/components/Header.tsx`

- [ ] **Step 1: Add DrawerNav import**

After the `import layoutService` line, add:
```tsx
import DrawerNav from './DrawerNav';
```

- [ ] **Step 2: Replace mobile nav list with DrawerNav**

Find this block (the mobile nav dropdown that shows when `mobileNavOpen` is true) and replace it with `<DrawerNav>`:
```tsx
<div className={`${mobileNavOpen ? '' : 'hidden'} md:hidden fixed inset-x-0 top-[72px] bg-surface border-t border-outline-variant/20 shadow-lg z-40 flex flex-col p-4 space-y-3`}>
  {layout?.menuItems && layout.menuItems.length > 0
    ? layout.menuItems.map((item) => renderMobileMenuItem(item))
    : (
      <>
        <Link ...>Trang chủ</Link>
        <Link ...>Cửa hàng</Link>
        <Link ...>Tin tức</Link>
        <Link ...>Giới thiệu</Link>
        <Link ...>Liên hệ</Link>
      </>
    )
  }
</div>
```
Replace with:
```tsx
<DrawerNav isOpen={mobileNavOpen} onClose={() => setMobileNavOpen(false)} />
```

- [ ] **Step 3: Remove renderMobileMenuItem function**

Delete the entire `renderMobileMenuItem` function (it takes `item: MenuItem` as parameter and renders mobile nav links). Keep the `MenuItem` import from `layoutService` since `renderMenuItem` still uses it.

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
