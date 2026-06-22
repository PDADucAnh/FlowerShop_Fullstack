import React, { useState, useEffect, useRef } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useCart } from '../context/CartContext';
import { useWishlist } from '../context/WishlistContext';
import { useProductCategories } from '../hooks/useCategories';

const Header: React.FC = () => {
  const { user, isAuthenticated, logout } = useAuth();
  const { cartCount } = useCart();
  const { favoritesCount } = useWishlist();
  useProductCategories();
  const location = useLocation();
  const navigate = useNavigate();
  const [searchOpen, setSearchOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const searchRef = useRef<HTMLDivElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  const isActive = (path: string) => {
    if (path === '/') return location.pathname === '/';
    return location.pathname.startsWith(path);
  };

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) setDropdownOpen(false);
      if (searchRef.current && !searchRef.current.contains(e.target as Node)) setSearchOpen(false);
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const navLinkClass = (path: string) => {
    const active = isActive(path);
    return `font-label-md text-label-md transition-colors duration-300 pb-1 ${
      active
        ? 'text-primary border-b-2 border-primary'
        : 'text-on-surface-variant hover:text-primary'
    }`;
  };

  const badgeClass = "absolute -top-1.5 -right-1.5 min-w-[18px] h-[18px] flex items-center justify-center bg-error text-on-error text-[10px] font-bold rounded-full leading-none px-1";

  return (
    <header className="sticky top-0 z-50 shadow-sm bg-surface w-full shadow-[0px_4px_20px_rgba(171,44,93,0.02)]">
      <div className="flex justify-between items-center px-margin-mobile md:px-margin-desktop py-4 max-w-container-max mx-auto w-full">
        <Link to="/" className="text-headline-md font-headline-md text-primary tracking-tight text-decoration-none">FlowerShop</Link>

        <nav className="hidden md:flex space-x-gutter items-center">
          <Link className={navLinkClass('/shop')} to="/shop">Shop</Link>
          <Link className={navLinkClass('/blog')} to="/blog">Journal</Link>
          <Link className={navLinkClass('/about')} to="/about">About</Link>
          <Link className={navLinkClass('/about')} to="/about">Contact</Link>
        </nav>

        <div className="flex items-center space-x-stack-sm text-primary">
          <div className="relative" ref={searchRef}>
            <button onClick={() => { setSearchOpen(prev => !prev); if (!searchOpen) setTimeout(() => searchInputRef.current?.focus(), 50); }} className="btn-ghost-luxury" aria-label="Search">
              <span className="material-symbols-outlined">search</span>
            </button>
            {searchOpen && (
              <div className="absolute right-0 top-full mt-2 w-[300px] bg-surface-container-lowest shadow-[0px_10px_30px_rgba(0,0,0,0.12)] p-4 z-50 border border-outline-variant">
                <p className="font-label-sm text-label-sm text-secondary uppercase tracking-[0.25em] mb-3">Search</p>
                <div className="relative">
                  <input ref={searchInputRef} value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)}
                    placeholder="Search for arrangements..." className="w-full bg-surface-container-low border-none text-sm text-on-surface placeholder:text-outline py-2.5 pl-3 pr-10 font-body-md outline-none focus:ring-1 focus:ring-primary transition-all"
                    onKeyDown={(e) => { if (e.key === 'Enter' && searchQuery.trim()) { navigate(`/shop?search=${encodeURIComponent(searchQuery.trim())}`); setSearchOpen(false); setSearchQuery(''); } }} />
                  <span className="material-symbols-outlined text-[18px] text-outline absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">search</span>
                </div>
              </div>
            )}
          </div>
          <Link to="/wishlist" className="btn-ghost-luxury text-decoration-none relative" aria-label="Wishlist">
            <span className="material-symbols-outlined">favorite</span>
            {favoritesCount > 0 && <span className={badgeClass}>{favoritesCount > 99 ? '99+' : favoritesCount}</span>}
          </Link>
          <Link to="/cart" className="btn-ghost-luxury text-decoration-none relative" aria-label="Cart">
            <span className="material-symbols-outlined">shopping_cart</span>
            {cartCount > 0 && <span className={badgeClass}>{cartCount > 99 ? '99+' : cartCount}</span>}
          </Link>
          {isAuthenticated ? (
            <div className="relative" ref={dropdownRef}>
              <button onClick={() => setDropdownOpen(prev => !prev)} className="btn-ghost-luxury">
                <span className="material-symbols-outlined">person</span>
              </button>
              {dropdownOpen && (
                <div className="absolute right-0 top-full mt-2 min-w-[220px] bg-surface-container-lowest border border-outline-variant shadow-[0px_10px_30px_rgba(0,0,0,0.08)] z-50">
                  <div className="px-md py-sm border-b border-outline-variant/30">
                    <p className="text-xs uppercase tracking-wider text-secondary font-bold truncate">{user?.fullName || user?.username}</p>
                    <p className="text-[10px] uppercase tracking-widest text-outline">{user?.role}</p>
                  </div>
                  <Link to="/profile" onClick={() => setDropdownOpen(false)} className="flex items-center gap-3 px-md py-sm text-xs uppercase tracking-widest text-secondary hover:text-primary hover:bg-surface-container transition-colors text-decoration-none">
                    <span className="material-symbols-outlined text-[18px]">person</span> My Profile
                  </Link>
                  <Link to="/my-orders" onClick={() => setDropdownOpen(false)} className="flex items-center gap-3 px-md py-sm text-xs uppercase tracking-widest text-secondary hover:text-primary hover:bg-surface-container transition-colors text-decoration-none">
                    <span className="material-symbols-outlined text-[18px]">receipt_long</span> My Orders
                  </Link>
                  <div className="border-t border-outline-variant/30">
                    <button onClick={() => { logout(); setDropdownOpen(false); }} className="flex items-center gap-3 w-full px-md py-sm text-xs uppercase tracking-widest text-secondary hover:text-error hover:bg-surface-container transition-colors">
                      <span className="material-symbols-outlined text-[18px]">logout</span> Sign Out
                    </button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            <Link to="/login" className="btn-ghost-luxury text-decoration-none">
              <span className="material-symbols-outlined">person</span>
            </Link>
          )}
          <button onClick={() => setMobileMenuOpen(!mobileMenuOpen)} aria-label="menu" className="md:hidden text-primary btn-ghost-luxury">
            <span className="material-symbols-outlined">{mobileMenuOpen ? 'close' : 'menu'}</span>
          </button>
        </div>
      </div>
      {mobileMenuOpen && (
        <div className="md:hidden border-t border-outline-variant/30 bg-surface">
          <div className="flex flex-col px-margin-mobile py-stack-md space-y-stack-md">
            <Link to="/shop" onClick={() => setMobileMenuOpen(false)} className="font-label-md text-label-md text-on-surface-variant hover:text-primary transition-colors">Shop</Link>
            <Link to="/blog" onClick={() => setMobileMenuOpen(false)} className="font-label-md text-label-md text-on-surface-variant hover:text-primary transition-colors">Journal</Link>
            <Link to="/about" onClick={() => setMobileMenuOpen(false)} className="font-label-md text-label-md text-on-surface-variant hover:text-primary transition-colors">About</Link>
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;
