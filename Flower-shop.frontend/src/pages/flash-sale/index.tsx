import React, { useEffect, useState } from 'react';
import axiosClient from '../../api/axiosClient';
import ProductCard from '../../components/ProductCard';
import CountdownTimer from '../../components/CountdownTimer';
import SEO from '../../components/SEO';
import type { Product } from '../../types/product';

interface FlashSaleItem {
  productId: number;
  productName: string;
  productImageUrl: string;
  originalPrice: number;
  salePrice: number;
  discountPercent: number;
  promotionName: string;
  promotionEndTime: string;
}

const FlashSalePage: React.FC = () => {
  const [items, setItems] = useState<FlashSaleItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    axiosClient.get('/api/FlashSales/active')
      .then((res: any) => {
        setItems(Array.isArray(res) ? res : res.data ?? []);
        setLoading(false);
      })
      .catch(() => {
        setError('Không thể tải danh sách Flash Sale.');
        setLoading(false);
      });
  }, []);

  const globalEndTime = items.length > 0
    ? items.reduce((earliest, item) =>
        item.promotionEndTime < earliest ? item.promotionEndTime : earliest,
        items[0].promotionEndTime
      )
    : null;

  const products: Product[] = items.map(item => ({
    id: item.productId,
    name: item.productName,
    price: item.originalPrice,
    imageUrl: item.productImageUrl,
    promotionPrice: item.salePrice,
    promotionPercent: item.discountPercent,
    hasFlashSale: true,
    isFlashSale: true,
    promotionEndTime: item.promotionEndTime,
    stockQuantity: 999,
    description: '',
    discountPrice: undefined,
    currentPrice: undefined,
    discountPercent: undefined,
    slug: '',
    sku: '',
    categoryId: 0,
    categoryName: '',
    weight: 0,
    length: 0,
    width: 0,
    height: 0,
    isActive: true,
    isFeatured: false,
    isNew: false,
    sizePrice: undefined,
    trendingBadge: undefined,
    promotionType: 'FlashSale',
    promotionName: item.promotionName,
    createdAt: '',
    updatedAt: '',
  }));

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Flash Sale" description="Các chương trình Flash Sale hot nhất" />
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <div className="relative bg-gradient-to-r from-red-600 to-red-500 rounded-2xl p-8 md:p-12 mb-stack-lg overflow-hidden">
          <div className="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/2" />
          <div className="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-1/2 -translate-x-1/2" />
          <div className="relative z-10">
            <div className="flex items-center gap-3 mb-4">
              <span className="material-symbols-outlined text-4xl text-yellow-300">bolt</span>
              <h1 className="font-display-lg text-display-lg text-white">Flash Sale</h1>
            </div>
            <p className="font-body-lg text-body-lg text-white/80 max-w-xl mb-6">
              Săn hoa đẹp giá tốt — số lượng có hạn, nhanh tay bạn nhé!
            </p>
            {globalEndTime && (
              <div className="inline-flex items-center gap-2 bg-white/20 backdrop-blur rounded-lg px-4 py-3">
                <span className="text-white/80 font-label-sm uppercase tracking-wider">Kết thúc trong:</span>
                <CountdownTimer endTime={globalEndTime} className="text-white text-lg" />
              </div>
            )}
          </div>
        </div>

        {loading ? (
          <div className="text-center py-12">
            <div className="animate-spin inline-block w-8 h-8 border-2 border-primary border-t-transparent rounded-full" />
            <p className="mt-4 text-on-surface-variant">Đang tải...</p>
          </div>
        ) : error ? (
          <div className="text-center py-12">
            <span className="material-symbols-outlined text-4xl text-error mb-2 block">error</span>
            <p className="text-on-surface-variant">{error}</p>
          </div>
        ) : items.length === 0 ? (
          <div className="text-center py-12">
            <span className="material-symbols-outlined text-4xl text-outline mb-2 block">local_fire_department</span>
            <h2 className="font-headline-sm text-headline-sm text-on-surface mb-2">Hiện không có Flash Sale nào</h2>
            <p className="text-on-surface-variant">Theo dõi để không bỏ lỡ chương trình tiếp theo!</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-gutter">
            {products.map(product => (
              <ProductCard key={product.id} item={product} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
};

export default FlashSalePage;
