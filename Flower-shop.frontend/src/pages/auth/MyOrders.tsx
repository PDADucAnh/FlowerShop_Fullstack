import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useMyOrders, useCancelOrder } from '../../hooks/useOrders';
import { formatCurrency } from '../../utils/currency';
import { CancelModal, OrderSkeleton, AccountSidebar } from '../../components/OrderComponents';
import { useScrollReveal } from '../../hooks/useScrollReveal';
import SEO from '../../components/SEO';
import { getImageUrl } from '../../utils/apiUtils';

const statusStyles: Record<string, string> = {
  Pending: 'bg-tertiary-fixed/40 text-tertiary',
  PendingVerification: 'bg-warning/10 text-warning',
  Confirmed: 'bg-info/10 text-info',
  Preparing: 'bg-secondary/10 text-secondary',
  Shipping: 'bg-blue-100 text-blue-700',
  Completed: 'bg-green-100 text-green-700',
  Cancelled: 'bg-red-100 text-red-600',
};

const MyOrders: React.FC = () => {
  const { data: orders, isLoading, isError } = useMyOrders();
  const cancelOrder = useCancelOrder(() => setCancelTarget(null));
  const [cancelTarget, setCancelTarget] = useState<number | null>(null);
  const { ref, isVisible } = useScrollReveal({ threshold: 0 });

  const handleCancel = () => {
    if (cancelTarget !== null) {
      cancelOrder.mutate(cancelTarget);
    }
  };

  if (isLoading) {
    return (
      <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
        <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-stack-lg">
            <AccountSidebar />
            <section className="flex-grow">
              <div className="bg-surface-container-lowest p-stack-lg rounded-xl petal-shadow">
                <h2 className="font-headline-sm text-headline-sm text-on-surface mb-stack-lg">Lịch sử đơn hàng</h2>
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
      <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
        <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-[calc(100vh-200px)]">
          <div className="flex flex-col md:flex-row gap-stack-lg">
            <AccountSidebar />
            <section className="flex-grow text-center py-xl max-w-md mx-auto">
              <span className="material-symbols-outlined text-5xl text-outline mb-md">error_outline</span>
              <h2 className="font-headline-sm text-headline-sm text-secondary uppercase mb-sm">Không thể tải đơn hàng</h2>
              <p className="font-body-md text-body-md text-secondary mb-lg">Đã xảy ra lỗi. Vui lòng thử lại sau.</p>
              <Link to="/" className="bg-primary text-on-primary px-8 py-3 font-label-sm text-label-sm uppercase tracking-widest border border-primary inline-block text-decoration-none btn-luxury btn-primary-luxury">Về trang chủ</Link>
            </section>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Đơn hàng của tôi" description="Danh sách đơn hàng" />
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
            <div className="bg-surface-container-lowest p-stack-lg rounded-xl petal-shadow overflow-x-auto">
              <h2 className="font-headline-sm text-headline-sm text-on-surface mb-stack-lg">
                Lịch sử đơn hàng
                {orders && orders.length > 0 && (
                  <span className="text-on-surface-variant font-body-md text-body-md font-normal ml-2">({orders.length} đơn hàng)</span>
                )}
              </h2>

              {!orders || orders.length === 0 ? (
                <div className="text-center py-12">
                  <span className="material-symbols-outlined text-4xl text-outline mb-md">receipt_long</span>
                  <p className="text-on-surface-variant font-body-md">Chưa có đơn hàng. Hãy mua sắm để thấy đơn hàng ở đây.</p>
                  <Link to="/shop" className="group inline-flex items-center gap-3 bg-primary text-on-primary px-8 py-3 mt-stack-md font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-decoration-none rounded-lg btn-luxury btn-primary-luxury">
                    Mua sắm ngay
                    <span className="w-5 h-5 rounded-full bg-white/20 flex items-center justify-center group-hover:translate-x-0.5 transition-transform duration-300">
                      <span className="material-symbols-outlined text-[12px]">arrow_forward</span>
                    </span>
                  </Link>
                </div>
              ) : (
                <table className="w-full text-left min-w-[600px]">
                  <thead>
                    <tr className="border-b border-outline-variant text-on-surface-variant font-label-md">
                      <th className="py-stack-sm px-base">Sản phẩm</th>
                      <th className="py-stack-sm px-base">Mã đơn</th>
                      <th className="py-stack-sm px-base">Ngày đặt</th>
                      <th className="py-stack-sm px-base">Tổng</th>
                      <th className="py-stack-sm px-base">Trạng thái</th>
                      <th className="py-stack-sm px-base" />
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-outline-variant/30">
                    {orders.map((order: any) => {
                      const preview = order.orderDetails?.[0];
                      const total = order.orderDetails?.reduce((sum: number, item: any) => sum + item.unitPrice * item.quantity, 0) ?? 0;
                      const statusClass = statusStyles[order.status] || statusStyles.Pending;

                      return (
                        <tr key={order.id} className="hover:bg-surface-container-low transition-colors">
                          <td className="py-stack-md px-base">
                            {preview?.productImageUrl ? (
                              <img
                                src={getImageUrl(preview.productImageUrl)}
                                alt={preview.productName || ''}
                                className="w-12 h-12 object-cover rounded-lg shadow-sm"
                                loading="lazy"
                              />
                            ) : (
                              <div className="w-12 h-12 rounded-lg bg-surface-container-low flex items-center justify-center">
                                <span className="material-symbols-outlined text-lg text-outline">inventory_2</span>
                              </div>
                            )}
                          </td>
                          <td className="py-stack-md px-base font-label-md text-on-surface">#{order.id}</td>
                          <td className="py-stack-md px-base text-on-surface-variant">
                            {new Date(order.orderDate).toLocaleDateString('vi-VN', {
                              year: 'numeric', month: '2-digit', day: '2-digit'
                            })}
                          </td>
                          <td className="py-stack-md px-base font-semibold text-primary">{formatCurrency(total)}</td>
                          <td className="py-stack-md px-base">
                            <span className={`px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider ${statusClass}`}>
                              {order.status === 'Completed' ? 'Hoàn thành' : order.status === 'Shipping' ? 'Đang giao' : order.status === 'Pending' ? 'Chờ xử lý' : order.status === 'PendingVerification' ? 'Chờ xác minh' : order.status === 'Confirmed' ? 'Đã xác nhận' : order.status === 'Preparing' ? 'Đang cắm hoa' : order.status === 'Cancelled' ? 'Đã hủy' : order.status}
                            </span>
                          </td>
                          <td className="py-stack-md px-base text-right whitespace-nowrap">
                            <Link
                              to={`/my-orders/${order.id}`}
                              className="text-primary hover:underline font-label-md text-decoration-none"
                            >
                              Chi tiết
                            </Link>
                            {(order.status === 'Pending' || order.status === 'PendingVerification' || order.status === 'Confirmed') && (
                              <>
                                <span className="text-outline mx-2">|</span>
                                <button
                                  onClick={() => setCancelTarget(order.id)}
                                  className="text-error hover:underline font-label-md bg-transparent border-0 cursor-pointer"
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
