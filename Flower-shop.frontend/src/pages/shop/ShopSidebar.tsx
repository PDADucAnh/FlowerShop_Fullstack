import React, { useState } from 'react';
import { useProductCategories } from '../../hooks/useCategories';

interface ShopSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeCategoryId: number | null;
  onPriceChange: (min: number | null, max: number | null) => void;
  activePricePreset: string | null;
  setActivePricePreset: (preset: string | null) => void;
}

const ShopSidebar = ({ 
  onCategoryChange, 
  activeCategoryId, 
  onPriceChange,
  activePricePreset,
  setActivePricePreset
}: ShopSidebarProps) => {
  const { data: categories = [] } = useProductCategories();
  const [minInput, setMinInput] = useState('');
  const [maxInput, setMaxInput] = useState('');

  const handlePresetSelect = (preset: string | null, min: number | null, max: number | null) => {
    setActivePricePreset(preset);
    setMinInput('');
    setMaxInput('');
    onPriceChange(min, max);
  };

  const handleCustomApply = (e: React.FormEvent) => {
    e.preventDefault();
    setActivePricePreset('custom');
    const minVal = minInput ? Number(minInput) : null;
    const maxVal = maxInput ? Number(maxInput) : null;
    onPriceChange(minVal, maxVal);
  };

  return (
    <div className="flex flex-col gap-stack-md bg-surface-container-lowest border border-outline-variant/20 rounded-2xl p-6 shadow-sm">
      <div className="mb-stack-sm border-b border-outline-variant/20 pb-4 flex justify-between items-center">
        <h2 className="font-headline-sm text-headline-sm text-on-surface">Lọc theo</h2>
        <button
          className="font-label-sm text-label-sm text-primary hover:underline bg-transparent border-0 p-0 cursor-pointer"
          onClick={() => {
            onCategoryChange(null);
            handlePresetSelect(null, null, null);
          }}
        >
          Xóa tất cả
        </button>
      </div>

      <div className="flex flex-col gap-stack-sm border-b border-outline-variant/20 pb-4">
        <h3 className="font-label-md text-label-md text-on-surface uppercase tracking-widest">Danh mục</h3>
        <div className="flex flex-col gap-2">
          <button
            className={`text-left bg-transparent border-0 p-0 font-body-md text-body-md transition-colors cursor-pointer ${
              activeCategoryId === null ? 'text-primary font-semibold' : 'text-on-surface-variant hover:text-primary'
            }`}
            onClick={() => onCategoryChange(null)}
          >
            Tất cả
          </button>
          {(categories as any[]).map((cat: any) => (
            <button
              key={cat.id}
              className={`text-left bg-transparent border-0 p-0 font-body-md text-body-md transition-colors cursor-pointer ${
                activeCategoryId === cat.id ? 'text-primary font-semibold' : 'text-on-surface-variant hover:text-primary'
              }`}
              onClick={() => onCategoryChange(cat.id)}
            >
              {cat.name}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-col gap-stack-sm border-b border-outline-variant/20 pb-4">
        <h3 className="font-label-md text-label-md text-on-surface uppercase tracking-widest">Khoảng giá</h3>
        <div className="flex flex-col gap-2">
          {[
            { id: 'all', label: 'Tất cả giá', min: null, max: null },
            { id: 'under200', label: 'Dưới 200.000đ', min: null, max: 200000 },
            { id: '200to500', label: '200.000đ - 500.000đ', min: 200000, max: 500000 },
            { id: '500to1000', label: '500.000đ - 1.000.000đ', min: 500000, max: 1000000 },
            { id: 'above1000', label: 'Trên 1.000.000đ', min: 1000000, max: null }
          ].map((preset) => (
            <label key={preset.id} className="flex items-center gap-3 cursor-pointer group">
              <input 
                className="border-outline-variant text-primary focus:ring-primary/20 w-4 h-4 transition-colors" 
                name="pricePreset" 
                type="radio"
                checked={activePricePreset === preset.id || (preset.id === 'all' && activePricePreset === null)}
                onChange={() => handlePresetSelect(preset.id === 'all' ? null : preset.id, preset.min, preset.max)}
              />
              <span className={`font-body-md text-body-md text-on-surface-variant group-hover:text-primary transition-colors ${
                (activePricePreset === preset.id || (preset.id === 'all' && activePricePreset === null)) ? 'text-primary font-semibold' : ''
              }`}>
                {preset.label}
              </span>
            </label>
          ))}
        </div>

        {/* Custom Price Range Input */}
        <form onSubmit={handleCustomApply} className="mt-4 space-y-3">
          <p className="font-label-sm text-label-sm text-on-surface-variant font-medium">Khoảng giá tự chọn (VND)</p>
          <div className="flex items-center gap-2">
            <input 
              type="number"
              placeholder="Từ"
              value={minInput}
              onChange={(e) => setMinInput(e.target.value)}
              className="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg px-3 py-1.5 text-sm placeholder:text-outline/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary/20"
            />
            <span className="text-outline">-</span>
            <input 
              type="number"
              placeholder="Đến"
              value={maxInput}
              onChange={(e) => setMaxInput(e.target.value)}
              className="w-full bg-surface-container-low border border-outline-variant/30 rounded-lg px-3 py-1.5 text-sm placeholder:text-outline/50 focus:outline-none focus:border-primary focus:ring-1 focus:ring-primary/20"
            />
          </div>
          <button 
            type="submit"
            className="w-full bg-primary text-on-primary py-2 rounded-lg font-label-sm text-label-sm hover:bg-primary/90 active:scale-[0.98] transition-all border-0 cursor-pointer"
          >
            Áp dụng
          </button>
        </form>
      </div>
    </div>
  );
};

export default ShopSidebar;
