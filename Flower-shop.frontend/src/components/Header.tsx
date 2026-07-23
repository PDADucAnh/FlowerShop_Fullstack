import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { useWishlist } from '../context/WishlistContext';
import { useProductCategories } from '../hooks/useCategories';
import { NotificationBell } from './NotificationBell';
import settingsService, { StoreInfo } from '../services/settingsService';
import layoutService, { HeaderLayout, MenuItem } from '../services/layoutService';

const Header: React.FC = () => {
  const { user, isAuthenticated, logout } = useAuth();
  const { cartCount } = useCart();
  const { favoritesCount } = useWishlist();
  const { data: categoriesData } = useProductCategories();
  const categories = Array.isArray(categoriesData) ? (categoriesData as any[]) : [];
  const navigate = useNavigate();
  const location = useLocation();
  const [searchQuery, setSearchQuery] = useState('');
  const [storeInfo, setStoreInfo] = useState<StoreInfo | null>(null);
  const [layout, setLayout] = useState<HeaderLayout | null>(null);
  const [searchOpen, setSearchOpen] = useState(false);

  useEffect(() => {
    settingsService.getStoreInfo().then((res) => {
      setStoreInfo(res as unknown as StoreInfo);
    });
    layoutService.getLayout().then((res) => {
      setLayout(res.header);
    }).catch(() => {
      // layout fetch failed, keep defaults
    });
  }, []);

  const s = storeInfo;

  if (location.pathname === '/order-confirmation') {
    return (
      <header className="w-full bg-surface py-base px-margin-desktop flex justify-between items-center shadow-sm max-w-[1400px] mx-auto pt-20 md:pt-4">
        <Link className="font-display-lg text-display-lg text-primary tracking-tight md:text-display-lg text-display-lg-mobile no-underline" to="/">
          {s?.storeName ?? 'FlowerShop'}
        </Link>
        <div className="flex items-center gap-2 text-on-surface-variant">
          <span className="material-symbols-outlined">person</span>
          <span className="font-label-md text-label-md">{user?.fullName || user?.username || 'Khách hàng'}</span>
        </div>
      </header>
    );
  }

  const isActive = (path: string) => {
    if (path === '/') return location.pathname === '/';
    return location.pathname.startsWith(path);
  };

  const navLinkClass = (path: string) => {
    const active = isActive(path);
    return `font-label-md text-label-md transition-colors duration-300 no-underline ${
      active
        ? 'text-primary border-b-2 border-primary pb-1'
        : 'text-on-surface-variant hover:text-primary'
    }`;
  };

  const getMenuUrl = (item: MenuItem): string => {
    if (item.isExternal) return item.url;
    if (item.url.startsWith('/')) return item.url;
    return `/${item.url}`;
  };

  const renderMenuItem = (item: MenuItem, depth: number = 0) => {
    if (depth >= 3) return null;
    const hasChildren = item.children && item.children.length > 0;
    const url = getMenuUrl(item);

    if (hasChildren) {
      return (
        <div key={item.id} className="relative group py-2">
          <span className="text-on-surface-variant font-label-md text-label-md hover:text-primary transition-colors duration-300 flex items-center gap-1 cursor-pointer">
            {item.label}
            <span className="material-symbols-outlined text-[18px]">expand_more</span>
          </span>
          <div className="absolute left-0 top-full w-48 bg-surface-container-lowest shadow-lg rounded-lg border border-outline-variant/20 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-300 z-50 overflow-hidden transform origin-top-left scale-95 group-hover:scale-100">
            {item.children.map((child, index) => (
              <div key={child.id}>
                {child.children && child.children.length > 0 ? (
                  <div className="relative group/sub">
                    <Link
                      className={`flex items-center justify-between px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors no-underline ${index > 0 ? 'border-t border-outline-variant/10' : ''}`}
                      to={getMenuUrl(child)}
                      target={child.isExternal ? '_blank' : undefined}
                      rel={child.isExternal ? 'noopener noreferrer' : undefined}
                    >
                      {child.label}
                      <span className="material-symbols-outlined text-[18px]">chevron_right</span>
                    </Link>
                    <div className="absolute left-full top-0 w-48 bg-surface-container-lowest shadow-lg rounded-lg border border-outline-variant/20 opacity-0 invisible group-hover/sub:opacity-100 group-hover/sub:visible transition-all duration-300 z-50 overflow-hidden">
                      {child.children.map((subChild, subIndex) => (
                        <Link
                          key={subChild.id}
                          className={`block px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors no-underline ${subIndex > 0 ? 'border-t border-outline-variant/10' : ''}`}
                          to={getMenuUrl(subChild)}
                          target={subChild.isExternal ? '_blank' : undefined}
                          rel={subChild.isExternal ? 'noopener noreferrer' : undefined}
                        >
                          {subChild.label}
                        </Link>
                      ))}
                    </div>
                  </div>
                ) : (
                  <Link
                    className={`block px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors no-underline ${index > 0 ? 'border-t border-outline-variant/10' : ''}`}
                    to={getMenuUrl(child)}
                    target={child.isExternal ? '_blank' : undefined}
                    rel={child.isExternal ? 'noopener noreferrer' : undefined}
                  >
                    {child.label}
                  </Link>
                )}
              </div>
            ))}
          </div>
        </div>
      );
    }

    return (
      <Link
        key={item.id}
        className={navLinkClass(url)}
        to={url}
        target={item.isExternal ? '_blank' : undefined}
        rel={item.isExternal ? 'noopener noreferrer' : undefined}
      >
        {item.label}
      </Link>
    );
  };

  const renderMobileMenuItem = (item: MenuItem) => {
    const hasChildren = item.children && item.children.length > 0;
    const url = getMenuUrl(item);

    if (hasChildren) {
      return (
        <div key={item.id}>
          <div className="flex items-center justify-between py-2">
            <span className="font-label-md text-label-md text-on-surface-variant">{item.label}</span>
            <span className="material-symbols-outlined text-[18px] text-on-surface-variant">expand_more</span>
          </div>
          <div className="pl-4">
            {item.children.map((child) => (
              <Link
                key={child.id}
                className="block font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline py-2"
                to={getMenuUrl(child)}
                target={child.isExternal ? '_blank' : undefined}
                rel={child.isExternal ? 'noopener noreferrer' : undefined}
                onClick={() => {
                  const nav = document.querySelector('.mobile-nav');
                  if (nav) nav.classList.add('hidden');
                }}
              >
                {child.label}
              </Link>
            ))}
          </div>
        </div>
      );
    }

    return (
      <Link
        key={item.id}
        className="block font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline py-2"
        to={url}
        target={item.isExternal ? '_blank' : undefined}
        rel={item.isExternal ? 'noopener noreferrer' : undefined}
        onClick={() => {
          const nav = document.querySelector('.mobile-nav');
          if (nav) nav.classList.add('hidden');
        }}
      >
        {item.label}
      </Link>
    );
  };

  const hotlineText = layout?.hotline.useDefault
    ? s?.hotline
    : (layout?.hotline.customText ?? s?.hotline);

  const ctaVariantClass = layout?.ctaButton.variant === 'outlined'
    ? 'border border-primary text-primary'
    : 'bg-primary text-on-primary';

  return (
    <>
      {layout?.topBar.isActive && (
        <div className="w-full bg-primary text-on-primary text-center py-1 text-xs">
          {layout.topBar.url ? (
            <a href={layout.topBar.url} className="text-on-primary no-underline hover:underline">
              {layout.topBar.text}
            </a>
          ) : (
            <span>{layout.topBar.text}</span>
          )}
        </div>
      )}
    <header className="sticky top-0 z-50 shadow-sm bg-surface w-full shadow-[0px_4px_20px_rgba(171,44,93,0.02)]">
      <div className="flex justify-between items-center px-margin-mobile md:px-margin-desktop py-4 max-w-[1400px] mx-auto w-full">
        <div className="flex items-center gap-10">
          <Link className="font-headline-md text-headline-md text-primary tracking-tight no-underline whitespace-nowrap" to="/">
            {s?.storeName ?? 'FlowerShop'}
          </Link>
          <nav className="hidden md:flex space-x-gutter items-center">
            {layout?.menuItems && layout.menuItems.length > 0
              ? layout.menuItems.map((item) => renderMenuItem(item))
              : (
                <>
                  <Link className={navLinkClass('/')} to="/">Trang chủ</Link>
                  <Link className={navLinkClass('/shop')} to="/shop">Cửa hàng</Link>
                  <div className="relative group py-2">
                    <span className="text-on-surface-variant font-label-md text-label-md hover:text-primary transition-colors duration-300 flex items-center gap-1 cursor-pointer">
                      Danh mục
                      <span className="material-symbols-outlined text-[18px]">expand_more</span>
                    </span>
                    <div className="absolute left-0 top-full w-48 bg-surface-container-lowest shadow-lg rounded-lg border border-outline-variant/20 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-300 z-50 overflow-hidden transform origin-top-left scale-95 group-hover:scale-100">
                      {categories.map((cat: any, index: number) => (
                        <Link
                          key={cat.id}
                          className={`block px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors no-underline ${index > 0 ? 'border-t border-outline-variant/10' : ''}`}
                          to={`/shop?category=${cat.id}`}
                        >
                          {cat.name}
                        </Link>
                      ))}
                      {categories.length === 0 && (
                        <span className="block px-4 py-3 text-label-sm text-outline uppercase tracking-widest text-center">Trống</span>
                      )}
                    </div>
                  </div>
                  <Link className={navLinkClass('/blog')} to="/blog">Tin tức</Link>
                  <Link className={navLinkClass('/about')} to="/about">Giới thiệu</Link>
                  <Link className={navLinkClass('/contact')} to="/contact">Liên hệ</Link>
                </>
              )
            }
          </nav>
        </div>

        <div className="hidden md:flex items-center flex-grow max-w-xs mx-gutter">
          {(() => {
            const mode = layout?.search.mode ?? 'input';
            if (mode === 'popup') {
              return (
                <>
                  <button
                    onClick={() => setSearchOpen(!searchOpen)}
                    className="text-primary hover:text-primary/80 transition-colors bg-transparent border-0 cursor-pointer flex items-center justify-center"
                    aria-label="search"
                  >
                    <span className="material-symbols-outlined">search</span>
                  </button>
                  {searchOpen && (
                    <div className="fixed inset-0 z-50 flex items-start justify-center pt-20 bg-black/20" onClick={() => setSearchOpen(false)}>
                      <div className="bg-surface w-full max-w-lg mx-4 rounded-lg shadow-xl p-4" onClick={(e) => e.stopPropagation()}>
                        <div className="relative w-full">
                          <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-primary/40 flex items-center justify-center pointer-events-none">search</span>
                          <input
                            className="w-full bg-surface-container-low border-none rounded-full py-2 pl-10 text-label-sm focus:ring-2 focus:ring-primary/20 transition-all pr-10 outline-none"
                            placeholder="Tìm kiếm hoa..."
                            type="text"
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            onKeyDown={(e) => {
                              if (e.key === 'Enter' && searchQuery.trim()) {
                                navigate(`/search?query=${encodeURIComponent(searchQuery.trim())}`);
                                setSearchOpen(false);
                              }
                            }}
                            autoFocus
                          />
                        </div>
                      </div>
                    </div>
                  )}
                </>
              );
            }
            return (
              <div className="relative w-full group">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-primary/40 group-focus-within:text-primary transition-colors flex items-center justify-center pointer-events-none">search</span>
                <input
                  className="w-full bg-surface-container-low border-none rounded-full py-2 pl-10 text-label-sm focus:ring-2 focus:ring-primary/20 transition-all pr-10 outline-none"
                  placeholder="Tìm kiếm hoa..."
                  type="text"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter' && searchQuery.trim()) {
                      navigate(`/search?query=${encodeURIComponent(searchQuery.trim())}`);
                    }
                  }}
                />
                <button
                  onClick={() => {
                    if (searchQuery.trim()) {
                      navigate(`/search?query=${encodeURIComponent(searchQuery.trim())}`);
                    }
                  }}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-primary/40 hover:text-primary transition-colors flex items-center justify-center border-0 bg-transparent cursor-pointer"
                  aria-label="Search"
                />
              </div>
            );
          })()}
        </div>

        <div className="flex items-center space-x-stack-sm text-primary">
          <div className="mobile-nav hidden md:hidden fixed inset-x-0 top-[72px] bg-surface border-t border-outline-variant/20 shadow-lg z-40 flex flex-col p-4 space-y-3">
            {layout?.menuItems && layout.menuItems.length > 0
              ? layout.menuItems.map((item) => renderMobileMenuItem(item))
              : (
                <>
                  <Link className="font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline" to="/" onClick={() => document.querySelector('.mobile-nav')?.classList.add('hidden')}>Trang chủ</Link>
                  <Link className="font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline" to="/shop" onClick={() => document.querySelector('.mobile-nav')?.classList.add('hidden')}>Cửa hàng</Link>
                  <Link className="font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline" to="/blog" onClick={() => document.querySelector('.mobile-nav')?.classList.add('hidden')}>Tin tức</Link>
                  <Link className="font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline" to="/about" onClick={() => document.querySelector('.mobile-nav')?.classList.add('hidden')}>Giới thiệu</Link>
                  <Link className="font-label-md text-label-md text-on-surface-variant hover:text-primary no-underline" to="/contact" onClick={() => document.querySelector('.mobile-nav')?.classList.add('hidden')}>Liên hệ</Link>
                </>
              )
            }
          </div>
            {hotlineText && (
              <a href={`tel:${hotlineText}`} className="hidden md:flex items-center gap-1 text-primary hover:text-primary/80 transition-colors no-underline">
                <span className="material-symbols-outlined text-[18px]">call</span>
                <span className="font-label-md text-label-md">{hotlineText}</span>
              </a>
            )}
            {layout?.zones && layout.zones.left.map((zone, i) => (
              <span key={`zl-${i}`} className="hidden md:block text-label-sm text-on-surface-variant">{zone}</span>
            ))}
            {layout?.ctaButton.isActive && (
              <Link
                to={layout.ctaButton.url || '#'}
                className={`hidden md:inline-flex items-center px-4 py-2 rounded-full font-label-md text-label-md no-underline transition-colors ${ctaVariantClass}`}
              >
                {layout.ctaButton.text}
              </Link>
            )}
          <Link to="/wishlist" aria-label="wishlist" className="hover:text-primary/80 transition-colors text-primary no-underline relative flex items-center justify-center">
            <span className="material-symbols-outlined" data-icon="favorite">favorite</span>
            {favoritesCount > 0 && (
              <span className="absolute -top-1 -right-1 w-2 h-2 bg-primary rounded-full animate-ping" />
            )}
          </Link>
          <Link to="/cart" aria-label="shopping_cart" className="hover:text-primary/80 transition-colors text-primary no-underline relative flex items-center justify-center">
            <span className="material-symbols-outlined" data-icon="shopping_cart">shopping_cart</span>
            {cartCount > 0 && (
              <span className="absolute -top-1.5 -right-1.5 min-w-[18px] h-[18px] flex items-center justify-center bg-error text-on-error text-[10px] font-bold rounded-full leading-none px-1">
                {cartCount > 99 ? '99+' : cartCount}
              </span>
            )}
          </Link>
          <NotificationBell />

          {isAuthenticated ? (
            <div className="relative group py-2">
              <button className="flex items-center space-x-2 hover:text-primary/80 transition-colors text-primary bg-transparent border-0 cursor-pointer">
                <span className="material-symbols-outlined" data-icon="person">person</span>
                <span className="font-label-md text-label-md">{user?.fullName || user?.username || 'Tài khoản'}</span>
              </button>
              <div className="absolute right-0 top-full w-48 bg-surface-container-lowest shadow-lg rounded-lg border border-outline-variant/20 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-300 z-50 overflow-hidden transform origin-top-right scale-95 group-hover:scale-100">
                <Link className="flex items-center gap-3 px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors no-underline" to="/profile">
                  <span className="material-symbols-outlined text-[20px]">person</span>
                  <span className="">Hồ sơ</span>
                </Link>
                <Link className="flex items-center gap-3 px-4 py-3 text-label-md text-on-surface hover:bg-surface-container-low hover:text-primary transition-colors border-t border-outline-variant/10 no-underline" to="/my-orders">
                  <span className="material-symbols-outlined text-[20px]">receipt_long</span>
                  <span className="">Đơn hàng</span>
                </Link>
                <button
                  onClick={logout}
                  className="w-full text-left flex items-center gap-3 px-4 py-3 text-label-md text-error hover:bg-error-container/20 transition-colors border-t border-outline-variant/10 bg-transparent border-0 cursor-pointer"
                >
                  <span className="material-symbols-outlined text-[20px]">logout</span>
                  <span className="">Đăng xuất</span>
                </button>
              </div>
            </div>
          ) : (
            <Link to="/login" className="flex items-center space-x-2 hover:text-primary/80 transition-colors text-primary no-underline">
              <span className="material-symbols-outlined" data-icon="person">person</span>
              <span className="font-label-md text-label-md">Đăng nhập</span>
            </Link>
          )}

          <button
            aria-label="menu"
            className="md:hidden text-primary bg-transparent border-0 cursor-pointer flex items-center justify-center"
            onClick={() => {
              const nav = document.querySelector('.mobile-nav');
              if (nav) {
                nav.classList.toggle('hidden');
              }
            }}
          >
            <span className="material-symbols-outlined">menu</span>
          </button>
        </div>
      </div>
    </header>
    </>
  );
};

export default Header;
