import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useScrollReveal } from '../hooks/useScrollReveal';

export const statusConfig: Record<string, { label: string; dot: string; bg: string; border: string }> = {
  Pending: { label: 'Chờ xử lý', dot: 'bg-tertiary', bg: 'bg-surface-variant text-on-surface-variant', border: 'border-outline-variant/30' },
  PendingVerification: { label: 'Chờ xác minh', dot: 'bg-warning', bg: 'bg-warning/10 text-warning', border: 'border-warning/30' },
  Confirmed: { label: 'Đã xác nhận', dot: 'bg-info', bg: 'bg-info/10 text-info', border: 'border-info/30' },
  Preparing: { label: 'Đang cắm hoa', dot: 'bg-secondary', bg: 'bg-secondary/10 text-secondary', border: 'border-secondary/30' },
  Shipping: { label: 'Đang giao', dot: 'bg-[#1E88E5]', bg: 'bg-[#E3F2FD] text-[#1565C0]', border: 'border-[#90CAF9]/50' },
  Completed: { label: 'Hoàn thành', dot: 'bg-[#43A047]', bg: 'bg-[#E8F5E9] text-[#2E7D32]', border: 'border-[#A5D6A7]/50' },
  Cancelled: { label: 'Đã hủy', dot: 'bg-[#E53935]', bg: 'bg-[#FFEBEE] text-[#C62828]', border: 'border-[#EF9A9A]/50' },
};

export const StatusBadge: React.FC<{ status: string }> = ({ status }) => {
  const s = statusConfig[status] || statusConfig.Pending;
  return (
    <span
      className={`inline-flex items-center gap-1.5 px-3 py-1 text-[10px] font-bold uppercase tracking-widest rounded-full border ${s.bg} ${s.border} transition-all duration-300 hover:scale-[1.02] cursor-default`}
    >
      <span className={`w-1.5 h-1.5 rounded-full ${s.dot} transition-transform duration-300`} />
      {s.label}
    </span>
  );
};

export const CancelModal: React.FC<{ open: boolean; onClose: () => void; onConfirm: () => void; loading: boolean }> = ({ open, onClose, onConfirm, loading }) => {
  const { ref, isVisible } = useScrollReveal({ threshold: 0, once: false });

  return (
    <div
      className={`fixed inset-0 z-[100] flex items-center justify-center p-4 backdrop-blur-sm bg-black/20 transition-all duration-300 ${open ? 'visible' : 'invisible'}`}
      onClick={onClose}
      style={{ opacity: open && isVisible ? 1 : 0 }}
    >
      <div
        ref={ref}
        className="relative bg-surface-container-lowest border border-outline-variant p-xl max-w-sm w-full shadow-xl transition-all duration-500"
        style={{
          transform: isVisible ? 'translateY(0) scale(1)' : 'translateY(24px) scale(0.95)',
          transitionTimingFunction: 'cubic-bezier(0.16, 1, 0.3, 1)',
        }}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="text-center space-y-md">
          <span className="material-symbols-outlined text-5xl text-error inline-block">cancel</span>
          <h3 className="font-headline-sm text-headline-sm uppercase tracking-widest">Hủy đơn hàng</h3>
          <p className="text-body-md text-secondary">Bạn có chắc chắn muốn hủy đơn hàng này? Hành động này không thể hoàn tác.</p>
          <div className="flex gap-md pt-md">
            <button
              onClick={onClose}
              disabled={loading}
              className="flex-1 border border-outline-variant px-lg py-3 text-label-sm uppercase tracking-widest font-bold bg-transparent cursor-pointer hover:bg-surface-container transition-all duration-300 disabled:opacity-50 btn-luxury"
              style={{ transitionTimingFunction: 'cubic-bezier(0.16, 1, 0.3, 1)' }}
            >
              Giữ đơn
            </button>
            <button
              onClick={onConfirm}
              disabled={loading || !open}
              className="flex-1 bg-error text-on-error px-lg py-3 text-label-sm uppercase tracking-widest font-bold border border-error cursor-pointer hover:bg-error/90 transition-all duration-300 disabled:opacity-50 btn-luxury"
              style={{ transitionTimingFunction: 'cubic-bezier(0.16, 1, 0.3, 1)' }}
            >
              {loading ? 'Đang hủy...' : 'Xác nhận hủy'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

const shimmerStyle = {
  background: 'linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.4) 50%, transparent 100%)',
  backgroundSize: '200% 100%',
  animation: 'shimmer 1.8s ease-in-out infinite',
};

export const OrderSkeleton: React.FC = () => (
  <div className="space-y-lg">
    {[1, 2, 3].map((i) => (
      <div key={i} className="bg-surface-container-lowest border border-outline-variant p-lg">
        <div className="flex justify-between items-start mb-md pb-md border-b border-outline-variant/50">
          <div className="space-y-2">
            <div className="h-3 w-24 bg-surface-container-low rounded relative overflow-hidden">
              <div style={shimmerStyle} className="absolute inset-0" />
            </div>
            <div className="h-5 w-20 bg-surface-container-low rounded relative overflow-hidden">
              <div style={shimmerStyle} className="absolute inset-0" />
            </div>
          </div>
          <div className="h-6 w-20 bg-surface-container-low rounded-full relative overflow-hidden">
            <div style={shimmerStyle} className="absolute inset-0" />
          </div>
        </div>
        <div className="flex justify-between items-center">
          <div className="space-y-2">
            <div className="h-3 w-32 bg-surface-container-low rounded relative overflow-hidden">
              <div style={shimmerStyle} className="absolute inset-0" />
            </div>
            <div className="h-5 w-24 bg-surface-container-low rounded relative overflow-hidden">
              <div style={shimmerStyle} className="absolute inset-0" />
            </div>
          </div>
          <div className="h-10 w-28 bg-surface-container-low rounded relative overflow-hidden">
            <div style={shimmerStyle} className="absolute inset-0" />
          </div>
        </div>
      </div>
    ))}
  </div>
);

const navItems = [
  { label: 'Thông tin cá nhân', path: '/profile', icon: 'person_outline' },
  { label: 'Lịch sử đơn hàng', path: '/my-orders', icon: 'receipt_long' },
  { label: 'Yêu thích', path: '/wishlist', icon: 'favorite' },
];

export const AccountSidebar: React.FC = () => {
  const location = useLocation();

  return (
    <aside className="hidden md:block w-full md:w-64 flex-shrink-0">
      <div className="flex flex-col space-y-base">
        <h1 className="font-headline-sm text-headline-sm text-on-surface mb-stack-md">Tài khoản</h1>
        <nav className="space-y-base">
          {navItems.map((item) => {
            const isActive = location.pathname === item.path;
            return (
              <Link
                key={item.path}
                to={item.path}
                className={`flex items-center gap-stack-sm p-stack-sm rounded-lg w-full text-left transition-all text-decoration-none font-label-md ${
                  isActive
                    ? 'text-primary bg-primary-fixed/30'
                    : 'text-on-surface-variant hover:bg-secondary-container/30'
                }`}
              >
                <span className="material-symbols-outlined text-primary">{item.icon}</span>
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>
        <div className="h-px bg-outline-variant my-base" />
      </div>
    </aside>
  );
};
