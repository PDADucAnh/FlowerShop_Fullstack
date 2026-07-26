import React, { useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useMyOrders, useCancelOrder } from '../../hooks/useOrders';
import { useAuth } from '../../context/AuthContext';
import { formatCurrency } from '../../utils/currency';
import { CancelModal, OrderSkeleton, AccountSidebar, statusStyles } from '../../components/OrderComponents';
import { getOrderStatusText } from '../../utils/statusMappers';
import type { Order } from '../../types/order';
import { useScrollReveal } from '../../hooks/useScrollReveal';
import SEO from '../../components/SEO';
import { getImageUrl } from '../../utils/apiUtils';
import axiosClient from '../../api/axiosClient';

const MyOrders: React.FC = () => {
  const { user, loading: authLoading } = useAuth();
  const navigate = useNavigate();
  const { data: orders, isPending, isError } = useMyOrders(user?.id, authLoading);
  const cancelOrder = useCancelOrder(() => setCancelTarget(null));
  const [cancelTarget, setCancelTarget] = React.useState<number | null>(null);
  const [retryingId, setRetryingId] = React.useState<number | null>(null);
  const { ref, isVisible } = useScrollReveal({ threshold: 0 });

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      navigate('/login', { replace: true });
    }
  }, [user, authLoading, navigate]);

  const handleRetryPayment = async (orderId: number) => {
    setRetryingId(orderId);
    try {
      const res: any = await axiosClient.post(`/Payment/retry/${orderId}`);
      if (res?.url) {
        window.location.href = res.url;
      }
    } catch {
      navigate(`/my-orders/${orderId}`);
    } finally {
      setRetryingId(null);
    }
  };

  const handleCancel = () => {
    if (cancelTarget !== null) {
      cancelOrder.mutate(cancelTarget);
    }
  };

  if (authLoading) {
    return (
      <div className="bg-gray-50 min-h-screen pt-20">
        <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-6">
            <AccountSidebar />
            <section className="flex-grow">
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-sm border border-gray-100">
                <h2 className="text-lg font-bold text-gray-900 mb-4">Lịch sử đơn hàng</h2>
                <OrderSkeleton />
              </div>
            </section>
          </div>
        </main>
      </div>
    );
  }

  if (!user) {
    return null;
  }

  if (isPending) {
    return (
      <div className="bg-gray-50 min-h-screen pt-20">
        <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-6">
            <AccountSidebar />
            <section className="flex-grow">
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-sm border border-gray-100">
                <h2 className="text-lg font-bold text-gray-900 mb-4">Lịch sử đơn hàng</h2>
                <OrderSkeleton />
              </div>
            </section>
          </div>
        </main>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="bg-gray-50 min-h-screen pt-20">
        <main className="max-w-container-max mx-auto px-4 md:px-6 py-6 min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-6">
            <AccountSidebar />
            <section className="flex-grow text-center py-12 max-w-md mx-auto">
              <span className="material-symbols-outlined text-5xl text-gray-300 mb-4 inline-block">error_outline</span>
              <h2 className="text-lg font-semibold text-gray-900 mb-2">Không thể tải đơn hàng</h2>
              <p className="text-sm text-gray-500 mb-6">Đã xảy ra lỗi. Vui lòng thử lại sau.</p>
              <Link to="/" className="inline-flex items-center gap-2 bg-[#9f224e] text-white px-6 py-2.5 text-sm font-semibold rounded-lg hover:bg-[#7d1b3d] transition-colors no-underline">Về trang chủ</Link>
            </section>
          </div>
        </main>
      </div>
    );
  }

  const safeOrders: Order[] = Array.isArray(orders) ? orders : [];

  return (
    <div className="bg-gray-50 min-h-screen pt-20">
      <SEO title="Đơn hàng của tôi" description="Danh sách đơn hàng" />
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
            <div className="bg-white rounded-xl p-4 md:p-6 shadow-sm border border-gray-100">
              <div className="flex items-baseline gap-2 mb-4">
                <h2 className="text-lg font-bold text-gray-900">Lịch sử đơn hàng</h2>
                {safeOrders.length > 0 && (
                  <span className="text-sm text-gray-500 font-normal">({safeOrders.length} đơn hàng)</span>
                )}
              </div>

              {safeOrders.length === 0 ? (
                <div className="text-center py-12">
                  <span className="material-symbols-outlined text-4xl text-gray-300 mb-4 inline-block">receipt_long</span>
                  <p className="text-sm text-gray-500 mb-4">Chưa có đơn hàng. Hãy mua sắm để thấy đơn hàng ở đây.</p>
                  <Link to="/shop" className="inline-flex items-center gap-2 bg-[#9f224e] text-white px-6 py-2.5 text-sm font-semibold rounded-lg hover:bg-[#7d1b3d] transition-colors no-underline">
                    Mua sắm ngay
                    <span className="material-symbols-outlined text-sm">arrow_forward</span>
                  </Link>
                </div>
              ) : (
                <>
                  <div className="md:hidden space-y-3">
                    {safeOrders.map((order: Order) => {
                      const preview = order.orderDetails?.[0];
                      const subtotal = order.orderDetails?.reduce((sum: number, item) => sum + item.unitPrice * item.quantity, 0) ?? 0;
                      const displayTotal = order.finalAmount > 0 ? order.finalAmount : subtotal;
                      const statusClass = statusStyles[order.status] || statusStyles.Pending;
                      const totalQty = order.orderDetails?.reduce((sum: number, item) => sum + item.quantity, 0) ?? 0;
                      return (
                        <div key={order.id} className="bg-white rounded-xl p-4 border border-gray-100 shadow-sm">
                          <div className="flex items-center justify-between mb-3">
                            <span className="text-sm font-semibold text-gray-900">Mã đơn: #{order.id}</span>
                            <span className={`px-2.5 py-0.5 rounded-full text-[10px] font-semibold ${statusClass}`}>
                              {getOrderStatusText(order.status)}
                            </span>
                          </div>
                          <div className="flex items-center gap-3 mb-3">
                            {preview?.productImageUrl ? (
                              <img src={getImageUrl(preview.productImageUrl)} alt={preview.productName || ''} className="w-14 h-14 rounded-lg object-cover" loading="lazy" />
                            ) : (
                              <div className="w-14 h-14 rounded-lg bg-gray-100 flex items-center justify-center">
                                <span className="material-symbols-outlined text-gray-400">inventory_2</span>
                              </div>
                            )}
                            <div className="flex-1 min-w-0">
                              <p className="text-xs text-gray-500">
                                {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                                  year: 'numeric', month: '2-digit', day: '2-digit'
                                })}
                              </p>
                              <p className="text-xs text-gray-500">{totalQty} sản phẩm</p>
                            </div>
                          </div>
                          <div className="flex items-center justify-between border-t border-gray-100 pt-3">
                            <span className="text-sm font-bold text-[#9f224e]">{formatCurrency(displayTotal)}</span>
                            <div className="flex items-center gap-2">
                              {order.status === 'PendingPayment' && (
                                <button
                                  onClick={() => handleRetryPayment(order.id)}
                                  disabled={retryingId === order.id}
                                  className="px-3 py-1 text-xs font-medium rounded-md border border-[#9f224e]/30 text-[#9f224e] bg-transparent hover:bg-[#9f224e]/5 transition-colors cursor-pointer disabled:opacity-50"
                                >
                                  {retryingId === order.id ? 'Đang xử lý...' : 'Thanh toán lại'}
                                </button>
                              )}
                              <Link
                                to={`/my-orders/${order.id}`}
                                className="px-3 py-1 text-xs font-medium rounded-md border border-gray-300 text-gray-700 hover:text-[#9f224e] hover:border-[#9f224e]/30 transition-colors no-underline"
                              >
                                Xem chi tiết ➔
                              </Link>
                            </div>
                          </div>
                        </div>
                      );
                    })}
                  </div>

                  <div className="hidden md:block overflow-x-auto">
                    <table className="w-full text-left">
                      <thead>
                        <tr className="border-b border-gray-200 text-gray-500 text-xs font-medium uppercase tracking-wider">
                          <th className="py-3 pr-4">Sản phẩm</th>
                          <th className="py-3 px-4">Mã đơn</th>
                          <th className="py-3 px-4">Ngày đặt</th>
                          <th className="py-3 px-4">Tổng</th>
                          <th className="py-3 px-4">Trạng thái</th>
                          <th className="py-3 pl-4" />
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-100">
                        {safeOrders.map((order: Order) => {
                          const preview = order.orderDetails?.[0];
                          const subtotal = order.orderDetails?.reduce((sum: number, item) => sum + item.unitPrice * item.quantity, 0) ?? 0;
                          const displayTotal = order.finalAmount > 0 ? order.finalAmount : subtotal;
                          const statusClass = statusStyles[order.status] || statusStyles.Pending;

                          return (
                            <tr key={order.id} className="hover:bg-gray-50 transition-colors">
                              <td className="py-4 pr-4">
                                {preview?.productImageUrl ? (
                                  <img src={getImageUrl(preview.productImageUrl)} alt={preview.productName || ''} className="w-12 h-12 object-cover rounded-lg" loading="lazy" />
                                ) : (
                                  <div className="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center">
                                    <span className="material-symbols-outlined text-gray-400">inventory_2</span>
                                  </div>
                                )}
                              </td>
                              <td className="py-4 px-4 text-sm font-medium text-gray-900">#{order.id}</td>
                              <td className="py-4 px-4 text-sm text-gray-500">
                                {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                                  year: 'numeric', month: '2-digit', day: '2-digit'
                                })}
                              </td>
                              <td className="py-4 px-4 text-sm font-semibold text-[#9f224e]">{formatCurrency(displayTotal)}</td>
                              <td className="py-4 px-4">
                                <span className={`px-2.5 py-0.5 rounded-full text-[10px] font-semibold ${statusClass}`}>
                                  {getOrderStatusText(order.status)}
                                </span>
                              </td>
                              <td className="py-4 pl-4 text-right whitespace-nowrap">
                                <Link to={`/my-orders/${order.id}`} className="text-sm text-[#9f224e] hover:underline font-medium no-underline">
                                  Chi tiết
                                </Link>
                                {order.status === 'PendingPayment' && (
                                  <>
                                    <span className="text-gray-300 mx-2">|</span>
                                    <button
                                      onClick={() => handleRetryPayment(order.id)}
                                      disabled={retryingId === order.id}
                                      className="text-sm text-[#9f224e] hover:underline font-medium bg-transparent border-0 cursor-pointer disabled:opacity-50"
                                    >
                                      {retryingId === order.id ? 'Đang xử lý...' : 'Thanh toán lại'}
                                    </button>
                                  </>
                                )}
                                {(order.status === 'Pending' || order.status === 'PendingVerification' || order.status === 'Confirmed' || order.status === 'PendingPayment') && (
                                  <>
                                    <span className="text-gray-300 mx-2">|</span>
                                    <button
                                      onClick={() => setCancelTarget(order.id)}
                                      className="text-sm text-red-600 hover:underline font-medium bg-transparent border-0 cursor-pointer"
                                    >
                                      Hủy
                                    </button>
                                  </>
                                )}
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                </>
              )}
            </div>
          </section>
        </div>
      </main>

      <CancelModal
        open={cancelTarget !== null}
        onClose={() => setCancelTarget(null)}
        onConfirm={handleCancel}
        loading={cancelOrder.isPending}
      />
    </div>
  );
};

export default MyOrders;
