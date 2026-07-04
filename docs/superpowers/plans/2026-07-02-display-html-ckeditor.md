# Display HTML CKEditor Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement clean styling for dynamic HTML structures generated from CKEditor (e.g. bold, italics, lists, blockquotes, centered figures/images, headings) inside the BlogDetail component.

**Architecture:** Inject CSS Descendant selectors scoping `.blog-detail-content` inside the global `<style>` tag in `public/index.html`.

**Tech Stack:** React, Tailwind CSS, DOMPurify.

---

### Task 1: Add CKEditor Typography Styling in public/index.html

**Files:**
- Modify: `cms.frontend/public/index.html`

**Interfaces:**
- Produces: Scoped styling rules targetting `.blog-detail-content`.

- [ ] **Step 1: Update public/index.html Styles**

Modify [index.html](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/public/index.html) by inserting the following rules inside the `<style>` block:
```css
        /* CKEditor HTML Rendering Styles */
        .blog-detail-content {
            font-family: 'Plus Jakarta Sans', sans-serif;
            font-size: 16px;
            line-height: 1.8;
            color: #3f001b;
        }
        
        .blog-detail-content strong {
            font-weight: 700;
            color: #1b1c1c;
        }
        
        .blog-detail-content em {
            font-style: italic;
        }

        .blog-detail-content p {
            margin-bottom: 1.5rem;
            text-align: justify;
        }
        
        .blog-detail-content h2 {
            font-family: 'Playfair Display', serif;
            font-size: 24px;
            font-weight: 600;
            margin-top: 2rem;
            margin-bottom: 1rem;
            color: #ab2c5d;
        }

        .blog-detail-content h3 {
            font-family: 'Playfair Display', serif;
            font-size: 20px;
            font-weight: 600;
            margin-top: 1.75rem;
            margin-bottom: 0.75rem;
            color: #ab2c5d;
        }

        .blog-detail-content ul {
            list-style-type: disc;
            margin-left: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .blog-detail-content ol {
            list-style-type: decimal;
            margin-left: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .blog-detail-content li {
            margin-bottom: 0.5rem;
        }

        .blog-detail-content blockquote {
            border-left: 4px solid #ab2c5d;
            padding-left: 1rem;
            margin-left: 0;
            margin-right: 0;
            margin-bottom: 1.5rem;
            font-style: italic;
            color: #6b5a60;
        }

        .blog-detail-content img {
            max-width: 100%;
            height: auto;
            display: block;
            margin: 20px auto;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(171, 44, 93, 0.08);
        }

        .blog-detail-content figure.image {
            margin: 30px auto;
            text-align: center;
        }

        .blog-detail-content figure.image figcaption {
            font-size: 14px;
            font-style: italic;
            color: #8a7176;
            margin-top: 8px;
        }
```

- [ ] **Step 2: Verify TypeScript Compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [ ] **Step 3: Commit**

```bash
git add cms.frontend/public/index.html
git commit -m "feat: add scoped styles for CKEditor HTML content in public/index.html"
```
