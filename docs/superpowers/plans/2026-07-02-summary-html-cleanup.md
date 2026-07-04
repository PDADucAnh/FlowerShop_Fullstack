# Summary HTML Cleanup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Prevent raw HTML tags from displaying in the blog post summary block under the title by stripping HTML tags at both Backend (during mapping/saving) and Frontend (during rendering).

**Architecture:** Implement regex-based HTML tag stripping in C# `MappingExtensions.cs` and DOM-based HTML tag stripping in React `BlogDetail` component.

**Tech Stack:** ASP.NET Core 8, React, TypeScript.

---

### Task 1: Clean Summary Generation on Backend

**Files:**
- Modify: `CMS.Backend/Models/DTOs/MappingExtensions.cs`

**Interfaces:**
- Produces: HTML-stripped plain-text strings for the DTO Summary property.

- [ ] **Step 1: Update MappingExtensions.cs**

Modify [MappingExtensions.cs](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/CMS.Backend/Models/DTOs/MappingExtensions.cs):
- Add `StripHtml` private method:
```csharp
        private static string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            var clean = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
            clean = clean.Replace("&nbsp;", " ")
                         .Replace("&amp;", "&")
                         .Replace("&lt;", "<")
                         .Replace("&gt;", ">");
            return clean.Trim();
        }
```
- Update `TruncateSummary` to call `StripHtml` when `summary` is empty:
```csharp
        private static string? TruncateSummary(string? summary, string? content)
        {
            const int maxLength = 500;
            var text = !string.IsNullOrWhiteSpace(summary)
                ? summary
                : StripHtml(content ?? "");
                
            return text?.Length > maxLength ? text.Substring(0, maxLength) : text;
        }
```

- [ ] **Step 2: Compile Backend**

Run: `dotnet build CMS.Backend/CMS.Backend.csproj`
Expected: Build successfully.

- [ ] **Step 3: Commit**

```bash
git add CMS.Backend/Models/DTOs/MappingExtensions.cs
git commit -m "fix: strip HTML tags from generated blog summary on backend"
```

---

### Task 2: Strip HTML from Summary on Frontend Detail Page

**Files:**
- Modify: `cms.frontend/src/pages/blog-detail/index.tsx`

**Interfaces:**
- Produces: HTML-stripped plain text display in subtitle area.

- [ ] **Step 1: Update BlogDetail component**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/blog-detail/index.tsx):
- Add `stripHtml` helper method:
```typescript
    const stripHtml = (html: string): string => {
        if (!html) return '';
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = html;
        const text = tempDiv.textContent || tempDiv.innerText || '';
        return text.trim();
    };
```
- Update summary rendering (line 62) to use `stripHtml`:
```tsx
                        {post.summary && <p className="font-body-lg text-body-lg text-white/80 max-w-2xl mx-auto">{stripHtml(post.summary)}</p>}
```

- [ ] **Step 2: Verify TypeScript Compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/src/pages/blog-detail/index.tsx
git commit -m "fix: strip HTML tags from summary on blog detail page frontend"
```
