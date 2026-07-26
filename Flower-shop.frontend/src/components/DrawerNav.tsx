import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import settingsService from '../services/settingsService';
import { useAuth } from '../context/AuthContext';
import { useWishlist } from '../context/WishlistContext';

interface DrawerNavProps {
  isOpen: boolean;
  onClose: () => void;
}

interface MenuItem {
  label: string;
  path: string;
  icon: string;
}

const menuItems: MenuItem[] = [
  { label: 'Trang chủ', path: '/', icon: 'home' },
  { label: 'Cửa hàng', path: '/shop', icon: 'local_florist' },
  { label: 'Bộ sưu tập', path: '/shop', icon: 'auto_awesome' },
  { label: 'Tin tức', path: '/blog', icon: 'auto_stories' },
  { label: 'Giới thiệu', path: '/about', icon: 'favorite' },
  { label: 'Liên hệ', path: '/contact', icon: 'mail' },
];

const DrawerNav: React.FC<DrawerNavProps> = ({ isOpen, onClose }) => {
  const location = useLocation();
  const { user, isAuthenticated, logout } = useAuth();
  const { favoritesCount } = useWishlist();
  const [storeName, setStoreName] = React.useState('Floraison Boutique');

  React.useEffect(() => {
    settingsService.getStoreInfo().then((res: any) => {
      if (res?.storeName) setStoreName(res.storeName);
    }).catch(() => {});
  }, []);

  React.useEffect(() => {
    const handleResize = () => {
      if (window.innerWidth >= 768 && isOpen) {
        onClose();
      }
    };
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, [isOpen, onClose]);

  React.useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }
    return () => { document.body.style.overflow = ''; };
  }, [isOpen]);

  const isActive = (path: string) => {
    if (path === '/') return location.pathname === '/';
    return location.pathname.startsWith(path);
  };

  return (
    <>
      <div
        className={`fixed inset-0 z-[100] drawer-overlay ${isOpen ? 'active' : ''}`}
        onClick={onClose}
      />
      <aside
        className={`fixed top-0 left-0 z-[101] bg-surface h-full w-80 rounded-r-xl shadow-xl flex flex-col py-stack-md drawer-content ${isOpen ? 'active' : ''}`}
      >
        <div className="px-margin-mobile mb-stack-lg">
          <h2 className="font-display-lg-mobile text-display-lg-mobile text-primary">
            {storeName}
          </h2>
        </div>
        <nav className="flex-1 flex flex-col gap-1">
          {menuItems.map((item) => {
            const active = isActive(item.path);
            return (
              <Link
                key={item.path}
                to={item.path}
                onClick={onClose}
                className={`flex items-center gap-4 py-3 px-6 mx-2 my-1 rounded-full transition-colors duration-200 no-underline ${
                  active
                    ? 'bg-secondary-container text-on-secondary-container'
                    : 'text-on-surface-variant hover:bg-surface-container'
                }`}
              >
                <span className="material-symbols-outlined">{item.icon}</span>
                <span className="font-body-lg text-body-lg">{item.label}</span>
              </Link>
            );
          })}
        </nav>

        <div className="border-t border-outline-variant/20 mx-6 my-2" />

        <div className="flex flex-col gap-1 px-2">
          <Link
            to="/wishlist"
            onClick={onClose}
            className="flex items-center gap-4 py-3 px-4 mx-2 my-1 rounded-full transition-colors duration-200 no-underline text-on-surface-variant hover:bg-surface-container relative"
          >
            <span className="material-symbols-outlined">favorite</span>
            <span className="font-body-lg text-body-lg">Yêu thích</span>
            {favoritesCount > 0 && (
              <span className="ml-auto min-w-[20px] h-[20px] flex items-center justify-center bg-error text-on-error text-[11px] font-bold rounded-full px-1">
                {favoritesCount}
              </span>
            )}
          </Link>

          {isAuthenticated ? (
            <>
              <Link
                to="/profile"
                onClick={onClose}
                className="flex items-center gap-4 py-3 px-4 mx-2 my-1 rounded-full transition-colors duration-200 no-underline text-on-surface-variant hover:bg-surface-container"
              >
                <span className="material-symbols-outlined">person</span>
                <span className="font-body-lg text-body-lg">Hồ sơ</span>
              </Link>
              <Link
                to="/my-orders"
                onClick={onClose}
                className="flex items-center gap-4 py-3 px-4 mx-2 my-1 rounded-full transition-colors duration-200 no-underline text-on-surface-variant hover:bg-surface-container"
              >
                <span className="material-symbols-outlined">receipt_long</span>
                <span className="font-body-lg text-body-lg">Đơn hàng</span>
              </Link>
              <button
                onClick={() => { logout(); onClose(); }}
                className="flex items-center gap-4 py-3 px-4 mx-2 my-1 rounded-full transition-colors duration-200 bg-transparent border-0 cursor-pointer text-left text-on-surface-variant hover:bg-surface-container"
              >
                <span className="material-symbols-outlined">logout</span>
                <span className="font-body-lg text-body-lg">Đăng xuất</span>
              </button>
            </>
          ) : (
            <Link
              to="/login"
              onClick={onClose}
              className="flex items-center gap-4 py-3 px-4 mx-2 my-1 rounded-full transition-colors duration-200 no-underline text-on-surface-variant hover:bg-surface-container"
            >
              <span className="material-symbols-outlined">login</span>
              <span className="font-body-lg text-body-lg">Đăng nhập</span>
            </Link>
          )}
        </div>
      </aside>
    </>
  );
};

export default DrawerNav;
