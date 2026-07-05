import React from 'react';
import { Link } from 'react-router-dom';
import { useWishlist } from '../../context/WishlistContext';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';
import { AccountSidebar } from '../../components/OrderComponents';
import SEO from '../../components/SEO';
import { useScrollReveal } from '../../hooks/useScrollReveal';

const WishlistPage: React.FC = () => {
  const { favorites, removeFavorite } = useWishlist();
  const { ref, isVisible } = useScrollReveal({ threshold: 0 });

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Yêu thích" description="Danh sách yêu thích" />
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
        <div className="flex flex-col md:flex-row gap-stack-lg">
          <AccountSidebar />

          <section
            ref={ref}
            className="flex-grow transition-all duration-700"
            style={{
              opacity: isVisible ? 1 : 0,
              transform: isVisible ? 'translateY(0)' : 'translateY(16px)',
              transitionTimingFunction: 'cubic-bezier(0.16, 1, 0.3, 1)',
            }}
          >
            {favorites.length === 0 ? (
              <div className="bg-surface-container-lowest p-stack-lg rounded-xl petal-shadow text-center py-12 max-w-md mx-auto">
                <span className="material-symbols-outlined text-5xl text-outline mb-md">favorite</span>
                <h2 className="font-headline-sm text-headline-sm text-on-surface mb-sm">Chưa có sản phẩm yêu thích</h2>
                <p className="text-on-surface-variant font-body-md mb-lg">Lưu các sản phẩm yêu thích của bạn — chúng sẽ ở đây chờ bạn.</p>
                <Link
                  to="/shop"
                  className="group inline-flex items-center gap-3 bg-primary text-on-primary px-10 py-4 font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none rounded-lg btn-luxury btn-primary-luxury"
                >
                  Xem sản phẩm
                  <span className="w-6 h-6 rounded-full bg-white/20 flex items-center justify-center group-hover:translate-x-0.5 group-hover:-translate-y-[1px] transition-transform duration-300">
                    <span className="material-symbols-outlined text-[14px]">arrow_forward</span>
                  </span>
                </Link>
              </div>
            ) : (
              <div>
                <h2 className="font-headline-sm text-headline-sm text-on-surface mb-stack-lg">
                  Yêu thích
                  <span className="text-on-surface-variant font-body-md text-body-md font-normal ml-2">({favorites.length} sản phẩm)</span>
                </h2>
                <div className="grid grid-cols-2 md:grid-cols-3 gap-gutter">
                  {favorites.map((product: any) => (
                    <div
                      key={product.id}
                      className="bg-surface-container-lowest rounded-xl petal-shadow group overflow-hidden border border-transparent hover:border-outline-variant transition-all duration-300"
                    >
                      <div className="aspect-[4/5] relative overflow-hidden">
                        <Link to={`/product/${product.id}`} className="block w-full h-full">
                          <img
                            src={getImageUrl(product.imageUrl)}
                            alt={product.name}
                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                            loading="lazy"
                          />
                        </Link>
                        <button
                          onClick={() => removeFavorite(product.id)}
                          className="absolute top-3 right-3 bg-white/80 backdrop-blur text-primary p-1.5 rounded-full shadow-sm border-0 cursor-pointer hover:bg-white transition-colors"
                          aria-label="Xóa khỏi yêu thích"
                        >
                          <span className="material-symbols-outlined" style={{ fontVariationSettings: '"FILL" 1' }}>favorite</span>
                        </button>
                      </div>
                      <div className="p-stack-sm text-center">
                        <Link to={`/product/${product.id}`} className="text-decoration-none">
                          <h3 className="font-headline-sm text-[18px] text-on-surface leading-tight mb-1">{product.name}</h3>
                        </Link>
                        <p className="text-primary font-bold">{formatCurrency(product.discountPrice || product.price)}</p>
                        <Link
                          to={`/product/${product.id}`}
                          className="mt-base w-full py-1.5 border border-primary text-primary rounded-lg text-sm font-label-md hover:bg-primary hover:text-white transition-colors block text-decoration-none"
                        >
                          Thêm vào giỏ
                        </Link>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </section>
        </div>
      </main>
    </div>
  );
};

export default WishlistPage;
