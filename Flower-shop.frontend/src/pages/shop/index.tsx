import React, { useState, useEffect, useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';
import ShopSidebar from './ShopSidebar';
import ShopHeader from './ShopHeader';
import ProductList from './ProductList';
import Pagination from '../../components/Pagination';
import SEO from '../../components/SEO';
import { useProductsPaged } from '../../hooks/useProducts';
import { useProductCategories } from '../../hooks/useCategories';

const ShopPage: React.FC = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const pageSize = 9;

  const [page, setPage] = useState(() => parseInt(searchParams.get('page') || '1', 10));
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(
    () => {
      const cat = searchParams.get('category');
      return cat ? parseInt(cat, 10) : null;
    }
  );
  
  // Realtime UI state
  const [priceRange, setPriceRange] = useState<{ min: number | null, max: number | null }>(() => ({
    min: searchParams.get('min') ? parseInt(searchParams.get('min')!, 10) : null,
    max: searchParams.get('max') ? parseInt(searchParams.get('max')!, 10) : null,
  }));
  const [activePricePreset, setActivePricePreset] = useState<string | null>(null);
  const [sortBy, setSortBy] = useState<string | null>(null);
  const [promotionOnly, setPromotionOnly] = useState(false);
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const { data: categoriesData } = useProductCategories();
  const categories = (categoriesData as unknown as any[]) ?? [];

  // Debounced API state
  const [debouncedMin, setDebouncedMin] = useState<number | null>(null);
  const [debouncedMax, setDebouncedMax] = useState<number | null>(null);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedMin(priceRange.min);
      setDebouncedMax(priceRange.max);
      setPage(1);
    }, 500);

    return () => clearTimeout(handler);
  }, [priceRange]);

  useEffect(() => {
    const params: Record<string, string> = {};
    if (page > 1) params.page = String(page);
    if (selectedCategoryId) params.category = String(selectedCategoryId);
    if (priceRange.min) params.min = String(priceRange.min);
    if (priceRange.max) params.max = String(priceRange.max);
    if (sortBy) params.sortBy = sortBy;
    if (promotionOnly) params.promotionOnly = 'true';
    setSearchParams(params, { replace: true });
  }, [page, selectedCategoryId, priceRange, sortBy, promotionOnly, setSearchParams]);

  const { data: paged, isLoading, error } = useProductsPaged(page, pageSize, debouncedMin, debouncedMax, selectedCategoryId, sortBy, promotionOnly);

  const products = paged?.items ?? [];

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
    setPage(1);
  };

  const handlePriceChange = (min: number | null, max: number | null) => {
    setPriceRange({ min, max });
  };

  return (
    <div className="flex-grow w-full max-w-container-max mx-auto px-margin-desktop py-4 md:py-stack-lg flex flex-col md:flex-row gap-gutter">
      <SEO title="Cửa hàng" description="Danh sách sản phẩm hoa tươi" />
      {/* Mobile filter toggle */}
      <button
        className="md:hidden w-full bg-surface-container-lowest border border-outline-variant/20 rounded-lg py-2.5 px-3 flex items-center justify-center gap-2 text-primary text-xs sm:text-sm font-semibold hover:bg-surface-container-low transition-colors cursor-pointer mb-2"
        onClick={() => setSidebarOpen(true)}
      >
        <span className="material-symbols-outlined text-[18px]">filter_list</span>
        Bộ lọc & sắp xếp
      </button>

      {/* Mobile sidebar backdrop */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/40 z-30 md:hidden"
          style={{ backdropFilter: 'blur(2px)' }}
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar: desktop always visible, mobile as drawer */}
      <aside
        className={`${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        } md:translate-x-0 fixed md:static top-[72px] md:top-0 left-0 z-40 h-full md:h-auto w-72 md:w-64 flex-shrink-0 transition-transform duration-300 ease-in-out overflow-y-auto`}
      >
        <ShopSidebar
          onCategoryChange={(id) => { handleCategoryChange(id); setSidebarOpen(false); }}
          activeCategoryId={selectedCategoryId}
          onPriceChange={handlePriceChange}
          activePricePreset={activePricePreset}
          setActivePricePreset={setActivePricePreset}
          onPromotionFilterChange={setPromotionOnly}
          activePromotionOnly={promotionOnly}
          onMobileClose={() => setSidebarOpen(false)}
        />
      </aside>
      <section className="flex-grow min-w-0">
        {/* Category pills */}
        <div className="flex gap-2 overflow-x-auto pb-3 mb-2 scrollbar-hide snap-x snap-mandatory -mx-margin-desktop px-margin-desktop">
          <button
            onClick={() => handleCategoryChange(null)}
            className={`snap-start shrink-0 px-4 py-1.5 rounded-full border text-xs sm:text-sm font-medium transition-colors cursor-pointer ${
              selectedCategoryId === null
                ? 'bg-primary text-white border-primary'
                : 'bg-surface-container-lowest text-on-surface-variant border-outline-variant hover:border-primary hover:text-primary'
            }`}
          >
            Tất cả
          </button>
          {categories.map((cat: { id: number; name: string }) => (
            <button
              key={cat.id}
              onClick={() => handleCategoryChange(cat.id)}
              className={`snap-start shrink-0 px-4 py-1.5 rounded-full border text-xs sm:text-sm font-medium transition-colors cursor-pointer ${
                selectedCategoryId === cat.id
                  ? 'bg-primary text-white border-primary'
                  : 'bg-surface-container-lowest text-on-surface-variant border-outline-variant hover:border-primary hover:text-primary'
              }`}
            >
              {cat.name}
            </button>
          ))}
        </div>
        <ShopHeader count={paged?.totalCount ?? 0} page={paged?.page} pageSize={paged?.pageSize} sortBy={sortBy || undefined} onSortChange={setSortBy} />
        <ProductList products={products} isLoading={isLoading} error={error ? "Không thể tải bộ sưu tập vào lúc này." : null} />
        {paged && paged.totalPages > 1 && (
          <Pagination
            page={paged.page}
            totalPages={paged.totalPages}
            onPageChange={setPage}
          />
        )}
      </section>
    </div>
  );
};

export default ShopPage;
