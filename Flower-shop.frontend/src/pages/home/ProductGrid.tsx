import React, { useState } from 'react';
import { useProductsPaged } from '../../hooks/useProducts';
import ProductCard from '../../components/ProductCard';
import Pagination from '../../components/Pagination';

interface ProductGridProps {
  categoryId?: number | null;
}

function ProductGrid({ categoryId }: ProductGridProps) {
  const [page, setPage] = useState(1);
  const pageSize = 8;
  const { data: paged, isLoading } = useProductsPaged(page, pageSize);

  if (isLoading) {
    return (
      <div className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full border-t border-outline-variant/30 text-center">
        <div className="animate-pulse flex flex-col items-center">
          <div className="size-12 bg-surface-container rounded-full mb-md"></div>
          <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Đang tải...</p>
        </div>
      </div>
    );
  }

  const items = paged?.items ?? [];

  return (
    <section className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full border-t border-outline-variant/30">
      <div className="text-center mb-xl">
        <h2 className="font-display-lg text-headline-md uppercase tracking-tight mb-sm">Tuyệt Tác Ngàn Hoa</h2>
        <div className="w-12 h-0.5 bg-primary mx-auto mt-md"></div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-gutter mb-xl">
        {items.map((product: any) => (
          <ProductCard key={product.id} item={product} variant="standard" />
        ))}
        {items.length === 0 && (
          <div className="col-span-full text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
            <span className="material-symbols-outlined text-4xl text-outline mb-md">inventory_2</span>
            <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Không tìm thấy sản phẩm</p>
          </div>
        )}
      </div>

      {paged && (
        <Pagination
          page={paged.page}
          totalPages={paged.totalPages}
          onPageChange={setPage}
        />
      )}
    </section>
  );
}

export default ProductGrid;

