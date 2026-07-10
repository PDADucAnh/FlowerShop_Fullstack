import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AdminLayout from '../../../components/AdminLayout';
import dashboardService, { type DashboardSummary } from '../../../services/dashboardService';

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);

const formatDate = (dateStr: string) => {
  const d = new Date(dateStr);
  return d.toLocaleDateString('vi-VN', { hour: '2-digit', minute: '2-digit' });
};

const Dashboard: React.FC = () => {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    dashboardService.getSummary()
      .then(setData)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <AdminLayout>
        <div className="flex justify-center items-center min-h-[60vh]">
          <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin" role="status" />
        </div>
      </AdminLayout>
    );
  }

  if (!data) {
    return (
      <AdminLayout>
        <div className="text-center py-20 text-on-surface-variant">Không thể tải dữ liệu dashboard</div>
      </AdminLayout>
    );
  }

  return (
    <AdminLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <h1 className="font-headline-sm text-headline-sm text-on-surface uppercase tracking-widest">Dashboard</h1>
          <button
            onClick={() => { setLoading(true); dashboardService.getSummary().then(setData).finally(() => setLoading(false)); }}
            className="flex items-center gap-2 px-4 py-2 bg-primary text-on-primary font-label-sm text-label-sm uppercase tracking-widest border-none cursor-pointer"
          >
            <span className="material-symbols-outlined text-[18px]">refresh</span>
            Làm mới
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <StatCard title="Doanh thu hôm nay" value={formatCurrency(data.revenue.today)} icon="trending_up" />
          <StatCard title="Doanh thu tuần" value={formatCurrency(data.revenue.week)} icon="trending_up" />
          <StatCard title="Doanh thu tháng" value={formatCurrency(data.revenue.month)} icon="trending_up" />
          <StatCard title="Doanh thu năm" value={formatCurrency(data.revenue.year)} icon="trending_up" />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Đơn hàng</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="Mới" value={data.orders.new} />
              <MiniStat label="Chờ XN" value={data.orders.pendingConfirmation} />
              <MiniStat label="Đang CB" value={data.orders.preparing} />
              <MiniStat label="Đang cắm" value={data.orders.arranging} />
              <MiniStat label="Sẵn sàng" value={data.orders.readyForDelivery} />
              <MiniStat label="Đang giao" value={data.orders.delivering} />
              <MiniStat label="Hoàn thành" value={data.orders.completed} color="text-success" />
              <MiniStat label="Đã hủy" value={data.orders.cancelled} color="text-error" />
            </div>
          </div>

          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Thanh toán</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="VNPAY" value={data.payments.vnPay} />
              <MiniStat label="CK" value={data.payments.transfer} />
              <MiniStat label="Tiền mặt" value={data.payments.cash} />
              <MiniStat label="Chờ TT" value={data.payments.pending} />
              <MiniStat label="Thất bại" value={data.payments.failed} color="text-error" />
              <MiniStat label="Hoàn tiền" value={data.payments.refunded} color="text-warning" />
            </div>
          </div>

          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Tồn kho</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="Còn nhiều" value={data.inventory.inStock} />
              <MiniStat label="Sắp hết" value={data.inventory.lowStock} color="text-warning" />
              <MiniStat label="Hết hàng" value={data.inventory.outOfStock} color="text-error" />
            </div>
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mt-6 mb-4">Khách hàng</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="Tổng" value={data.customers.total} />
              <MiniStat label="Mới" value={data.customers.new} />
              <MiniStat label="HĐ" value={data.customers.active} />
              <MiniStat label="Khóa" value={data.customers.locked} color="text-error" />
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Sản phẩm</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="Tổng" value={data.products.total} />
              <MiniStat label="Đang bán" value={data.products.active} />
              <MiniStat label="Hết hàng" value={data.products.outOfStock} color="text-error" />
              <MiniStat label="Ngừng KD" value={data.products.discontinued} color="text-on-surface-variant" />
            </div>
          </div>

          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Khuyến mãi & Đánh giá</h3>
            <div className="grid grid-cols-2 gap-3">
              <MiniStat label="Banner HĐ" value={data.banners.active} />
              <MiniStat label="Banner hết hạn" value={data.banners.expired} />
              <MiniStat label="Đánh giá" value={data.reviews.totalReviews} />
            </div>
          </div>

          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Công việc</h3>
            <div className="space-y-2">
              <ShortcutLink to="/admin/orders" label="Quản lý đơn hàng" />
              <ShortcutLink to="/admin/products" label="Quản lý sản phẩm" />
              <ShortcutLink to="/admin/customers" label="Quản lý khách hàng" />
              <ShortcutLink to="/admin/payments" label="Quản lý thanh toán" />
              <ShortcutLink to="/admin/posts" label="Quản lý bài viết" />
              <ShortcutLink to="/admin/advertisements" label="Quản lý Banner" />
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Top sản phẩm bán chạy</h3>
            {data.topProducts.length === 0 ? (
              <p className="text-on-surface-variant text-sm">Chưa có dữ liệu</p>
            ) : (
              <div className="space-y-3">
                {data.topProducts.slice(0, 5).map((p, i) => (
                  <div key={p.id} className="flex items-center gap-3">
                    <span className="w-6 h-6 rounded-full bg-surface-dim flex items-center justify-center font-label-sm text-label-sm text-on-surface-variant shrink-0">{i + 1}</span>
                    <div className="flex-1 min-w-0">
                      <p className="font-body-md text-body-md text-on-surface truncate">{p.name}</p>
                      <p className="font-body-md text-body-md text-on-surface-variant">Đã bán: {p.totalSold}</p>
                    </div>
                    <span className="font-label-sm text-label-sm text-primary shrink-0">{formatCurrency(p.totalRevenue)}</span>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="bg-surface-container-lowest border border-outline-variant p-5">
            <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Khách hàng thân thiết</h3>
            {data.topCustomers.length === 0 ? (
              <p className="text-on-surface-variant text-sm">Chưa có dữ liệu</p>
            ) : (
              <div className="space-y-3">
                {data.topCustomers.slice(0, 5).map((c, i) => (
                  <div key={c.id} className="flex items-center gap-3">
                    <span className="w-6 h-6 rounded-full bg-surface-dim flex items-center justify-center font-label-sm text-label-sm text-on-surface-variant shrink-0">{i + 1}</span>
                    <div className="flex-1 min-w-0">
                      <p className="font-body-md text-body-md text-on-surface truncate">{c.fullName}</p>
                      <p className="font-body-md text-body-md text-on-surface-variant truncate">{c.email}</p>
                    </div>
                    <span className="font-label-sm text-label-sm text-primary shrink-0">{c.totalOrders} đơn</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <div className="bg-surface-container-lowest border border-outline-variant p-5">
          <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Thông báo</h3>
          {data.notifications.length === 0 ? (
            <p className="text-on-surface-variant text-sm">Không có thông báo</p>
          ) : (
            <div className="space-y-2 max-h-80 overflow-y-auto">
              {data.notifications.map((n) => (
                <div key={n.id} className={`flex items-start gap-3 p-3 rounded-lg ${n.isRead ? '' : 'bg-primary-container/30'}`}>
                  <div className={`w-2 h-2 rounded-full mt-2 shrink-0 ${n.isRead ? 'bg-outline' : 'bg-primary'}`} />
                  <div className="flex-1 min-w-0">
                    <p className="font-label-sm text-label-sm text-on-surface">{n.title}</p>
                    <p className="font-body-md text-body-md text-on-surface-variant truncate">{n.content}</p>
                    <p className="font-body-md text-body-md text-on-surface-variant text-xs mt-1">{formatDate(n.createdAt)}</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </AdminLayout>
  );
};

const StatCard: React.FC<{ title: string; value: string; icon: string }> = ({ title, value, icon }) => (
  <div className="bg-surface-container-lowest border border-outline-variant p-5">
    <div className="flex items-center justify-between mb-3">
      <span className="material-symbols-outlined text-primary text-2xl">{icon}</span>
    </div>
    <p className="font-headline-sm text-headline-sm text-on-surface mb-1">{value}</p>
    <p className="font-body-md text-body-md text-on-surface-variant">{title}</p>
  </div>
);

const MiniStat: React.FC<{ label: string; value: number; color?: string }> = ({ label, value, color }) => (
  <div className="flex items-center justify-between py-1">
    <span className="font-body-md text-body-md text-on-surface-variant">{label}</span>
    <span className={`font-label-sm text-label-sm ${color || 'text-on-surface'}`}>{value}</span>
  </div>
);

const ShortcutLink: React.FC<{ to: string; label: string }> = ({ to, label }) => (
  <Link to={to} className="flex items-center gap-2 px-3 py-2 rounded-lg font-body-md text-body-md text-on-surface-variant hover:bg-surface-dim hover:text-primary no-underline transition-colors">
    <span className="material-symbols-outlined text-[16px]">arrow_forward</span>
    {label}
  </Link>
);

export default Dashboard;
