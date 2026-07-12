import React, { type MouseEvent } from 'react';
import { useCart } from '../context/CartContext';
import { Link, useNavigate } from 'react-router-dom';
import { getImageUrl } from '../utils/apiUtils';
import { formatCurrency } from '../utils/currency';
import type { Product } from '../types/product';

interface ProductCardProps {
  item: Product;
  variant?: 'standard' | 'featured';
}

const ProductCard: React.FC<ProductCardProps> = ({ item, variant = 'standard' }) => {
    const { addToCart } = useCart();
    const navigate = useNavigate();

    const imageUrl = getImageUrl(item.imageUrl);
    const isOutOfStock = item.stockQuantity === 0;
    const displayPrice = item.promotionPrice ?? item.currentPrice ?? item.discountPrice ?? item.price;
    const hasPromotion = !!item.promotionPrice || (!!item.currentPrice && item.currentPrice < item.price);
    const isFlashSale = !!item.hasFlashSale || !!item.isFlashSale || item.promotionType === 'FlashSale';
    const percent = item.promotionPercent ?? item.discountPercent;

    const handleAddToCart = (e: MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (isOutOfStock) return;
        addToCart(item);
    };

    const handleBuyNow = (e: MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (isOutOfStock) return;
        addToCart(item);
        navigate('/checkout');
    };

    const handleCardClick = () => {
        navigate(`/product/${item.id}`);
    };

    if (variant === 'featured') {
        return (
            <div 
                onClick={handleCardClick}
                className="group cursor-pointer flex flex-col h-full"
            >
                <div className="relative aspect-[4/5] overflow-hidden rounded-xl petal-shadow mb-4">
                    <Link to={`/product/${item.id}`} onClick={(e) => e.stopPropagation()}>
                        <img
                            className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                            src={imageUrl}
                            alt={item.name}
                            loading="lazy"
                        />
                    </Link>
                    {isFlashSale && (
                        <div className="absolute top-2 left-2 bg-red-600 text-white px-2.5 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider flex items-center gap-1 shadow-lg z-10 animate-pulse">
                            <span className="material-symbols-outlined text-[12px] font-bold">bolt</span>
                            Flash Sale {percent ? `-${percent}%` : ''}
                        </div>
                    )}
                    {!isFlashSale && hasPromotion && (
                        <div className="absolute top-2 left-2 bg-primary text-on-primary px-2.5 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-lg z-10">
                            KM {percent ? `-${percent}%` : ''}
                        </div>
                    )}
                    {item.trendingBadge && (
                        <div className="absolute top-2 right-2 bg-amber-500/90 text-white px-2 py-1 rounded text-[10px] font-label-sm uppercase tracking-widest shadow-sm z-10">
                            {item.trendingBadge}
                        </div>
                    )}
                    {item.stockQuantity <= 5 && item.stockQuantity > 0 && (
                        <div className="absolute bottom-2 left-2 bg-primary/90 text-on-primary px-2 py-1 rounded text-[10px] font-label-sm uppercase tracking-widest z-10">
                            Chỉ còn {item.stockQuantity}
                        </div>
                    )}
                </div>
                <div className="flex flex-col flex-grow">
                    <h3 className="font-headline-sm text-headline-sm text-on-surface mb-1">
                        <Link to={`/product/${item.id}`} className="no-underline text-on-surface hover:text-primary transition-colors" onClick={(e) => e.stopPropagation()}>
                            {item.name}
                        </Link>
                    </h3>
                    <p className="text-primary font-label-md mb-4">
                      {hasPromotion ? (
                        <>
                          <span className="text-error font-headline-sm">{formatCurrency(displayPrice)}</span>
                          <span className="text-on-surface-variant line-through text-xs ml-2">{formatCurrency(item.price)}</span>
                        </>
                      ) : formatCurrency(displayPrice)}
                    </p>
                    <div className="flex gap-2 w-full mt-auto" onClick={(e) => e.stopPropagation()}>
                        <button
                            onClick={handleAddToCart}
                            disabled={isOutOfStock}
                            className="flex-1 flex items-center justify-center gap-1 py-2 border border-primary text-primary font-label-sm rounded hover:bg-primary hover:text-on-primary transition-colors cursor-pointer bg-transparent disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:bg-transparent disabled:hover:text-primary"
                        >
                            <span className="material-symbols-outlined text-[18px]">shopping_cart</span>
                            {isOutOfStock ? 'Hết hàng' : 'Thêm vào giỏ'}
                        </button>
                        <button
                            onClick={handleBuyNow}
                            disabled={isOutOfStock}
                            className="flex-1 flex items-center justify-center gap-1 py-2 bg-primary-container text-on-primary font-label-sm rounded hover:opacity-90 transition-opacity cursor-pointer border-0 disabled:opacity-40 disabled:cursor-not-allowed"
                        >
                            <span className="material-symbols-outlined text-[18px]">bolt</span>
                            {isOutOfStock ? 'Hết hàng' : 'Mua ngay'}
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    // Default 'standard' variant
    return (
        <div 
            onClick={handleCardClick}
            className="group flex flex-col h-full"
        >
            <div className="relative aspect-[1/1] aspect-square overflow-hidden rounded-lg bg-surface-container-low mb-4">
                <Link to={`/product/${item.id}`} onClick={(e) => e.stopPropagation()}>
                    <img
                        className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                        src={imageUrl}
                        alt={item.name}
                        loading="lazy"
                    />
                </Link>
                {isFlashSale && (
                    <div className="absolute top-2 left-2 bg-red-600 text-white px-2 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider flex items-center gap-0.5 shadow-lg z-10 animate-pulse">
                        <span className="material-symbols-outlined text-[12px] font-bold">bolt</span>
                        Flash Sale {percent ? `-${percent}%` : ''}
                    </div>
                )}
                {!isFlashSale && hasPromotion && (
                    <div className="absolute top-2 left-2 bg-primary text-on-primary px-2 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-lg z-10">
                        KM {percent ? `-${percent}%` : ''}
                    </div>
                )}
                {item.trendingBadge && (
                    <div className="absolute top-2 right-2 bg-amber-500/90 text-white px-2 py-1 rounded text-[10px] font-label-sm uppercase tracking-widest shadow-sm z-10">
                        {item.trendingBadge}
                    </div>
                )}
                {item.stockQuantity <= 5 && item.stockQuantity > 0 && (
                    <div className="absolute bottom-2 left-2 bg-primary/90 text-on-primary px-2 py-1 rounded text-[10px] font-label-sm uppercase tracking-widest z-10">
                        Chỉ còn {item.stockQuantity}
                    </div>
                )}
            </div>
            <div className="flex flex-col flex-grow">
                <h3 className="font-label-md text-on-surface mb-1">
                    <Link to={`/product/${item.id}`} className="no-underline text-on-surface hover:text-primary transition-colors" onClick={(e) => e.stopPropagation()}>
                        {item.name}
                    </Link>
                </h3>
                <p className="text-label-sm mb-2" style={{ color: 'rgb(128, 0, 0)' }}>
                  {hasPromotion ? (
                    <>
                      <span className="text-error font-headline-sm">{formatCurrency(displayPrice)}</span>
                      <span className="text-gray-400 line-through text-xs ml-2">{formatCurrency(item.price)}</span>
                    </>
                  ) : formatCurrency(displayPrice)}
                </p>
                <div className="flex gap-2 w-full mt-auto" onClick={(e) => e.stopPropagation()}>
                    <button
                        onClick={handleAddToCart}
                        disabled={isOutOfStock}
                        className="flex-1 flex items-center justify-center gap-1 py-2 border border-primary text-primary font-label-sm rounded hover:bg-primary hover:text-on-primary transition-colors cursor-pointer bg-transparent disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:bg-transparent disabled:hover:text-primary"
                    >
                        <span className="material-symbols-outlined text-[18px]">shopping_cart</span>
                        {isOutOfStock ? 'Hết hàng' : 'Thêm vào giỏ'}
                    </button>
                    <button
                        onClick={handleBuyNow}
                        disabled={isOutOfStock}
                        className="flex-1 flex items-center justify-center gap-1 py-2 bg-primary-container text-on-primary font-label-sm rounded hover:opacity-90 transition-opacity cursor-pointer border-0 disabled:opacity-40 disabled:cursor-not-allowed"
                    >
                        <span className="material-symbols-outlined text-[18px]">bolt</span>
                        {isOutOfStock ? 'Hết hàng' : 'Mua ngay'}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;

