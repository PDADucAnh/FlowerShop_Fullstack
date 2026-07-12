import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import CartTable from './CartTable';
import SEO from '../../components/SEO';
import { formatCurrency } from '../../utils/currency';

const ShoppingCartPage: React.FC = () => {
  const navigate = useNavigate();
  const { cartItems, removeFromCart, updateQuantity, cartTotal, recalculateCartPrices } = useCart();

  React.useEffect(() => {
    recalculateCartPrices().catch(console.error);
  }, [recalculateCartPrices]);

  if (cartItems.length === 0) {
    return (
      <div className="text-center py-[100px] px-margin-mobile md:px-margin-desktop">
        <div className="size-20 bg-surface-container flex items-center justify-center text-outline mb-4 mx-auto rounded-full">
          <span className="material-symbols-outlined text-4xl">shopping_bag</span>
        </div>
        <div className="space-y-md mb-6">
          <h2 className="font-display-lg text-display-lg text-on-background">Giỏ hàng trống</h2>
          <p className="text-on-surface-variant max-w-md mx-auto font-body-md">Khám phá bộ sưu tập mới nhất và chọn những đóa hoa tuyệt vời nhất cho bạn.</p>
        </div>
        <Link to="/shop" className="bg-primary text-on-primary px-6 py-4 rounded-lg font-label-md text-label-md inline-block no-underline hover:opacity-90 transition-opacity">
          Mua sắm ngay
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-md antialiased min-h-screen pt-20">
      <SEO title="Giỏ hàng" description="Giỏ hàng của bạn" />
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        {/* Title */}
        <div className="mb-stack-lg">
          <h1 className="font-display-lg text-display-lg text-on-background mb-base">Giỏ hàng của bạn</h1>
          <div className="w-16 h-1 bg-primary rounded-full"></div>
        </div>

        {/* Checkout Layout */}
        <div className="flex flex-col lg:flex-row gap-gutter">
          {/* Left Column: Cart Items (70%) */}
          <div className="lg:w-[70%] space-y-gutter">
            <CartTable
              items={cartItems}
              onUpdateQuantity={updateQuantity}
              onRemove={removeFromCart}
            />
            
            <div className="pt-4">
              <Link to="/shop" className="inline-flex items-center gap-2 text-primary font-label-md no-underline hover:underline decoration-primary/30 transition-all">
                <span className="material-symbols-outlined text-lg">arrow_back</span>
                Tiếp tục mua sắm
              </Link>
            </div>
          </div>

          {/* Right Column: Summary (30%) */}
          <aside className="lg:w-[30%] space-y-gutter">
            <div className="bg-surface p-6 rounded-xl border border-outline-variant">
              <h3 className="font-headline-sm text-headline-sm text-on-surface mb-6 pb-4 border-b border-outline-variant/30">
                Tóm tắt đơn hàng
              </h3>
              
              <div className="space-y-4 mb-6">
                <div className="flex justify-between items-center text-on-surface-variant">
                  <span className="font-body-md">Tạm tính</span>
                  <span className="font-label-md">{formatCurrency(cartTotal)}</span>
                </div>
                <div className="flex justify-between items-center text-on-surface-variant">
                  <span className="font-body-md">Phí vận chuyển</span>
                  <span className="font-label-md text-primary">Miễn phí</span>
                </div>
              </div>

              <div className="border-t border-outline-variant/30 pt-6 mb-6 flex justify-between items-center text-on-surface">
                <span className="font-headline-sm text-headline-sm">Tổng cộng</span>
                <span className="font-headline-sm text-headline-sm text-primary">
                  {formatCurrency(cartTotal)}
                </span>
              </div>
              
              <button
                className="w-full bg-primary text-on-primary py-4 rounded-lg font-label-md text-label-md interactive-lift hover:opacity-90 transition-all flex items-center justify-center space-x-2 group border-0 cursor-pointer"
                onClick={() => navigate('/checkout')}
              >
                <span>TIẾN HÀNH THANH TOÁN</span>
                <span className="material-symbols-outlined group-hover:translate-x-1 transition-transform">arrow_forward</span>
              </button>
              
              {/* Trust Badges */}
              <div className="mt-8 grid grid-cols-2 gap-4 text-center">
                <div className="p-3 bg-surface-container-low rounded-lg flex flex-col items-center">
                  <span className="material-symbols-outlined text-primary mb-1">verified_user</span>
                  <p className="font-label-sm text-[10px] uppercase tracking-tighter">Bảo mật 100%</p>
                </div>
                <div className="p-3 bg-surface-container-low rounded-lg flex flex-col items-center">
                  <span className="material-symbols-outlined text-primary mb-1">local_shipping</span>
                  <p className="font-label-sm text-[10px] uppercase tracking-tighter">Giao nhanh 2h</p>
                </div>
              </div>
            </div>
          </aside>
        </div>
      </main>
    </div>
  );
};

export default ShoppingCartPage;
