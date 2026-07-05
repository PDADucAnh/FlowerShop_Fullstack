import React from 'react';
import { useSearchParams } from 'react-router-dom';
import { useSearchProducts } from '../../hooks/useProducts';
import SEO from '../../components/SEO';
import ProductCard from '../../components/ProductCard';

const SearchPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const query = searchParams.get('query') || '';

  const { data: products = [], isLoading, error } = useSearchProducts(query);

  return (
    <div className="flex-grow w-full max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
      <SEO title="Tìm kiếm" description="Kết quả tìm kiếm" />
      <div className="mb-stack-lg">
        <h1 className="font-headline-lg text-headline-lg text-on-surface mb-2">Kết quả tìm kiếm</h1>
        <p className="font-body-md text-body-md text-on-surface-variant">
          {query ? (
            <>Có {products.length} kết quả khớp với từ khóa <span className="font-semibold text-primary">"{query}"</span></>
          ) : (
            'Vui lòng nhập từ khóa để bắt đầu tìm kiếm.'
          )}
        </p>
      </div>

      {isLoading && (
        <div className="flex justify-center items-center py-20">
          <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin"></div>
        </div>
      )}

      {error && (
        <div className="text-center py-20 bg-surface-container-lowest rounded-2xl border border-outline-variant/20 p-8">
          <span className="material-symbols-outlined text-4xl text-error mb-4">error_outline</span>
          <p className="font-body-lg text-body-lg text-on-surface-variant">Đã xảy ra lỗi khi tìm kiếm sản phẩm. Vui lòng thử lại sau.</p>
        </div>
      )}

      {!isLoading && !error && products.length === 0 && query && (
        <div className="text-center py-20 bg-surface-container-lowest rounded-2xl border border-outline-variant/20 p-8">
          <span className="material-symbols-outlined text-4xl text-outline mb-4">search_off</span>
          <p className="font-body-lg text-body-lg text-on-surface-variant mb-2">Không tìm thấy sản phẩm nào phù hợp.</p>
          <p className="font-body-md text-body-md text-outline">Hãy thử tìm kiếm với các từ khóa khác hoặc đơn giản hơn.</p>
        </div>
      )}

      {!isLoading && !error && products.length > 0 && (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-gutter">
          {products.map((product) => (
            <ProductCard key={product.id} item={product} />
          ))}
        </div>
      )}
    </div>
  );
};

export default SearchPage;
