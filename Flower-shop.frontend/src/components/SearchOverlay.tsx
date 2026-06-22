import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { useProducts } from '../hooks/useProducts';
import { getImageUrl } from '../utils/apiUtils';

const SearchOverlay: React.FC<{ onClose: () => void }> = ({ onClose }) => {
  const [query, setQuery] = useState('');
  const { data: allProducts = [] } = useProducts();
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
    document.body.style.overflow = 'hidden';
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    document.addEventListener('keydown', handleKeyDown);
    return () => {
      document.body.style.overflow = '';
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, [onClose]);

  const results = Array.isArray(allProducts)
    ? allProducts.filter((p: any) =>
        p.name?.toLowerCase().includes(query.toLowerCase())
      )
    : [];

  const handleResultClick = useCallback(() => {
    setQuery('');
    onClose();
  }, [onClose]);

  return (
    <div
      className="fixed inset-0 z-[100] bg-surface/95 backdrop-blur-md flex flex-col"
      onClick={(e) => { if (e.target === e.currentTarget) onClose(); }}
    >
      <div className="max-w-[1000px] mx-auto w-full px-6 pt-16 md:pt-24">
        <div className="flex items-center justify-between mb-8 md:mb-12">
          <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary">Search</span>
          <button
            onClick={onClose}
            className="bg-transparent border-0 text-primary hover:text-secondary transition-colors btn-ghost-luxury"
            aria-label="Close search"
          >
            <span className="material-symbols-outlined text-2xl">close</span>
          </button>
        </div>

        <div className="border-b border-primary pb-3 flex items-center gap-4">
          <span className="material-symbols-outlined text-outline">search</span>
          <input
            ref={inputRef}
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            placeholder="What are you looking for?"
            className="flex-1 bg-transparent border-none text-display-xl-mobile md:text-headline-lg serif text-primary placeholder:text-outline focus:ring-0 p-0 font-display-xl outline-none"
          />
          {query && (
            <button
              onClick={() => setQuery('')}
              className="bg-transparent border-0 text-outline hover:text-primary transition-colors"
              aria-label="Clear search"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
          )}
        </div>
      </div>

      {query && (
        <div className="flex-1 overflow-y-auto mt-6">
          <div className="max-w-[1000px] mx-auto w-full px-6">
            <p className="font-label-sm text-label-sm uppercase tracking-[0.2em] text-secondary mb-6">
              {results.length} {results.length === 1 ? 'Result' : 'Results'}
            </p>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 md:gap-6 pb-16">
              {results.map((product: any) => (
                <Link
                  key={product.id}
                  to={`/product/${product.id}`}
                  onClick={handleResultClick}
                  className="group text-decoration-none"
                >
                  <div className="aspect-[4/5] bg-surface-container relative overflow-hidden mb-3">
                    <img
                      src={getImageUrl(product.imageUrl)}
                      alt={product.name}
                      className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700"
                    />
                  </div>
                  <h4 className="font-body-md text-body-md text-primary mb-0.5 truncate">{product.name}</h4>
                  <p className="font-label-sm text-label-sm text-secondary">
                    {(product.discountPrice || product.price).toLocaleString()} ₫
                  </p>
                </Link>
              ))}
            </div>
          </div>
        </div>
      )}

      {query && results.length === 0 && (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <span className="material-symbols-outlined text-5xl text-outline mb-4 block">search_off</span>
            <p className="font-body-md text-body-md text-secondary">No results for &ldquo;{query}&rdquo;</p>
            <p className="font-label-sm text-label-sm text-outline uppercase tracking-widest mt-2">Try a different search term</p>
          </div>
        </div>
      )}
    </div>
  );
};

export default SearchOverlay;
