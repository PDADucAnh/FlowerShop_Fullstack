import React, { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import axiosClient from '../../api/axiosClient';
import orderService from '../../services/orderService';
import { type Order } from '../../types/order';
import { formatCurrency } from '../../utils/currency';
import toast from 'react-hot-toast';

const MomoMock: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const orderIdParam = searchParams.get('orderId');
  const orderId = orderIdParam ? parseInt(orderIdParam, 10) : null;

  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    if (!orderId) {
      toast.error('Mã đơn hàng không hợp lệ.');
      navigate('/cart');
      return;
    }

    const fetchOrder = async () => {
      try {
        const data = await orderService.getOrderById(orderId);
        setOrder(data);
      } catch (error) {
        console.error('Lỗi tải thông tin đơn hàng:', error);
        toast.error('Không thể tải thông tin đơn hàng.');
        navigate('/cart');
      } finally {
        setLoading(false);
      }
    };

    fetchOrder();
  }, [orderId, navigate]);

  const calculateTotal = (orderData: Order) => {
    return orderData.orderDetails?.reduce((sum, item) => sum + item.quantity * item.unitPrice, 0) || 0;
  };

  const handlePayment = async (status: 'success' | 'failed') => {
    if (!orderId || !order) return;
    setProcessing(true);
    const amount = calculateTotal(order);
    const transactionId = `MOMO_MOCK_TX_${Date.now()}_${Math.floor(1000 + Math.random() * 9000)}`;

    try {
      // Gọi webhook backend cập nhật trạng thái thanh toán đơn hàng
      await axiosClient.post('/Payment/webhook', {
        orderId,
        amount,
        status,
        transactionId,
      });

      if (status === 'success') {
        toast.success('Thanh toán giả lập qua MoMo thành công!');
        navigate(`/order-confirmation?orderId=${orderId}`);
      } else {
        toast.error('Thanh toán giả lập thất bại. Đơn hàng đã bị hủy.');
        navigate('/cart');
      }
    } catch (error) {
      console.error('Lỗi xử lý webhook thanh toán:', error);
      toast.error('Có lỗi xảy ra khi cập nhật trạng thái thanh toán.');
    } finally {
      setProcessing(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-background flex flex-col items-center justify-center pt-20">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-[#D82D8B] mb-4"></div>
        <p className="text-secondary font-body-md uppercase tracking-wider">Đang kết nối cổng thanh toán MoMo...</p>
      </div>
    );
  }

  if (!order) return null;

  const totalAmount = calculateTotal(order);

  return (
    <div className="bg-[#f5f5f7] min-h-screen text-[#1d1d1f] font-sans pt-28 pb-12 px-margin flex items-center justify-center">
      <div className="w-full max-w-md bg-white rounded-3xl shadow-xl overflow-hidden border border-gray-100 transition-all duration-300 hover:shadow-2xl">
        {/* Momo Brand Header */}
        <div className="bg-[#D82D8B] p-6 text-white text-center flex flex-col items-center gap-3">
          <div className="w-16 h-16 bg-white rounded-2xl flex items-center justify-center shadow-md animate-bounce">
            <svg viewBox="0 0 100 100" className="w-12 h-12 fill-[#D82D8B]">
              <path d="M75 15H25C19.5 15 15 19.5 15 25v50c0 5.5 4.5 10 10 10h50c5.5 0 10-4.5 10-10V25c0-5.5-4.5-10-10-10zm-35 55c-2.8 0-5-2.2-5-5V45c0-2.8 2.2-5 5-5s5 2.2 5 5v20c0 2.8-2.2 5-5 5zm20 0c-2.8 0-5-2.2-5-5V45c0-2.8 2.2-5 5-5s5 2.2 5 5v20c0 2.8-2.2 5-5 5z" />
            </svg>
          </div>
          <div>
            <h1 className="text-lg font-bold uppercase tracking-wider">Cổng Thanh Toán Giả Lập MoMo</h1>
            <p className="text-xs text-white/80 mt-1">Mã đơn hàng: #{order.id}</p>
          </div>
        </div>

        {/* Order Details Details */}
        <div className="p-6 space-y-6">
          <div className="text-center space-y-2 pb-4 border-b border-gray-100">
            <span className="text-xs uppercase tracking-widest text-gray-400 font-semibold">Số tiền cần thanh toán</span>
            <div className="text-3xl font-bold text-[#D82D8B]">{formatCurrency(totalAmount)}</div>
          </div>

          <div className="space-y-3 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-400">Khách hàng:</span>
              <span className="font-semibold">{order.customerName || 'N/A'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-400">Email:</span>
              <span className="font-semibold text-gray-600">{order.customerEmail || 'N/A'}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-400">Phương thức:</span>
              <span className="font-semibold text-[#D82D8B] flex items-center gap-1">
                <span className="w-2 h-2 rounded-full bg-[#D82D8B]"></span>
                Ví MoMo
              </span>
            </div>
          </div>

          <div className="bg-[#fff0f6] rounded-2xl p-4 border border-[#ffd8e6] text-xs text-[#ab2c5d] leading-relaxed">
            <strong>Lưu ý giả lập:</strong> Bạn đang ở môi trường thử nghiệm.
            Vui lòng chọn nút tương ứng dưới đây để giả lập kết quả trả về từ webhook MoMo.
          </div>

          {/* Action Buttons */}
          <div className="flex flex-col gap-3 pt-2">
            <button
              onClick={() => handlePayment('success')}
              disabled={processing}
              id="momo-btn-success"
              className="w-full bg-[#D82D8B] hover:bg-[#b01e6c] text-white py-3.5 px-6 rounded-2xl font-semibold text-sm transition-all duration-300 transform active:scale-[0.98] disabled:opacity-50 flex items-center justify-center gap-2 shadow-lg shadow-[#D82D8B]/20"
            >
              {processing ? (
                <div className="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-white"></div>
              ) : (
                <>
                  <span className="material-symbols-outlined text-base">check_circle</span>
                  Thanh toán thành công
                </>
              )}
            </button>
            <button
              onClick={() => handlePayment('failed')}
              disabled={processing}
              id="momo-btn-failed"
              className="w-full bg-white hover:bg-gray-50 border border-gray-200 text-gray-500 hover:text-gray-700 py-3.5 px-6 rounded-2xl font-semibold text-sm transition-all duration-300 transform active:scale-[0.98] disabled:opacity-50 flex items-center justify-center gap-2"
            >
              {processing ? (
                <div className="animate-spin rounded-full h-5 w-5 border-t-2 border-b-2 border-gray-400"></div>
              ) : (
                <>
                  <span className="material-symbols-outlined text-base">cancel</span>
                  Thanh toán thất bại
                </>
              )}
            </button>
          </div>
        </div>

        {/* Footer info */}
        <div className="bg-gray-50 p-4 text-center text-xs text-gray-400 border-t border-gray-100 flex items-center justify-center gap-1.5">
          <span className="material-symbols-outlined text-xs">lock</span>
          Giao dịch được bảo mật bởi MoMo Security
        </div>
      </div>
    </div>
  );
};

export default MomoMock;
