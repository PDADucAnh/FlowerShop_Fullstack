import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface AdminLayoutProps {
  children: React.ReactNode;
}

const navigation = [
  { path: '/admin', label: 'Dashboard', icon: 'dashboard' },
  { path: '/admin/orders', label: 'Đơn hàng', icon: 'receipt_long' },
  { path: '/admin/products', label: 'Sản phẩm', icon: 'inventory_2' },
  { path: '/admin/customers', label: 'Khách hàng', icon: 'people' },
  { path: '/admin/payments', label: 'Thanh toán', icon: 'payments' },
  { path: '/admin/promotions', label: 'Khuyến mãi', icon: 'local_offer' },
  { path: '/admin/posts', label: 'Bài viết', icon: 'article' },
  { path: '/admin/advertisements', label: 'Banner', icon: 'slideshow' },
];

const AdminLayout: React.FC<AdminLayoutProps> = ({ children }) => {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-surface flex">
      <aside className={`fixed inset-y-0 left-0 z-50 w-64 bg-surface-container-lowest border-r border-outline-variant transform transition-transform duration-200 ease-in-out ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'} lg:translate-x-0 lg:static lg:z-auto`}>
        <div className="h-16 flex items-center px-6 border-b border-outline-variant">
          <Link to="/admin" className="font-headline-sm text-headline-sm text-primary no-underline">PDA FLOWER</Link>
        </div>

        <nav className="mt-4 px-3 space-y-1">
          {navigation.map((item) => {
            const isActive = location.pathname === item.path || (item.path !== '/admin' && location.pathname.startsWith(item.path));
            return (
              <Link
                key={item.path}
                to={item.path}
                className={`flex items-center gap-3 px-3 py-2.5 rounded-lg font-body-md text-body-md no-underline transition-colors ${
                  isActive
                    ? 'bg-primary-container text-on-primary-container'
                    : 'text-on-surface-variant hover:bg-surface-dim hover:text-on-surface'
                }`}
              >
                <span className="material-symbols-outlined text-[20px]">{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-outline-variant">
          <div className="flex items-center gap-3 mb-3 px-3">
            <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-center text-on-primary font-label-sm text-label-sm">
              {user?.fullName?.charAt(0) || 'A'}
            </div>
            <div className="flex-1 min-w-0">
              <p className="font-label-sm text-label-sm text-on-surface truncate">{user?.fullName || 'Admin'}</p>
              <p className="font-body-md text-body-md text-on-surface-variant truncate">{user?.role}</p>
            </div>
          </div>
          <button onClick={handleLogout} className="flex items-center gap-3 px-3 py-2 w-full rounded-lg font-body-md text-body-md text-on-surface-variant hover:bg-surface-dim hover:text-on-surface no-underline transition-colors bg-transparent border-none cursor-pointer">
            <span className="material-symbols-outlined text-[20px]">logout</span>
            Đăng xuất
          </button>
        </div>
      </aside>

      <div className="flex-1 flex flex-col min-h-screen lg:ml-0">
        <header className="h-16 bg-surface-container-lowest border-b border-outline-variant flex items-center justify-between px-4 lg:px-8 sticky top-0 z-40">
          <button onClick={() => setSidebarOpen(!sidebarOpen)} className="lg:hidden p-2 rounded-lg hover:bg-surface-dim bg-transparent border-none cursor-pointer">
            <span className="material-symbols-outlined text-on-surface">{sidebarOpen ? 'close' : 'menu'}</span>
          </button>
          <div className="flex-1" />
          <Link to="/" className="flex items-center gap-2 font-body-md text-body-md text-on-surface-variant hover:text-primary no-underline transition-colors">
            <span className="material-symbols-outlined text-[20px]">store</span>
            Về trang chủ
          </Link>
        </header>

        <main className="flex-1 p-4 lg:p-8">
          {children}
        </main>
      </div>

      {sidebarOpen && (
        <div onClick={() => setSidebarOpen(false)} className="fixed inset-0 bg-black/30 z-40 lg:hidden" />
      )}
    </div>
  );
};

export default AdminLayout;
