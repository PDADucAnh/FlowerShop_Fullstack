# Product Details Design Refinements Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refine the React Product Details page to match the luxury boutique visual style and features from `docs/design/productdetails.md` with interactive gallery enhancements, video care modal, and premium typography/spacing.

**Architecture:** Extend `ProductDetailPage` to support a video Care modal, full-screen image lightbox, and polished style alignments using Tailwind theme classes, custom CSS classes, and React state.

**Tech Stack:** React, TypeScript, Tailwind CSS, TanStack Query, React Icons.

## Global Constraints
- Do not import external CSS libraries; use existing Tailwind utility classes.
- Retain dynamic multi-language integrations (Vietnamese labels).
- Ensure typescript compilation passes successfully before commits.

---

### Task 1: Implement Interactive Video Playback & Lightbox Modal

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`

**Interfaces:**
- Produces: State handles for `showVideoModal` and `showLightbox`, rendering a beautiful fullscreen video player overlay and a zoomable image carousel when clicking the main product image or the 4th thumbnail.

- [x] **Step 1: Declare state and gallery video definitions**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/product-detail/index.tsx) to add state variables and video data:
```typescript
  const [showVideoModal, setShowVideoModal] = useState(false);
  const [showLightbox, setShowLightbox] = useState(false);
  const [lightboxIndex, setLightboxIndex] = useState(0);

  const videoUrl = "https://www.youtube.com/embed/g20T21s0uVw"; // Aesthetic Flower Care Tutorial
```

- [x] **Step 2: Update Gallery rendering**

Update the image gallery and thumbnails layout. The 4th item in the thumbnail grid should render a play button overlay instead of a static image:
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
              
              {/* Video Thumbnail (4th Slot) */}
              <div
                onClick={() => setShowVideoModal(true)}
                className="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors bg-surface-container flex items-center justify-center relative group"
              >
                {galleryImages[3] && (
                  <img
                    alt="Video thumbnail"
                    className="absolute inset-0 w-full h-full object-cover opacity-60 group-hover:scale-105 transition-transform duration-300"
                    src={formatImageUrl(galleryImages[3])}
                  />
                )}
                <span className="material-symbols-outlined text-primary text-3xl z-10 transition-transform group-hover:scale-110">play_circle</span>
              </div>
            </div>
```

- [x] **Step 3: Render Video Modal & Lightbox Overlays**

Append the fullscreen modal overlays for both video playback and image lightbox at the bottom of the page before the closing tags:
```typescript
      {/* Video Care Modal */}
      {showVideoModal && (
        <div className="fixed inset-0 z-[100] bg-black/80 backdrop-blur-md flex items-center justify-center p-4">
          <div className="relative w-full max-w-4xl aspect-video bg-black rounded-2xl overflow-hidden shadow-2xl">
            <button
              onClick={() => setShowVideoModal(false)}
              className="absolute top-4 right-4 text-white hover:text-primary z-10 p-2 bg-black/40 rounded-full hover:bg-black/60 transition-colors"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
            <iframe
              className="w-full h-full"
              src={`${videoUrl}?autoplay=1`}
              title="Flower Care Tutorial"
              frameBorder="0"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowFullScreen
            />
          </div>
        </div>
      )}

      {/* Lightbox Modal */}
      {showLightbox && (
        <div className="fixed inset-0 z-[100] bg-black/90 backdrop-blur-md flex flex-col justify-between py-6">
          <div className="flex justify-between items-center px-6">
            <span className="text-white font-label-md">
              {lightboxIndex + 1} / {galleryImages.length}
            </span>
            <button
              onClick={() => setShowLightbox(false)}
              className="text-white hover:text-primary p-2 bg-white/10 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
          </div>

          <div className="flex items-center justify-between px-4 max-h-[80vh]">
            <button
              onClick={() => setLightboxIndex((prev) => (prev === 0 ? galleryImages.length - 1 : prev - 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_left</span>
            </button>
            <img
              src={formatImageUrl(galleryImages[lightboxIndex])}
              alt="Zoomed view"
              className="max-w-full max-h-[75vh] object-contain rounded-lg shadow-2xl"
            />
            <button
              onClick={() => setLightboxIndex((prev) => (prev === galleryImages.length - 1 ? 0 : prev + 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_right</span>
            </button>
          </div>

          <div className="flex justify-center gap-2 overflow-x-auto px-6">
            {galleryImages.map((img, idx) => (
              <button
                key={idx}
                onClick={() => setLightboxIndex(idx)}
                className={`w-16 h-16 rounded-md overflow-hidden border-2 transition-all ${
                  lightboxIndex === idx ? 'border-primary scale-105' : 'border-transparent opacity-60'
                }`}
              >
                <img src={formatImageUrl(img)} className="w-full h-full object-cover" alt="" />
              </button>
            ))}
          </div>
        </div>
      )}
```

- [x] **Step 4: Connect main image click to Lightbox**

Make the main image clickable to open the Lightbox view:
```typescript
            <div 
              onClick={() => {
                const currentIdx = galleryImages.indexOf(activeImage);
                setLightboxIndex(currentIdx !== -1 ? currentIdx : 0);
                setShowLightbox(true);
              }}
              className="w-full aspect-[4/5] rounded-xl overflow-hidden shadow-[0_4px_20px_rgba(171,44,93,0.02)] bg-surface-container-low group cursor-zoom-in relative"
            >
```

- [x] **Step 5: Verify change compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [x] **Step 6: Commit changes**

```bash
git add cms.frontend/src/pages/product-detail/index.tsx
git commit -m "feat: add interactive care video modal and image lightbox on product detail page"
```

---

### Task 2: Standardize Typography, Spacing, and Button Micro-interactions

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`

**Interfaces:**
- Produces: Polished luxury branding styling with appropriate class configurations, exact spacing layout offsets, and hover transitions.

- [x] **Step 1: Update typography and spacing classes**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/product-detail/index.tsx) to adjust standard heading, pricing, description, and button classes. Remove the redundant SKU display from the top layout block to match the clean design mockup:
```typescript
            {/* Header badge & Clean Title */}
            <div className="flex items-center gap-2 mb-2">
              <span className="bg-secondary-container text-on-secondary-container px-3 py-1 rounded-full font-label-sm text-label-sm">Bán chạy nhất</span>
              {product.categoryProductName && (
                <span className="bg-surface-container text-on-surface-variant px-3 py-1 rounded-full font-label-sm text-label-sm">{product.categoryProductName}</span>
              )}
            </div>
            
            <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-2 font-bold tracking-tight">{product.name}</h1>
```

- [x] **Step 2: Add hover transformations to selector options**

Ensure size configuration buttons have custom transitional states and shadow values:
```typescript
                      <button
                        key={size}
                        onClick={() => setSelectedSize(size)}
                        className={`rounded-lg py-3 text-center transition-all duration-300 font-label-sm text-label-sm hover:scale-[1.02] active:scale-[0.98] ${
                          isSelected
                            ? 'border-2 border-primary bg-surface-container-lowest shadow-[0_4px_20px_rgba(171,44,93,0.08)] text-primary font-semibold'
                            : 'border border-outline-variant hover:border-primary hover:bg-surface-container-low text-on-surface-variant'
                        }`}
                      >
```

- [x] **Step 3: Stylize buy action buttons with custom luxury styling**

Add premium action styling to the Add to Cart and Mua Ngay buttons:
```typescript
              <button
                className="flex-1 bg-primary text-on-primary h-[52px] rounded-lg font-label-md text-label-md shadow-[0_4px_20px_rgba(171,44,93,0.2)] hover:shadow-[0_8px_30px_rgba(171,44,93,0.3)] hover:-translate-y-0.5 active:translate-y-0 active:scale-[0.98] transition-all flex items-center justify-center gap-2 disabled:opacity-40 disabled:cursor-not-allowed"
                disabled={!canAddToCart}
                onClick={handleAddToCart}
              >
                <span className="material-symbols-outlined">shopping_bag</span>
                {isOutOfStock ? 'Hết hàng' : 'Thêm vào giỏ'}
              </button>
```
AndUpdate the "Mua ngay" button:
```typescript
            <button
              disabled={!canAddToCart}
              onClick={handleBuyNow}
              className="w-full h-[52px] bg-transparent text-primary border border-primary hover:bg-primary-container/10 active:scale-[0.98] transition-all rounded-lg font-label-md text-label-md flex items-center justify-center gap-2 disabled:opacity-40 disabled:cursor-not-allowed mb-stack-lg"
            >
              <span className="material-symbols-outlined">flash_on</span>
              Mua ngay
            </button>
```

- [x] **Step 4: Verify TypeScript compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [x] **Step 5: Commit changes**

```bash
git add cms.frontend/src/pages/product-detail/index.tsx
git commit -m "feat: align typography, spacing, and hover states with luxury boutique guidelines"
```
