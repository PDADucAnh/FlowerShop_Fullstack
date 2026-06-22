import React from 'react';

interface ShopHeaderProps {
  count: number;
}

const ShopHeader = ({ count }: ShopHeaderProps) => {
  return (
    <div className="flex justify-between items-center mb-lg border-b border-outline-variant pb-md">
      <span className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-semibold">{count || 0} Distinctive Pieces</span>
      <div className="flex items-center gap-md">
        <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Sequence:</label>
        <select className="bg-transparent border-none font-label-sm text-label-sm uppercase tracking-widest font-bold focus:ring-0 cursor-pointer text-primary">
          <option>New Arrivals</option>
          <option>Value: Ascending</option>
          <option>Value: Descending</option>
        </select>
      </div>
    </div>
  );
};

export default ShopHeader;
