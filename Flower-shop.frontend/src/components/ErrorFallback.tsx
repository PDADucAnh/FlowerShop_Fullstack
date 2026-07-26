import React from 'react';
import type { FallbackProps } from 'react-error-boundary';

const ErrorFallback: React.FC<FallbackProps> = ({ error, resetErrorBoundary }) => (
  <div className="flex flex-col items-center justify-center min-h-screen bg-gray-50 px-6 text-center">
    <div className="flex items-center justify-center bg-red-100 text-red-600 mb-6 rounded-full" style={{ width: '64px', height: '64px' }}>
      <span className="material-symbols-outlined text-4xl">error_outline</span>
    </div>
    <h2 className="text-lg font-bold text-gray-900 mb-2">Lỗi hệ thống</h2>
    <p className="text-sm text-gray-500 mb-6 max-w-md">{(error as Error).message || 'Đã xảy ra lỗi không mong muốn.'}</p>
    <button className="bg-[#9f224e] hover:bg-[#7d1b3d] text-white px-6 py-2.5 text-sm font-semibold rounded-lg transition-colors cursor-pointer border-0" onClick={resetErrorBoundary}>Thử lại</button>
  </div>
);

export default ErrorFallback;
