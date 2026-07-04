# Cart Page Redesign and API Integration Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Modify the frontend cart page to match the luxury mockup design, styling, and grid/flex layout, and ensure it links perfectly to the backend API hooks and local state handlers.

**Tech Stack:** React, Tailwind CSS, TypeScript

---

### Task 1: Redesign Cart Components

**Files:**
- Modify: `cms.frontend/src/pages/cart/CartTable.tsx`
- Modify: `cms.frontend/src/pages/cart/index.tsx`

- [ ] **Step 1: Rewrite CartTable.tsx**
  Update `CartTable.tsx` to render a grid-based list matching the template rows:
  * Replace the table structure with CSS Grid (`grid-cols-1 md:grid-cols-12`) matching the mockup.
  * Use image container class: `w-24 h-32 rounded-lg bg-surface-variant flex-shrink-0 overflow-hidden petal-shadow`
  * Add the styled "Xóa" button with delete icon.
  * Use input control layout: a wrapper `div` with class `flex items-center border border-outline-variant rounded-lg overflow-hidden h-10` containing decrease, input, and increase buttons.

- [ ] **Step 2: Update cart/index.tsx**
  Update `ShoppingCartPage` component layout structure:
  * Title block with horizontal primary color divider.
  * Checkout layout with `flex flex-col lg:flex-row gap-gutter`
  * Left Column: `lg:w-[70%] space-y-gutter` containing the redesigned `CartTable` list.
  * Right Column (Summary): `lg:w-[30%] space-y-gutter` with billing information, trust badges (Verified User, Fast delivery), and the payment button.
  * Preserving routing gating state: checking delivery district and launching `LocationGatingModal` if absent.

- [ ] **Step 3: Verify type safety**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 4: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/pages/cart/
  git commit -m "feat: redesign shopping cart page structure and styles to match luxury mockup"
  ```
