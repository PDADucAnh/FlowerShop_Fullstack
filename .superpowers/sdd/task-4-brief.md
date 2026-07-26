### Task 4: Polish HeroBanner

**Files:**
- Modify: `Flower-shop.frontend/src/pages/home/HeroBanner.tsx`

- [ ] **Step 1: Update fallback hero text and button**

In the empty/fallback state section, make these changes:
- Change title from "Tuyệt Tác Hoa Tươi Nghệ Thuật" to "Artistry in Every Bloom"
- Change subtitle from "Khám phá những thiết kế hoa độc đáo..." to "Tinh hoa nghệ thuật cắm hoa — những thiết kế hoa độc đáo, sang trọng dành cho bạn."
- Change button text from "Mua ngay" to "Khám phá ngay"
- Add `active:scale-95` class to the CTA Link button

- [ ] **Step 2: Update tag badge text**

Change the tag badge from "Lãng Mạn Đương Đại" to "Floraison Boutique" in BOTH places:
1. In the fallback/empty state section
2. In the carousel slide section (the badge inside the slides.map)

- [ ] **Step 3: Update carousel slide button**

In the carousel slide section:
- Change button text from "Mua ngay" to "Khám phá ngay"
- Add `active:scale-95` class to the button Link element

- [ ] **Step 4: Verify TypeScript compiles**

```bash
cd Flower-shop.frontend && npx tsc --noEmit
```

Expected: No errors.

- [ ] **Step 5: Commit**

```bash
git add Flower-shop.frontend/src/pages/home/HeroBanner.tsx
git commit -m "feat: update HeroBanner text and button polish"
```
