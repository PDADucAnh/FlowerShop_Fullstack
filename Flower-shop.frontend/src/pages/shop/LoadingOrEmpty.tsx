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
            <div className="size-12 bg-surface-container rounded-full mb-md"></div>
            <p className="text-label-sm uppercase tracking-widest text-secondary">Curating Content...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
      <span className="material-symbols-outlined text-4xl text-outline mb-md">inventory_2</span>
      <p className="text-label-sm uppercase tracking-widest text-secondary">{message || 'No distinctive pieces found.'}</p>
    </div>
  );
};

export default LoadingOrEmpty;
