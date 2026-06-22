import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useProduct, useProducts } from '../../hooks/useProducts';
import { useCart } from '../../context/CartContext';
import { useWishlist } from '../../context/WishlistContext';
import { getImageUrl } from '../../utils/apiUtils';

const formatImageUrl = (url?: string): string => {
  return getImageUrl(url) || '';
};

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const ProductDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const { isFavorite, toggleFavorite } = useWishlist();
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
    }
  };

  if (isLoading) {
    return (
      <div className="bg-surface min-h-screen flex items-center justify-center">
        <div className="animate-pulse flex flex-col items-center">
          <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin mb-md" role="status">
            <span className="sr-only">Loading...</span>
          </div>
          <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Curating Product...</p>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center py-xl px-margin-mobile max-w-container-max mx-auto">
        <h2 className="font-headline-md text-headline-md text-secondary uppercase tracking-tight mb-md">Product Not Found</h2>
        <Link to="/shop" className="text-primary font-label-sm uppercase tracking-widest inline-block text-decoration-none btn-link-luxury">Back to Shop</Link>
      </div>
    );
  }

  return (
    <div className="bg-surface text-on-surface font-body-md antialiased">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <div className="font-label-sm text-label-sm text-secondary uppercase tracking-widest flex items-center space-x-2 mb-md">
          <Link to="/" className="hover:text-primary transition-colors text-decoration-none">Home</Link>
          <span>/</span>
          <Link to="/shop" className="hover:text-primary transition-colors text-decoration-none">Shop</Link>
          <span>/</span>
          <span className="text-primary truncate">{product.name}</span>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-12 gap-gutter">
          <div className="md:col-span-7 lg:col-span-8">
            <div className="w-full aspect-[4/5] bg-surface-container rounded-xl overflow-hidden relative group">
              <img alt={product.name} className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" src={formatImageUrl(product.imageUrl)} />
            </div>
          </div>

          <div className="md:col-span-5 lg:col-span-4 md:sticky md:top-32 h-fit flex flex-col pt-md md:pt-0">
            <div className="mb-lg">
              <span className="font-label-sm text-label-sm text-secondary uppercase tracking-widest mb-2 block">SKU: FLW-{product.id}</span>
              <h1 className="font-headline-md text-headline-md text-on-surface mb-4">{product.name}</h1>
              <p className="text-primary font-bold text-2xl">{formatCurrency(product.discountPrice || product.price)}</p>
              {product.discountPrice > 0 && (
                <p className="text-secondary line-through text-sm">{formatCurrency(product.price)}</p>
              )}
            </div>

            <div className="flex flex-col gap-sm mb-xl">
              <div className="flex gap-sm h-14">
                <div className="w-1/3 border border-primary flex items-center justify-between px-3">
                  <button aria-label="Decrease quantity" className="text-primary hover:text-secondary p-1 bg-transparent border-0"
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}>
                    <span className="material-symbols-outlined text-sm">remove</span>
                  </button>
                  <span className="font-body-md text-body-md">{quantity}</span>
                  <button aria-label="Increase quantity" className="text-primary hover:text-secondary p-1 bg-transparent border-0"
                    onClick={() => setQuantity(quantity + 1)}>
                    <span className="material-symbols-outlined text-sm">add</span>
                  </button>
                </div>
                <button className="w-2/3 bg-primary text-on-primary border border-primary font-label-sm text-label-sm uppercase tracking-widest btn-luxury btn-primary-luxury"
                  onClick={handleAddToCart}>
                  Add to Cart
                </button>
              </div>
              <button onClick={() => { addToCart(product, quantity); navigate('/checkout'); }}
                className="w-full h-14 bg-transparent text-primary border border-primary font-label-sm text-label-sm uppercase tracking-widest btn-luxury btn-outline-luxury">
                Buy Now
              </button>
              <button className="flex items-center justify-center gap-2 mt-xs py-2 bg-transparent border-0 btn-link-luxury"
                onClick={() => toggleFavorite(product)}>
                <span className={`material-symbols-outlined ${isFavorite(product.id) ? 'text-error fill' : 'text-secondary'}`}>
                  favorite
                </span>
                <span className="font-label-sm text-label-sm uppercase tracking-widest">
                  {isFavorite(product.id) ? 'Saved' : 'Add to Wishlist'}
                </span>
              </button>
            </div>

            <div className="border-t border-outline-variant pt-sm space-y-1">
              <button className="w-full py-sm flex justify-between items-center group bg-transparent border-0" onClick={() => toggleTab('desc')}>
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Description</span>
                <span className="material-symbols-outlined text-secondary group-hover:text-primary transition-transform duration-300">
                  {activeTab === 'desc' ? 'remove' : 'add'}
                </span>
              </button>
              {activeTab === 'desc' && (
                <div className="pb-md text-secondary font-body-md text-body-md leading-relaxed">
                  {product.description || 'No description available for this arrangement.'}
                </div>
              )}

              <div className="border-t border-outline-variant">
                <button className="w-full py-sm flex justify-between items-center group bg-transparent border-0" onClick={() => toggleTab('shipping')}>
                  <span className="font-label-sm text-label-sm uppercase tracking-widest text-primary">Shipping & Returns</span>
                  <span className="material-symbols-outlined text-secondary group-hover:text-primary transition-transform duration-300">
                    {activeTab === 'shipping' ? 'remove' : 'add'}
                  </span>
                </button>
                {activeTab === 'shipping' && (
                  <div className="pb-md text-secondary font-body-md text-body-md">
                    Complimentary delivery on all acquisitions over 500.000 ₫. Return within 7 days for store credit.
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </main>

      {relatedProducts.length > 0 && (
        <section className="border-t border-outline-variant/50 bg-surface py-stack-lg">
          <div className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop">
            <div className="flex justify-between items-end mb-lg">
              <h2 className="font-headline-sm text-headline-sm text-on-surface">Complete the Look</h2>
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
              {relatedProducts.map((rp: any) => (
                <Link key={rp.id} to={`/product/${rp.id}`} className="group cursor-pointer text-decoration-none">
                  <div className="w-full aspect-[4/5] bg-surface-container rounded-xl overflow-hidden relative mb-3">
                    <img alt={rp.name} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700" src={formatImageUrl(rp.imageUrl)} />
                  </div>
                  <h3 className="font-label-md text-label-md text-on-surface truncate">{rp.name}</h3>
                  <p className="font-body-md text-body-md text-primary font-semibold">{formatCurrency(rp.discountPrice || rp.price)}</p>
                </Link>
              ))}
            </div>
          </div>
        </section>
      )}
    </div>
  );
};

export default ProductDetailPage;
