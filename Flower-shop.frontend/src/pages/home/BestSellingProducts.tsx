import React from 'react';
import { Link } from 'react-router-dom';
import { useBestSellingProducts } from '../../hooks/useProducts';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';

function BestSellingProducts() {
  const { data: products = [], isLoading } = useBestSellingProducts(4);

  if (isLoading) {
    return (
      <section className="mt-stack-lg px-margin-mobile">
        <div className="flex justify-between items-end mb-stack-md">
          <h3 className="font-headline-md text-headline-md text-on-surface">Bán Chạy Nhất</h3>
        </div>
        <div className="flex overflow-x-auto snap-x snap-mandatory gap-3 px-4 no-scrollbar pb-2">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="w-[160px] sm:w-[200px] flex-shrink-0 snap-start">
              <div className="aspect-square rounded-xl overflow-hidden mb-2 bg-surface-container-high animate-pulse" />
              <div className="h-4 bg-surface-container-high rounded animate-pulse mb-1" />
              <div className="h-3 bg-surface-container-high rounded w-1/3 animate-pulse mx-auto" />
            </div>
          ))}
        </div>
      </section>
    );
  }

  const allProducts = Array.isArray(products) ? products : [];
  const displayProducts = allProducts.slice(0, 6);

  if (displayProducts.length === 0) {
    return null;
  }

  return (
    <section className="mt-stack-lg px-margin-mobile">
      <div className="flex justify-between items-end mb-stack-md">
        <h3 className="font-headline-md text-headline-md text-on-surface">Bán Chạy Nhất</h3>
        <Link to="/shop?sort=best-selling" className="font-label-md text-primary uppercase tracking-widest text-xs no-underline">
          Xem tất cả
        </Link>
      </div>
      <div className="flex overflow-x-auto snap-x snap-mandatory gap-3 px-4 no-scrollbar pb-2">
        {displayProducts.map((product: any) => {
          const imageUrl = getImageUrl(product.imageUrl);
          const displayPrice = product.promotionPrice ?? product.currentPrice ?? product.discountPrice ?? product.price;
          return (
            <Link
              key={product.id}
              to={`/product/${product.id}`}
              className="w-[160px] sm:w-[200px] flex-shrink-0 snap-start group no-underline"
            >
              <div className="relative aspect-square rounded-xl overflow-hidden mb-2 petal-shadow">
                <img
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                  src={imageUrl}
                  alt={product.name}
                  loading="lazy"
                />
              </div>
              <div className="text-center">
                <h4 className="font-label-md text-xs sm:text-sm text-on-surface line-clamp-2 leading-tight">{product.name}</h4>
                <p className="font-label-sm text-tertiary mt-1">{formatCurrency(displayPrice)}</p>
              </div>
            </Link>
          );
        })}
      </div>
    </section>
  );
}

export default BestSellingProducts;
