import React from 'react';
import { useProductCategories } from '../../hooks/useCategories';

interface ShopSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeId: number | null;
}

const ShopSidebar = ({ onCategoryChange, activeId }: ShopSidebarProps) => {
  const { data: categories = [] } = useProductCategories();

  return (
    <div className="space-y-xl">
      <div className="space-y-md">
        <h6 className="font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Division</h6>
        <ul className="space-y-sm list-none p-0">
          <li>
            <button
              className={`bg-transparent border-0 p-0 font-label-sm text-label-sm uppercase tracking-widest transition-all ${activeId === null ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
              onClick={() => onCategoryChange(null)}
            >
              The Full Collection
            </button>
          </li>
          {(categories as any[]).map((cat: any) => (
            <li key={cat.id}>
              <button
                className={`bg-transparent border-0 p-0 font-label-sm text-label-sm uppercase tracking-widest transition-all ${activeId === cat.id ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
                onClick={() => onCategoryChange(cat.id)}
              >
                {cat.name}
              </button>
            </li>
          ))}
        </ul>
      </div>

      <div className="space-y-md">
        <h6 className="font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Price Bracket</h6>
        <div className="space-y-sm">
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">Under 500k đ</span>
            </label>
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">500k - 2M đ</span>
            </label>
            <label className="flex items-center gap-3 cursor-pointer group">
                <input type="checkbox" className="size-4 border-outline-variant text-primary focus:ring-primary rounded-none" />
                <span className="font-label-sm text-label-sm uppercase tracking-widest text-secondary group-hover:text-primary transition-all">Over 2M đ</span>
            </label>
        </div>
      </div>
    </div>
  );
};

export default ShopSidebar;
