import React from 'react';
import { useProductCategories } from '../../hooks/useCategories';
import { Link } from 'react-router-dom';

interface CategoryMenuProps {
  onSelectCategory: (id: number | null) => void;
  activeId: number | null;
}

function CategoryMenu({ onSelectCategory, activeId }: CategoryMenuProps) {
    const { data: categories = [], isLoading } = useProductCategories();

    if (isLoading) return null;

    const handleCategoryClick = (id: number | null) => {
        if (onSelectCategory) {
            onSelectCategory(id);
        }
    };

    const btnClass = (isActive: boolean) =>
        'flex-shrink-0 px-6 py-3 font-label-sm text-label-sm uppercase tracking-widest rounded-full transition-all duration-300 cursor-pointer border-0 ' +
        (isActive
            ? 'bg-primary text-on-primary shadow-[0_4px_14px_rgba(171,44,93,0.3)]'
            : 'bg-surface-container-low text-on-surface-variant hover:bg-surface-container hover:text-primary');

    return (
        <section className="px-margin mb-xl">
            <div className="flex justify-between items-end mb-lg">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tight">Khám phá bộ sưu tập</h2>
                <Link to="/shop" className="font-label-sm text-label-sm uppercase tracking-widest hover:text-secondary transition-colors border-b border-primary pb-1 text-decoration-none text-primary">Xem tất cả</Link>
            </div>

            <div className="flex gap-3 overflow-x-auto no-scrollbar pb-2">
                <button
                    onClick={() => handleCategoryClick(null)}
                    className={btnClass(activeId === null)}
                >
                    Tất cả
                </button>
                {(categories as any[]).map((cat: any) => (
                    <button
                        key={cat.id}
                        onClick={() => handleCategoryClick(cat.id)}
                        className={btnClass(activeId === cat.id)}
                    >
                        {cat.name}
                    </button>
                ))}
            </div>
        </section>
    );
}

export default CategoryMenu;
