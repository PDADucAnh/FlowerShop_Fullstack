import React, { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useOrderDetail, useCancelOrder } from '../../hooks/useOrders';
import { formatCurrency } from '../../utils/currency';
import { StatusBadge, CancelModal, AccountSidebar } from '../../components/OrderComponents';
import { useScrollReveal } from '../../hooks/useScrollReveal';
import SEO from '../../components/SEO';
import { getImageUrl } from '../../utils/apiUtils';
import axiosClient from '../../api/axiosClient';

const shimmerStyle = {
  background: 'linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.4) 50%, transparent 100%)',
  backgroundSize: '200% 100%',
  animation: 'shimmer 1.8s ease-in-out infinite',
};

const OrderDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const orderId = Number(id);
  const { data: order, isLoading, isError } = useOrderDetail(orderId);
  const cancelOrder = useCancelOrder(() => setShowCancel(false));
  const [showCancel, setShowCancel] = useState(false);
  const [retrying, setRetrying] = useState(false);
  const { ref, isVisible } = useScrollReveal({ threshold: 0 });
  const navigate = useNavigate();

  const handleRetryPayment = async () => {
    setRetrying(true);
    try {
      const res: any = await axiosClient.post(`/Payment/retry/${orderId}`);
      if (res?.url) {
        window.location.href = res.url;
      }
    } catch {
      navigate(`/my-orders/${orderId}`);
    } finally {
      setRetrying(false);
    }
  };

  if (isLoading) {
    return (
      <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
        <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-stack-lg">
            <AccountSidebar />
            <section className="flex-grow">
              <div className="bg-surface-container-lowest p-stack-lg rounded-xl petal-shadow space-y-lg">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="h-6 bg-surface-container-low rounded relative overflow-hidden">
                    <div style={shimmerStyle} className="absolute inset-0" />
                  </div>
                ))}
              </div>
            </section>
          </div>
        </main>
      </div>
    );
  }

  if (isError || !order) {
    return (
      <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
        <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-stack-lg">
            <AccountSidebar />
            <section className="flex-grow text-center py-xl max-w-md mx-auto">
              <span className="material-symbols-outlined text-5xl text-outline mb-md">search_off</span>
              <h2 className="font-headline-sm text-headline-sm text-secondary uppercase tracking-widest mb-sm">Không tìm thấy đơn hàng</h2>
              <p className="font-body-md text-body-md text-secondary mb-lg">Đơn hàng này không tồn tại hoặc bạn không có quyền truy cập.</p>
              <Link to="/my-orders" className="group inline-flex items-center gap-2 bg-primary text-on-primary px-8 py-3 font-label-sm text-label-sm uppercase tracking-widest border border-primary text-decoration-none rounded-lg btn-luxury btn-primary-luxury">
                Quay lại đơn hàng
              </Link>
            </section>
          </div>
        </main>
      </div>
    );
  }

  const items = order.orderDetails ?? [];
  const subtotal = items.reduce((sum: number, item: any) => sum + item.unitPrice * item.quantity, 0);
  const totalAfterDiscount = order.finalAmount > 0 ? order.finalAmount : subtotal - order.discountAmount;

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Chi tiết đơn hàng" description="Chi tiết đơn hàng" />
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
        <div className="flex flex-col md:flex-row gap-stack-lg">
          <AccountSidebar />

          <section
            ref={ref}
            className="flex-grow transition-all duration-700"
            style={{
              opacity: isVisible ? 1 : 0,
              transform: isVisible ? 'translateY(0)' : 'translateY(16px)',
              transitionTimingFunction: 'cubic-bezier(0.16, 1, 0.3, 1)',
            }}
          >
            <Link
              to="/my-orders"
              className="inline-flex items-center gap-2 text-on-surface-variant hover:text-primary transition-colors mb-stack-md font-label-md text-decoration-none"
            >
              <span className="material-symbols-outlined text-lg">arrow_back</span>
              Quay lại đơn hàng
            </Link>

            <div className="bg-surface-container-lowest p-stack-lg rounded-xl petal-shadow space-y-stack-lg">
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 pb-stack-lg border-b border-outline-variant/30">
                <div>
                  <p className="text-on-surface-variant font-label-md mb-1">Mã đơn hàng</p>
                  <h2 className="font-bold text-2xl serif text-on-surface">#{order.id}</h2>
                </div>
                <StatusBadge status={order.status} />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-stack-md pb-stack-lg border-b border-outline-variant/30">
                <div className="space-y-1">
                  <p className="font-label-md text-on-surface-variant">Ngày đặt</p>
                  <p className="font-body-md">
                    {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                      year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
                    })}
                  </p>
                </div>
                <div className="space-y-1">
                  <p className="font-label-md text-on-surface-variant">Khách hàng</p>
                  <p className="font-body-md font-medium">{order.customerName || 'N/A'}</p>
                  {order.customerEmail && <p className="font-body-md text-sm text-on-surface-variant">{order.customerEmail}</p>}
                  {order.customerPhone && <p className="font-body-md text-sm text-on-surface-variant">{order.customerPhone}</p>}
                </div>
              </div>

              {(order.deliveryDate || order.deliveryTimeSlot || order.deliveryDistrict || order.deliveryAddress) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-stack-md pb-stack-lg border-b border-outline-variant/30">
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Giao hàng</p>
                    {order.deliveryDate && <p className="font-body-md">{new Date(order.deliveryDate).toLocaleDateString('vi-VN', { year: 'numeric', month: 'long', day: 'numeric' })}</p>}
                    {order.deliveryTimeSlot && <p className="font-body-md text-sm text-on-surface-variant">Khung giờ: {order.deliveryTimeSlot}</p>}
                  </div>
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Địa chỉ nhận</p>
                    {order.deliveryDistrict && <p className="font-body-md">{order.deliveryDistrict}</p>}
                    {order.deliveryAddress && <p className="font-body-md text-sm text-on-surface-variant">{order.deliveryAddress}</p>}
                  </div>
                </div>
              )}

              {(order.recipientName || order.recipientPhone) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-stack-md pb-stack-lg border-b border-outline-variant/30">
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Người đặt</p>
                    <p className="font-body-md font-medium">{order.customerName || 'N/A'}</p>
                  </div>
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Người nhận</p>
                    <p className="font-body-md font-medium">{order.recipientName || 'N/A'}</p>
                    {order.recipientPhone && <p className="font-body-md text-sm text-on-surface-variant">SĐT: {order.recipientPhone}</p>}
                  </div>
                </div>
              )}

              {(order.paymentMethod !== undefined || order.paymentStatus !== undefined) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-stack-md pb-stack-lg border-b border-outline-variant/30">
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Thanh toán</p>
                    <p className="font-body-md">{order.paymentMethod === 1 || order.paymentMethod === 'COD' ? 'COD (tiền mặt)' : 'Chuyển khoản / Online'}</p>
                  </div>
                  <div className="space-y-1">
                    <p className="font-label-md text-on-surface-variant">Trạng thái thanh toán</p>
                    <p className="font-body-md">
                      {order.paymentStatus === 1 || order.paymentStatus === 'Completed' ? 'Đã thanh toán' :
                       order.paymentStatus === 2 || order.paymentStatus === 'Failed' ? 'Thất bại' :
                       order.paymentStatus === 3 || order.paymentStatus === 'Refunded' ? 'Đã hoàn tiền' :
                       order.paymentStatus === 4 || order.paymentStatus === 'PartialRefund' ? 'Hoàn tiền một phần' :
                       order.paymentStatus === 5 || order.paymentStatus === 'Expired' ? 'Hết hạn' :
                       order.paymentStatus === 6 || order.paymentStatus === 'Cancelled' ? 'Đã hủy' :
                       order.paymentStatus === 7 || order.paymentStatus === 'RefundPending' ? 'Chờ hoàn tiền' :
                       order.paymentStatus === 8 || order.paymentStatus === 'PartialRefundPending' ? 'Chờ hoàn tiền một phần' :
                       order.paymentStatus === 9 || order.paymentStatus === 'PartialRefunded' ? 'Đã hoàn tiền một phần' :
                       'Chưa thanh toán'}
                    </p>
                  </div>
                </div>
              )}

              <div>
                <h3 className="font-label-md text-on-surface-variant mb-stack-md flex items-center gap-2">
                  <span className="material-symbols-outlined text-lg">inventory_2</span>
                  Sản phẩm ({items.length})
                </h3>
                <div className="overflow-x-auto -mx-stack-lg">
                  <table className="w-full text-left">
                    <thead>
                      <tr className="border-b border-outline-variant text-on-surface-variant font-label-md">
                        <th className="py-3 px-stack-lg">Sản phẩm</th>
                        <th className="py-3 px-stack-lg text-center">SL</th>
                        <th className="py-3 px-stack-lg text-right">Đơn giá</th>
                        <th className="py-3 px-stack-lg text-right">Tổng</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-outline-variant/10">
                      {items.map((item: any) => (
                        <tr key={item.id} className="hover:bg-surface-container-low/30 transition-colors">
                          <td className="py-4 px-stack-lg">
                            <div className="flex items-center gap-3">
                              {item.productImageUrl ? (
                                <div className="w-12 h-12 rounded-lg overflow-hidden bg-surface-container-low shrink-0 border border-outline-variant/20">
                                  <img src={getImageUrl(item.productImageUrl)} alt={item.productName || ''} className="w-full h-full object-cover" loading="lazy" />
                                </div>
                              ) : (
                                <div className="w-12 h-12 rounded-lg bg-surface-container-low flex items-center justify-center shrink-0">
                                  <span className="material-symbols-outlined text-lg text-outline">image</span>
                                </div>
                              )}
                              <span className="font-body-md text-sm uppercase tracking-wider font-medium">{item.productName || `Product #${item.productId}`}</span>
                            </div>
                          </td>
                          <td className="py-4 px-stack-lg text-center font-body-md">{item.quantity}</td>
                          <td className="py-4 px-stack-lg text-right font-body-md">{formatCurrency(item.unitPrice)}</td>
                          <td className="py-4 px-stack-lg text-right font-bold">{formatCurrency(item.unitPrice * item.quantity)}</td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot>
                      <tr className="border-t border-outline-variant/50">
                        <td colSpan={3} className="py-5 px-stack-lg text-right font-bold text-sm uppercase tracking-[0.2em]">Tạm tính</td>
                        <td className="py-5 px-stack-lg text-right font-bold text-xl serif">{formatCurrency(subtotal)}</td>
                      </tr>
                      {order.discountAmount > 0 && (
                        <tr>
                          <td colSpan={3} className="py-3 px-stack-lg text-right font-body-md text-sm">Giảm giá{order.couponCode ? ` (${order.couponCode})` : ''}</td>
                          <td className="py-3 px-stack-lg text-right font-body-md text-error">-{formatCurrency(order.discountAmount)}</td>
                        </tr>
                      )}
                      <tr className="border-t border-outline-variant/30">
                        <td colSpan={3} className="py-5 px-stack-lg text-right font-bold text-sm uppercase tracking-[0.2em]">Tổng cộng</td>
                        <td className="py-5 px-stack-lg text-right font-bold text-xl serif text-primary">{formatCurrency(totalAfterDiscount)}</td>
                      </tr>
                    </tfoot>
                  </table>
                </div>
              </div>

              {order.notes && (
                <div className="pt-stack-lg border-t border-outline-variant/30">
                  <h3 className="font-label-md text-on-surface-variant mb-stack-sm flex items-center gap-2">
                    <span className="material-symbols-outlined text-lg">notes</span>
                    Ghi chú
                  </h3>
                  <p className="font-body-md text-on-surface-variant italic leading-relaxed text-sm">{order.notes}</p>
                </div>
              )}

              {order.status === 'Cancelled' && order.cancelledAt && (
                <div className="pt-stack-lg border-t border-outline-variant/30 bg-error-container/10 rounded-lg p-stack-md">
                  <h3 className="font-label-md text-error mb-stack-sm flex items-center gap-2">
                    <span className="material-symbols-outlined text-lg">cancel</span>
                    Thông tin hủy đơn
                  </h3>
                  <p className="font-body-md text-sm">Đã hủy lúc: {new Date(order.cancelledAt).toLocaleString('vi-VN')}</p>
                  {order.cancellationReason && <p className="font-body-md text-sm">Lý do: {order.cancellationReason}</p>}
                  {order.refundAmount > 0 && <p className="font-body-md text-sm">Tiền hoàn: {formatCurrency(order.refundAmount)}</p>}
                </div>
              )}

              {order.status === 'PendingPayment' && (
                <div className="pt-stack-lg border-t border-outline-variant/30 flex justify-end gap-md">
                  <button
                    onClick={handleRetryPayment}
                    disabled={retrying}
                    className="inline-flex items-center gap-2 bg-primary text-on-primary px-stack-md py-stack-sm text-label-sm uppercase tracking-[0.2em] font-bold rounded-lg hover:bg-primary/90 transition-all duration-300 border-0 cursor-pointer btn-luxury disabled:opacity-50"
                  >
                    {retrying ? 'Đang xử lý...' : 'Thanh toán lại'}
                    <span className="material-symbols-outlined text-lg">payments</span>
                  </button>
                </div>
              )}
              {order.canCancel && (
                <div className="pt-stack-lg border-t border-outline-variant/30 flex justify-end">
                  <button
                    onClick={() => setShowCancel(true)}
                    className="inline-flex items-center gap-2 border border-error/30 px-stack-md py-stack-sm text-label-sm uppercase tracking-[0.2em] font-bold text-error rounded-lg hover:bg-error hover:text-on-error transition-all duration-300 bg-transparent cursor-pointer btn-luxury"
                  >
                    Hủy đơn
                    <span className="material-symbols-outlined text-lg">close</span>
                  </button>
                </div>
              )}
            </div>
          </section>
        </div>
      </main>

      <CancelModal
        open={showCancel}
        onClose={() => setShowCancel(false)}
        onConfirm={() => cancelOrder.mutate(order.id)}
        loading={cancelOrder.isPending}
      />
    </div>
  );
};

export default OrderDetailPage;
