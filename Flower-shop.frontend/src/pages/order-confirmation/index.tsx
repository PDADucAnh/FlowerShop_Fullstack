import SEO from '../../components/SEO';
import React from 'react';
import { Link, useSearchParams } from 'react-router-dom';

const OrderConfirmationPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const orderId = searchParams.get('orderId');

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-[calc(100vh-200px)] flex items-center justify-center py-xl px-margin">
      <SEO title="Xác nhận đơn hàng" description="Xác nhận đơn hàng thành công" />
      <div className="w-full max-w-2xl bg-surface-container-lowest rounded-xl p-8 md:p-12 text-center relative overflow-hidden border border-outline-variant/30 shadow-sm">
        
        {/* Floating decorative circles */}
        <div className="absolute -top-20 -right-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />
        <div className="absolute -bottom-20 -left-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />
        
        {/* Icon check_circle with FILL 1 */}
        <div className="size-20 bg-primary/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
          <span 
            className="material-symbols-outlined text-4xl text-primary"
            style={{ fontVariationSettings: "'FILL' 1" }}
          >
            check_circle
          </span>
        </div>

        {/* Content header */}
        <div className="space-y-sm mb-8 relative z-10">
          <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Giao dịch hoàn tất</h3>
          <h2 className="font-display-lg text-display-lg-mobile md:text-display-lg uppercase tracking-tight text-on-surface">
            Xác nhận đơn hàng
          </h2>
          <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </div>

        {/* Detailed box with border border-outline-variant displaying #orderId in primary color */}
        {orderId && (
          <div className="border border-outline-variant bg-surface-container-low rounded-lg p-6 max-w-md mx-auto mb-8 space-y-3 relative z-10">
            <span className="text-[10px] uppercase tracking-widest text-secondary block font-bold">Mã đơn hàng</span>
            <span className="font-headline-lg text-headline-lg-mobile md:text-headline-lg text-primary font-bold block">
              #{orderId}
            </span>
          </div>
        )}

        {/* Description / Instruction */}
        <div className="space-y-md text-secondary max-w-md mx-auto mb-8 relative z-10">
          <p className="text-body-md">
            Đơn hàng của bạn đã được ghi nhận và đang được chuẩn bị giao.
            Email xác nhận sẽ được gửi đến địa chỉ của bạn.
          </p>
          <p className="text-[10px] uppercase tracking-widest font-bold">
            Thời gian giao dự kiến: 5–7 Ngày làm việc
          </p>
        </div>

        {/* Actions section linking to /shop and /my-orders */}
        <div className="flex flex-col sm:flex-row gap-4 justify-center items-center relative z-10">
          <Link
            to="/shop"
            className="w-full sm:w-auto bg-primary text-on-primary px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none btn-luxury btn-primary-luxury text-center rounded-lg"
          >
            Tiếp tục mua sắm
          </Link>
          <Link
            to="/my-orders"
            className="w-full sm:w-auto border border-outline-variant px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none text-on-background btn-luxury text-center rounded-lg"
          >
            Xem đơn hàng
          </Link>
        </div>

      </div>
    </div>
  );
};

export default OrderConfirmationPage;
