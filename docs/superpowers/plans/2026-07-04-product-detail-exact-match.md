# Product Details Exact Match Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor the Product Detail page component to match the layout and design details of the mockup `docs/design/productdetails.md` exactly.

**Architecture:** Remove layout and font weight deviations in headings, clean up the video thumbnail styling to use a plain background container, and reorder thumbnail images to ensure all secondary views are selectable.

**Tech Stack:** React, TypeScript, Tailwind CSS.

## Global Constraints
- Do not import external CSS libraries; use existing Tailwind utility classes.
- Retain dynamic multi-language integrations (Vietnamese labels).
- Ensure typescript compilation passes successfully before commits.

---

### Task 1: Refactor Gallery Thumbnails and Heading Styles

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`

**Interfaces:**
- Produces: Visual styling matching the mockup exactly with no extra font weight classes, clean grey video thumbnail container, and corrected margins.

- [x] **Step 1: Update Heading Typography**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/product-detail/index.tsx) to remove `font-bold tracking-tight` from the main `h1` heading so that it uses the natural Playfair Display font configuration:
```typescript
            <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-2">{product.name}</h1>
```

- [x] **Step 2: Update Thumbnail Gallery List**

Modify the thumbnail loop to render the first 3 images (main image, detail view 1, and top view) and use the 3rd Google Photo (lifestyle setting) as a selectable thumbnail, keeping the play button as the 5th item or adjusting the slice to match the mockup's 3 secondary images:
```typescript
            <div className="grid grid-cols-4 gap-stack-sm">
              {galleryImages.slice(0, 3).map((imgUrl, idx) => (
                <div
                  key={idx}
                  onClick={() => {
                    setActiveImage(imgUrl);
                    setLightboxIndex(idx);
                  }}
                  className={`aspect-square rounded-lg overflow-hidden cursor-pointer border-2 transition-colors ${
                    activeImage === imgUrl ? 'border-primary' : 'border-transparent hover:border-outline-variant'
                  }`}
                >
                  <img
                    alt={`${product.name} detail view ${idx + 1}`}
                    className="w-full h-full object-cover"
                    src={formatImageUrl(imgUrl)}
                  />
                </div>
              ))}
              
              {/* Video Thumbnail (4th Slot) - Clean grey background as mockup */}
              <div
                onClick={() => setShowVideoModal(true)}
                className="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors bg-surface-container flex items-center justify-center relative group"
              >
                <span className="material-symbols-outlined text-outline text-3xl z-10 transition-transform group-hover:scale-110">play_circle</span>
              </div>
            </div>
```

- [x] **Step 3: Align Spacing to Mockup**

Ensure the margins on action container matches `mb-stack-lg` instead of `mb-stack-md`:
```typescript
            {/* Add to Cart Area */}
            <div className="flex items-center gap-stack-sm mb-stack-lg">
```

- [x] **Step 4: Verify TypeScript compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [x] **Step 5: Commit changes**

```bash
git add cms.frontend/src/pages/product-detail/index.tsx
git commit -m "feat: refine product details page layout to match mockup exactly"
```
