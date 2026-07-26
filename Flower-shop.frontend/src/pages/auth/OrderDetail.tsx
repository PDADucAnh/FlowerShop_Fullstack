import React, { useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useOrderDetail, useCancelOrder } from '../../hooks/useOrders';
import { formatCurrency } from '../../utils/currency';
import { getPaymentMethodText, getPaymentStatusText } from '../../utils/statusMappers';
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
      <div className="bg-gray-50 min-h-screen pt-20">
        <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-6">
            <AccountSidebar />
            <section className="flex-grow">
              <div className="bg-white rounded-xl p-6 shadow-sm border border-gray-100 space-y-4">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="h-6 bg-gray-100 rounded relative overflow-hidden">
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
      <div className="bg-gray-50 min-h-screen pt-20">
        <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-6">
            <AccountSidebar />
            <section className="flex-grow text-center py-12 max-w-md mx-auto">
              <span className="material-symbols-outlined text-5xl text-gray-300 mb-4 inline-block">search_off</span>
              <h2 className="text-lg font-semibold text-gray-900 mb-2">Không tìm thấy đơn hàng</h2>
              <p className="text-sm text-gray-500 mb-6">Đơn hàng này không tồn tại hoặc bạn không có quyền truy cập.</p>
              <Link to="/my-orders" className="inline-flex items-center gap-2 bg-[#9f224e] text-white px-6 py-2.5 text-sm font-semibold rounded-lg hover:bg-[#7d1b3d] transition-colors no-underline">
                Quay lại đơn hàng
              </Link>
            </section>
          </div>
        </main>
      </div>
    );
  }

  const items = order.orderDetails ?? [];
  const totalAfterDiscount = order.finalAmount > 0 ? order.finalAmount : items.reduce((sum: number, item: any) => sum + item.originalPrice * item.quantity, 0) - order.discountAmount;

  return (
    <div className="bg-gray-50 min-h-screen pt-20">
      <SEO title="Chi tiết đơn hàng" description="Chi tiết đơn hàng" />
      <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
        <div className="flex flex-col md:flex-row gap-6">
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
              className="inline-flex items-center gap-1.5 text-gray-500 hover:text-[#9f224e] transition-colors mb-4 text-sm font-medium no-underline"
            >
              <span className="material-symbols-outlined text-lg">arrow_back</span>
              Quay lại đơn hàng
            </Link>

            <div className="bg-white rounded-xl p-4 md:p-6 shadow-sm border border-gray-100 space-y-4">
              <div className="flex flex-col md:flex-row md:items-center justify-between gap-3 pb-4 border-b border-gray-200">
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Mã đơn hàng</p>
                  <h2 className="font-bold text-xl text-gray-900">#{order.id}</h2>
                </div>
                <StatusBadge status={order.status} />
              </div>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pb-4 border-b border-gray-200 text-sm">
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Ngày đặt</p>
                  <p className="text-gray-900 font-medium">
                    {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                      year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'
                    })}
                  </p>
                </div>
                <div>
                  <p className="text-xs text-gray-500 mb-0.5">Khách hàng</p>
                  <p className="text-gray-900 font-medium">{order.customerName || 'N/A'}</p>
                  {order.customerEmail && <p className="text-gray-500 text-xs mt-0.5">{order.customerEmail}</p>}
                  {order.customerPhone && <p className="text-gray-500 text-xs">{order.customerPhone}</p>}
                </div>
              </div>

              {(order.deliveryDate || order.deliveryTimeSlot || order.deliveryDistrict || order.deliveryAddress) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pb-4 border-b border-gray-200 text-sm">
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Giao hàng</p>
                    {order.deliveryDate && <p className="text-gray-900 font-medium">{new Date(order.deliveryDate).toLocaleDateString('vi-VN', { year: 'numeric', month: 'long', day: 'numeric' })}</p>}
                    {order.deliveryTimeSlot && <p className="text-gray-500 text-xs mt-0.5">Khung giờ: {order.deliveryTimeSlot}</p>}
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Địa chỉ nhận</p>
                    {order.deliveryDistrict && <p className="text-gray-900 font-medium">{order.deliveryDistrict}</p>}
                    {order.deliveryAddress && <p className="text-gray-500 text-xs mt-0.5">{order.deliveryAddress}</p>}
                  </div>
                </div>
              )}

              {(order.recipientName || order.recipientPhone) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pb-4 border-b border-gray-200 text-sm">
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Người đặt</p>
                    <p className="text-gray-900 font-medium">{order.customerName || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Người nhận</p>
                    <p className="text-gray-900 font-medium">{order.recipientName || 'N/A'}</p>
                    {order.recipientPhone && <p className="text-gray-500 text-xs mt-0.5">SĐT: {order.recipientPhone}</p>}
                  </div>
                </div>
              )}

              {(order.paymentMethod !== undefined || order.paymentStatus !== undefined) && (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 pb-4 border-b border-gray-200 text-sm">
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Phương thức</p>
                    <p className="text-gray-900 font-medium">{getPaymentMethodText(order.paymentMethod)}</p>
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-0.5">Trạng thái thanh toán</p>
                    <p className={`text-xs font-semibold ${
                      order.paymentStatus === 1 || order.paymentStatus === 'Completed' ? 'text-green-600' : 
                      order.paymentStatus === 2 || order.paymentStatus === 'Failed' ? 'text-red-600' : 'text-gray-500'
                    }`}>
                      {getPaymentStatusText(order.paymentStatus)}
                    </p>
                  </div>
                </div>
              )}

              <div>
                <h3 className="text-sm font-medium text-gray-500 mb-3 flex items-center gap-2">
                  <span className="material-symbols-outlined text-lg">inventory_2</span>
                  Sản phẩm ({items.length})
                </h3>

                {/* Mobile product list */}
                <div className="md:hidden space-y-3">
                  {items.map((item: any) => (
                    <div key={item.id} className="flex items-center gap-3 bg-gray-50 rounded-lg p-3">
                      {item.productImageUrl ? (
                        <div className="w-16 h-16 rounded-lg overflow-hidden bg-gray-100 shrink-0">
                          <img src={getImageUrl(item.productImageUrl)} alt={item.productName || ''} className="w-full h-full object-cover" loading="lazy" />
                        </div>
                      ) : (
                        <div className="w-16 h-16 rounded-lg bg-gray-100 flex items-center justify-center shrink-0">
                          <span className="material-symbols-outlined text-gray-400">image</span>
                        </div>
                      )}
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-gray-900 capitalize leading-tight">{item.productName || `Product #${item.productId}`}</p>
                        <p className="text-xs text-gray-500 mt-0.5">{formatCurrency(item.unitPrice)}</p>
                      </div>
                      <div className="text-right shrink-0">
                        <p className="text-xs text-gray-500">SL: x{item.quantity}</p>
                        <p className="text-sm font-bold text-[#ab2c5d]">{formatCurrency(item.unitPrice * item.quantity)}</p>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Desktop product table */}
                <div className="hidden md:block overflow-x-auto">
                  <table className="w-full text-left">
                    <thead>
                      <tr className="border-b border-gray-200 text-gray-500 text-xs font-medium uppercase tracking-wider">
                        <th className="py-3 pr-4">Sản phẩm</th>
                        <th className="py-3 px-4 text-center">SL</th>
                        <th className="py-3 px-4 text-right">Đơn giá</th>
                        <th className="py-3 pl-4 text-right">Tổng</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                      {items.map((item: any) => (
                        <tr key={item.id} className="hover:bg-gray-50/50 transition-colors">
                          <td className="py-4 pr-4">
                            <div className="flex items-center gap-3">
                              {item.productImageUrl ? (
                                <div className="w-12 h-12 rounded-lg overflow-hidden bg-gray-100 shrink-0">
                                  <img src={getImageUrl(item.productImageUrl)} alt={item.productName || ''} className="w-full h-full object-cover" loading="lazy" />
                                </div>
                              ) : (
                                <div className="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center shrink-0">
                                  <span className="material-symbols-outlined text-gray-400">image</span>
                                </div>
                              )}
                              <span className="text-sm font-medium text-gray-900 capitalize">{item.productName || `Product #${item.productId}`}</span>
                            </div>
                          </td>
                          <td className="py-4 px-4 text-center text-sm text-gray-700">{item.quantity}</td>
                          <td className="py-4 px-4 text-right text-sm text-gray-700">{formatCurrency(item.unitPrice)}</td>
                          <td className="py-4 pl-4 text-right text-sm font-bold text-gray-900">{formatCurrency(item.unitPrice * item.quantity)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {/* Order Summary */}
                <div className="mt-4 pt-4 border-t border-gray-200 space-y-2">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500">Tạm tính</span>
                    <span className="text-gray-700 font-medium">{formatCurrency(order.originalAmount)}</span>
                  </div>
                  {order.discountAmount > 0 && (
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Giảm giá{order.couponCode ? ` (${order.couponCode})` : ''}</span>
                      <span className="text-red-600 font-medium">-{formatCurrency(order.discountAmount)}</span>
                    </div>
                  )}
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-500 flex items-center gap-1">
                      <span className="material-symbols-outlined text-[14px]">local_shipping</span>
                      Phí giao hàng
                    </span>
                    <span className="text-gray-700 font-medium">{order.shippingFee > 0 ? formatCurrency(order.shippingFee) : 'Miễn phí'}</span>
                  </div>
                  <div className="flex justify-between pt-3 border-t border-gray-200">
                    <span className="text-sm font-semibold text-gray-900">TỔNG CỘNG</span>
                    <span className="text-lg font-bold text-[#ab2c5d]">{formatCurrency(totalAfterDiscount)}</span>
                  </div>
                </div>
              </div>

              {order.notes && (
                <div className="pt-4 border-t border-gray-200">
                  <h3 className="text-xs text-gray-500 mb-1.5 flex items-center gap-1">
                    <span className="material-symbols-outlined text-sm">notes</span>
                    Ghi chú
                  </h3>
                  <p className="text-sm text-gray-700 italic leading-relaxed">{order.notes}</p>
                </div>
              )}

              {order.status === 'Cancelled' && order.cancelledAt && (
                <div className="pt-4 border-t border-gray-200">
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <h3 className="text-sm font-semibold text-red-700 mb-2 flex items-center gap-2">
                      <span className="material-symbols-outlined text-lg">cancel</span>
                      Thông tin hủy đơn
                    </h3>
                    <p className="text-sm text-red-600">Đã hủy lúc: {new Date(order.cancelledAt).toLocaleString('vi-VN')}</p>
                    {order.cancellationReason && <p className="text-sm text-red-600 mt-1">Lý do: {order.cancellationReason}</p>}
                    {order.refundAmount > 0 && <p className="text-sm text-red-600 mt-1">Tiền hoàn: {formatCurrency(order.refundAmount)}</p>}
                  </div>
                </div>
              )}

              {order.status === 'PendingPayment' && (
                <div className="pt-4 border-t border-gray-200 flex justify-end">
                  <button
                    onClick={handleRetryPayment}
                    disabled={retrying}
                    className="inline-flex items-center gap-2 bg-[#9f224e] text-white px-5 h-10 text-sm font-semibold rounded-lg hover:bg-[#7d1b3d] transition-all duration-300 border-0 cursor-pointer disabled:opacity-50"
                  >
                    {retrying ? 'Đang xử lý...' : 'Thanh toán lại'}
                    <span className="material-symbols-outlined text-lg">payments</span>
                  </button>
                </div>
              )}
              {(order.canCancel || String(order.status) === 'PendingVerification' || String(order.status) === '4') && (
                <div className="pt-4 border-t border-gray-200 flex flex-col sm:flex-row gap-3">
                  <a
                    href="tel:19006789"
                    className="flex-1 inline-flex items-center justify-center gap-2 h-10 px-4 rounded-lg border border-[#9f224e]/30 text-[#9f224e] text-sm font-medium bg-transparent hover:bg-[#9f224e]/5 transition-colors no-underline"
                  >
                    <span>📞</span>
                    Liên hệ hỗ trợ
                  </a>
                  <button
                    onClick={() => setShowCancel(true)}
                    className="flex-1 inline-flex items-center justify-center gap-2 h-10 px-4 rounded-lg border border-red-300 text-red-600 text-sm font-medium bg-transparent hover:bg-red-50 transition-colors cursor-pointer"
                  >
                    HỦY ĐƠN
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
