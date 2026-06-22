import React, { createContext, useState, useContext, useEffect, useCallback, useMemo, type ReactNode } from 'react';
import type { Product } from '../types/product';

interface WishlistContextType {
  favorites: Product[];
  toggleFavorite: (product: Product) => void;
  isFavorite: (productId: number) => boolean;
  favoritesCount: number;
  removeFavorite: (productId: number) => void;
}

const WishlistContext = createContext<WishlistContextType | undefined>(undefined);

export const useWishlist = (): WishlistContextType => {
  const context = useContext(WishlistContext);
  if (!context) throw new Error('useWishlist must be used within WishlistProvider');
  return context;
};

export const WishlistProvider = ({ children }: { children: ReactNode }) => {
  const [favorites, setFavorites] = useState<Product[]>(() => {
    try {
      const saved = localStorage.getItem('wishlist');
      return saved ? JSON.parse(saved) : [];
    } catch {
      return [];
    }
  });

  useEffect(() => {
    localStorage.setItem('wishlist', JSON.stringify(favorites));
  }, [favorites]);

  const toggleFavorite = useCallback((product: Product) => {
    setFavorites((prev) => {
      const exists = prev.find((p) => p.id === product.id);
      if (exists) return prev.filter((p) => p.id !== product.id);
      return [...prev, product];
    });
  }, []);

  const isFavorite = useCallback((productId: number) => {
    return favorites.some((p) => p.id === productId);
  }, [favorites]);

  const removeFavorite = useCallback((productId: number) => {
    setFavorites((prev) => prev.filter((p) => p.id !== productId));
  }, []);

  const favoritesCount = favorites.length;

  const value = useMemo(() => ({
    favorites, toggleFavorite, isFavorite, favoritesCount, removeFavorite,
  }), [favorites, toggleFavorite, isFavorite, removeFavorite, favoritesCount]);

  return (
    <WishlistContext.Provider value={value}>
      {children}
    </WishlistContext.Provider>
  );
};
