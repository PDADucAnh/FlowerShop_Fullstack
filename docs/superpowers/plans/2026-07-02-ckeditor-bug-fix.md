# CKEditor HTML Rendering Bug Fix Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Correct the raw HTML tag rendering (double encoding issue) and fix broken relative image URLs `/uploads/ckeditor/` in blog posts on the customer page.

**Architecture:** Decode HTML Entities, replace relative `/uploads/` URLs with absolute `API_BASE_URL` paths, and sanitize via DOMPurify before dangerouslySetInnerHTML binding.

**Tech Stack:** React, TypeScript, DOMPurify.

---

### Task 1: Update BlogDetail Component HTML Rendering

**Files:**
- Modify: `cms.frontend/src/pages/blog-detail/index.tsx`

**Interfaces:**
- Produces: Sanitized and fully resolved absolute HTML content strings for blog posts.

- [ ] **Step 1: Update BlogDetail page**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/blog-detail/index.tsx):
- Import `API_BASE_URL` from `../../utils/apiUtils`.
- Add helper methods to decode HTML entities and replace relative URLs:
```typescript
    const decodeHtmlEntities = (str: string): string => {
        const txt = document.createElement('textarea');
        txt.innerHTML = str;
        return txt.value;
    };

    const getProcessedContent = (content: string) => {
        if (!content) return '';
        const decoded = decodeHtmlEntities(content);
        const withAbsoluteUrls = decoded.replace(
            /src=["']\/uploads\/(.*?)["']/g,
            `src="${API_BASE_URL}/uploads/$1"`
        );
        return DOMPurify.sanitize(withAbsoluteUrls);
    };
```
- Update the dangerouslySetInnerHTML line (line 89) to use `getProcessedContent`:
```tsx
                            <div
                                className="blog-detail-content font-body-lg text-body-lg text-on-surface-variant mb-lg leading-relaxed first-letter:text-5xl first-letter:font-display-xl first-letter:float-left first-letter:mr-3 first-letter:text-primary"
                                dangerouslySetInnerHTML={{ __html: getProcessedContent(post.content) }}
                            ></div>
```

- [ ] **Step 2: Verify TypeScript Compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/src/pages/blog-detail/index.tsx
git commit -m "fix: resolve CKEditor HTML entities double encoding and relative image URLs in BlogDetail"
```
