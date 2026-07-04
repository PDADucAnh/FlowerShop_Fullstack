# Phase 4b+c: Frontend Professional UX — Error Boundaries, Toast, React Query, Form Validation

**Goal:** Add enterprise-grade UX patterns: crash resilience (error boundaries), user feedback (toast), server state management (React Query), and client-side form validation (react-hook-form + zod).

**Tech Stack:** react-error-boundary v5, react-hot-toast v2, @tanstack/react-query v5, react-hook-form v7, zod v3, @hookform/resolvers

---

## 1. Error Boundaries

### 1.1 Library

Use `react-error-boundary` — provides `ErrorBoundary` component, `useErrorHandler` hook, automatic error logging, and `resetErrorBoundary` for recovery.

### 1.2 Global Error Boundary

Wrap `<Suspense>` / `<Routes>` in `App.tsx` with `ErrorBoundary`:

```tsx
import { ErrorBoundary } from 'react-error-boundary';

const ErrorFallback: React.FC<{ error: Error; resetErrorBoundary: () => void }> = ({ error, resetErrorBoundary }) => (
  <div className="min-vh-100 d-flex flex-column align-items-center justify-content-center bg-light px-3 text-center">
    <div className="size-16 bg-surface-container flex items-center justify-center text-outline mb-4 rounded-full">
      <span className="material-symbols-outlined text-4xl">error_outline</span>
    </div>
    <h2 className="fw-bold text-secondary mb-2">System Error</h2>
    <p className="text-muted small mb-4 max-w-md">{error.message}</p>
    <button className="btn btn-dark btn-sm px-4" onClick={resetErrorBoundary}>Retry</button>
  </div>
);

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <Router>
          <ErrorBoundary FallbackComponent={ErrorFallback} onError={(error) => console.error('[Global Error]', error)}>
            <AuthRedirectHandler />
            <Toaster ... />
            <div className="d-flex flex-column min-vh-100 bg-light">
              <Header />
              <main className="flex-grow-1">
                <Suspense fallback={<PageLoader />}>
                  <Routes>...</Routes>
                </Suspense>
              </main>
              <Footer />
            </div>
          </ErrorBoundary>
        </Router>
      </CartProvider>
    </AuthProvider>
  );
}
```

### 1.3 Error Logging

`onError` callback logs to console. Extensible to server-side logging later.

### 1.4 Files to Create/Modify

| File | Action |
|---|---|
| `src/components/ErrorFallback.tsx` | Create |
| `src/App.tsx` | Modify — wrap Routes with ErrorBoundary |

---

## 2. Toast Notification System

### 2.1 Library

Use `react-hot-toast` — 2kB, imperative API, customizable, works outside React components.

### 2.2 Toaster Placement

Render `<Toaster />` once in `App.tsx` above `<Routes>` but inside `<ErrorBoundary>`:

```tsx
import { Toaster } from 'react-hot-toast';

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
```

### 2.3 Migration from `alert()`

| File | Line(s) | Before | After |
|---|---|---|---|
| `pages/product-detail/index.tsx` | 49 | `alert("Added...")` | `toast.success("Added to cart")` |
| `pages/checkout/index.tsx` | 27 | `alert("empty...")` | `toast.error("Cart is empty")` |
| `pages/checkout/index.tsx` | 47 | `alert("Transaction complete")` | `toast.success("Order placed")` |
| `pages/checkout/index.tsx` | 53 | `alert("error...")` | `toast.error("Order failed")` |
| `pages/register/index.tsx` | 42 | `alert("Account commissioned")` | `toast.success("Registered")` |

### 2.4 Files to Create/Modify

| File | Action |
|---|---|
| `src/App.tsx` | Modify — add Toaster |
| `src/pages/product-detail/index.tsx` | Modify — alert → toast |
| `src/pages/checkout/index.tsx` | Modify — alert → toast |
| `src/pages/register/index.tsx` | Modify — alert → toast |

---

## 3. React Query (TanStack Query v5)

### 3.1 QueryClient Setup

Create `src/api/queryClient.ts`:

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

### 3.2 Provider in App.tsx

```tsx
<QueryClientProvider client={queryClient}>
  <AuthProvider>
    ...
  </AuthProvider>
</QueryClientProvider>
```

### 3.3 Custom Hooks

Create `src/hooks/` directory with one hook file per domain:

**`src/hooks/useProducts.ts`:**
```ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import productService from '../services/productService';
import toast from 'react-hot-toast';

export const useProducts = () =>
  useQuery({ queryKey: ['products'], queryFn: () => productService.getAllProducts() });

export const useProduct = (id: string | number) =>
  useQuery({ queryKey: ['products', id], queryFn: () => productService.getProductById(id), enabled: !!id });
```

**`src/hooks/usePosts.ts`:**
```ts
export const usePosts = () =>
  useQuery({ queryKey: ['posts'], queryFn: () => postService.getAllPosts() });

export const usePost = (id: string | number) =>
  useQuery({ queryKey: ['posts', id], queryFn: () => postService.getPostById(id), enabled: !!id });
```

**`src/hooks/useCategories.ts`:**
```ts
export const useCategories = () =>
  useQuery({ queryKey: ['categories'], queryFn: () => categoryService.getAllCategories() });
```

**`src/hooks/useOrders.ts`:**
```ts
export const useCreateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: OrderInput) => orderService.submitOrder(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      toast.success('Order placed successfully');
    },
    onError: () => {
      toast.error('Order failed. Please try again.');
    },
  });
};
```

### 3.4 Page Migration

Replace manual `useState` + `useEffect` + `fetchProducts()` pattern with custom hooks.

**Before (shop/index.tsx):**
```tsx
const [products, setProducts] = useState<any[]>([]);
const [loading, setLoading] = useState(true);
const [error, setError] = useState<string | null>(null);

useEffect(() => {
  const fetch = async () => {
    try { setLoading(true); const data = await productService.getAllProducts(); setProducts(data); }
    catch (err) { setError('Failed to load'); }
    finally { setLoading(false); }
  };
  fetch();
}, [selectedCategoryId]);
```

**After:**
```tsx
const { data: products = [], isLoading, error } = useProducts();
// filter on client or use query key dependency
```

### 3.5 Pages Affected

| Page | Current Pattern | React Query Hook |
|---|---|---|
| `shop/index.tsx` | useState + useEffect + loading | `useProducts()` |
| `home/ProductGrid.tsx` | useState + useEffect | `useProducts()` |
| `home/LatestBlog.tsx` | useState + useEffect | `usePosts()` |
| `product-detail/index.tsx` | useState + useEffect | `useProduct(id)` |
| `blog/index.tsx` | useState + useEffect + loading | `usePosts()` |
| `blog-detail/index.tsx` | useState + useEffect | `usePost(id)` + `useProducts()` |
| `home/CategoryMenu.tsx` | useState + useEffect | `useCategories()` |
| `shop/ShopSidebar.tsx` | useState + useEffect | `useCategories()` |
| `blog/BlogSidebar.tsx` | useState + useEffect | `useCategories()` |
| `checkout/index.tsx` | call + loading | `useCreateOrder()` mutation |

### 3.6 Files to Create/Modify

| File | Action |
|---|---|
| `src/api/queryClient.ts` | Create |
| `src/hooks/useProducts.ts` | Create |
| `src/hooks/usePosts.ts` | Create |
| `src/hooks/useCategories.ts` | Create |
| `src/hooks/useOrders.ts` | Create |
| `src/App.tsx` | Modify — add QueryClientProvider |
| 10 page files | Modify — use hooks |

---

## 4. Form Validation (react-hook-form + zod)

### 4.1 Schema Files

Create `src/schemas/` directory:

**`src/schemas/loginSchema.ts`:**
```ts
import { z } from 'zod';

export const loginSchema = z.object({
  username: z.string().min(1, 'Username is required').max(50),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export type LoginFormData = z.infer<typeof loginSchema>;
```

**`src/schemas/registerSchema.ts`:**
```ts
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

**`src/schemas/checkoutSchema.ts`:**
```ts
export const checkoutSchema = z.object({
  fullname: z.string().min(1, 'Full name is required'),
  phone: z.string().min(10, 'Valid phone required').max(15),
  address: z.string().min(5, 'Address is required'),
  notes: z.string().optional(),
});

export type CheckoutFormData = z.infer<typeof checkoutSchema>;
```

### 4.2 Login Page Integration

```tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema, type LoginFormData } from '../../schemas/loginSchema';

const { register, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
  resolver: zodResolver(loginSchema),
});

<form onSubmit={handleSubmit((data) => login(data.username, data.password))}>
  <input {...register('username')} className="..." placeholder="Username" />
  {errors.username && <p className="text-error text-[10px] mt-1">{errors.username.message}</p>}
  
  <input type="password" {...register('password')} className="..." placeholder="Password" />
  {errors.password && <p className="text-error text-[10px] mt-1">{errors.password.message}</p>}
  
  <button type="submit">Authenticate</button>
</form>
```

### 4.3 Files to Create/Modify

| File | Action |
|---|---|
| `src/schemas/loginSchema.ts` | Create |
| `src/schemas/registerSchema.ts` | Create |
| `src/schemas/checkoutSchema.ts` | Create |
| `src/pages/login/index.tsx` | Modify — use react-hook-form |
| `src/pages/register/index.tsx` | Modify — use react-hook-form |
| `src/pages/checkout/index.tsx` | Modify — use react-hook-form |

---

## 5. Installation

```bash
npm install react-error-boundary react-hot-toast @tanstack/react-query react-hook-form @hookform/resolvers zod
```

No @types needed — all libraries ship TypeScript declarations.

---

## 6. Build & Test Verification

```bash
cd cms.frontend && npm run build 2>&1
# Expected: 0 errors

cd .. && dotnet test CMS.Tests --configuration Release --no-restore 2>&1
# Expected: 27/27 pass
```

---

## 7. Risk Mitigation

| Risk | Likelihood | Mitigation |
|---|---|---|
| react-hook-form re-render perf on large forms | Low | RHF is designed for perf, register() minimal re-renders |
| React Query stale cache | Low | staleTime=5min, refetchOnWindowFocus=false, invalidate on mutation |
| toast duplicates on rapid clicks | Low | react-hot-toast has built-in dedup via toastId |
| Error boundary catches React dev warnings | Low | Only catches render errors, not warnings |
