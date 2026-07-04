# Order Confirmation Page Redesign Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Redesign the frontend order confirmation page to match the luxury mockup design, styling, minimal header/footer structure, and background decorative blur circles, while keeping the dynamic SearchParams logic intact.

**Tech Stack:** React, Tailwind CSS, TypeScript

---

### Task 1: Update Header, Footer, and Page Component

**Files:**
- Modify: `cms.frontend/src/components/Header.tsx`
- Modify: `cms.frontend/src/components/Footer.tsx`
- Modify: `cms.frontend/src/pages/order-confirmation/index.tsx`

- [ ] **Step 1: Update Header.tsx for order-confirmation**
  Open `cms.frontend/src/components/Header.tsx` and check `location.pathname === '/order-confirmation'`.
  If true, render the minimal header layout:
  * Brand logo `FlowerShop` linking to `/`.
  * User profile container display `user.fullName || user.username` with a person icon.

- [ ] **Step 2: Update Footer.tsx for order-confirmation**
  Open `cms.frontend/src/components/Footer.tsx` and check `location.pathname === '/order-confirmation'`.
  If true, render the minimal footer layout:
  * Brand logo `PDA FLOWER` and copyright text.
  * Links: Delivery Policy (`/shop`), Return Policy (`/shop`), Privacy Policy (`/shop`), Contact Us (`/contact`).

- [ ] **Step 3: Rewrite order-confirmation/index.tsx**
  Open `cms.frontend/src/pages/order-confirmation/index.tsx` and replace content:
  * Card wrapper with class: `w-full max-w-2xl bg-surface-container-lowest rounded-xl p-8 md:p-12 text-center relative overflow-hidden`
  * Add the decorative background blur absolute circles:
    * Top-right circle: `absolute top-0 right-0 -mr-8 -mt-8 w-32 h-32 bg-secondary-fixed rounded-full opacity-20 blur-2xl`
    * Bottom-left circle: `absolute bottom-0 left-0 -ml-8 -mb-8 w-32 h-32 bg-primary-fixed rounded-full opacity-20 blur-2xl`
  * CheckCircle icon styled with text-primary and check_circle value.
  * Billing info box with border-outline-variant.
  * Map navigation links for "Tiếp tục mua sắm" to `/shop` and "Xem đơn hàng" to `/my-orders`.

- [ ] **Step 4: Verify type safety**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 5: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/components/Header.tsx cms.frontend/src/components/Footer.tsx cms.frontend/src/pages/order-confirmation/index.tsx
  git commit -m "feat: redesign order confirmation page and its header/footer layouts to match mockup"
  ```
