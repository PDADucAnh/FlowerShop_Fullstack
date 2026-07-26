import React from 'react';

interface ShopHeaderProps {
  count: number;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  onSortChange?: (value: string) => void;
}

const ShopHeader = ({ count, page = 1, pageSize = 9, sortBy, onSortChange }: ShopHeaderProps) => {
  return (
    <div className="flex justify-between items-center mb-3 pb-3 border-b border-surface-variant">
      <p className="text-xs sm:text-sm text-on-surface-variant font-medium">
        {count > 0 ? `${count} sản phẩm` : 'Không có kết quả'}
      </p>
      <div className="flex items-center gap-1.5">
        <span className="hidden sm:inline text-xs text-on-surface font-medium">Sắp xếp:</span>
        <select
          className="bg-surface-container-lowest border border-outline-variant text-on-surface text-xs sm:text-sm rounded-lg px-2 py-1.5 focus:ring-primary focus:border-primary outline-none transition-colors"
          value={sortBy || ''}
          onChange={(e) => onSortChange?.(e.target.value)}
        >
          <option value="">Nổi bật</option>
          <option value="price_asc">Giá thấp đến cao</option>
          <option value="price_desc">Giá cao đến thấp</option>
          <option value="newest">Hàng mới</option>
          <option value="discount_desc">Giảm nhiều nhất</option>
        </select>
      </div>
    </div>
  );
};

export default ShopHeader;
