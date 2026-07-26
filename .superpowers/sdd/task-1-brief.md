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
