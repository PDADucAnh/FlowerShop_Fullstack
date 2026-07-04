# Product Details Design Revamp Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Redesign the Product Details page to match the luxury floral boutique layout specified in `docs/design/productdetails.md`.

**Architecture:** Use a dynamic multi-image gallery with high-res thumbnails, implement an interactive size selector that updates the cart item's price and label dynamically, and fetch related products from the same category.

**Tech Stack:** React, TypeScript, Tailwind CSS, TanStack Query.

---

### Task 1: Update ProductDetail Giao diện & Thư viện Ảnh Tương tác

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`

**Interfaces:**
- Produces: A premium 12-column grid details page with breadcrumbs, badges, sticky column info, care accordions, and interactive main image switching.

- [x] **Step 1: Update ProductDetailPage component UI**

Modify [index.tsx](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/cms.frontend/src/pages/product-detail/index.tsx) to integrate the visual elements from [productdetails.md](file:///D:/TrenLop/ASP.NET/AnhCMS_Solution/docs/design/productdetails.md):
- Declare states for size selection and active gallery image:
```typescript
  const [selectedSize, setSelectedSize] = useState<'Classic' | 'Deluxe' | 'Grand'>('Classic');
  const [activeImage, setActiveImage] = useState<string>('');
```
- Set `activeImage` when product finishes loading inside a `useEffect`:
```typescript
  useEffect(() => {
    if (product) {
      setActiveImage(product.imageUrl || '');
    }
  }, [product]);
```
- Define the premium gallery thumbnails array using Google public flower photos as backups:
```typescript
  const galleryImages = product ? [
    product.imageUrl || '',
    "https://lh3.googleusercontent.com/aida-public/AB6AXuB9CddqN2JsuVI2rYrLjfhM9YJEFtmNp6f_UaBbD8XuKJ2qE5FLElGt1sSIizcpFIBWITclUw4cq9Zhpzs1vGtTEpSTzy6UOcn8Uf3146Ih0LPSnin2xvbSLqAc08l1_MwIKWQmPF5wXwQrMQBKupE_0bN9EZ4UW86h9zRflczjzRqvbIbsUFzIELmDwiL61nlxefYQguY7IW2PnRp72LshlZWLRnxebPiOJ0fpdgIhYGXjGLuVtDt4aPlSGy5hNcdAWNjn4O8NxTs",
    "https://lh3.googleusercontent.com/aida-public/AB6AXuARzZ2qpRYpsU19whOUnfBzCxrqw3zwzDMv9zTU5J2TAmeacj0BaWjrlo-IrlQjJBljoQbGoSoIzF21u9dh3bo3b2jHjQy2W8jd31qKC3K7kd0UPuDp3iUuEIUkfpGc1e_GJKTgrk9ZcdqGWOgIDg80Ulq6XjEjJwfpZxB9zidUfXHEwrxinQBnbjR5Cly7HOst4MdMJ_fdSnZ4arKWILEdaahJabulvl9C0Ro1R3yq3W49q7veNH8L0L6P1YrTeExOXmoSnGQXuus",
    "https://lh3.googleusercontent.com/aida-public/AB6AXuA8_xORpxwk86bL3I4VJlBsIa21JwThIYnfT_IvKL09XAhTuhrJHtHVTYz3lv02Uj2dU8HpKpZNGGsh4ULoq3Sf9ZZoFSqsoCcjVd8PdjIl8hPNpARjUw9RlqbsPb4b-tUKUfv8kb8ZMBB-QxF2PzIgAvpINuwnbQKOTvnXgZioxC-lZ1b13_z8DIbBBKuD7TGNLS6RPFGP5zE5X9SyhVB7zS_3FSH5utYQwqGx-gBglQ-m9DBJBpbcNJ_mvsxXQFVdTzgZjW--ZVk"
  ] : [];
```
- Implement the "Select Size" selector and size pricing adjustments:
```typescript
  const sizePriceAdjustment = selectedSize === 'Deluxe' ? 300000 : selectedSize === 'Grand' ? 600000 : 0;
  const basePrice = product ? (product.discountPrice || product.price) : 0;
  const finalPrice = basePrice + sizePriceAdjustment;
```
- Update `handleAddToCart` and `handleBuyNow` to dynamically construct a customized product name and price before pushing to the cart context:
```typescript
  const handleAddToCart = useCallback(() => {
    if (!product) return;
    if (quantity > product.stockQuantity) {
      toast.error(`Chỉ còn ${product.stockQuantity} sản phẩm trong kho`);
      return;
    }
    const customizedProduct = {
      ...product,
      name: selectedSize !== 'Classic' ? `${product.name} (${selectedSize})` : product.name,
      price: finalPrice
    };
    addToCart(customizedProduct, quantity);
    toast.success(`Đã thêm "${customizedProduct.name}" (x${quantity}) vào giỏ hàng`);
  }, [product, quantity, selectedSize, finalPrice, addToCart]);
```
- Design the premium details grid, rendering badges ("Bestseller", "Spring Collection"), breadcrumbs, Care and Delivery `<details>` accordion tabs, and a dynamic "You May Also Love" related products block using the category products data:
```typescript
  const { data: relatedResult } = useProductsPaged(1, 4, null, null, product.categoryProductId);
  const relatedProducts = relatedResult?.items?.filter((p: any) => p.id !== product.id).slice(0, 4) || [];
```

- [x] **Step 2: Verify TypeScript Compilation**

Run: `npx tsc --noEmit` inside `cms.frontend/`
Expected: Compile successfully.

- [x] **Step 3: Commit**

```bash
git add cms.frontend/src/pages/product-detail/index.tsx
git commit -m "feat: redesign product details page with interactive gallery, size configurations, and related products"
```
