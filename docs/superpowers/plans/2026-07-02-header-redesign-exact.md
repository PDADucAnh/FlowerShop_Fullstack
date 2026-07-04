# Header Redesign Exact Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Modify the header component in `cms.frontend/src/components/Header.tsx` to match the exact template markup, while pulling data (categories, search queries, cart counters, user sessions) dynamically from backend services.

**Tech Stack:** React, Tailwind CSS, TypeScript

---

### Task 1: Rewrite Header Component

**Files:**
- Modify: `cms.frontend/src/components/Header.tsx`

- [ ] **Step 1: Rewrite Header.tsx**
  Open `cms.frontend/src/components/Header.tsx` and rewrite it to match the exact HTML structure, styling, and design.
  * Import `useProductCategories` from `../hooks/useCategories`.
  * Call `const { data: categories = [] } = useProductCategories();` to load categories dynamically.
  * Implement input bindings for the search query and trigger navigation on enter/click.
  * Implement dynamic rendering for Cart, Wishlist, and Profile groups based on context state.

- [ ] **Step 2: Verify type safety**
  Run `npx tsc --noEmit` inside `cms.frontend/`.

- [ ] **Step 3: Commit changes**
  Run:
  ```bash
  git add cms.frontend/src/components/Header.tsx
  git commit -m "feat: rewrite header to match exact template markup with dynamic API integration"
  ```
