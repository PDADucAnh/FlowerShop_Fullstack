import React, { createContext, useState, useContext, useEffect, useCallback, useMemo, type ReactNode } from 'react';
import type { Product } from '../types/product';
import toast from 'react-hot-toast';
import productService from '../services/productService';

export interface CartItem extends Product {
  quantity: number;
}

interface CartContextType {
  cartItems: CartItem[];
  addToCart: (product: Product, quantity?: number) => void;
  removeFromCart: (productId: number) => void;
  updateQuantity: (productId: number, quantity: number) => void;
  clearCart: () => void;
  cartCount: number;
  cartTotal: number;
  recalculateCartPrices: () => Promise<void>;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const useCart = (): CartContextType => {
  const context = useContext(CartContext);
  if (!context) throw new Error('useCart must be used within CartProvider');
  return context;
};

export const CartProvider = ({ children }: { children: ReactNode }) => {
  const [cartItems, setCartItems] = useState<CartItem[]>(() => {
    try {
      const savedCart = localStorage.getItem('cart');
      return savedCart ? JSON.parse(savedCart) : [];
    } catch {
      return [];
    }
  });

  useEffect(() => {
    localStorage.setItem('cart', JSON.stringify(cartItems));
  }, [cartItems]);

  const addToCart = useCallback((product: Product, quantity = 1) => {
    productService.trackAddToCart(product.id);
    setCartItems((prevItems) => {
      const existingItem = prevItems.find((item) => item.id === product.id);
      const currentQty = existingItem ? existingItem.quantity : 0;
      const requestedQty = currentQty + quantity;
      const maxStock = product.stockQuantity;

      if (requestedQty > maxStock) {
        const cappedQty = Math.max(maxStock, 0);
        if (cappedQty <= 0) {
          toast.error(`"${product.name}" đã hết hàng.`);
          return prevItems;
        }
        toast.error(`"${product.name}" chỉ còn ${maxStock} sản phẩm trong kho.`);
        if (existingItem) {
          return prevItems.map((item) =>
            item.id === product.id ? { ...item, quantity: cappedQty } : item
          );
        }
        return [...prevItems, { ...product, quantity: cappedQty }];
      }

      if (existingItem) {
        return prevItems.map((item) =>
          item.id === product.id ? { ...item, quantity: requestedQty } : item
        );
      }
      return [...prevItems, { ...product, quantity }];
    });
  }, []);

  const removeFromCart = useCallback((productId: number) => {
    setCartItems((prevItems) => prevItems.filter((item) => item.id !== productId));
  }, []);

  const updateQuantity = useCallback((productId: number, quantity: number) => {
    if (quantity < 1) return;
    setCartItems((prevItems) =>
      prevItems.map((item) => {
        if (item.id !== productId) return item;
        const capped = Math.min(quantity, item.stockQuantity);
        if (capped !== quantity) {
          toast.error(`"${item.name}" chỉ còn ${item.stockQuantity} sản phẩm trong kho.`);
        }
        return { ...item, quantity: capped };
      })
    );
  }, []);

  const cartItemsRef = React.useRef(cartItems);
  useEffect(() => {
    cartItemsRef.current = cartItems;
  }, [cartItems]);

  const recalculateCartPrices = useCallback(async () => {
    const currentItems = cartItemsRef.current;
    if (currentItems.length === 0) return;
    try {
      const payload = currentItems.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        name: item.name,
        price: item.price,
        promotionPrice: item.promotionPrice
      }));

      const res = await productService.recalculateCart(payload);
      if (res && res.items) {
        const updatedItems = currentItems.map(item => {
          const matched = res.items.find((i: any) => i.productId === item.id);
          if (matched) {
            return {
              ...item,
              price: matched.price,
              promotionPrice: matched.promotionPrice ?? undefined,
              currentPrice: matched.currentPrice,
              hasFlashSale: matched.hasFlashSale,
              promotionPercent: matched.promotionPercent ?? undefined,
              promotionName: matched.promotionName ?? undefined,
              stockQuantity: matched.stockQuantity,
              imageUrl: matched.imageUrl ?? item.imageUrl,
              description: matched.description ?? item.description
            };
          }
          return item;
        });

        setCartItems(updatedItems);
        if (res.priceChanged) {
          toast.error(res.message || "Giá của một hoặc nhiều sản phẩm đã được cập nhật do chương trình khuyến mãi đã kết thúc hoặc thay đổi.", {
            duration: 5000,
            position: 'top-center'
          });
        }
      }
    } catch (err) {
      console.error("Lỗi khi đồng bộ giá giỏ hàng với Backend:", err);
    }
  }, [setCartItems]);

  const clearCart = useCallback(() => {
    setCartItems([]);
  }, []);

  const cartCount = useMemo(() => {
    return cartItems.reduce((total, item) => total + item.quantity, 0);
  }, [cartItems]);

  const cartTotal = useMemo(() => {
    return cartItems.reduce((total, item) => total + (item.promotionPrice ?? item.discountPrice ?? item.price) * item.quantity, 0);
  }, [cartItems]);

  const contextValue = useMemo(() => ({
    cartItems,
    addToCart,
    removeFromCart,
    updateQuantity,
    clearCart,
    cartCount,
    cartTotal,
    recalculateCartPrices,
  }), [cartItems, addToCart, removeFromCart, updateQuantity, clearCart, cartCount, cartTotal, recalculateCartPrices]);

  return (
    <CartContext.Provider value={contextValue}>
      {children}
    </CartContext.Provider>
  );
};
