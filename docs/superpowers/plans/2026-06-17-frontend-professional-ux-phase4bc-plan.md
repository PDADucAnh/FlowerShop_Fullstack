# Phase 4b+c: Frontend Professional UX Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add error boundaries, toast notifications, React Query with custom hooks, and react-hook-form+zod form validation to the AnhCMS frontend.

**Architecture:** All 4 components integrate into existing `App.tsx` as providers/wrappers + update individual page files. React Query replaces `useState`+`useEffect` patterns with `useQuery`/`useMutation` hooks. Form validation replaces manual state with `react-hook-form` + `zod` schemas.

**Tech Stack:** react-error-boundary v5, react-hot-toast v2, @tanstack/react-query v5, react-hook-form v7, zod v3, @hookform/resolvers, TypeScript strict

---

### Task 1: Install packages

**Files:**
- Modify: `cms.frontend/package.json`

- [ ] **Step 1: Install all 6 packages**

```bash
cd cms.frontend && npm install react-error-boundary react-hot-toast @tanstack/react-query react-hook-form @hookform/resolvers zod
```

Expected: packages added to `package.json` + `node_modules`, no errors

- [ ] **Step 2: Verify TypeScript resolution**

Check that the packages have type declarations (all ship their own):

```bash
cd cms.frontend && node -e "require('react-error-boundary'); require('react-hot-toast'); require('@tanstack/react-query'); require('react-hook-form'); require('@hookform/resolvers/zod'); require('zod'); console.log('All modules resolve OK');"
```

Expected: "All modules resolve OK"

---

### Task 2: Create ErrorFallback component and wrap App.tsx

**Files:**
- Create: `cms.frontend/src/components/ErrorFallback.tsx`
- Modify: `cms.frontend/src/App.tsx`

- [ ] **Step 1: Create ErrorFallback component**

```tsx
import React from 'react';

interface ErrorFallbackProps {
  error: Error;
  resetErrorBoundary: () => void;
}

const ErrorFallback: React.FC<ErrorFallbackProps> = ({ error, resetErrorBoundary }) => (
  <div className="d-flex flex-column align-items-center justify-content-center min-vh-100 bg-light px-3 text-center">
    <div className="d-flex align-items-center justify-content-center bg-secondary bg-opacity-10 text-secondary mb-4 rounded-circle" style={{ width: '64px', height: '64px' }}>
      <span className="material-symbols-outlined" style={{ fontSize: '40px' }}>error_outline</span>
    </div>
    <h2 className="fw-bold text-secondary mb-2">System Error</h2>
    <p className="text-muted small mb-4" style={{ maxWidth: '400px' }}>{error.message}</p>
    <button className="btn btn-dark btn-sm px-4" onClick={resetErrorBoundary}>Retry</button>
  </div>
);

export default ErrorFallback;
```

- [ ] **Step 2: Wrap Routes with ErrorBoundary in App.tsx**

Edit `src/App.tsx`:

Old (lines 1-8):
```tsx
import React, { lazy, Suspense, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate, Link } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { authEvents } from './utils/eventEmitter';
```

New:
```tsx
import React, { lazy, Suspense, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate, Link } from 'react-router-dom';
import { ErrorBoundary } from 'react-error-boundary';
import Header from './components/Header';
import Footer from './components/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { authEvents } from './utils/eventEmitter';
import ErrorFallback from './components/ErrorFallback';
```

Old (lines 54-83):
```tsx
const App: React.FC = () => {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <AuthRedirectHandler />
          <div className="d-flex flex-column min-vh-100 bg-light">
            <Header />
            <main className="flex-grow-1">
              <Suspense fallback={<PageLoader />}>
                <Routes>
                  <Route path="/" element={<Home />} />
                  ...
                </Routes>
              </Suspense>
            </main>
            <Footer />
          </div>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
};
```

New:
```tsx
const App: React.FC = () => {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <AuthRedirectHandler />
          <ErrorBoundary FallbackComponent={ErrorFallback} onError={(error) => console.error('[Global Error]', error)}>
            <div className="d-flex flex-column min-vh-100 bg-light">
              <Header />
              <main className="flex-grow-1">
                <Suspense fallback={<PageLoader />}>
                  <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/shop" element={<Shop />} />
                    <Route path="/product/:id" element={<ProductDetail />} />
                    <Route path="/blog" element={<Blog />} />
                    <Route path="/blog/:id" element={<BlogDetail />} />
                    <Route path="/cart" element={<Cart />} />
                    <Route path="/checkout" element={<ProtectedRoute><Checkout /></ProtectedRoute>} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="*" element={<NotFound />} />
                  </Routes>
                </Suspense>
              </main>
              <Footer />
            </div>
          </ErrorBoundary>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
};
```

- [ ] **Step 3: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 3: Add Toaster to App.tsx and migrate alert() calls (4 pages)

**Files:**
- Modify: `cms.frontend/src/App.tsx`
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`
- Modify: `cms.frontend/src/pages/checkout/index.tsx`
- Modify: `cms.frontend/src/pages/register/index.tsx`

- [ ] **Step 1: Add Toaster to App.tsx**

Add import after `ErrorBoundary` import:
```tsx
import { Toaster } from 'react-hot-toast';
```

Add `<Toaster />` inside `ErrorBoundary`, before the `<div className="d-flex flex-column...">`:

```tsx
<ErrorBoundary FallbackComponent={ErrorFallback} onError={(error) => console.error('[Global Error]', error)}>
  <Toaster
    position="top-right"
    gutter={12}
    containerClassName="font-body-md"
    toastOptions={{
      duration: 4000,
      style: { fontFamily: 'Inter', fontSize: '12px', textTransform: 'uppercase', letterSpacing: '0.1em', background: '#1a1c1c', color: '#f9f9f9' },
      success: { iconTheme: { primary: '#000', secondary: '#fff' }, style: { background: '#000' } },
      error: { iconTheme: { primary: '#ba1a1a', secondary: '#fff' }, style: { background: '#ba1a1a' } },
    }}
  />
  <div className="d-flex flex-column min-vh-100 bg-light">
```

- [ ] **Step 2: product-detail/index.tsx — replace alert with toast**

Add import:
```tsx
import toast from 'react-hot-toast';
```

Replace:
```tsx
alert(`Added ${quantity} [${product.name}] to cart!`);
```

With:
```tsx
toast.success(`Added ${quantity} x ${product.name} to cart`);
```

- [ ] **Step 3: checkout/index.tsx — replace 3 alerts with toast**

Add import:
```tsx
import toast from 'react-hot-toast';
```

Replace line 27:
```tsx
alert("Your acquisition manifest is empty.");
```
With:
```tsx
toast.error("Your acquisition manifest is empty.");
```

Replace line 47:
```tsx
alert("Transaction complete. Your acquisition has been recorded.");
```
With:
```tsx
toast.success("Transaction complete. Your acquisition has been recorded.");
```

Replace line 53:
```tsx
alert("An error occurred during the transaction. Please try again.");
```
With:
```tsx
toast.error("An error occurred during the transaction. Please try again.");
```

- [ ] **Step 4: register/index.tsx — replace alert with toast**

Add import:
```tsx
import toast from 'react-hot-toast';
```

Replace line 40:
```tsx
alert('Account commissioned successfully. Please authenticate.');
```
With:
```tsx
toast.success('Account commissioned successfully. Please authenticate.');
```

- [ ] **Step 5: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 4: Create QueryClient and QueryClientProvider

**Files:**
- Create: `cms.frontend/src/api/queryClient.ts`
- Modify: `cms.frontend/src/App.tsx`

- [ ] **Step 1: Create queryClient.ts**

```ts
import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 2,
      refetchOnWindowFocus: false,
    },
  },
});
```

- [ ] **Step 2: Add QueryClientProvider to App.tsx**

Add import:
```tsx
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './api/queryClient';
```

Wrap the entire render tree:
```tsx
<QueryClientProvider client={queryClient}>
  <AuthProvider>
    ...
  </AuthProvider>
</QueryClientProvider>
```

Final structure:
```tsx
<QueryClientProvider client={queryClient}>
  <AuthProvider>
    <CartProvider>
      <Router>
        <AuthRedirectHandler />
        <ErrorBoundary FallbackComponent={ErrorFallback} onError={(error) => console.error('[Global Error]', error)}>
          <Toaster ... />
          <div className="d-flex flex-column min-vh-100 bg-light">
            <Header />
            <main className="flex-grow-1">
              <Suspense fallback={<PageLoader />}>
                <Routes>
                  <Route path="/" element={<Home />} />
                  <Route path="/shop" element={<Shop />} />
                  <Route path="/product/:id" element={<ProductDetail />} />
                  <Route path="/blog" element={<Blog />} />
                  <Route path="/blog/:id" element={<BlogDetail />} />
                  <Route path="/cart" element={<Cart />} />
                  <Route path="/checkout" element={<ProtectedRoute><Checkout /></ProtectedRoute>} />
                  <Route path="/login" element={<Login />} />
                  <Route path="/register" element={<Register />} />
                  <Route path="*" element={<NotFound />} />
                </Routes>
              </Suspense>
            </main>
            <Footer />
          </div>
        </ErrorBoundary>
      </Router>
    </CartProvider>
  </AuthProvider>
</QueryClientProvider>
```

- [ ] **Step 3: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 5: Create React Query hooks (4 files)

**Files:**
- Create: `cms.frontend/src/hooks/useProducts.ts`
- Create: `cms.frontend/src/hooks/usePosts.ts`
- Create: `cms.frontend/src/hooks/useCategories.ts`
- Create: `cms.frontend/src/hooks/useOrders.ts`

- [ ] **Step 1: Create src/hooks/ directory**

```bash
mkdir -p cms.frontend/src/hooks
```

- [ ] **Step 2: Create useProducts.ts**

```ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import productService from '../services/productService';
import toast from 'react-hot-toast';

export const useProducts = () =>
  useQuery({ queryKey: ['products'], queryFn: () => productService.getAllProducts() });

export const useProduct = (id: string | number) =>
  useQuery({ queryKey: ['products', id], queryFn: () => productService.getProductById(id), enabled: !!id });

export const useProductsByCategory = (categoryId: number | null) =>
  useQuery({
    queryKey: ['products', 'category', categoryId],
    queryFn: () => productService.getProductsByCategory(categoryId),
    enabled: categoryId !== null,
  });
```

- [ ] **Step 3: Create usePosts.ts**

```ts
import { useQuery } from '@tanstack/react-query';
import postService from '../services/postService';

export const usePosts = () =>
  useQuery({ queryKey: ['posts'], queryFn: () => postService.getAllPosts() });

export const usePost = (id: string | number) =>
  useQuery({ queryKey: ['posts', id], queryFn: () => postService.getPostById(id), enabled: !!id });

export const usePostsByCategory = (categoryId: number | null) =>
  useQuery({
    queryKey: ['posts', 'category', categoryId],
    queryFn: () => postService.getPostsByCategory(categoryId),
    enabled: categoryId !== null,
  });
```

- [ ] **Step 4: Create useCategories.ts**

```ts
import { useQuery } from '@tanstack/react-query';
import categoryProductService from '../services/categoryProductService';
import categoryService from '../services/categoryService';

export const useProductCategories = () =>
  useQuery({ queryKey: ['categories', 'products'], queryFn: () => categoryProductService.getAllCategoryProducts() });

export const useBlogCategories = () =>
  useQuery({ queryKey: ['categories', 'blog'], queryFn: () => categoryService.getBlogCategories() });
```

- [ ] **Step 5: Create useOrders.ts**

```ts
import { useMutation, useQueryClient } from '@tanstack/react-query';
import orderService from '../services/orderService';
import toast from 'react-hot-toast';

export const useCreateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: any) => orderService.submitOrder(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      toast.success('Transaction complete. Your acquisition has been recorded.');
    },
    onError: () => {
      toast.error('An error occurred during the transaction. Please try again.');
    },
  });
};
```

- [ ] **Step 6: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 6: Migrate checkout page to React Query + form validation

**Files:**
- Modify: `cms.frontend/src/pages/checkout/index.tsx`
- Create: `cms.frontend/src/schemas/checkoutSchema.ts`

- [ ] **Step 1: Create checkout schema**

```ts
import { z } from 'zod';

export const checkoutSchema = z.object({
  fullname: z.string().min(1, 'Full name is required'),
  phone: z.string().min(10, 'Valid phone required').max(15),
  address: z.string().min(5, 'Address is required'),
  notes: z.string().optional(),
});

export type CheckoutFormData = z.infer<typeof checkoutSchema>;
```

- [ ] **Step 2: Rewrite checkout page**

Full file replacement `src/pages/checkout/index.tsx`:

```tsx
import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { useCreateOrder } from '../../hooks/useOrders';
import { checkoutSchema, type CheckoutFormData } from '../../schemas/checkoutSchema';

const CheckoutPage: React.FC = () => {
  const navigate = useNavigate();
  const { cartItems, cartTotal, clearCart } = useCart();
  const { user } = useAuth();
  const createOrder = useCreateOrder();

  const { register, handleSubmit, formState: { errors } } = useForm<CheckoutFormData>({
    resolver: zodResolver(checkoutSchema),
  });

  const onSubmit = async (formData: CheckoutFormData) => {
    if (cartItems.length === 0) return;

    const orderPayload = {
      customerId: user?.id || 0,
      notes: `Delivery to: ${formData.fullname}, Contact: ${formData.phone}, Location: ${formData.address}. Narrative: ${formData.notes}`,
      items: cartItems.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.discountPrice || item.price,
      })),
    };

    try {
      await createOrder.mutateAsync(orderPayload);
      clearCart();
      navigate('/');
    } catch {
      // toast handled by useCreateOrder onError
    }
  };

  if (cartItems.length === 0) {
    return (
      <div className="container text-center py-20">
        <div className="size-20 bg-surface-container flex items-center justify-center text-outline mb-4 rounded-full mx-auto">
          <span className="material-symbols-outlined text-4xl">shopping_bag</span>
        </div>
        <div className="space-y-md">
          <h2 className="font-display-xl text-display-xl-mobile md:text-headline-lg uppercase tracking-tighter">Manifest Empty</h2>
          <p className="text-secondary italic serif max-w-md mx-auto">You cannot proceed to checkout without items in your collection.</p>
        </div>
        <Link to="/shop" className="bg-primary text-on-primary px-xl py-4 font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold hover:bg-neutral-800 transition-all text-decoration-none inline-block mt-4">
          Return to Boutique
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20">
      <main className="max-w-[1440px] mx-auto px-margin py-xl">
        <header className="mb-xl text-center space-y-md">
            <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Secure Portal</h3>
            <h2 className="font-display-xl text-display-xl uppercase tracking-tighter">Finalize Acquisition</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col lg:flex-row gap-xl">
            <div className="flex-1 space-y-lg">
                <div className="bg-surface-container-lowest border border-outline-variant p-xl space-y-xl">
                    <h5 className="font-display-xl text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Delivery Credentials</h5>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-lg">
                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-secondary font-bold">Full Identity Name</label>
                            <input
                                type="text"
                                {...register('fullname')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-outline-variant"
                                placeholder="Recipient Name"
                            />
                            {errors.fullname && <p className="text-error text-[10px] mt-1">{errors.fullname.message}</p>}
                        </div>
                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-secondary font-bold">Telephonic Connection</label>
                            <input
                                type="text"
                                {...register('phone')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-outline-variant"
                                placeholder="Contact Number"
                            />
                            {errors.phone && <p className="text-error text-[10px] mt-1">{errors.phone.message}</p>}
                        </div>
                    </div>

                    <div className="space-y-sm">
                        <label className="text-[10px] uppercase tracking-widest text-secondary font-bold">Physical Residence</label>
                        <input
                            type="text"
                            {...register('address')}
                            className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-outline-variant"
                            placeholder="Delivery Address"
                        />
                        {errors.address && <p className="text-error text-[10px] mt-1">{errors.address.message}</p>}
                    </div>

                    <div className="space-y-sm">
                        <label className="text-[10px] uppercase tracking-widest text-secondary font-bold">Narrative Notes</label>
                        <textarea
                            {...register('notes')}
                            className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-body-md italic leading-relaxed placeholder:text-outline-variant resize-none"
                            rows={4}
                            placeholder="Special delivery instructions..."
                        ></textarea>
                    </div>
                </div>

                <div className="bg-surface-container-lowest border border-outline-variant p-xl space-y-lg">
                    <h5 className="font-display-xl text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Transaction Method</h5>
                    <label className="flex items-start gap-md p-md border border-primary bg-surface-container-low cursor-pointer">
                        <div className="flex items-center h-6">
                            <input type="radio" name="payment" defaultChecked className="size-4 text-primary focus:ring-primary border-primary bg-transparent" />
                        </div>
                        <div className="space-y-1">
                            <span className="text-label-sm uppercase tracking-widest font-bold block">Cash on Delivery (COD)</span>
                            <span className="text-[10px] text-secondary uppercase tracking-widest block">Settle transaction upon receipt of goods.</span>
                        </div>
                    </label>
                </div>
            </div>

            <aside className="w-full lg:w-96 flex-shrink-0">
                <div className="bg-surface-container-low border border-outline-variant p-lg space-y-xl sticky top-32">
                    <h5 className="text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Manifest Overview</h5>

                    <div className="space-y-md border-b border-outline-variant pb-md max-h-60 overflow-y-auto no-scrollbar">
                        {cartItems.map(item => (
                            <div className="flex justify-between items-start gap-md" key={item.id}>
                                <div className="flex-1 space-y-1">
                                    <span className="text-label-sm uppercase tracking-widest font-bold block">{item.name}</span>
                                    <span className="text-[10px] text-secondary uppercase tracking-widest block">Qty: {item.quantity}</span>
                                </div>
                                <span className="font-bold text-sm">{((item.discountPrice || item.price) * item.quantity).toLocaleString()} ₫</span>
                            </div>
                        ))}
                    </div>

                    <div className="space-y-md">
                        <div className="flex justify-between items-center text-label-sm uppercase tracking-widest">
                            <span className="text-secondary">Subtotal</span>
                            <span className="font-bold">{cartTotal.toLocaleString()} ₫</span>
                        </div>
                        <div className="flex justify-between items-center text-label-sm uppercase tracking-widest">
                            <span className="text-secondary">Delivery Insight</span>
                            <span className="text-primary font-bold uppercase tracking-widest text-[10px]">Complimentary</span>
                        </div>
                    </div>

                    <div className="border-t border-outline-variant pt-lg flex justify-between items-center">
                        <span className="text-label-sm uppercase tracking-[0.2em] font-bold">Total Acquisition</span>
                        <span className="serif text-2xl font-bold">
                            {cartTotal.toLocaleString()} ₫
                        </span>
                    </div>

                    <button
                        type="submit"
                        disabled={createOrder.isPending || cartItems.length === 0}
                        className="w-full bg-primary text-on-primary py-5 text-label-sm uppercase tracking-[0.3em] font-bold hover:bg-neutral-800 transition-all border-0 outline-none disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {createOrder.isPending ? 'Processing...' : 'Confirm Transaction'}
                    </button>

                    <div className="bg-white border border-outline-variant p-md flex items-start gap-md">
                        <span className="material-symbols-outlined text-secondary">lock</span>
                        <p className="text-[10px] text-secondary uppercase tracking-widest leading-relaxed">Secure end-to-end encryption. Your credentials are protected.</p>
                    </div>
                </div>
            </aside>
        </form>
      </main>
    </div>
  );
};

export default CheckoutPage;
```

- [ ] **Step 3: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 7: Migrate product-detail page to React Query

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.tsx`

- [ ] **Step 1: Rewrite product-detail page**

Full file replacement `src/pages/product-detail/index.tsx`:

```tsx
import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useProduct, useProducts } from '../../hooks/useProducts';
import { useCart } from '../../context/CartContext';
import { getImageUrl } from '../../utils/apiUtils';

const formatImageUrl = (url?: string, fallback?: string): string => {
  return getImageUrl(url) || fallback || '';
};

const ProductDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const { data: product, isLoading } = useProduct(id as string);
  const { data: allProducts = [] } = useProducts();
  const [quantity, setQuantity] = useState(1);
  const [activeTab, setActiveTab] = useState('desc');

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [id]);

  const toggleTab = (tabId: string) => {
    setActiveTab(activeTab === tabId ? '' : tabId);
  };

  const relatedProducts = product
    ? allProducts.filter((p: any) => p.id !== product.id).slice(0, 4)
    : [];

  const handleAddToCart = () => {
    if (product) {
      addToCart(product, quantity);
      toast.success(`Added ${quantity} x ${product.name} to cart`);
    }
  };

  // Loading, empty, and render states are identical to current code
  // — keep all JSX below exactly as-is, only replacing the loading variable name
  // from `loading` to `isLoading`

  if (isLoading) {
    return (
      <div className="bg-background min-vh-100 flex items-center justify-center">
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="container py-20 text-center">
        <h2 className="font-display-xl text-headline-lg">Sản phẩm không tồn tại</h2>
        <Link to="/shop" className="text-primary underline mt-4 inline-block">Quay lại cửa hàng</Link>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-md text-body-md antialiased overflow-x-hidden pt-20">
      <style>{`
        .hide-scrollbar::-webkit-scrollbar { display: none; }
        .hide-scrollbar { -ms-overflow-style: none; scrollbar-width: none; }
      `}</style>

      <main className="w-full max-w-[1440px] mx-auto px-5 md:px-margin pt-lg pb-xl grid grid-cols-1 md:grid-cols-12 gap-gutter relative">
        <div className="md:col-span-12 font-label-sm text-label-sm text-secondary uppercase tracking-widest flex items-center space-x-2 mb-md">
          <Link className="hover:text-primary transition-colors text-decoration-none" to="/">Home</Link>
          <span>/</span>
          <Link className="hover:text-primary transition-colors text-decoration-none" to="/shop">Shop</Link>
          <span>/</span>
          <span className="text-primary truncate">{product.name}</span>
        </div>

        <div className="md:col-span-7 lg:col-span-8 flex flex-col gap-gutter">
          <div className="w-full aspect-[4/5] bg-surface-container relative group overflow-hidden">
            <img
              alt={product.name}
              className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
              src={formatImageUrl(product.imageUrl, "https://via.placeholder.com/800x1000")}
            />
          </div>
        </div>

        <div className="md:col-span-5 lg:col-span-4 md:sticky md:top-32 h-fit flex flex-col pt-md md:pt-0">
          <div className="mb-lg">
            <span className="font-label-sm text-label-sm text-secondary uppercase tracking-widest mb-2 block">SKU: ANH-{product.id}</span>
            <h1 className="font-display-xl-mobile md:font-headline-lg text-display-xl-mobile md:text-headline-lg text-primary mb-4">{product.name}</h1>
            <div className="flex gap-3 items-center">
              <p className="font-body-lg text-body-lg text-primary mb-0">
                {(product.discountPrice || product.price).toLocaleString()} ₫
              </p>
              {product.discountPrice > 0 && (
                <p className="font-body-md text-body-md text-secondary line-through mb-0 opacity-60">
                  {product.price.toLocaleString()} ₫
                </p>
              )}
            </div>
          </div>

          <div className="mb-md">
            <div className="flex justify-between items-center mb-xs">
              <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Color: Default</span>
            </div>
            <div className="flex space-x-3">
              <button aria-label="Default" className="w-8 h-8 rounded-full border border-primary relative bg-transparent">
                <span className="absolute inset-[2px] rounded-full bg-[#000000]"></span>
              </button>
            </div>
          </div>

          <div className="mb-lg">
            <div className="flex justify-between items-center mb-sm">
              <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Size</span>
              <button className="font-label-sm text-label-sm uppercase tracking-widest text-secondary hover:text-primary underline underline-offset-4 transition-colors bg-transparent border-0">Size Guide</button>
            </div>
            <div className="grid grid-cols-5 gap-2">
              {['XS', 'S', 'M', 'L', 'XL'].map(size => (
                <button
                  key={size}
                  className={`py-3 border ${size === 'S' ? 'border-primary bg-primary text-on-primary' : 'border-outline hover:border-primary text-primary'} font-label-sm text-label-sm transition-colors uppercase bg-transparent`}
                >
                  {size}
                </button>
              ))}
            </div>
          </div>

          <div className="flex flex-col gap-sm mb-xl">
            <div className="flex gap-sm h-14">
              <div className="w-1/3 border border-primary flex items-center justify-between px-3">
                <button
                  aria-label="Decrease quantity"
                  className="text-primary hover:text-secondary p-1 bg-transparent border-0"
                  onClick={() => setQuantity(Math.max(1, quantity - 1))}
                >
                  <span className="material-symbols-outlined text-sm">remove</span>
                </button>
                <span className="font-body-md text-body-md">{quantity}</span>
                <button
                  aria-label="Increase quantity"
                  className="text-primary hover:text-secondary p-1 bg-transparent border-0"
                  onClick={() => setQuantity(quantity + 1)}
                >
                  <span className="material-symbols-outlined text-sm">add</span>
                </button>
              </div>
              <button
                className="w-2/3 bg-primary text-on-primary border border-primary font-label-sm text-label-sm uppercase tracking-widest hover:bg-transparent hover:text-primary transition-colors duration-300"
                onClick={handleAddToCart}
              >
                Add to Cart
              </button>
            </div>
            <button
                onClick={() => { addToCart(product, quantity); navigate('/checkout'); }}
                className="w-full h-14 bg-transparent text-primary border border-primary font-label-sm text-label-sm uppercase tracking-widest hover:bg-primary hover:text-on-primary transition-colors duration-300"
            >
              Buy Now
            </button>
            <button className="flex items-center justify-center gap-2 text-secondary hover:text-primary transition-colors mt-xs py-2 bg-transparent border-0">
              <span className="material-symbols-outlined">favorite</span>
              <span className="font-label-sm text-label-sm uppercase tracking-widest">Add to Wishlist</span>
            </button>
          </div>

          <div className="border-t border-outline-variant pt-sm">
            <button className="w-full py-sm flex justify-between items-center group bg-transparent border-0" onClick={() => toggleTab('desc')}>
              <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Description</span>
              <span className="material-symbols-outlined text-secondary group-hover:text-primary transition-transform duration-300">
                {activeTab === 'desc' ? 'remove' : 'add'}
              </span>
            </button>
            {activeTab === 'desc' && (
              <div className="pb-md text-secondary font-body-md text-body-md leading-relaxed">
                {product.description || 'Chưa có mô tả chi tiết cho sản phẩm này.'}
              </div>
            )}

            <div className="border-t border-surface-variant">
              <button className="w-full py-sm flex justify-between items-center group bg-transparent border-0" onClick={() => toggleTab('specs')}>
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Specifications</span>
                <span className="material-symbols-outlined text-secondary group-hover:text-primary transition-transform duration-300">
                  {activeTab === 'specs' ? 'remove' : 'add'}
                </span>
              </button>
              {activeTab === 'specs' && (
                <div className="pb-md text-secondary font-body-md text-body-md">
                  <ul className="list-none space-y-2 p-0 m-0">
                    <li><span className="text-primary font-medium">Material:</span> Premium Fabric</li>
                    <li><span className="text-primary font-medium">Care:</span> Dry clean only</li>
                    <li><span className="text-primary font-medium">Origin:</span> Imported</li>
                  </ul>
                </div>
              )}
            </div>

            <div className="border-t border-surface-variant border-b mb-xl">
              <button className="w-full py-sm flex justify-between items-center group bg-transparent border-0" onClick={() => toggleTab('shipping')}>
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Shipping & Returns</span>
                <span className="material-symbols-outlined text-secondary group-hover:text-primary transition-transform duration-300">
                  {activeTab === 'shipping' ? 'remove' : 'add'}
                </span>
              </button>
              {activeTab === 'shipping' && (
                <div className="pb-md text-secondary font-body-md text-body-md">
                  Giao hàng miễn phí cho đơn hàng trên 2.000.000 ₫. Đổi trả trong vòng 7 ngày nếu có lỗi sản xuất.
                </div>
              )}
            </div>
          </div>
        </div>
      </main>

      <section className="w-full border-t border-secondary-container bg-surface py-xl">
        <div className="max-w-[1440px] mx-auto px-5 md:px-margin">
          <div className="flex justify-between items-end mb-lg">
            <h2 className="font-headline-sm text-headline-sm uppercase tracking-widest text-primary">Complete the Look</h2>
          </div>
          <div className="flex gap-gutter overflow-x-auto hide-scrollbar pb-sm snap-x">
            {relatedProducts.map((rp) => (
              <Link key={rp.id} to={`/product/${rp.id}`} className="min-w-[280px] md:min-w-[320px] w-[280px] md:w-[320px] snap-start group cursor-pointer text-decoration-none">
                <div className="w-full aspect-[4/5] bg-surface-container relative mb-4 overflow-hidden">
                  <img
                    alt={rp.name}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700"
                    src={formatImageUrl(rp.imageUrl, "https://via.placeholder.com/400x500")}
                  />
                  <div className="absolute bottom-4 right-4 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                    <button
                      className="w-10 h-10 bg-primary text-on-primary rounded-full flex items-center justify-center hover:scale-110 transition-transform border-0"
                      onClick={(e) => { e.preventDefault(); addToCart(rp); }}
                    >
                      <span className="material-symbols-outlined text-sm">shopping_bag</span>
                    </button>
                  </div>
                </div>
                <h3 className="font-body-md text-body-md text-primary mb-1">{rp.name}</h3>
                <p className="font-body-md text-body-md text-secondary">{(rp.discountPrice || rp.price).toLocaleString()} ₫</p>
              </Link>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
};

export default ProductDetailPage;
```

- [ ] **Step 2: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 8: Migrate shop and blog pages to React Query

**Files:**
- Modify: `cms.frontend/src/pages/shop/index.tsx`
- Modify: `cms.frontend/src/pages/blog/index.tsx`
- Modify: `cms.frontend/src/pages/blog-detail/index.tsx`

- [ ] **Step 1: Rewrite shop/index.tsx**

Full file replacement `src/pages/shop/index.tsx`:

```tsx
import React, { useState } from 'react';
import ShopSidebar from './ShopSidebar';
import ShopHeader from './ShopHeader';
import ProductList from './ProductList';
import { useProducts } from '../../hooks/useProducts';

const ShopPage: React.FC = () => {
  const { data: products = [], isLoading, error } = useProducts();
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

  const filteredProducts = selectedCategoryId
    ? products.filter((p: any) => p.categoryProductId === selectedCategoryId)
    : products;

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
  };

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20">
      <main className="max-w-[1440px] mx-auto px-margin py-xl">
        <header className="mb-xl text-center space-y-md">
            <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Curated Boutique</h3>
            <h2 className="font-display-xl text-display-xl uppercase tracking-tighter">The Collection</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <div className="flex flex-col lg:flex-row gap-xl">
          <aside className="w-full lg:w-72 flex-shrink-0">
            <ShopSidebar onCategoryChange={handleCategoryChange} activeId={selectedCategoryId} />
          </aside>

          <div className="flex-1 space-y-lg">
            <ShopHeader count={filteredProducts.length} />
            <ProductList products={filteredProducts} isLoading={isLoading} error={error ? "Unable to curate the collection at this time." : null} />
          </div>
        </div>
      </main>
    </div>
  );
};

export default ShopPage;
```

- [ ] **Step 2: Rewrite blog/index.tsx**

Full file replacement `src/pages/blog/index.tsx`:

```tsx
import React, { useState } from 'react';
import BlogSidebar from './BlogSidebar';
import PostCard from '../../components/PostCard';
import { usePosts } from '../../hooks/usePosts';
import type { Post } from '../../types/post';

const BlogPage: React.FC = () => {
  const { data: posts = [], isLoading, error } = usePosts();
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

  const filteredPosts = selectedCategoryId
    ? posts.filter((p: Post) => p.categoryId === selectedCategoryId)
    : posts;

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
  };

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20">
      <main className="max-w-[1440px] mx-auto px-margin py-xl">
        <header className="mb-xl text-center space-y-md">
            <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Editorial Narrative</h3>
            <h2 className="font-display-xl text-display-xl uppercase tracking-tighter text-primary">The Fashion Journal</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <div className="flex flex-col lg:flex-row gap-xl">
          <div className="flex-1 order-2 lg:order-1">
            {isLoading ? (
              <div className="text-center py-20">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-12 bg-surface-container rounded-full mb-md"></div>
                    <p className="text-label-sm uppercase tracking-widest text-secondary">Retrieving Narratives...</p>
                </div>
              </div>
            ) : error ? (
              <div className="p-lg bg-error-container text-error text-label-sm uppercase tracking-widest font-bold text-center border border-error">Unable to retrieve editorial stories at this time.</div>
            ) : filteredPosts.length === 0 ? (
              <div className="text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
                <span className="material-symbols-outlined text-4xl text-outline mb-md">article</span>
                <p className="text-label-sm uppercase tracking-widest text-secondary">No editorial stories found in this pillar.</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-xl">
                {filteredPosts.map((post) => (
                    <PostCard key={post.id} post={post} />
                ))}
              </div>
            )}
          </div>

          <aside className="w-full lg:w-80 flex-shrink-0 order-1 lg:order-2">
            <BlogSidebar onCategoryChange={handleCategoryChange} activeId={selectedCategoryId} />
          </aside>
        </div>
      </main>
    </div>
  );
};

export default BlogPage;
```

- [ ] **Step 3: Rewrite blog-detail/index.tsx**

Full file replacement `src/pages/blog-detail/index.tsx`:

```tsx
import React, { useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { usePost } from '../../hooks/usePosts';
import { useProducts } from '../../hooks/useProducts';
import { useCart } from '../../context/CartContext';
import DOMPurify from 'dompurify';
import { getImageUrl } from '../../utils/apiUtils';
import type { Product } from '../../types/product';

const BlogDetail: React.FC = () => {
    const { id } = useParams();
    const { addToCart } = useCart();
    const { data: post, isLoading } = usePost(id as string);
    const { data: allProducts = [] } = useProducts();

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [id]);

    const products = allProducts.slice(0, 4);

    if (isLoading) {
        return (
            <div className="bg-background min-vh-100 flex items-center justify-center">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-16 bg-surface-container rounded-full mb-md"></div>
                    <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Retrieving Narrative...</p>
                </div>
            </div>
        );
    }

    if (!post) {
        return (
            <div className="container py-20 text-center">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tighter">Narrative Not Found</h2>
                <Link to="/blog" className="text-primary font-label-sm uppercase tracking-widest border-b border-primary pb-1 mt-4 inline-block text-decoration-none">Back to Journal</Link>
            </div>
        );
    }

    // JSX from here is identical to current code — keep as-is
    // (postImage uses `getImageUrl(post.imageUrl)` which is unchanged)
    const postImage = getImageUrl(post.imageUrl);

    return (
        <div className="bg-background text-on-background antialiased selection:bg-primary selection:text-on-primary font-body-md pt-20">
            <main>
                <section className="w-full relative h-[614px] md:h-[819px] flex items-end pb-xl px-margin bg-surface-variant overflow-hidden">
                    <img
                        alt={post.title}
                        className="absolute inset-0 w-full h-full object-cover z-0"
                        src={postImage || "https://via.placeholder.com/1920x1080"}
                    />
                    <div className="absolute inset-0 bg-gradient-to-t from-primary/80 to-transparent z-10"></div>
                    <div className="relative z-20 max-w-4xl mx-auto text-center w-full">
                        <div className="mb-sm">
                            <span className="font-label-sm text-label-sm text-surface-container-highest uppercase border border-surface-container-highest px-xs py-[2px] rounded-DEFAULT">
                                {post.categoryName || 'Style Guide'}
                            </span>
                        </div>
                        <h1 className="font-display-xl text-display-xl-mobile md:text-display-xl text-on-primary mb-md uppercase tracking-tighter drop-shadow-lg">{post.title}</h1>
                        <p className="font-body-lg text-body-lg text-white/80 max-w-2xl mx-auto italic serif">{post.summary || 'Mastering transitions with structured silhouettes and fine textiles.'}</p>
                        <div className="mt-lg flex items-center justify-center gap-xs text-white/60 font-label-sm text-[10px] uppercase tracking-[0.2em] font-bold">
                            <span>By Editorial Team</span>
                            <span className="mx-2">/</span>
                            <span>{new Date(post.createdDate || Date.now()).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}</span>
                        </div>
                    </div>
                </section>

                <div className="max-w-[1440px] mx-auto px-margin py-xl md:py-[120px] grid grid-cols-1 md:grid-cols-12 gap-gutter relative">
                    <aside className="md:col-span-1 hidden md:flex flex-col items-center gap-md sticky top-32 h-fit">
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">share</span>
                        </button>
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">bookmark</span>
                        </button>
                        <div className="w-px h-12 bg-outline-variant my-xs"></div>
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">print</span>
                        </button>
                    </aside>

                    <article className="md:col-span-7 lg:col-span-6 md:col-start-3 lg:col-start-4">
                        <div className="prose prose-lg max-w-none">
                            <div
                                className="blog-detail-content font-body-lg text-body-lg text-on-surface-variant mb-lg leading-relaxed first-letter:text-5xl first-letter:font-display-xl first-letter:float-left first-letter:mr-3 first-letter:text-primary"
                                dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(post.content) }}
                            ></div>
                        </div>

                        <div className="flex flex-wrap gap-sm mt-xl pt-lg border-t border-outline-variant">
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">EDITORIAL</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">LATEST TRENDS</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">STYLING</span>
                        </div>
                    </article>
                </div>

                <section className="bg-surface-container-low py-[120px] px-margin border-y border-outline-variant">
                    <div className="max-w-[1440px] mx-auto">
                        <div className="flex flex-col md:flex-row justify-between items-end mb-xl gap-md">
                            <div>
                                <h3 className="font-display-xl text-headline-lg text-primary mb-xs uppercase tracking-tighter">Shop The Editorial</h3>
                                <p className="font-body-md text-body-md text-secondary italic serif">Curated pieces to recreate the layered aesthetic.</p>
                            </div>
                            <Link className="font-label-sm text-label-sm text-primary uppercase tracking-widest border-b border-primary pb-1 hover:text-secondary hover:border-secondary transition-colors duration-300 text-decoration-none font-bold" to="/shop">View All</Link>
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-gutter">
                            {products.map((p) => (
                                <Link className="group block text-decoration-none" to={`/product/${p.id}`} key={p.id}>
                                    <div className="aspect-[4/5] bg-surface mb-sm overflow-hidden relative">
                                        <img
                                            alt={p.name}
                                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700 ease-out"
                                            src={getImageUrl(p.imageUrl)}
                                        />
                                        <div className="absolute inset-0 bg-black/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                    </div>
                                    <h4 className="font-body-md text-sm text-primary font-bold uppercase tracking-tight mb-1">{p.name}</h4>
                                    <p className="font-body-md text-sm text-secondary">{(p.price).toLocaleString()} ₫</p>
                                </Link>
                            ))}
                        </div>
                    </div>
                </section>

                <div className="max-w-[1440px] mx-auto px-margin py-[120px] grid grid-cols-1 md:grid-cols-12 gap-gutter">
                    <div className="md:col-span-5 lg:col-span-4 bg-surface p-xl border border-outline-variant flex flex-col justify-center">
                        <h3 className="font-display-xl text-headline-sm text-primary mb-sm uppercase tracking-tighter">The Journal</h3>
                        <p className="font-body-md text-body-md text-secondary mb-lg italic serif">Subscribe to receive editorial updates, early access to collections, and styling insights.</p>
                        <form className="flex flex-col gap-md">
                            <div className="relative">
                                <input className="block w-full px-0 py-sm text-body-md bg-transparent border-0 border-b border-outline appearance-none focus:outline-none focus:ring-0 focus:border-primary peer transition-colors" id="email" placeholder=" " type="email" />
                                <label className="absolute font-label-sm text-label-sm text-secondary duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:left-0 peer-focus:text-primary peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6 uppercase tracking-widest" htmlFor="email">Email Address</label>
                            </div>
                            <button className="w-full bg-primary text-on-primary font-label-sm text-label-sm uppercase py-sm rounded-none border border-primary hover:bg-transparent hover:text-primary transition-colors duration-300 font-bold tracking-[0.2em]" type="button">Subscribe</button>
                        </form>
                    </div>

                    <div className="md:col-span-7 lg:col-span-7 md:col-start-6 lg:col-start-6 mt-xl md:mt-0">
                        <h3 className="font-headline-sm text-headline-sm text-primary mb-lg border-b border-outline-variant pb-sm uppercase tracking-widest">Discussion</h3>
                        <div className="space-y-lg mb-xl">
                            <p className="text-secondary italic serif">The conversation has not yet begun. Share your perspective below.</p>
                        </div>
                        <div>
                            <h4 className="font-body-md text-sm font-bold text-primary mb-sm uppercase tracking-widest">Leave a comment</h4>
                            <form className="flex flex-col gap-md">
                                <div className="relative">
                                    <textarea className="block w-full px-0 py-sm text-body-md bg-transparent border-0 border-b border-outline appearance-none focus:outline-none focus:ring-0 focus:border-primary peer transition-colors resize-none" id="comment" placeholder=" " rows={4}></textarea>
                                    <label className="absolute font-label-sm text-label-sm text-secondary duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:left-0 peer-focus:text-primary peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6 uppercase tracking-widest" htmlFor="comment">Your narrative</label>
                                </div>
                                <button className="self-start bg-transparent text-primary font-label-sm text-label-sm uppercase py-sm px-lg rounded-none border border-primary hover:bg-primary hover:text-on-primary transition-colors duration-300 font-bold tracking-[0.2em]" type="button">Post Comment</button>
                            </form>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default BlogDetail;
```

- [ ] **Step 4: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 9: Migrate home page components to React Query

**Files:**
- Modify: `cms.frontend/src/pages/home/ProductGrid.tsx`
- Modify: `cms.frontend/src/pages/home/LatestBlog.tsx`
- Modify: `cms.frontend/src/pages/home/CategoryMenu.tsx`

- [ ] **Step 1: Rewrite ProductGrid.tsx**

Full file replacement `src/pages/home/ProductGrid.tsx`:

```tsx
import React from 'react';
import { useProducts } from '../../hooks/useProducts';
import ProductCard from '../../components/ProductCard';

interface ProductGridProps {
  categoryId: number | null;
}

function ProductGrid({ categoryId }: ProductGridProps) {
    const { data: products = [], isLoading } = useProducts();

    if (isLoading) {
        return (
            <div className="px-margin my-xl text-center">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-12 bg-surface-container rounded-full mb-md"></div>
                    <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Curating Collection...</p>
                </div>
            </div>
        );
    }

    const displayProducts = categoryId
        ? products.filter((p: any) => p.categoryProductId === categoryId)
        : products;

    return (
        <section className="px-margin mb-xl">
            <div className="flex justify-between items-end mb-lg">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tight">Trending Now</h2>
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">
                    Displaying ({displayProducts.length}) Pieces
                </span>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter">
                {displayProducts.map((product) => (
                    <ProductCard key={product.id} item={product} />
                ))}
                {displayProducts.length === 0 && (
                    <div className="col-span-full text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
                        <span className="material-symbols-outlined text-4xl text-outline mb-md">inventory_2</span>
                        <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">No pieces found in this collection</p>
                    </div>
                )}
            </div>
        </section>
    );
}

export default ProductGrid;
```

- [ ] **Step 2: Rewrite LatestBlog.tsx**

Full file replacement `src/pages/home/LatestBlog.tsx`:

```tsx
import React from 'react';
import { usePosts } from '../../hooks/usePosts';
import PostCard from '../../components/PostCard';

function LatestBlog() {
    const { data: posts = [], isLoading } = usePosts();

    if (isLoading) return null;

    const topThreePosts = [...posts].sort((a: any, b: any) => b.id - a.id).slice(0, 3);

    return (
        <section className="px-margin mb-xl py-xl bg-surface">
            <div className="text-center mb-xl">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tight mb-sm">XU HƯỚNG THỜI TRANG</h2>
                <p className="text-secondary font-body-md max-w-xl mx-auto italic serif">Cập nhật những mẹo phối đồ và tin tức phong cách mới nhất cùng AnhCMS.Fashion</p>
                <div className="w-12 h-0.5 bg-primary mx-auto mt-md"></div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-gutter">
                {topThreePosts.map((item) => (
                    <PostCard key={item.id} post={item} />
                ))}
            </div>
        </section>
    );
}

export default LatestBlog;
```

- [ ] **Step 3: Rewrite CategoryMenu.tsx**

Full file replacement `src/pages/home/CategoryMenu.tsx`:

```tsx
import React from 'react';
import { useProductCategories } from '../../hooks/useCategories';
import { Link } from 'react-router-dom';
import type { Category } from '../../types/category';

interface CategoryMenuProps {
  onSelectCategory: (id: number | null) => void;
  activeId: number | null;
}

function CategoryMenu({ onSelectCategory, activeId }: CategoryMenuProps) {
    const { data: categories = [], isLoading } = useProductCategories();

    if (isLoading) return null;

    const handleCategoryClick = (id: number | null) => {
        if (onSelectCategory) {
            onSelectCategory(id);
        }
    };

    return (
        <section className="px-margin mb-xl">
            <div className="flex justify-between items-end mb-lg">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tight">Explore Collections</h2>
                <Link to="/shop" className="font-label-sm text-label-sm uppercase tracking-widest hover:text-secondary transition-colors border-b border-primary pb-1 text-decoration-none text-primary">View All</Link>
            </div>

            <div className="flex gap-md overflow-x-auto no-scrollbar pb-4">
                <button
                    onClick={() => handleCategoryClick(null)}
                    className={`flex-shrink-0 px-lg py-4 font-label-sm text-label-sm uppercase tracking-widest transition-all border ${activeId === null ? 'bg-primary text-on-primary border-primary' : 'bg-surface text-secondary border-outline-variant hover:border-primary hover:text-primary'}`}
                >
                    All Collections
                </button>
                {(categories as any[]).map((cat: any) => (
                    <button
                        key={cat.id}
                        onClick={() => handleCategoryClick(cat.id)}
                        className={`flex-shrink-0 px-lg py-4 font-label-sm text-label-sm uppercase tracking-widest transition-all border ${activeId === cat.id ? 'bg-primary text-on-primary border-primary' : 'bg-surface text-secondary border-outline-variant hover:border-primary hover:text-primary'}`}
                    >
                        {cat.name}
                    </button>
                ))}
            </div>
        </section>
    );
}

export default CategoryMenu;
```

- [ ] **Step 4: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 10: Migrate sidebar components to React Query

**Files:**
- Modify: `cms.frontend/src/pages/shop/ShopSidebar.tsx`
- Modify: `cms.frontend/src/pages/blog/BlogSidebar.tsx`

- [ ] **Step 1: Rewrite ShopSidebar.tsx**

Full file replacement `src/pages/shop/ShopSidebar.tsx`:

```tsx
import React from 'react';
import { useProductCategories } from '../../hooks/useCategories';

interface ShopSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeId: number | null;
}

const ShopSidebar = ({ onCategoryChange, activeId }: ShopSidebarProps) => {
  const { data: categories = [] } = useProductCategories();

  return (
    <div className="space-y-xl">
      <div className="space-y-md">
        <h6 className="text-[10px] uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Division</h6>
        <ul className="space-y-sm list-none p-0">
          <li>
            <button
              className={`bg-transparent border-0 p-0 text-label-sm uppercase tracking-widest transition-all ${activeId === null ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
              onClick={() => onCategoryChange(null)}
            >
              The Full Collection
            </button>
          </li>
          {(categories as any[]).map((cat: any) => (
            <li key={cat.id}>
              <button
                className={`bg-transparent border-0 p-0 text-label-sm uppercase tracking-widest transition-all ${activeId === cat.id ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
                onClick={() => onCategoryChange(cat.id)}
              >
                {cat.name}
              </button>
            </li>
          ))}
        </ul>
      </div>

      <div className="space-y-md">
        <h6 className="text-[10px] uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Price Bracket</h6>
        <div className="space-y-sm">
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">Under 500k đ</span>
            </label>
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">500k - 2M đ</span>
            </label>
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">Over 2M đ</span>
            </label>
        </div>
      </div>
    </div>
  );
};

export default ShopSidebar;
```

- [ ] **Step 2: Rewrite BlogSidebar.tsx**

Full file replacement `src/pages/blog/BlogSidebar.tsx`:

```tsx
import React from 'react';
import { useBlogCategories } from '../../hooks/useCategories';
import type { Category } from '../../types/category';

interface BlogSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeId: number | null;
}

const BlogSidebar = ({ onCategoryChange, activeId }: BlogSidebarProps) => {
  const { data: categories = [] } = useBlogCategories();

  return (
    <div className="space-y-xl">
      <div className="space-y-md">
        <h6 className="text-[10px] uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Editorial Pillars</h6>
        <ul className="space-y-sm list-none p-0">
          <li>
            <button
              className={`bg-transparent border-0 p-0 text-label-sm uppercase tracking-widest transition-all ${activeId === null ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
              onClick={() => onCategoryChange(null)}
            >
              The Full Narrative
            </button>
          </li>
          {(categories as Category[]).map((cat) => (
            <li key={cat.id}>
              <button
                className={`bg-transparent border-0 p-0 text-label-sm uppercase tracking-widest transition-all ${activeId === cat.id ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
                onClick={() => onCategoryChange(cat.id)}
              >
                {cat.name}
              </button>
            </li>
          ))}
        </ul>
      </div>

      <div className="space-y-md">
        <h6 className="text-[10px] uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Journal Highlights</h6>
        <div className="space-y-lg">
          <div className="group cursor-pointer">
            <a href="#" className="text-body-md font-bold uppercase tracking-tight text-primary group-hover:text-secondary transition-colors text-decoration-none block mb-1 leading-tight">Mastering the Evening Silhouette</a>
            <span className="text-[10px] text-outline uppercase tracking-widest font-bold">12 June 2026</span>
          </div>
          <div className="group cursor-pointer">
            <a href="#" className="text-body-md font-bold uppercase tracking-tight text-primary group-hover:text-secondary transition-colors text-decoration-none block mb-1 leading-tight">Essential Minimalism for the Office</a>
            <span className="text-[10px] text-outline uppercase tracking-widest font-bold">10 June 2026</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BlogSidebar;
```

- [ ] **Step 3: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 11: Create form validation schemas + integrate Login and Register pages

**Files:**
- Create: `cms.frontend/src/schemas/loginSchema.ts`
- Create: `cms.frontend/src/schemas/registerSchema.ts`
- Modify: `cms.frontend/src/pages/login/index.tsx`
- Modify: `cms.frontend/src/pages/register/index.tsx`

- [ ] **Step 1: Create loginSchema.ts**

```ts
import { z } from 'zod';

export const loginSchema = z.object({
  username: z.string().min(1, 'Username is required').max(50),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export type LoginFormData = z.infer<typeof loginSchema>;
```

- [ ] **Step 2: Create registerSchema.ts**

```ts
import { z } from 'zod';

export const registerSchema = z.object({
  username: z.string().min(3, 'Min 3 characters').max(50),
  fullName: z.string().min(1, 'Full name is required').max(100),
  email: z.string().email('Invalid email'),
  password: z.string().min(6, 'Min 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
});

export type RegisterFormData = z.infer<typeof registerSchema>;
```

- [ ] **Step 3: Rewrite login/index.tsx with react-hook-form**

Full file replacement `src/pages/login/index.tsx`:

```tsx
import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useAuth } from '../../context/AuthContext';
import { loginSchema, type LoginFormData } from '../../schemas/loginSchema';

const LoginPage: React.FC = () => {
    const [error, setError] = React.useState<string>('');
    const [loading, setLoading] = React.useState<boolean>(false);
    const navigate = useNavigate();
    const { login } = useAuth();

    const { register, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = async (data: LoginFormData) => {
        setError('');
        setLoading(true);
        try {
            await login(data.username, data.password);
            navigate('/');
        } catch {
            setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="bg-background text-on-background font-body-md antialiased pt-20 flex flex-col min-h-screen">
            <main className="flex-1 flex items-center justify-center p-sm py-20">
                <div className="w-full max-w-md space-y-xl">
                    <div className="text-center space-y-md">
                        <div className="inline-flex size-16 bg-black items-center justify-center mb-4">
                            <svg className="text-white size-10" fill="none" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
                                <path d="M42.4379 44C42.4379 44 36.0744 33.9038 41.1692 24C46.8624 12.9336 42.2078 4 42.2078 4L7.01134 4C7.01134 4 11.6577 12.932 5.96912 23.9969C0.876273 33.9029 7.27094 44 7.27094 44L42.4379 44Z" fill="currentColor"></path>
                            </svg>
                        </div>
                        <h1 className="serif text-4xl font-bold tracking-tighter uppercase">AnhCMS.Fashion</h1>
                        <p className="text-[10px] text-neutral-500 uppercase tracking-[0.4em] font-bold">Secure Personnel Authentication</p>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)} className="bg-white border border-neutral-200 p-lg space-y-lg shadow-sm">
                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Identity Code</label>
                            <input
                                type="text"
                                {...register('username')}
                                className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-neutral-300"
                                placeholder="Username"
                            />
                            {errors.username && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.username.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Security Token</label>
                            <input
                                type="password"
                                {...register('password')}
                                className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm tracking-widest placeholder:text-neutral-300"
                                placeholder="Password"
                            />
                            {errors.password && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.password.message}</p>}
                        </div>

                        {error && (
                            <div className="p-md bg-red-50 border border-red-100 text-red-600 text-[10px] uppercase tracking-widest font-bold text-center">
                                {error}
                            </div>
                        )}

                        <button
                            type="submit"
                            disabled={loading}
                            className="w-full bg-black text-white py-4 text-[10px] font-bold uppercase tracking-[0.3em] hover:bg-neutral-800 transition-all border-0 outline-none"
                        >
                            {loading ? 'Authenticating...' : 'Authenticate'}
                        </button>

                        <div className="text-center pt-4 border-t border-outline-variant">
                            <p className="text-[10px] text-secondary uppercase tracking-widest">
                                New Member? <Link to="/register" className="text-primary font-bold hover:underline ml-2">Register Access</Link>
                            </p>
                        </div>
                    </form>

                    <div className="text-center">
                        <p className="text-[10px] text-neutral-400 uppercase tracking-widest">© 2024 International Holdings. All Rights Reserved.</p>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default LoginPage;
```

- [ ] **Step 4: Rewrite register/index.tsx with react-hook-form**

Full file replacement `src/pages/register/index.tsx`:

```tsx
import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import authService from '../../services/authService';
import toast from 'react-hot-toast';
import { registerSchema, type RegisterFormData } from '../../schemas/registerSchema';

const RegisterPage: React.FC = () => {
    const [error, setError] = React.useState<string>('');
    const [loading, setLoading] = React.useState<boolean>(false);
    const navigate = useNavigate();

    const { register, handleSubmit, formState: { errors } } = useForm<RegisterFormData>({
        resolver: zodResolver(registerSchema),
    });

    const onSubmit = async (data: RegisterFormData) => {
        setError('');
        setLoading(true);
        try {
            const success = await authService.register({
                username: data.username,
                fullName: data.fullName,
                email: data.email,
                password: data.password,
            });
            if (success) {
                toast.success('Account commissioned successfully. Please authenticate.');
                navigate('/login');
            } else {
                setError('Registration failed. Identity code may already be in use.');
            }
        } catch {
            setError('An error occurred during commissioning.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="bg-background text-on-background font-body-md antialiased pt-20 flex flex-col min-h-screen">
            <main className="flex-1 flex items-center justify-center p-sm py-20">
                <div className="w-full max-w-md space-y-xl">
                    <div className="text-center space-y-md">
                        <div className="inline-flex size-16 bg-black items-center justify-center mb-4">
                            <svg className="text-white size-10" fill="none" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
                                <path d="M42.4379 44C42.4379 44 36.0744 33.9038 41.1692 24C46.8624 12.9336 42.2078 4 42.2078 4L7.01134 4C7.01134 4 11.6577 12.932 5.96912 23.9969C0.876273 33.9029 7.27094 44 7.27094 44L42.4379 44Z" fill="currentColor"></path>
                            </svg>
                        </div>
                        <h1 className="serif text-4xl font-bold tracking-tighter uppercase">AnhCMS.Fashion</h1>
                        <p className="text-[10px] text-neutral-500 uppercase tracking-[0.4em] font-bold">New Identity Commissioning</p>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)} className="bg-white border border-neutral-200 p-lg space-y-lg shadow-sm">
                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Identity Code (Username)</label>
                            <input
                                type="text"
                                {...register('username')}
                                className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-neutral-300"
                                placeholder="Username"
                            />
                            {errors.username && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.username.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Full Nomenclature</label>
                            <input
                                type="text"
                                {...register('fullName')}
                                className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm font-semibold tracking-widest uppercase placeholder:text-neutral-300"
                                placeholder="Full Name"
                            />
                            {errors.fullName && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.fullName.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Electronic Mail</label>
                            <input
                                type="email"
                                {...register('email')}
                                className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm tracking-widest placeholder:text-neutral-300"
                                placeholder="Email"
                            />
                            {errors.email && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.email.message}</p>}
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-md">
                            <div className="space-y-sm">
                                <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Security Token</label>
                                <input
                                    type="password"
                                    {...register('password')}
                                    className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm tracking-widest placeholder:text-neutral-300"
                                    placeholder="Password"
                                />
                                {errors.password && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.password.message}</p>}
                            </div>
                            <div className="space-y-sm">
                                <label className="text-[10px] uppercase tracking-widest text-neutral-500 font-bold">Verify Token</label>
                                <input
                                    type="password"
                                    {...register('confirmPassword')}
                                    className="w-full bg-neutral-50 border-none focus:ring-1 focus:ring-black px-md py-4 text-sm tracking-widest placeholder:text-neutral-300"
                                    placeholder="Confirm"
                                />
                                {errors.confirmPassword && <p className="text-red-600 text-[10px] uppercase tracking-widest font-bold mt-1">{errors.confirmPassword.message}</p>}
                            </div>
                        </div>

                        {error && (
                            <div className="p-md bg-red-50 border border-red-100 text-red-600 text-[10px] uppercase tracking-widest font-bold text-center">
                                {error}
                            </div>
                        )}

                        <button
                            type="submit"
                            disabled={loading}
                            className="w-full bg-black text-white py-4 text-[10px] font-bold uppercase tracking-[0.3em] hover:bg-neutral-800 transition-all border-0 outline-none"
                        >
                            {loading ? 'Commissioning...' : 'Commission Account'}
                        </button>

                        <div className="text-center pt-4 border-t border-outline-variant">
                            <p className="text-[10px] text-secondary uppercase tracking-widest">
                                Already Commissioned? <Link to="/login" className="text-primary font-bold hover:underline ml-2">Authenticate Access</Link>
                            </p>
                        </div>
                    </form>

                    <div className="text-center">
                        <p className="text-[10px] text-neutral-400 uppercase tracking-widest">© 2024 International Holdings. All Rights Reserved.</p>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default RegisterPage;
```

- [ ] **Step 5: Build to verify**

```bash
cd cms.frontend && npx react-scripts build 2>&1 | tail -10
```

Expected: "Compiled successfully." with 0 errors

---

### Task 12: Run backend tests

**Files:**
- Test: `CMS.Tests/`

- [ ] **Step 1: Run all backend tests**

```bash
dotnet test CMS.Tests --configuration Release --no-restore 2>&1
```

Expected: "Passed! - Failed: 0" (27/27 pass)

---

### Task 13: Commit and push

- [ ] **Step 1: Verify git status**

```bash
git status
```

Expected: staged tracked files, all new files shown as untracked

- [ ] **Step 2: Stage all files**

```bash
git add cms.frontend/src/package.json cms.frontend/package-lock.json cms.frontend/src/components/ErrorFallback.tsx cms.frontend/src/App.tsx cms.frontend/src/api/queryClient.ts cms.frontend/src/hooks/useProducts.ts cms.frontend/src/hooks/usePosts.ts cms.frontend/src/hooks/useCategories.ts cms.frontend/src/hooks/useOrders.ts cms.frontend/src/schemas/loginSchema.ts cms.frontend/src/schemas/registerSchema.ts cms.frontend/src/schemas/checkoutSchema.ts cms.frontend/src/pages/checkout/index.tsx cms.frontend/src/pages/product-detail/index.tsx cms.frontend/src/pages/register/index.tsx cms.frontend/src/pages/shop/index.tsx cms.frontend/src/pages/blog/index.tsx cms.frontend/src/pages/blog-detail/index.tsx cms.frontend/src/pages/home/ProductGrid.tsx cms.frontend/src/pages/home/LatestBlog.tsx cms.frontend/src/pages/home/CategoryMenu.tsx cms.frontend/src/pages/shop/ShopSidebar.tsx cms.frontend/src/pages/blog/BlogSidebar.tsx cms.frontend/src/pages/login/index.tsx
```

- [ ] **Step 3: Commit**

```bash
git commit -m "feat: add error boundaries, toast system, React Query hooks, and form validation"
```

- [ ] **Step 4: Push**

```bash
git push origin Buoi_09
```
