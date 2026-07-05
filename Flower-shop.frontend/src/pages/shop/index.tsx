import React, { useState, useEffect } from 'react';
import ShopSidebar from './ShopSidebar';
import ShopHeader from './ShopHeader';
import ProductList from './ProductList';
import Pagination from '../../components/Pagination';
import SEO from '../../components/SEO';
import { useProductsPaged } from '../../hooks/useProducts';

const ShopPage: React.FC = () => {
  const [page, setPage] = useState(1);
  const pageSize = 9;

  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);
  
  // Realtime UI state
  const [priceRange, setPriceRange] = useState<{ min: number | null, max: number | null }>({ min: null, max: null });
  const [activePricePreset, setActivePricePreset] = useState<string | null>(null);

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

  const { data: paged, isLoading, error } = useProductsPaged(page, pageSize, debouncedMin, debouncedMax, selectedCategoryId);

  const products = paged?.items ?? [];

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
    setPage(1);
  };

  const handlePriceChange = (min: number | null, max: number | null) => {
    setPriceRange({ min, max });
  };

  return (
    <div className="flex-grow w-full max-w-container-max mx-auto px-margin-desktop py-stack-lg flex flex-col md:flex-row gap-gutter">
      <SEO title="Cửa hàng" description="Danh sách sản phẩm hoa tươi" />
      <aside className="w-full md:w-64 flex-shrink-0">
        <ShopSidebar 
          onCategoryChange={handleCategoryChange} 
          activeCategoryId={selectedCategoryId}
          onPriceChange={handlePriceChange}
          activePricePreset={activePricePreset}
          setActivePricePreset={setActivePricePreset}
        />
      </aside>
      <section className="flex-grow">
        <ShopHeader count={paged?.totalCount ?? 0} page={paged?.page} pageSize={paged?.pageSize} />
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
