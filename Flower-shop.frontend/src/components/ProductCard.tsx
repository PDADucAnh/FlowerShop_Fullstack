import React, { type MouseEvent } from 'react';
import { useCart } from '../context/CartContext';
import { Link } from 'react-router-dom';
import { getImageUrl } from '../utils/apiUtils';
import type { Product } from '../types/product';

interface ProductCardProps {
  item: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ item }) => {
    const { addToCart } = useCart();

    const formatCurrency = (value: number): string => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(value);
    };

    const imageUrl = getImageUrl(item.imageUrl);

    const handleAddToCart = (e: MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        addToCart(item);
    };

    return (
        <div className="group relative overflow-hidden rounded-xl petal-shadow-hover cursor-pointer bg-surface-container-lowest flex flex-col">
            <Link to={`/product/${item.id}`} className="block w-full aspect-[4/5] overflow-hidden relative">
                <img src={imageUrl} className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" alt={item.name} />
                {item.stockQuantity <= 5 && (
                    <div className="absolute top-3 left-3 bg-primary-fixed text-on-primary-fixed text-label-sm font-label-sm px-3 py-1 rounded-full">
                        Limited / {item.stockQuantity} Left
                    </div>
                )}
                <div className="absolute inset-0 bg-gradient-to-t from-on-surface/40 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex items-end p-stack-md">
                    <button onClick={handleAddToCart} className="w-full bg-surface text-on-surface font-label-md text-label-md py-3 rounded-lg hover:bg-primary hover:text-on-primary transition-all">
                        Add to Cart
                    </button>
                </div>
            </Link>
            <div className="p-stack-sm space-y-1">
                <h3 className="font-label-md text-label-md text-on-surface truncate">
                    <Link to={`/product/${item.id}`} className="text-on-surface text-decoration-none hover:text-primary transition-colors">{item.name}</Link>
                </h3>
                <p className="font-body-md text-body-md text-primary font-semibold">{formatCurrency(item.price)}</p>
            </div>
        </div>
    );
}

export default ProductCard;
