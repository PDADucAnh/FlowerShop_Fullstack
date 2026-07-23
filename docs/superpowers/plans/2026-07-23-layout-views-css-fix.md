# Layout Views CSS Fix — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix Tailwind CSS rendering on admin Layout views so they match existing admin views (Page, Contact, etc.)

**Architecture:** Add missing `Layout = "_LayoutAdmin"` to Index.cshtml, replace raw Tailwind classes with design system tokens (`bg-white` → `bg-surface`, `p-6` → `p-lg`, `px-6` → `px-lg`, `px-5` → `px-lg`, `mb-6` → `mb-stack-md`).

**Tech Stack:** ASP.NET Core 8, Razor Views, Tailwind CSS (CDN)

## Global Constraints

- All color classes must use design system tokens (`bg-surface`, `text-on-primary`, `border-outline-variant`) not raw CSS colors
- All spacing must use design tokens (`p-lg`, `px-lg`, `mb-stack-md`) not raw Tailwind units
- Layout view must explicitly declare `Layout = "_LayoutAdmin"`
- No `bg-white` anywhere — use `bg-surface`

---

### Task 1: Fix Layout/Index.cshtml

**Files:**
- Modify: `Flower.Backend/Views/Layout/Index.cshtml`

- [ ] **Step 1: Add Layout declaration**

```cshtml
@model LayoutViewModel
@{
    ViewData["Title"] = "Quản lý giao diện";
    Layout = "_LayoutAdmin";
    var activeTab = ViewBag.ActiveTab as string ?? "header";
}
```

- [ ] **Step 2: Replace raw heading with admin-style heading**

Replace:
```html
<h1 class="text-headline-md font-headline-md mb-6">Quản lý giao diện</h1>
```
With:
```html
<div>
    <h3 class="text-label-sm uppercase tracking-[0.3em] text-secondary">Tùy chỉnh</h3>
    <p class="serif text-3xl font-bold">Quản lý giao diện</p>
</div>
```

- [ ] **Step 3: Align tab container spacing**

```html
<div class="flex border-b border-outline-variant/30 mb-stack-md">
    <a href="@Url.Action("Index", new { tab = "header" })"
       class="px-lg py-3 font-label-md text-label-md @(activeTab == "header" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Trình đơn</a>
    <a href="@Url.Action("Index", new { tab = "footer" })"
       class="px-lg py-3 font-label-md text-label-md @(activeTab == "footer" ? "text-primary border-b-2 border-primary" : "text-on-surface-variant")">Chân trang</a>
</div>
```

- [ ] **Step 4: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 5: Commit**

```bash
git add Flower.Backend/Views/Layout/Index.cshtml
git commit -m "fix: add Layout=_LayoutAdmin and align heading/spacing tokens in Layout/Index.cshtml"
```

---

### Task 2: Fix _HeaderTab.cshtml

**Files:**
- Modify: `Flower.Backend/Views/Layout/_HeaderTab.cshtml`

- [ ] **Step 1: Replace card padding tokens**

Change every `p-6` → `p-lg` (lines 12, 31, 65, 103, the card wrapper divs)
Change every `mb-6` → `mb-stack-md` (same lines)

- [ ] **Step 2: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 3: Commit**

```bash
git add Flower.Backend/Views/Layout/_HeaderTab.cshtml
git commit -m "fix: use design token spacing in _HeaderTab.cshtml"
```

---

### Task 3: Fix _FooterTab.cshtml

**Files:**
- Modify: `Flower.Backend/Views/Layout/_FooterTab.cshtml`

- [ ] **Step 1: Fix color tokens**

Replace `bg-white` → `bg-surface` (appears in lines 14, 109 — the footer column card divs)
Replace `text-white` → `text-on-primary` (line 73, the "Thêm cột mới" button)

- [ ] **Step 2: Fix spacing tokens**

Replace first `p-6` → `p-lg` in column card div (line 14)
Replace `mt-4` → `mb-stack-md` (line 73)

- [ ] **Step 3: Build to verify**

Run: `cd Flower.Backend && dotnet build`
Expected: `Build succeeded`

- [ ] **Step 4: Commit**

```bash
git add Flower.Backend/Views/Layout/_FooterTab.cshtml
git commit -m "fix: use bg-surface/text-on-primary design tokens in _FooterTab.cshtml"
```

---

### Task 4: Verify end-to-end

- [ ] **Step 1: Run backend**

Run `cd Flower.Backend && dotnet run --urls https://localhost:7224`

- [ ] **Step 2: Navigate to admin Layout page**

Open `https://localhost:7224/Admin/Layout`

- [ ] **Step 3: Verify visual consistency**

Check that the page has:
- Admin sidebar on the left
- Admin top bar with notifications
- Tab navigation (Trình đơn / Chân trang) using admin styling
- Cards with proper background (not white)
- Material Symbols rendering correctly
- All spacing matches existing admin views

- [ ] **Step 4: Push to GitHub**

```bash
git push origin main
```

---

## Spec Coverage Check

| Spec Requirement | Task |
|-----------------|------|
| Add `Layout = "_LayoutAdmin"` to Index.cshtml | Task 1 |
| Replace raw spacing with design tokens | Tasks 1, 2, 3 |
| Replace `bg-white` → `bg-surface` | Task 3 |
| Replace `text-white` → `text-on-primary` | Task 3 |
| All three files updated | Tasks 1-3 |
