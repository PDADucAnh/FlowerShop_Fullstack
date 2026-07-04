import { z } from 'zod';

export const checkoutSchema = z.object({
  // Người mua (Buyer)
  fullname: z.string().min(1, 'Họ tên người mua là bắt buộc'),
  email: z.string().email('Email không hợp lệ'),
  phone: z.string().min(10, 'Số điện thoại người mua phải từ 10 số').max(15, 'Số điện thoại quá dài'),

  // Người nhận (Recipient)
  recipientName: z.string().min(1, 'Họ tên người nhận là bắt buộc'),
  recipientPhone: z.string().min(10, 'Số điện thoại người nhận phải từ 10 số').max(15, 'Số điện thoại quá dài'),
  greetingCard: z.string().optional(),

  // Thông tin giao hàng
  deliveryDate: z.string().min(1, 'Vui lòng chọn ngày giao hàng'),
  deliveryTimeSlot: z.string().min(1, 'Vui lòng chọn khung giờ giao hàng'),
  deliveryAddress: z.string().min(5, 'Địa chỉ giao hàng phải từ 5 ký tự'),

  // Ghi chú thêm
  notes: z.string().optional(),

  // Phương thức thanh toán
  paymentMethod: z.string(),
});

export type CheckoutFormData = z.infer<typeof checkoutSchema>;
