import React from 'react';

interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null;

  const getPages = () => {
    const pages: (number | 'ellipsis')[] = [];
    const delta = 2;
    const start = Math.max(2, page - delta);
    const end = Math.min(totalPages - 1, page + delta);

    pages.push(1);
    if (start > 2) pages.push('ellipsis');
    for (let i = start; i <= end; i++) pages.push(i);
    if (end < totalPages - 1) pages.push('ellipsis');
    if (totalPages > 1) pages.push(totalPages);

    return pages;
  };

  return (
    <nav className="flex items-center justify-center gap-1 mt-xl" aria-label="Phân trang">
      <button
        onClick={() => onPageChange(page - 1)}
        disabled={page <= 1}
        className="flex items-center justify-center size-10 rounded-full text-label-sm transition-colors duration-200 disabled:opacity-30 disabled:cursor-not-allowed hover:bg-surface-container text-on-background"
        aria-label="Trang trước"
      >
        <span className="material-symbols-outlined text-lg">chevron_left</span>
      </button>

      {getPages().map((p, i) =>
        p === 'ellipsis' ? (
          <span key={`ellipsis-${i}`} className="flex items-center justify-center size-10 text-label-sm text-secondary">
            ...
          </span>
        ) : (
          <button
            key={p}
            onClick={() => onPageChange(p)}
            className={`flex items-center justify-center size-10 rounded-full text-label-sm font-bold transition-colors duration-200 ${
              p === page
                ? 'bg-primary text-on-primary'
                : 'hover:bg-surface-container text-on-background'
            }`}
            aria-current={p === page ? 'page' : undefined}
          >
            {p}
          </button>
        )
      )}

      <button
        onClick={() => onPageChange(page + 1)}
        disabled={page >= totalPages}
        className="flex items-center justify-center size-10 rounded-full text-label-sm transition-colors duration-200 disabled:opacity-30 disabled:cursor-not-allowed hover:bg-surface-container text-on-background"
        aria-label="Trang sau"
      >
        <span className="material-symbols-outlined text-lg">chevron_right</span>
      </button>
    </nav>
  );
}

export default Pagination;
