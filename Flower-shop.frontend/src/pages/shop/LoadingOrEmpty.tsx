import React from 'react';

interface LoadingOrEmptyProps {
  isLoading: boolean;
  message?: string;
}

const LoadingOrEmpty = ({ isLoading, message }: LoadingOrEmptyProps) => {
  if (isLoading) {
    return (
      <div className="w-full text-center py-20">
        <div className="animate-pulse flex flex-col items-center">
            <div className="size-12 bg-surface-container rounded-full mb-stack-sm"></div>
            <p className="font-label-sm text-label-sm text-on-surface-variant">Đang tải...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
      <span className="material-symbols-outlined text-4xl text-outline mb-stack-sm inline-block">inventory_2</span>
      <p className="font-label-sm text-label-sm text-on-surface-variant">{message || 'Không tìm thấy sản phẩm phù hợp.'}</p>
    </div>
  );
};

export default LoadingOrEmpty;
