import React from 'react';
import { Link } from 'react-router-dom';
import { useWishlist } from '../../context/WishlistContext';
import { getImageUrl } from '../../utils/apiUtils';

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const WishlistPage: React.FC = () => {
  const { favorites, removeFavorite } = useWishlist();

  return (
    <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen">
      <main className="flex-grow w-full max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <div className="mb-stack-lg pb-8 border-b border-primary">
          <h1 className="font-headline-md text-headline-md text-on-surface uppercase text-center md:text-left">
            Wishlist
          </h1>
          <p className="font-label-sm text-label-sm text-secondary uppercase tracking-[0.2em] text-center md:text-left mt-4">
            {favorites.length} {favorites.length === 1 ? 'Saved Item' : 'Saved Items'}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-12 gap-gutter">
          <aside className="md:col-span-3 border-r border-primary pr-8 hidden md:block">
            <ul className="space-y-6">
              <li>
                <Link to="/profile" className="font-label-sm text-label-sm uppercase tracking-[0.2em] text-secondary hover:text-primary transition-colors pl-4 block text-decoration-none">
                  General Info
                </Link>
              </li>
              <li>
                <Link to="/my-orders" className="font-label-sm text-label-sm uppercase tracking-[0.2em] text-secondary hover:text-primary transition-colors pl-4 block text-decoration-none">
                  Order History
                </Link>
              </li>
              <li>
                <span className="font-label-sm text-label-sm uppercase tracking-[0.2em] text-primary font-bold border-l-2 border-primary pl-4 block cursor-default">
                  Wishlist
                </span>
              </li>
            </ul>
          </aside>

          <div className="md:col-span-9 md:pl-12">
            {favorites.length === 0 ? (
              <div className="text-center py-xl border border-dashed border-outline-variant rounded-xl">
                <span className="material-symbols-outlined text-6xl text-outline mb-md">favorite</span>
                <h2 className="font-headline-sm text-headline-sm text-secondary uppercase mb-sm">Your wishlist is empty</h2>
                <p className="font-body-md text-body-md text-secondary">Save your favorite pieces for later.</p>
                <Link to="/shop" className="inline-block mt-md bg-primary text-on-primary px-8 py-3 font-label-md text-label-md uppercase tracking-widest border border-primary rounded-lg btn-luxury btn-primary-luxury text-decoration-none">
                  Explore Collection
                </Link>
              </div>
            ) : (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 md:gap-8">
                {favorites.map((product) => (
                  <div key={product.id} className="group">
                    <Link to={`/product/${product.id}`} className="text-decoration-none block">
                      <div className="aspect-[4/5] bg-surface-container relative overflow-hidden mb-4 rounded-xl">
                        <img src={getImageUrl(product.imageUrl)} alt={product.name}
                          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700" />
                        <button onClick={(e) => { e.preventDefault(); removeFavorite(product.id); }}
                          className="absolute top-3 right-3 w-9 h-9 bg-surface/80 backdrop-blur-sm border-0 flex items-center justify-center hover:bg-surface transition-colors rounded-full"
                          aria-label="Remove from wishlist">
                          <span className="material-symbols-outlined text-lg text-error fill">favorite</span>
                        </button>
                      </div>
                    </Link>
                    <h3 className="font-label-md text-label-md text-on-surface mb-1 truncate">{product.name}</h3>
                    <p className="font-body-md text-body-md text-primary font-semibold">{formatCurrency(product.discountPrice || product.price)}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default WishlistPage;
