import SEO from '../../components/SEO';
import React, { useMemo } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { useOrderDetail } from '../../hooks/useOrders';

const OrderConfirmationPage: React.FC = () => {
  const [searchParams] = useSearchParams();
  const orderIdParam = searchParams.get('orderId');
  const orderId = orderIdParam ? parseInt(orderIdParam, 10) : 0;
  
  const { data: order, isLoading } = useOrderDetail(orderId);

  const { title, header, description, showCheck, isError } = useMemo(() => {
    if (!orderId) {
       return { title: 'Lỗi', header: 'Không tìm thấy đơn hàng', description: 'Không tìm thấy thông tin đơn hàng này.', showCheck: false, isError: true };
    }
    
    if (isLoading) {
      return { title: 'Đang tải...', header: 'Đang tải thông tin đơn hàng', description: 'Vui lòng chờ trong giây lát.', showCheck: false, isError: false };
    }
    
    if (!order) {
      return { title: 'Lỗi', header: 'Không tìm thấy đơn hàng', description: 'Không tìm thấy thông tin đơn hàng này.', showCheck: false, isError: true };
    }

    const isVNPay = order.paymentMethod === 0 || order.paymentMethod === 'OnlinePayment' || order.paymentMethod === 'VNPay';
    
    const isPaid = order.paymentStatus === 1 || order.paymentStatus === 'Completed';
    const isPending = order.paymentStatus === 0 || order.paymentStatus === 'Pending';

    if (isVNPay) {
      if (isPaid) {
        return {
          title: 'Giao dịch thành công',
          header: 'Giao dịch thành công',
          description: 'Đơn hàng đã được thanh toán.',
          showCheck: true,
          isError: false
        };
      } else if (isPending) {
        return {
          title: 'Đang chờ thanh toán',
          header: 'Đơn hàng đã được tạo.',
          description: 'Đang chờ thanh toán.',
          showCheck: false,
          isError: false
        };
      } else {
        return {
          title: 'Thanh toán thất bại',
          header: 'Giao dịch không thành công',
          description: 'Đã có lỗi xảy ra trong quá trình thanh toán.',
          showCheck: false,
          isError: true
        };
      }
    } else {
      // COD
      return {
        title: 'Chờ xác nhận',
        header: 'Đặt hàng thành công',
        description: 'Đơn hàng của bạn đã được ghi nhận.\nĐơn hàng đang chờ cửa hàng xác nhận.',
        showCheck: true,
        isError: false
      };
    }
  }, [order, isLoading, orderId]);

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-[calc(100vh-200px)] flex items-center justify-center py-xl px-margin">
      <SEO title={header} description={title} />
      <div className="w-full max-w-2xl bg-surface-container-lowest rounded-xl p-8 md:p-12 text-center relative overflow-hidden border border-outline-variant/30 shadow-sm">

        <div className="absolute -top-20 -right-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />
        <div className="absolute -bottom-20 -left-20 w-48 h-48 rounded-full bg-primary/5 pointer-events-none" />

        {showCheck ? (
          <div className="size-20 bg-primary/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
            <span
              className="material-symbols-outlined text-4xl text-primary"
              style={{ fontVariationSettings: "'FILL' 1" }}
            >
              check_circle
            </span>
          </div>
        ) : isError ? (
          <div className="size-20 bg-error/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
            <span
              className="material-symbols-outlined text-4xl text-error"
              style={{ fontVariationSettings: "'FILL' 1" }}
            >
              error
            </span>
          </div>
        ) : (
          <div className="size-20 bg-secondary/10 flex items-center justify-center mx-auto rounded-full mb-6 relative z-10">
            <span
              className="material-symbols-outlined text-4xl text-secondary"
              style={{ fontVariationSettings: "'FILL' 1" }}
            >
              hourglass_empty
            </span>
          </div>
        )}

        <div className="space-y-sm mb-8 relative z-10">
          <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">{title}</h3>
          <h2 className="font-display-lg text-display-lg-mobile md:text-display-lg uppercase tracking-tight text-on-surface">
            {header}
          </h2>
          <div className={`w-12 h-0.5 mx-auto ${isError ? 'bg-error' : 'bg-primary'}`}></div>
        </div>

        {orderId > 0 && (
          <div className="border border-outline-variant bg-surface-container-low rounded-lg p-6 max-w-md mx-auto mb-8 space-y-3 relative z-10">
            <span className="text-[10px] uppercase tracking-widest text-secondary block font-bold">Mã đơn hàng</span>
            <span className={`font-headline-lg text-headline-lg-mobile md:text-headline-lg font-bold block ${isError ? 'text-error' : 'text-primary'}`}>
              #{orderId}
            </span>
          </div>
        )}

        <div className="space-y-md text-secondary max-w-md mx-auto mb-8 relative z-10">
          <p className="text-body-md whitespace-pre-line">
            {description}
          </p>
          {!isError && (
             <p className="text-[10px] uppercase tracking-widest font-bold">
               Thời gian giao dự kiến: 5–7 Ngày làm việc
             </p>
          )}
        </div>

        <div className="flex flex-col sm:flex-row gap-4 justify-center items-center relative z-10">
          {isError ? (
            <Link
              to={orderId ? `/my-orders/${orderId}` : '/my-orders'}
              className="w-full sm:w-auto bg-primary text-on-primary px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none btn-luxury btn-primary-luxury text-center rounded-lg"
            >
              Thanh toán lại
            </Link>
          ) : (
             <Link
              to="/shop"
              className="w-full sm:w-auto bg-primary text-on-primary px-8 py-4 text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none btn-luxury btn-primary-luxury text-center rounded-lg"
            >
              Tiếp tục mua sắm
            </Link>
          )}
          <Link
            to={orderId ? `/my-orders/${orderId}` : '/my-orders'}
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
