export const getOrderStatusText = (status: string | number | undefined): string => {
  if (status === undefined) return 'Không xác định';
  const s = status.toString();
  switch (s) {
    case '0':
    case 'Pending':
      return 'Chờ xử lý';
    case '1':
    case 'Shipping':
      return 'Đang giao';
    case '2':
    case 'Completed':
      return 'Hoàn thành';
    case '3':
    case 'Cancelled':
      return 'Đã hủy (Cũ)';
    case '4':
    case 'PendingVerification':
      return 'Chờ xác nhận';
    case '5':
    case 'Confirmed':
      return 'Đã xác nhận';
    case '6':
    case 'Preparing':
      return 'Đang cắm hoa';
    case '7':
    case 'PendingPayment':
      return 'Chờ thanh toán';
    case '8':
    case 'Paid':
      return 'Đã thanh toán';
    case '9':
    case 'ReadyForDelivery':
      return 'Sẵn sàng giao';
    case '10':
    case 'Refunded':
      return 'Đã hoàn tiền';
    case '11':
    case 'CancelledByCustomer':
      return 'Khách hủy';
    case '12':
    case 'CancelledByShop':
      return 'Shop hủy';
    case '13':
    case 'RefundPending':
      return 'Chờ hoàn tiền';
    default:
      return s;
  }
};

export const getPaymentStatusText = (status: string | number | undefined): string => {
  if (status === undefined) return 'Không xác định';
  const s = status.toString();
  switch (s) {
    case '0':
    case 'Pending':
      return 'Chờ thanh toán';
    case '1':
    case 'Completed':
      return 'Đã thanh toán';
    case '2':
    case 'Failed':
      return 'Thanh toán thất bại';
    case '3':
    case 'Refunded':
      return 'Đã hoàn tiền';
    case '4':
    case 'Cancelled':
      return 'Đã hủy';
    case '5':
    case 'PendingRefund':
      return 'Chờ hoàn tiền';
    case '6':
    case 'PartialRefund':
      return 'Đã hoàn tiền một phần';
    default:
      return s;
  }
};

export const getPaymentMethodText = (method: string | number | undefined): string => {
  if (method === undefined) return 'Không xác định';
  const m = method.toString();
  switch (m) {
    case '0':
    case 'OnlinePayment':
    case 'VNPay':
      return 'Thanh toán VNPay';
    case '1':
    case 'COD':
    case 'CashOnDelivery':
      return 'Thanh toán khi nhận hàng';
    default:
      return m;
  }
};
