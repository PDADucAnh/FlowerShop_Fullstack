import React, { useState } from 'react';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';
import type { CartItem } from '../../context/CartContext';

interface CartTableProps {
  items: CartItem[];
  selectedIds: Set<number>;
  onToggleSelect: (id: number) => void;
  onUpdateQuantity: (id: number, qty: number) => void;
  onRemove: (id: number) => void;
}

const CartTable = ({ items, selectedIds, onToggleSelect, onUpdateQuantity, onRemove }: CartTableProps) => {
  const allSelected = items.length > 0 && items.every(item => selectedIds.has(item.id));
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
        <div className="col-span-1 flex items-center">
          <input
            type="checkbox"
            checked={allSelected}
            onChange={() => {
              if (allSelected) {
                items.forEach(item => { if (selectedIds.has(item.id)) onToggleSelect(item.id); });
              } else {
                items.forEach(item => { if (!selectedIds.has(item.id)) onToggleSelect(item.id); });
              }
            }}
            className="rounded border-outline-variant text-primary focus:ring-primary focus:ring-offset-0 cursor-pointer"
          />
        </div>
        <div className="col-span-5 uppercase">Sản phẩm</div>
        <div className="col-span-2 text-center uppercase">Giá</div>
        <div className="col-span-2 text-center uppercase">Số lượng</div>
        <div className="col-span-2 text-right uppercase">Tổng cộng</div>
      </div>

      {items.map((item) => {
        const imageUrl = getImageUrl(item.imageUrl);
        const unitPrice = item.promotionPrice ?? item.discountPrice ?? item.price;
        const totalPrice = unitPrice * item.quantity;
        return (
          <div
            key={item.id}
            className="border-b border-outline-variant hover:bg-surface-container-lowest transition-colors duration-300 group"
          >
            {/* Mobile layout */}
            <div className="md:hidden flex gap-3 px-4 py-4">
              <input
                type="checkbox"
                checked={selectedIds.has(item.id)}
                onChange={() => onToggleSelect(item.id)}
                className="rounded border-outline-variant text-primary focus:ring-primary cursor-pointer mt-1 shrink-0"
              />
              <div className="w-20 h-20 rounded-md bg-surface-variant overflow-hidden shrink-0">
                <img className="w-full h-full object-cover" src={imageUrl} alt={item.name} loading="lazy" />
              </div>
              <div className="flex-1 min-w-0">
                <div className="flex justify-between items-start gap-1">
                  <h3 className="text-sm font-medium text-on-surface line-clamp-2">{item.name}</h3>
                  <button
                    onClick={() => onRemove(item.id)}
                    className="text-on-surface-variant hover:text-error bg-transparent border-0 p-0 cursor-pointer shrink-0"
                  >
                    <span className="material-symbols-outlined text-[18px]">delete</span>
                  </button>
                </div>
                <p className="text-xs text-on-surface-variant mt-0.5">{formatCurrency(unitPrice)}</p>
                <div className="flex justify-between items-center mt-2">
                  <div className="flex items-center border border-outline-variant rounded-md overflow-hidden h-8 bg-surface-container-lowest">
                    <button
                      onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}
                      className="px-2 hover:bg-secondary-container transition-colors text-primary bg-transparent border-0 cursor-pointer h-full text-sm leading-none"
                    >
                      -
                    </button>
                    <input
                      type="number"
                      min={1}
                      max={item.stockQuantity ?? 999}
                      value={inputValues[item.id] ?? item.quantity}
                      onChange={(e) => handleInputChange(item.id, e.target.value)}
                      onBlur={() => handleInputBlur(item.id, item.stockQuantity ?? 999)}
                      onKeyDown={(e) => handleInputKeyDown(e, item.id, item.stockQuantity ?? 999)}
                      className="w-10 text-center text-xs font-medium border-0 bg-transparent outline-none [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none"
                    />
                    <button
                      onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}
                      className="px-2 hover:bg-secondary-container transition-colors text-primary bg-transparent border-0 cursor-pointer h-full text-sm leading-none"
                    >
                      +
                    </button>
                  </div>
                  <span className="text-[#ab2c5d] font-bold text-sm">{formatCurrency(totalPrice)}</span>
                </div>
              </div>
            </div>

            {/* Desktop layout */}
            <div className="hidden md:grid grid-cols-12 gap-4 px-6 py-8 items-center">
              <div className="flex col-span-1 items-center justify-center">
                <input
                  type="checkbox"
                  checked={selectedIds.has(item.id)}
                  onChange={() => onToggleSelect(item.id)}
                  className="rounded border-outline-variant text-primary focus:ring-primary focus:ring-offset-0 cursor-pointer"
                />
              </div>
              <div className="col-span-5 flex items-center space-x-6">
                <div className="w-24 h-32 rounded-lg bg-surface-variant flex-shrink-0 overflow-hidden petal-shadow">
                  <img className="w-full h-full object-cover" src={imageUrl} alt={item.name} loading="lazy" />
                </div>
                <div>
                  <h3 className="font-headline-sm text-headline-sm text-on-surface mb-1">{item.name}</h3>
                  {item.hasFlashSale ? (
                    <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-red-100 text-red-700 rounded text-[10px] font-bold uppercase tracking-wider">
                      <span className="material-symbols-outlined text-[12px]">bolt</span>
                      Flash Sale
                    </span>
                  ) : item.promotionPrice ? (
                    <span className="inline-flex items-center px-2 py-0.5 bg-primary/10 text-primary rounded text-[10px] font-bold uppercase tracking-wider">
                      KM {item.promotionPercent ? `-${item.promotionPercent}%` : ''}
                    </span>
                  ) : null}
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
              <div className="col-span-2 text-center font-body-md text-on-surface">
                {item.promotionPrice || item.discountPrice ? (
                  <>
                    <span className="text-error font-bold">{formatCurrency(item.promotionPrice ?? item.discountPrice ?? 0)}</span>
                    <br /><span className="line-through text-on-surface-variant text-xs">{formatCurrency(item.price)}</span>
                  </>
                ) : formatCurrency(item.price)}
              </div>
              <div className="col-span-2 flex justify-center">
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
                    max={item.stockQuantity ?? 999}
                    value={inputValues[item.id] ?? item.quantity}
                    onChange={(e) => handleInputChange(item.id, e.target.value)}
                    onBlur={() => handleInputBlur(item.id, item.stockQuantity ?? 999)}
                    onKeyDown={(e) => handleInputKeyDown(e, item.id, item.stockQuantity ?? 999)}
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
              <div className="col-span-2 text-right font-headline-sm text-primary text-[20px]">
                {formatCurrency(totalPrice)}
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
};

export default CartTable;
