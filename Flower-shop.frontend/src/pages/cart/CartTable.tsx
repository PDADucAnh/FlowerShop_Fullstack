import React from 'react';
import { getImageUrl } from '../../utils/apiUtils';
import type { CartItem } from '../../context/CartContext';

interface CartTableProps {
  items: CartItem[];
  onUpdateQuantity: (id: number, qty: number) => void;
  onRemove: (id: number) => void;
}

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const CartTable = ({ items, onUpdateQuantity, onRemove }: CartTableProps) => {
  return (
    <div className="bg-surface-container-lowest border border-outline-variant overflow-hidden rounded-xl">
      <div className="overflow-x-auto">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="border-b border-outline-variant font-label-sm text-label-sm uppercase text-secondary tracking-[0.2em] bg-surface-container-low font-bold">
              <th className="px-lg py-4">Item</th>
              <th className="px-lg py-4 text-center">Price</th>
              <th className="px-lg py-4 text-center">Quantity</th>
              <th className="px-lg py-4 text-right">Total</th>
              <th className="px-lg py-4"></th>
            </tr>
          </thead>
          <tbody className="divide-y divide-outline-variant">
            {items.map((item) => {
              const imageUrl = getImageUrl(item.imageUrl);
              return (
                <tr key={item.id} className="transition-colors hover:bg-surface-container-low">
                  <td className="px-lg py-6">
                    <div className="flex items-center gap-lg">
                      <div className="size-20 flex-shrink-0 bg-surface-container overflow-hidden border border-outline-variant rounded-lg">
                        <img src={imageUrl} alt={item.name} className="w-full h-full object-cover" />
                      </div>
                      <div className="space-y-1">
                        <span className="font-label-md text-label-md text-on-surface font-bold block">{item.name}</span>
                        <span className="font-label-sm text-label-sm text-outline block">ID: #FLW-{item.id}</span>
                      </div>
                    </div>
                  </td>
                  <td className="px-lg py-6 text-center font-body-md text-primary font-semibold">{formatCurrency(item.discountPrice || item.price)}</td>
                  <td className="px-lg py-6">
                    <div className="flex items-center justify-center gap-md">
                      <button className="size-8 flex items-center justify-center border border-outline-variant bg-transparent text-secondary outline-none btn-luxury rounded"
                        onClick={() => onUpdateQuantity(item.id, item.quantity - 1)}>
                        <span className="material-symbols-outlined text-sm">remove</span>
                      </button>
                      <span className="font-bold text-sm w-4 text-center">{item.quantity}</span>
                      <button className="size-8 flex items-center justify-center border border-outline-variant bg-transparent text-secondary outline-none btn-luxury rounded"
                        onClick={() => onUpdateQuantity(item.id, item.quantity + 1)}>
                        <span className="material-symbols-outlined text-sm">add</span>
                      </button>
                    </div>
                  </td>
                  <td className="px-lg py-6 text-right font-bold text-on-surface">{formatCurrency((item.discountPrice || item.price) * item.quantity)}</td>
                  <td className="px-lg py-6 text-right">
                    <button className="bg-transparent border-0 text-error p-2 outline-none btn-ghost-luxury" onClick={() => onRemove(item.id)}>
                      <span className="material-symbols-outlined">delete_outline</span>
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default CartTable;
