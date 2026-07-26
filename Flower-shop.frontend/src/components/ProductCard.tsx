import React, { type MouseEvent } from 'react';
import { useCart } from '../context/CartContext';
import { useNavigate } from 'react-router-dom';
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

    const handleCardClick = () => {
        navigate(`/product/${item.id}`);
    };

    const cardImage = (
        <div className="relative aspect-square overflow-hidden rounded-lg bg-surface-container-low">
            <img
                className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                src={imageUrl}
                alt={item.name}
                loading="lazy"
            />
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
            <button
                onClick={handleAddToCart}
                disabled={isOutOfStock}
                className="absolute top-2 right-2 bg-white/80 backdrop-blur-sm p-1.5 rounded-full shadow opacity-0 group-hover:opacity-100 transition-opacity duration-200 hover:bg-white border-0 cursor-pointer disabled:opacity-0"
                aria-label="Thêm vào giỏ"
            >
                <span className="material-symbols-outlined text-[18px] text-primary">shopping_cart</span>
            </button>
        </div>
    );

    if (variant === 'featured') {
        return (
            <div
                onClick={handleCardClick}
                className="group cursor-pointer flex flex-col h-full"
            >
                {cardImage}
                <div className="flex flex-col flex-grow mt-3">
                    <h3 className="text-sm font-medium text-on-surface line-clamp-2 leading-snug">
                        {item.name}
                    </h3>
                    <p className="mt-1">
                        {hasPromotion ? (
                            <>
                                <span className="text-emerald-700 font-semibold text-sm">{formatCurrency(displayPrice)}</span>
                                <span className="text-on-surface-variant line-through text-xs ml-2">{formatCurrency(item.price)}</span>
                            </>
                        ) : (
                            <span className="text-emerald-700 font-semibold text-sm">{formatCurrency(displayPrice)}</span>
                        )}
                    </p>
                </div>
            </div>
        );
    }

    return (
        <div
            onClick={handleCardClick}
            className="group cursor-pointer flex flex-col h-full"
        >
            {cardImage}
            <div className="flex flex-col flex-grow mt-2">
                <h3 className="text-sm font-medium text-on-surface line-clamp-2 leading-snug">
                    {item.name}
                </h3>
                <p className="mt-1">
                    {hasPromotion ? (
                        <>
                            <span className="text-emerald-700 font-semibold text-sm">{formatCurrency(displayPrice)}</span>
                            <span className="text-on-surface-variant line-through text-xs ml-2">{formatCurrency(item.price)}</span>
                        </>
                    ) : (
                        <span className="text-emerald-700 font-semibold text-sm">{formatCurrency(displayPrice)}</span>
                    )}
                </p>
            </div>
        </div>
    );
};

export default ProductCard;

