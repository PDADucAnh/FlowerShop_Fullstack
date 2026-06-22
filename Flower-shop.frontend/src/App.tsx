import React, { lazy, Suspense, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate, Link } from 'react-router-dom';
import { ErrorBoundary } from 'react-error-boundary';
import Header from './components/Header';
import Footer from './components/Footer';
import ProtectedRoute from './components/ProtectedRoute';
import { AuthProvider } from './context/AuthContext';
import { CartProvider } from './context/CartContext';
import { WishlistProvider } from './context/WishlistContext';
import { authEvents } from './utils/eventEmitter';
import ErrorFallback from './components/ErrorFallback';
import { Toaster } from 'react-hot-toast';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './api/queryClient';

const Home = lazy(() => import('./pages/home/index'));
const Shop = lazy(() => import('./pages/shop/index'));
const ProductDetail = lazy(() => import('./pages/product-detail/index'));
const Blog = lazy(() => import('./pages/blog/index'));
const BlogDetail = lazy(() => import('./pages/blog-detail/index'));
const Cart = lazy(() => import('./pages/cart/index'));
const Checkout = lazy(() => import('./pages/checkout/index'));
const Login = lazy(() => import('./pages/login/index'));
const Register = lazy(() => import('./pages/register/index'));
const Profile = lazy(() => import('./pages/auth/Profile'));
const MyOrders = lazy(() => import('./pages/auth/MyOrders'));
const Wishlist = lazy(() => import('./pages/wishlist/index'));
const About = lazy(() => import('./pages/about/index'));

const PageLoader: React.FC = () => (
  <div className="flex justify-center items-center min-h-screen bg-surface">
    <div className="text-center">
      <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin mb-md mx-auto" role="status">
        <span className="sr-only">Loading...</span>
      </div>
      <p className="font-label-sm text-label-sm text-secondary uppercase tracking-widest">Curating Narrative...</p>
    </div>
  </div>
);

const AuthRedirectHandler: React.FC = () => {
  const navigate = useNavigate();
  useEffect(() => {
    const unsubscribe = authEvents.on('unauthorized', () => navigate('/login'));
    return unsubscribe;
  }, [navigate]);
  return null;
};

const NotFound: React.FC = () => (
  <div className="text-center py-xl px-margin-mobile max-w-container-max mx-auto">
    <div className="w-24 h-24 mx-auto mb-md opacity-60 flex items-center justify-center">
      <span className="material-symbols-outlined text-6xl text-outline">error_outline</span>
    </div>
    <h2 className="font-headline-md text-headline-md text-secondary uppercase tracking-tight mb-sm">404 - Page Not Found</h2>
    <p className="text-secondary font-body-md mb-lg">The page you're looking for doesn't exist.</p>
    <Link to="/" className="bg-primary text-on-primary px-8 py-3 font-label-sm text-label-sm uppercase tracking-widest border border-primary inline-block text-decoration-none btn-luxury btn-primary-luxury">Back to Home</Link>
  </div>
);

const App: React.FC = () => {
  return (
    <QueryClientProvider client={queryClient}>
    <AuthProvider>
      <CartProvider>
        <WishlistProvider>
        <Router>
          <AuthRedirectHandler />
          <ErrorBoundary FallbackComponent={ErrorFallback} onError={(error) => console.error('[Global Error]', error)}>
            <Toaster
              position="top-right"
              gutter={12}
              containerClassName="font-body-md"
              toastOptions={{
                duration: 4000,
                style: { fontFamily: 'Plus Jakarta Sans', fontSize: '12px', textTransform: 'uppercase', letterSpacing: '0.1em', background: '#1b1c1c', color: '#fbf9f8' },
                success: { iconTheme: { primary: '#ab2c5d', secondary: '#fff' }, style: { background: '#1b1c1c' } },
                error: { iconTheme: { primary: '#ba1a1a', secondary: '#fff' }, style: { background: '#ba1a1a' } },
              }}
            />
            <div className="flex flex-col min-h-screen bg-surface">
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
                    <Route path="/profile" element={<ProtectedRoute><Profile /></ProtectedRoute>} />
                    <Route path="/my-orders" element={<ProtectedRoute><MyOrders /></ProtectedRoute>} />
                    <Route path="/wishlist" element={<Wishlist />} />
                    <Route path="/about" element={<About />} />
                    <Route path="*" element={<NotFound />} />
                  </Routes>
                </Suspense>
              </main>
              <Footer />
            </div>
          </ErrorBoundary>
        </Router>
        </WishlistProvider>
      </CartProvider>
    </AuthProvider>
    </QueryClientProvider>
  );
};

export default App;
