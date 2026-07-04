import React, { useState } from 'react';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';
import type { CartItem } from '../../context/CartContext';

interface CartTableProps {
  items: CartItem[];
  onUpdateQuantity: (id: number, qty: number) => void;
  onRemove: (id: number) => void;
}

const CartTable = ({ items, onUpdateQuantity, onRemove }: CartTableProps) => {
  const [inputValues, setInputValues] = useState<Record<number, string>>({});

  const handleInputChange = (id: number, value: string) => {
    setInputValues((prev) => ({ ...prev, [id]: value }));
  };

  const handleInputBlur = (id: number, stockQuantity: number) => {
    const raw = inputValues[id];
    const parsed = parseInt(raw, 10);
    if (isNaN(parsed) || parsed < 1) {
      onUpdateQuantity(id, 1);
      setInputValues((prev) => {
        const next = { ...prev };
        delete next[id];
        return next;
      });
      return;
    }
    const clamped = Math.min(parsed, stockQuantity);
    onUpdateQuantity(id, clamped);
    setInputValues((prev) => {
      const next = { ...prev };
      delete next[id];
      return next;
    });
  };

  const handleInputKeyDown = (e: React.KeyboardEvent, id: number, stockQuantity: number) => {
    if (e.key === 'Enter') {
      (e.target as HTMLInputElement).blur();
    }
  };
  return (
    <div className="bg-surface rounded-xl overflow-hidden">
      {/* Table Header */}
      <div className="hidden md:grid grid-cols-12 gap-4 px-6 py-4 border-b border-outline-variant bg-surface-container-low text-on-surface-variant font-label-md">
        <div className="col-span-6 uppercase">Sản phẩm</div>
        <div className="col-span-2 text-center uppercase">Giá</div>
        <div className="col-span-2 text-center uppercase">Số lượng</div>
        <div className="col-span-2 text-right uppercase">Tổng cộng</div>
      </div>

      {items.map((item) => {
        const imageUrl = getImageUrl(item.imageUrl);
        return (
          <div
            key={item.id}
            className="grid grid-cols-1 md:grid-cols-12 gap-4 px-6 py-8 items-center border-b border-outline-variant hover:bg-surface-container-lowest transition-colors duration-300 group"
          >
            <div className="col-span-1 md:col-span-6 flex items-center space-x-6">
              <div className="w-24 h-32 rounded-lg bg-surface-variant flex-shrink-0 overflow-hidden petal-shadow">
                <img
                  className="w-full h-full object-cover"
                  src={imageUrl}
                  alt={item.name}
                  loading="lazy"
                />
              </div>
              <div>
                <h3 className="font-headline-sm text-headline-sm text-on-surface mb-1">{item.name}</h3>
                <p className="font-body-md text-on-surface-variant text-sm">
                  {item.description ? item.description.substring(0, 50) + (item.description.length > 50 ? '...' : '') : 'Sắp xếp hoa cao cấp'}
                </p>
                <button
                  onClick={() => onRemove(item.id)}
                  className="mt-2 text-primary font-label-sm flex items-center hover:underline bg-transparent border-0 cursor-pointer"
                >
                  <span className="material-symbols-outlined text-[16px] mr-1">delete</span> Xóa
                </button>
              </div>
            </div>

            <div className="col-span-1 md:col-span-2 text-center font-body-md text-on-surface">
              <span className="md:hidden font-label-md text-on-surface-variant">Giá: </span>
              {formatCurrency(item.discountPrice || item.price)}
            </div>

            <div className="col-span-1 md:col-span-2 flex justify-center">
              <div className="flex items-center border border-outline-variant rounded-lg overflow-hidden h-10 bg-surface-container-lowest">
                <button
                  onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}
                  className="px-3 hover:bg-secondary-container transition-colors text-primary bg-transparent border-0 cursor-pointer h-full"
                >
                  -
                </button>
                <input
                  type="number"
                  min={1}
                  max={item.stockQuantity}
                  value={inputValues[item.id] ?? item.quantity}
                  onChange={(e) => handleInputChange(item.id, e.target.value)}
                  onBlur={() => handleInputBlur(item.id, item.stockQuantity)}
                  onKeyDown={(e) => handleInputKeyDown(e, item.id, item.stockQuantity)}
                  className="w-14 text-center font-label-md border-0 bg-transparent outline-none [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                />
                <button
                  onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}
                  className="px-3 hover:bg-secondary-container transition-colors text-primary bg-transparent border-0 cursor-pointer h-full"
                >
                  +
                </button>
              </div>
            </div>

            <div className="col-span-1 md:col-span-2 text-right font-headline-sm text-primary text-[20px]">
              <span className="md:hidden font-label-md text-on-surface-variant">Tổng: </span>
              {formatCurrency((item.discountPrice || item.price) * item.quantity)}
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default CartTable;
