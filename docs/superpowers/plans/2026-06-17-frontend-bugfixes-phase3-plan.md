# Phase 3: Frontend Bug Fixes — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fix all critical and major frontend bugs in the React SPA.

**Architecture:** Minimal changes — fix bugs in-place, no refactoring. Add `dompurify` for XSS sanitization, `getImageUrl()` utility for centralized image URL handling.

**Tech Stack:** React 19, CRA 5, Tailwind CSS (CDN), Axios, React Router v7

---

### Task 1: Fix Login (🔴 Critical)

**Files:**
- Modify: `cms.frontend/src/pages/login/index.jsx`

**Bug:** `login/index.jsx:20` calls `authService.login()` directly instead of `useAuth().login()`. Token never saved, auth state never set.

- [ ] **Step 1: Read current login/index.jsx**

```bash
cat cms.frontend/src/pages/login/index.jsx
```

- [ ] **Step 2: Fix the login function**

Current:
```jsx
const handleLogin = async (e) => {
  e.preventDefault();
  const data = await authService.login(username, password);
  if (data?.token) {
    navigate('/');
  }
};
```

Replace with:
```jsx
const { login } = useAuth();

const handleLogin = async (e) => {
  e.preventDefault();
  try {
    await login(username, password);
    navigate('/');
  } catch (err) {
    setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
  }
};
```

Also add `import { useAuth } from '../../context/AuthContext';` at top.
Remove the unused `import authService from '../../services/authService';` (or keep it if other things use it — check first).

- [ ] **Step 3: Build and verify**

```bash
cd cms.frontend && npm run build 2>&1 | head -20
```

Expected: Build succeeds (0 errors).

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/login/index.jsx
git commit -m "fix: login now uses useAuth context to properly save token and set auth state"
```

---

### Task 2: Fix XSS in Blog Detail (🔴 Critical)

**Files:**
- Modify: `cms.frontend/src/pages/blog-detail/index.jsx`

**Bug:** `dangerouslySetInnerHTML` renders `post.content` without sanitization.

- [ ] **Step 1: Install dompurify**

```bash
cd cms.frontend && npm install dompurify
```

- [ ] **Step 2: Fix the sanitization**

In `blog-detail/index.jsx`, add import:
```jsx
import DOMPurify from 'dompurify';
```

Replace:
```jsx
dangerouslySetInnerHTML={{ __html: post.content }}
```

With:
```jsx
dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(post.content) }}
```

- [ ] **Step 3: Build and verify**

```bash
npm run build 2>&1 | head -20
```

Expected: Build succeeds.

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/blog-detail/index.jsx cms.frontend/package.json cms.frontend/package-lock.json
git commit -m "fix: sanitize blog content with DOMPurify to prevent XSS"
```

---

### Task 3: Fix Hardcoded customerId in Checkout (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/pages/checkout/index.jsx`

**Bug:** `customerId: 1` hardcoded instead of real user ID.

- [ ] **Step 1: Read checkout/index.jsx current code**

- [ ] **Step 2: Fix to use AuthContext**

```jsx
import { useAuth } from '../../context/AuthContext';

// Inside component:
const { user } = useAuth();

// In the API call, replace:
// customerId: 1
// with:
customerId: user?.id || 0
```

- [ ] **Step 3: Build and verify**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/checkout/index.jsx
git commit -m "fix: use real user ID from AuthContext instead of hardcoded customerId"
```

---

### Task 4: Fix Footer Newsletter Form Reload (🔴 Critical)

**Files:**
- Modify: `cms.frontend/src/components/Footer.jsx`

**Bug:** `<form>` has no `onSubmit` → full page POST reload.

- [ ] **Step 1: Find the form in Footer.jsx**

Look for `<form>` around line 21.

- [ ] **Step 2: Add onSubmit handler**

```jsx
<form
  onSubmit={(e) => {
    e.preventDefault();
    console.log('Newsletter subscribe would happen here');
  }}
>
```

Also add a `useState` for the email input and wire it to the `<input>`:
```jsx
const [email, setEmail] = useState('');

// In JSX:
<input
  type="email"
  placeholder="Địa chỉ email..."
  value={email}
  onChange={(e) => setEmail(e.target.value)}
/>
```

Don't forget `import { useState } from 'react';` at the top.

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/components/Footer.jsx
git commit -m "fix: add onSubmit handler to footer newsletter form to prevent page reload"
```

---

### Task 5: Fix 401 Redirect Loses State (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/api/axiosClient.js`

**Bug:** `window.location.href = '/login'` causes full browser navigation → all React state lost.

- [ ] **Step 1: Create a simple event emitter utility**

Create `cms.frontend/src/utils/eventEmitter.js`:
```js
class EventEmitter {
  constructor() {
    this.listeners = {};
  }
  on(event, callback) {
    if (!this.listeners[event]) this.listeners[event] = [];
    this.listeners[event].push(callback);
    return () => {
      this.listeners[event] = this.listeners[event].filter(cb => cb !== callback);
    };
  }
  emit(event, ...args) {
    (this.listeners[event] || []).forEach(cb => cb(...args));
  }
}

export const authEvents = new EventEmitter();
```

- [ ] **Step 2: Update axiosClient to emit event instead of redirect**

In `cms.frontend/src/api/axiosClient.js`, replace:
```js
window.location.href = '/login';
```
With:
```js
import { authEvents } from '../utils/eventEmitter';
authEvents.emit('unauthorized');
```

Add the import at the top of the file.

- [ ] **Step 3: Update App.jsx to listen for auth events**

In `cms.frontend/src/App.jsx`:
```jsx
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authEvents } from './utils/eventEmitter';

// Inside App component:
const navigate = useNavigate();
useEffect(() => {
  const unsubscribe = authEvents.on('unauthorized', () => {
    navigate('/login');
  });
  return unsubscribe;
}, [navigate]);
```

- [ ] **Step 4: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 5: Commit**

```bash
git add cms.frontend/src/utils/eventEmitter.js cms.frontend/src/api/axiosClient.js cms.frontend/src/App.jsx
git commit -m "fix: replace window.location redirect on 401 with SPA event-based navigation"
```

---

### Task 6: Fix 404 Page Uses `<a>` Instead of `<Link>` (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/App.jsx`

- [ ] **Step 1: Find the 404 Not Found component in App.jsx**

- [ ] **Step 2: Replace `<a href="/">` with `<Link to="/">`**

```jsx
import { Link } from 'react-router-dom';

// Replace:
// <a href="/" className="...">Quay lại trang chủ</a>
// With:
<Link to="/" className="...">Quay lại trang chủ</Link>
```

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/App.jsx
git commit -m "fix: use Link instead of a tag to preserve SPA navigation on 404 page"
```

---

### Task 7: Fix Cart Ignores discountPrice (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/pages/cart/CartTable.jsx`

**Bug:** Uses `item.price` instead of `(item.discountPrice || item.price)`.

- [ ] **Step 1: Read CartTable.jsx to find the price calculations**

Look for `item.price` references around lines 42 and 62.

- [ ] **Step 2: Fix price to use discountPrice**

Replace every occurrence of `item.price` used for display/calculation with:
```jsx
const displayPrice = item.discountPrice || item.price;
```

Or inline: `{(item.discountPrice || item.price)}`

Specific changes:
- Line ~42: change `item.price` to `(item.discountPrice || item.price)`
- Line ~62: change `item.price * item.quantity` to `(item.discountPrice || item.price) * item.quantity`

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/cart/CartTable.jsx
git commit -m "fix: use discountPrice when calculating cart line totals"
```

---

### Task 8: Fix Related Products Button Missing onClick (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.jsx`

**Bug:** "Add to cart" button in related products section is non-functional.

- [ ] **Step 1: Find the related products section**

Look for the related products button around line 281.

- [ ] **Step 2: Wire up the button**

Replace:
```jsx
<button className="...">Add to Cart</button>
```
With:
```jsx
<button className="..." onClick={() => addToCart(rp)}>Add to Cart</button>
```

Make sure `addToCart` from `useCart()` is available in scope.

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/product-detail/index.jsx
git commit -m "fix: wire up related products add-to-cart button"
```

---

### Task 9: Fix Image Gallery — Show Single Image (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/pages/product-detail/index.jsx`

**Bug:** Three identical images displayed when backend only returns one.

- [ ] **Step 1: Find the image gallery section (lines ~107-126)**

- [ ] **Step 2: Collapse to single image**

Replace the gallery layout that shows 3 images with a single main image:

```jsx
{/* Main product image */}
<div className="mb-4">
  <img
    src={product.imageUrl || '/placeholder.png'}
    alt={product.name}
    className="w-full h-96 object-cover rounded-lg"
  />
</div>
```

Remove the thumbnail row below.

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/product-detail/index.jsx
git commit -m "fix: collapse product image gallery to single image (backend has no multi-image support)"
```

---

### Task 10: Fix sort() Mutates Original Array (🟠 Major)

**Files:**
- Modify: `cms.frontend/src/pages/home/LatestBlog.jsx`

- [ ] **Step 1: Find the sort call at line 14**

- [ ] **Step 2: Fix to avoid mutation**

Replace:
```js
data.sort((a, b) => b.id - a.id).slice(0, 4)
```
With:
```js
[...data].sort((a, b) => b.id - a.id).slice(0, 4)
```

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/src/pages/home/LatestBlog.jsx
git commit -m "fix: use spread to avoid mutating array in LatestBlog sort"
```

---

### Task 11: Fix Hardcoded API URL Fallback — Create Utilities (🟠 Major)

**Files:**
- Create: `cms.frontend/src/utils/apiUtils.js`
- Modify: `cms.frontend/src/api/axiosClient.js`
- Modify: `cms.frontend/src/components/ProductCard.jsx`
- Modify: `cms.frontend/src/components/PostCard.jsx`
- Modify: `cms.frontend/src/pages/product-detail/index.jsx`
- Modify: `cms.frontend/src/pages/blog-detail/index.jsx`
- Modify: `cms.frontend/src/pages/cart/CartTable.jsx`

**Bug:** `https://localhost:7224` hardcoded everywhere as fallback. Production fails silently if `.env` missing.

- [ ] **Step 1: Create apiUtils.js**

```js
// cms.frontend/src/utils/apiUtils.js

const getBaseUrl = () => {
  const url = process.env.REACT_APP_API_URL;
  if (!url) {
    if (process.env.NODE_ENV === 'production') {
      throw new Error('REACT_APP_API_URL environment variable is required in production');
    }
    return 'https://localhost:7224';
  }
  return url;
};

export const API_BASE_URL = getBaseUrl();

export const getImageUrl = (path) => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${API_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
```

- [ ] **Step 2: Update axiosClient.js to use API_BASE_URL**

In `cms.frontend/src/api/axiosClient.js`:
```js
import { API_BASE_URL } from '../utils/apiUtils';

const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' }
});
```

- [ ] **Step 3: Update all image references**

In each file, replace the hardcoded `https://localhost:7224` prefix with `getImageUrl()`:

**ProductCard.jsx** — replace:
```js
const imageUrl = product.imageUrl
  ? `https://localhost:7224${product.imageUrl}`
  : `https://via.placeholder.com/...`;
```
With:
```js
import { getImageUrl } from '../../utils/apiUtils';
const imageUrl = getImageUrl(product.imageUrl);
```

**PostCard.jsx, CartTable.jsx, product-detail/index.jsx, blog-detail/index.jsx** — same pattern.

In each file, check how the image URL is currently constructed and replace the hardcoded base URL with `getImageUrl()`.

- [ ] **Step 4: Build**

```bash
npm run build 2>&1 | head -20
```

- [ ] **Step 5: Commit**

```bash
git add cms.frontend/src/utils/apiUtils.js cms.frontend/src/api/axiosClient.js cms.frontend/src/components/ProductCard.jsx cms.frontend/src/components/PostCard.jsx cms.frontend/src/pages/cart/CartTable.jsx cms.frontend/src/pages/product-detail/index.jsx cms.frontend/src/pages/blog-detail/index.jsx
git commit -m "fix: centralize API URL and image URL construction, validate env in production"
```

---

### Task 12: Remove Unused Packages + Fix index.html (🟡 Minor)

**Files:**
- Modify: `cms.frontend/package.json`
- Modify: `cms.frontend/public/index.html`

- [ ] **Step 1: Uninstall unused packages**

```bash
cd cms.frontend && npm uninstall react-transition-group @mui/material @emotion/react @emotion/styled
```

- [ ] **Step 2: Fix index.html metadata**

In `cms.frontend/public/index.html`:
- Change `<title>React App</title>` to `<title>AnhCMS.Fashion</title>`
- Fix canonical URL from `https://localhost:7224` to `https://anhcm.shop` (or whatever the real domain is — if unknown, remove the canonical link entirely)
- Update copyright references to use current year

- [ ] **Step 3: Build**

```bash
npm run build 2>&1 | head -10
```

- [ ] **Step 4: Commit**

```bash
git add cms.frontend/package.json cms.frontend/public/index.html cms.frontend/package-lock.json
git commit -m "chore: remove unused MUI and react-transition-group dependencies, fix HTML metadata"
```

---

### Task 13: Final Build and Verification

- [ ] **Step 1: Full build**

```bash
cd cms.frontend && npm run build 2>&1
```

Expected: Build succeeds with 0 errors.

- [ ] **Step 2: Check for leftover hardcoded localhost URLs**

```bash
rg "localhost:7224" cms.frontend/src/
```

Expected: No matches (should all be replaced by `getImageUrl()` or `API_BASE_URL`).

- [ ] **Step 3: Commit any remaining changes**

```bash
git add -A
git status
```

- [ ] **Step 4: Push to Buoi_09**

```bash
git push origin Buoi_09
```
