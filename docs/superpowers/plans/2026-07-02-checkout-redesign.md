# Checkout Page Redesign Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Redesign the frontend checkout page in `cms.frontend/src/pages/checkout/index.tsx` to match the exact mockup design and styling (premium pink fields, left vertical accent bars on sections, custom radio layouts, and compact product card rows), while keeping all validation, phone blacklist checking, React Hook Form integrations, and API order creation logic intact.

**Tech Stack:** React, Tailwind CSS, TypeScript

---

### Task 1: Redesign Checkout Page Component

**Files:**
- Modify: `cms.frontend/src/pages/checkout/index.tsx`

- [ ] **Step 1: Rewrite checkout/index.tsx**
  Open `cms.frontend/src/pages/checkout/index.tsx` and apply the new luxury template layout:
  * Header layout with "FlowerShop" brand link and "Trở lại giỏ hàng" button.
  * Form Sections (Left Column):
    * Section 1 (Thông tin người mua): Họ tên (fullname), SĐT (phone) with `handlePhoneBlur` hook, Email (email). Inputs styled with `bg-[#FCE4EC]`.
    * Section 2 (Thông tin người nhận): Checkbox for syncing buyer's info into receiver's info (`recipientIsBuyer` state). Receiver Name (recipientName), Receiver Phone (recipientPhone), Greeting message (greetingCard).
    * Section 3 (Thời gian & Địa điểm): District (deliveryDistrict, disabled), Address (deliveryAddress), Delivery Date (deliveryDate), Delivery Time Slot (deliveryTimeSlot), and Extra notes (notes).
    * Section 4 (Phương thức thanh toán): COD and OnlinePayment radios with active border highlights, blacklisted phone COD gating.
  * Sidebar Order Summary (Right Column):
    * Display product list using the compact card styling: 20x24 aspect image box, name, quantity, and currency formatted total price.
    * Billing totals (Tạm tính, Phí vận chuyển, Tổng cộng).
    * "Đặt hàng" button with dynamic text loading state.
    * Lock icon security note.

- [ ] **Step 2: Verify type safety**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 3: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/pages/checkout/index.tsx
  git commit -m "feat: redesign checkout page structure and styles to match luxury mockup"
  ```
