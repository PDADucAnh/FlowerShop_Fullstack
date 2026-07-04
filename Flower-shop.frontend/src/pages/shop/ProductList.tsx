import React from 'react';
import LoadingOrEmpty from './LoadingOrEmpty';
import ProductCard from '../../components/ProductCard';

interface ProductListProps {
  products: any[];
  isLoading: boolean;
  error: string | null;
}

const ProductList = ({ products, isLoading, error }: ProductListProps) => {
  if (isLoading) {
    return <LoadingOrEmpty isLoading={true} />;
  }

  if (error) {
    return (
      <div className="p-lg bg-error-container text-error text-label-sm uppercase tracking-widest font-bold text-center border border-error">
        {error}
      </div>
    );
  }

  if (!products || products.length === 0) {
    return <LoadingOrEmpty isLoading={false} message="Không tìm thấy sản phẩm trong danh mục này." />;
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-gutter">
      {products.map((product) => (
        <ProductCard key={product.id} item={product} />
      ))}
    </div>
  );
};

export default ProductList;
