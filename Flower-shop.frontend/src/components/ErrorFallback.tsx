import React from 'react';

import { FallbackProps } from 'react-error-boundary';

type ErrorFallbackProps = FallbackProps;

const ErrorFallback: React.FC<ErrorFallbackProps> = ({ error, resetErrorBoundary }) => (
  <div className="d-flex flex-column align-items-center justify-content-center min-vh-100 bg-light px-3 text-center">
    <div className="d-flex align-items-center justify-content-center bg-secondary bg-opacity-10 text-secondary mb-4 rounded-circle" style={{ width: '64px', height: '64px' }}>
      <span className="material-symbols-outlined" style={{ fontSize: '40px' }}>error_outline</span>
    </div>
    <h2 className="fw-bold text-secondary mb-2">System Error</h2>
    <p className="text-muted small mb-4" style={{ maxWidth: '400px' }}>{(error as Error).message}</p>
    <button className="btn btn-dark btn-sm px-4" onClick={resetErrorBoundary}>Retry</button>
  </div>
);

export default ErrorFallback;
