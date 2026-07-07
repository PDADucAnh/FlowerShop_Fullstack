import SEO from '../../components/SEO';
import React from 'react';
import { Link, useSearchParams } from 'react-router-dom';

const OrderConfirmationPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const orderId = searchParams.get('orderId');
  const payment = searchParams.get('payment');
  const errorMsg = searchParams.get('error');
  const isSuccess = payment === 'success';

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-[calc(100vh-200px)] flex items-center justify-center py-xl px-margin">
      <SEO title={isSuccess ? 'Xác nhận đơn hàng' : 'Thanh toán thất bại'} description={isSuccess ? 'Xác nhận đơn hàng thành công' : 'Thanh toán thất bại'} />
      <div className="w-full max-w-2xl bg-surface-container-lowest rounded-xl p-8 md:p-12 text-center relative overflow-hidden border border-outline-variant/30 shadow-sm">

        <div className="absolute -top-20 -right-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />
        <div className="absolute -bottom-20 -left-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />

        {isSuccess ? (
          <>
            <div className="size-20 bg-primary/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
              <span
                className="material-symbols-outlined text-4xl text-primary"
                style={{ fontVariationSettings: "'FILL' 1" }}
              >
                check_circle
              </span>
            </div>

            <div className="space-y-sm mb-8 relative z-10">
              <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Giao dịch hoàn tất</h3>
              <h2 className="font-display-lg text-display-lg-mobile md:text-display-lg uppercase tracking-tight text-on-surface">
                Xác nhận đơn hàng
              </h2>
              <div className="w-12 h-0.5 bg-primary mx-auto"></div>
            </div>

            {orderId && (
              <div className="border border-outline-variant bg-surface-container-low rounded-lg p-6 max-w-md mx-auto mb-8 space-y-3 relative z-10">
                <span className="text-[10px] uppercase tracking-widest text-secondary block font-bold">Mã đơn hàng</span>
                <span className="font-headline-lg text-headline-lg-mobile md:text-headline-lg text-primary font-bold block">
                  #{orderId}
                </span>
              </div>
            )}

            <div className="space-y-md text-secondary max-w-md mx-auto mb-8 relative z-10">
              <p className="text-body-md">
                Đơn hàng của bạn đã được ghi nhận và đang được chuẩn bị giao.
                Email xác nhận sẽ được gửi đến địa chỉ của bạn.
              </p>
              <p className="text-[10px] uppercase tracking-widest font-bold">
                Thời gian giao dự kiến: 5–7 Ngày làm việc
              </p>
            </div>

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
          </>
        ) : (
          <>
            <div className="size-20 bg-error/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
              <span
                className="material-symbols-outlined text-4xl text-error"
                style={{ fontVariationSettings: "'FILL' 1" }}
              >
                cancel
              </span>
            </div>

            <div className="space-y-sm mb-8 relative z-10">
              <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Thanh toán thất bại</h3>
              <h2 className="font-display-lg text-display-lg-mobile md:text-display-lg uppercase tracking-tight text-on-surface">
                Giao dịch không thành công
              </h2>
              <div className="w-12 h-0.5 bg-error mx-auto"></div>
            </div>

            {orderId && (
              <div className="border border-outline-variant bg-surface-container-low rounded-lg p-6 max-w-md mx-auto mb-8 space-y-3 relative z-10">
                <span className="text-[10px] uppercase tracking-widest text-secondary block font-bold">Mã đơn hàng</span>
                <span className="font-headline-lg text-headline-lg-mobile md:text-headline-lg text-error font-bold block">
                  #{orderId}
                </span>
              </div>
            )}

            <div className="space-y-md text-secondary max-w-md mx-auto mb-8 relative z-10">
              <p className="text-body-md">
                {errorMsg || 'Giao dịch không thành công do lỗi từ cổng thanh toán hoặc bạn đã hủy giao dịch.'}
              </p>
              <p className="text-[10px] uppercase tracking-widest font-bold">
                Vui lòng thử lại hoặc chọn phương thức thanh toán khác
              </p>
            </div>

            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center relative z-10">
              <Link
                to={orderId ? `/my-orders/${orderId}` : '/my-orders'}
                className="w-full sm:w-auto bg-primary text-on-primary px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none btn-luxury btn-primary-luxury text-center rounded-lg"
              >
                Thanh toán lại
              </Link>
              <Link
                to="/shop"
                className="w-full sm:w-auto border border-outline-variant px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none text-on-background btn-luxury text-center rounded-lg"
              >
                Tiếp tục mua sắm
              </Link>
            </div>
          </>
        )}

      </div>
    </div>
  );
};

export default OrderConfirmationPage;
