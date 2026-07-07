import React, { useEffect, useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { useCreateOrder } from '../../hooks/useOrders';
import { formatCurrency } from '../../utils/currency';
import { getImageUrl } from '../../utils/apiUtils';
import { checkoutSchema, type CheckoutFormData } from '../../schemas/checkoutSchema';
import axiosClient from '../../api/axiosClient';
import SEO from '../../components/SEO';
import toast from 'react-hot-toast';

const getVietnamTodayString = () => {
  const options = { timeZone: 'Asia/Ho_Chi_Minh', year: 'numeric' as const, month: '2-digit' as const, day: '2-digit' as const };
  const formatter = new Intl.DateTimeFormat('en-CA', options);
  return formatter.format(new Date());
};

const DEFAULT_SLOTS = [
  { value: '08:00-10:00', label: '08:00 - 10:00 (Sáng)', startHour: 8 },
  { value: '10:00-12:00', label: '10:00 - 12:00 (Sáng)', startHour: 10 },
  { value: '13:00-15:00', label: '13:00 - 15:00 (Chiều)', startHour: 13 },
  { value: '15:00-17:00', label: '15:00 - 17:00 (Chiều)', startHour: 15 },
  { value: '17:00-19:00', label: '17:00 - 19:00 (Tối)', startHour: 17 },
  { value: '19:00-21:00', label: '19:00 - 21:00 (Tối)', startHour: 19 },
];

const CheckoutPage: React.FC = () => {
  const navigate = useNavigate();
  const { cartItems, cartTotal, clearCart } = useCart();
  const { user, refreshProfile } = useAuth();
  const createOrder = useCreateOrder();

  const [isBlacklisted, setIsBlacklisted] = useState(false);
  const [checkingBlacklist, setCheckingBlacklist] = useState(false);
  const [recipientIsBuyer, setRecipientIsBuyer] = useState(false);

  const { register, handleSubmit, setValue, watch, formState: { errors } } = useForm<CheckoutFormData>({
    resolver: zodResolver(checkoutSchema),
    defaultValues: {
      paymentMethod: 'COD',
    }
  });

  const watchPaymentMethod = watch('paymentMethod');
  const selectedDate = watch('deliveryDate');
  const watchFullname = watch('fullname');
  const watchPhone = watch('phone');

  // Auto-sync recipient only when "recipientIsBuyer" is toggled on
  useEffect(() => {
    if (recipientIsBuyer) {
      setValue('recipientName', watchFullname || '', { shouldValidate: true });
      setValue('recipientPhone', watchPhone || '', { shouldValidate: true });
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [recipientIsBuyer]);

  const getFilteredSlots = () => {
    const todayStr = getVietnamTodayString();
    if (selectedDate === todayStr) {
      const now = new Date();
      const formatter = new Intl.DateTimeFormat('en-US', {
        timeZone: 'Asia/Ho_Chi_Minh',
        hour: 'numeric',
        minute: 'numeric',
        hour12: false
      });
      const parts = formatter.formatToParts(now);
      const hourPart = parts.find(p => p.type === 'hour')?.value;
      const minutePart = parts.find(p => p.type === 'minute')?.value;
      
      const currentVnHour = hourPart ? parseInt(hourPart, 10) : now.getHours();
      const currentVnMinute = minutePart ? parseInt(minutePart, 10) : now.getMinutes();
      
      const currentVnTime = currentVnHour + (currentVnMinute / 60.0);
      const limitTime = currentVnTime + 2.0; // Lead-Time Rule = 2 hours

      return DEFAULT_SLOTS.filter(slot => slot.startHour >= limitTime);
    }
    return DEFAULT_SLOTS;
  };

  useEffect(() => {
    refreshProfile();
  }, [refreshProfile]);

  const phoneBlurRef = React.useRef<NodeJS.Timeout>();
  const handlePhoneBlur = async (phoneVal: string) => {
    if (!phoneVal || phoneVal.length < 10) return;
    if (phoneBlurRef.current) clearTimeout(phoneBlurRef.current);
    phoneBlurRef.current = setTimeout(async () => {
      setCheckingBlacklist(true);
      try {
        const res: any = await axiosClient.get('/Orders/check-blacklist', {
          params: { phone: phoneVal }
        });
        if (res && res.isBlacklisted) {
          setIsBlacklisted(true);
          setValue('paymentMethod', 'OnlinePayment');
          toast.error('Số điện thoại này có lịch sử bùng hàng. Bạn bắt buộc phải thanh toán Online.');
        } else {
          setIsBlacklisted(false);
        }
      } catch (err) {
        console.error('Lỗi kiểm tra blacklist:', err);
      } finally {
        setCheckingBlacklist(false);
      }
    }, 500);
  };

  const hasAutoFilled = React.useRef(false);
  useEffect(() => {
    if (user && !hasAutoFilled.current) {
      hasAutoFilled.current = true;
      if (user.fullName) setValue('fullname', user.fullName);
      if (user.email) setValue('email', user.email);
      if (user.phone) {
        setValue('phone', user.phone);
        handlePhoneBlur(user.phone);
      }
      if (user.address) setValue('deliveryAddress', user.address);
    }
  }, [user, setValue]);

  const onSubmit = async (formData: CheckoutFormData) => {
    if (cartItems.length === 0) return;

    const orderPayload = {
      customerId: user?.id || 0,
      notes: [formData.notes, formData.greetingCard ? `Lời chúc: ${formData.greetingCard}` : ''].filter(Boolean).join(' | '),
      items: cartItems.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.discountPrice || item.price,
      })),
      paymentMethod: formData.paymentMethod === 'OnlinePayment' ? 0 : 1,
      deliveryDate: formData.deliveryDate,
      deliveryTimeSlot: formData.deliveryTimeSlot,
      deliveryAddress: formData.deliveryAddress,
      recipientName: formData.recipientName,
      recipientPhone: formData.recipientPhone,
    };

    try {
      const result = await createOrder.mutateAsync(orderPayload);
      clearCart();
      if (formData.paymentMethod === 'OnlinePayment') {
        const res: any = await axiosClient.post('/Payment/create-vnpay-url', {
          orderId: result.orderId,
          orderDescription: `Thanh toán đơn hàng #${result.orderId}`,
          name: formData.fullname,
        });
        if (res?.url) {
          window.location.href = res.url;
        }
      } else {
        navigate(`/order-confirmation?orderId=${result.orderId}`);
      }
    } catch {
      // toast handled by useCreateOrder onError
    }
  };

  const phoneRegister = register('phone');

  if (cartItems.length === 0) {
    return (
      <div className="text-center py-20 px-margin-mobile md:px-margin-desktop min-h-screen pt-40 bg-background text-on-background">
        <div className="size-20 bg-surface-container flex items-center justify-center text-outline mb-4 mx-auto rounded-full">
          <span className="material-symbols-outlined text-4xl">shopping_bag</span>
        </div>
        <div className="space-y-md mb-6">
          <h2 className="font-display-lg text-display-lg text-on-background">Giỏ hàng trống</h2>
          <p className="text-on-surface-variant max-w-md mx-auto font-body-md">Bạn cần có sản phẩm trong giỏ để thanh toán.</p>
        </div>
        <Link to="/shop" className="bg-primary text-on-primary px-6 py-4 rounded-lg font-label-md text-label-md inline-block no-underline hover:opacity-90 transition-opacity">
          Quay lại cửa hàng
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-lg min-h-screen flex flex-col antialiased pt-20">
      <SEO title="Thanh toán" description="Thanh toán đơn hàng" />
      <main className="flex-grow pt-stack-lg pb-stack-lg px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full">
        {/* Header */}
        <div className="mb-stack-lg text-center md:text-left border-b border-outline-variant pb-stack-md flex justify-between items-end">
          <div>
            <Link className="font-headline-md text-headline-md text-primary mb-2 inline-block no-underline" to="/">
              FlowerShop
            </Link>
            <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mt-4">
              Thanh toán
            </h1>
            <p className="font-body-md text-body-md text-on-surface-variant mt-2">Hoàn tất đơn hàng</p>
          </div>
          <Link
            className="hidden md:flex items-center text-primary font-label-md text-label-md hover:opacity-80 transition-opacity no-underline"
            to="/cart"
          >
            <span className="material-symbols-outlined mr-1" style={{ fontVariationSettings: "'FILL' 0" }}>
              arrow_back
            </span>
            Trở lại giỏ hàng
          </Link>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col lg:flex-row gap-gutter">
          {/* Form Sections (Left) */}
          <div className="w-full lg:w-2/3 space-y-stack-md">
            {/* Section 1: Thông tin người mua */}
            <section className="bg-surface-container-lowest rounded-xl p-6 md:p-8 shadow-[0_4px_20px_rgba(255,177,197,0.02)] border border-outline-variant/30 relative overflow-hidden group">
              <div className="absolute top-0 left-0 w-1 h-full bg-primary/20 group-hover:bg-primary transition-colors duration-300"></div>
              <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6 flex items-center">
                <span className="material-symbols-outlined mr-2 text-primary">person</span>
                Thông tin người mua
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="col-span-1 md:col-span-2">
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="buyer-name">
                    Họ và tên người mua
                  </label>
                  <input
                    type="text"
                    id="buyer-name"
                    {...register('fullname')}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim"
                    placeholder="Nhập họ và tên"
                  />
                  {errors.fullname && <p className="text-error text-xs mt-1">{errors.fullname.message}</p>}
                </div>
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="buyer-phone">
                    Số điện thoại người mua
                  </label>
                  <input
                    type="tel"
                    id="buyer-phone"
                    {...phoneRegister}
                    onBlur={async (e) => {
                      phoneRegister.onBlur(e);
                      await handlePhoneBlur(e.target.value);
                    }}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim"
                    placeholder="Nhập số điện thoại"
                  />
                  {errors.phone && <p className="text-error text-xs mt-1">{errors.phone.message}</p>}
                  {checkingBlacklist && <p className="text-secondary text-[10px] uppercase tracking-widest mt-1">Đang kiểm tra lịch sử đơn hàng...</p>}
                  {isBlacklisted && (
                    <p className="text-error text-xs font-bold uppercase tracking-wider mt-1">
                      Số điện thoại này có lịch sử bùng hàng. Bạn bắt buộc phải thanh toán Online.
                    </p>
                  )}
                </div>
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="buyer-email">
                    Email người mua
                  </label>
                  <input
                    type="email"
                    id="buyer-email"
                    {...register('email')}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim"
                    placeholder="Nhập email"
                  />
                  {errors.email && <p className="text-error text-xs mt-1">{errors.email.message}</p>}
                </div>
              </div>
            </section>

            {/* Section 2: Thông tin người nhận */}
            <section className="bg-surface-container-lowest rounded-xl p-6 md:p-8 shadow-[0_4px_20px_rgba(255,177,197,0.02)] border border-outline-variant/30 relative overflow-hidden group">
              <div className="absolute top-0 left-0 w-1 h-full bg-primary/20 group-hover:bg-primary transition-colors duration-300"></div>
              <div className="flex justify-between items-center mb-6">
                <h2 className="font-headline-sm text-headline-sm text-on-surface flex items-center">
                  <span className="material-symbols-outlined mr-2 text-primary">local_florist</span>
                  Thông tin người nhận
                </h2>
                <label className="flex items-center space-x-2 cursor-pointer group/check">
                  <input
                    type="checkbox"
                    checked={recipientIsBuyer}
                    onChange={(e) => setRecipientIsBuyer(e.target.checked)}
                    className="form-checkbox text-primary rounded border-outline-variant focus:ring-primary focus:ring-offset-0 transition-colors"
                  />
                  <span className="font-label-sm text-label-sm text-on-surface-variant group-hover/check:text-primary transition-colors">
                    Người nhận là người mua
                  </span>
                </label>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="receiver-name">
                    Họ và tên người nhận
                  </label>
                  <input
                    type="text"
                    id="receiver-name"
                    disabled={recipientIsBuyer}
                    {...register('recipientName')}
                    className={`w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim ${recipientIsBuyer ? 'opacity-60 cursor-not-allowed' : ''}`}
                    placeholder="Nhập họ và tên người nhận"
                  />
                  {errors.recipientName && <p className="text-error text-xs mt-1">{errors.recipientName.message}</p>}
                </div>
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="receiver-phone">
                    Số điện thoại người nhận
                  </label>
                  <input
                    type="tel"
                    id="receiver-phone"
                    disabled={recipientIsBuyer}
                    {...register('recipientPhone')}
                    className={`w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim ${recipientIsBuyer ? 'opacity-60 cursor-not-allowed' : ''}`}
                    placeholder="Nhập số điện thoại người nhận"
                  />
                  {errors.recipientPhone && <p className="text-error text-xs mt-1">{errors.recipientPhone.message}</p>}
                </div>
                <div className="col-span-1 md:col-span-2">
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2 flex items-center" htmlFor="card-message">
                    Lời chúc trên thiệp
                    <span className="material-symbols-outlined ml-1 text-[16px] text-outline" title="Tặng kèm thiệp thiết kế riêng">
                      info
                    </span>
                  </label>
                  <textarea
                    id="card-message"
                    {...register('greetingCard')}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim resize-none"
                    placeholder="Nhập lời chúc muốn gửi gắm..."
                    rows={3}
                  ></textarea>
                  {errors.greetingCard && <p className="text-error text-xs mt-1">{errors.greetingCard.message}</p>}
                </div>
              </div>
            </section>

            {/* Section 3: Thời gian & Địa điểm */}
            <section className="bg-surface-container-lowest rounded-xl p-6 md:p-8 shadow-[0_4px_20px_rgba(255,177,197,0.02)] border border-outline-variant/30 relative overflow-hidden group">
              <div className="absolute top-0 left-0 w-1 h-full bg-primary/20 group-hover:bg-primary transition-colors duration-300"></div>
              <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6 flex items-center">
                <span className="material-symbols-outlined mr-2 text-primary">location_on</span>
                Thời gian &amp; Địa điểm nhận hàng
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="col-span-1 md:col-span-2">
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="address">
                    Địa chỉ chi tiết
                  </label>
                  <input
                    type="text"
                    id="address"
                    {...register('deliveryAddress')}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim"
                    placeholder="Số nhà, tên đường, phường/xã..."
                  />
                  {errors.deliveryAddress && <p className="text-error text-xs mt-1">{errors.deliveryAddress.message}</p>}
                </div>
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="delivery-date">
                    Ngày nhận hoa
                  </label>
                  <div className="relative">
                    <input
                      type="date"
                      id="delivery-date"
                      {...register('deliveryDate')}
                      min={getVietnamTodayString()}
                      className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md"
                    />
                  </div>
                  {errors.deliveryDate && <p className="text-error text-xs mt-1">{errors.deliveryDate.message}</p>}
                </div>
                <div>
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="delivery-time">
                    Khung giờ giao hoa
                  </label>
                  <div className="relative">
                    <select
                      id="delivery-time"
                      {...register('deliveryTimeSlot')}
                      className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md appearance-none cursor-pointer"
                    >
                      <option disabled value="">Chọn khung giờ</option>
                      {getFilteredSlots().map(slot => (
                        <option key={slot.value} value={slot.value}>{slot.label}</option>
                      ))}
                    </select>
                    <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-4 text-on-surface-variant">
                      <span className="material-symbols-outlined">expand_more</span>
                    </div>
                  </div>
                  {errors.deliveryTimeSlot && <p className="text-error text-xs mt-1">{errors.deliveryTimeSlot.message}</p>}
                </div>
                <div className="col-span-1 md:col-span-2">
                  <label className="block font-label-md text-label-md text-on-surface-variant mb-2" htmlFor="extra-notes">
                    Ghi chú thêm
                  </label>
                  <textarea
                    id="extra-notes"
                    {...register('notes')}
                    className="w-full bg-[#FCE4EC] border-outline-variant rounded-lg px-4 py-3 text-on-surface focus:outline-none form-input-pink transition-all font-body-md text-body-md placeholder-secondary-fixed-dim resize-none"
                    placeholder="Lưu ý về giao hàng, bảo vệ, gọi điện trước..."
                    rows={2}
                  ></textarea>
                </div>
              </div>
            </section>

            {/* Section 4: Phương thức thanh toán */}
            <section className="bg-surface-container-lowest rounded-xl p-6 md:p-8 shadow-[0_4px_20px_rgba(255,177,197,0.02)] border border-outline-variant/30 relative overflow-hidden group">
              <div className="absolute top-0 left-0 w-1 h-full bg-primary/20 group-hover:bg-primary transition-colors duration-300"></div>
              <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6 flex items-center">
                <span className="material-symbols-outlined mr-2 text-primary">payments</span>
                Phương thức thanh toán
              </h2>
              <div className="space-y-4">
                {/* Option 1: COD */}
                <label className={`flex items-start p-4 border rounded-lg cursor-pointer transition-colors group/radio ${watchPaymentMethod === 'COD' ? 'border-primary bg-surface-container-low' : 'border-outline-variant hover:bg-surface-container-low'} ${isBlacklisted ? 'opacity-50 cursor-not-allowed' : ''}`}>
                  <div className="flex items-center h-6 mr-4">
                    <input
                      type="radio"
                      value="COD"
                      disabled={isBlacklisted}
                      {...register('paymentMethod')}
                      className="w-5 h-5 text-primary form-radio-pink bg-[#FCE4EC] border-outline-variant focus:ring-primary focus:ring-offset-0 transition-all cursor-pointer"
                    />
                  </div>
                  <div className="flex-grow">
                    <div className="flex items-center justify-between">
                      <span className="font-label-md text-label-md text-on-surface group-hover/radio:text-primary transition-colors">
                        Thanh toán khi nhận hàng (COD)
                      </span>
                      <span className="material-symbols-outlined text-outline">local_shipping</span>
                    </div>
                    <p className="font-body-md text-sm text-on-surface-variant mt-1">Thanh toán bằng tiền mặt khi nhận được hoa</p>
                  </div>
                </label>
                {/* Option 2: VNPAY */}
                <label className={`flex items-start p-4 border rounded-lg cursor-pointer transition-colors group/radio ${watchPaymentMethod === 'OnlinePayment' ? 'border-primary bg-surface-container-low' : 'border-outline-variant hover:bg-surface-container-low'}`}>
                  <div className="flex items-center h-6 mr-4">
                    <input
                      type="radio"
                      value="OnlinePayment"
                      {...register('paymentMethod')}
                      className="w-5 h-5 text-primary form-radio-pink bg-[#FCE4EC] border-outline-variant focus:ring-primary focus:ring-offset-0 transition-all cursor-pointer"
                    />
                  </div>
                  <div className="flex-grow">
                    <div className="flex items-center justify-between">
                      <span className="font-label-md text-label-md text-on-surface group-hover/radio:text-primary transition-colors">
                        Chuyển khoản trực tuyến (VNPAY)
                      </span>
                      <span className="material-symbols-outlined text-outline">account_balance_wallet</span>
                    </div>
                    <p className="font-body-md text-sm text-on-surface-variant mt-1">Thanh toán an toàn qua cổng VNPAY</p>
                  </div>
                </label>
              </div>
            </section>
          </div>

          {/* Sidebar (Right) - Order Summary */}
          <div className="w-full lg:w-1/3 mt-8 lg:mt-0">
            <div className="bg-surface-container-lowest rounded-xl p-6 shadow-[0_4px_20px_rgba(255,177,197,0.05)] border border-outline-variant/30 sticky top-8">
              <h2 className="font-headline-sm text-headline-sm text-on-surface mb-6 border-b border-[#FCE4EC] pb-4">
                Chi tiết đơn hàng
              </h2>
              {/* Product Item List */}
              <div className="space-y-6 max-h-96 overflow-y-auto no-scrollbar">
                {cartItems.map(item => (
                  <div className="flex items-start pb-6 border-b border-[#FCE4EC]" key={item.id}>
                    <div className="w-20 h-24 bg-surface-container-low rounded-lg overflow-hidden flex-shrink-0 mr-4 petal-shadow">
                      <img
                        className="w-full h-full object-cover"
                        src={getImageUrl(item.imageUrl)}
                        alt={item.name}
                        loading="lazy"
                      />
                    </div>
                    <div className="flex-grow">
                      <h3 className="font-label-md text-label-md text-on-surface mb-1">{item.name}</h3>
                      <p className="font-label-sm text-label-sm text-on-surface-variant mb-2">SL: {item.quantity}</p>
                      <p className="font-headline-sm text-lg text-primary">
                        {formatCurrency((item.discountPrice || item.price) * item.quantity)}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
              {/* Subtotals */}
              <div className="space-y-3 font-body-md text-body-md text-on-surface-variant mb-6 pb-6 border-b border-[#FCE4EC] mt-6">
                <div className="flex justify-between">
                  <span>Tạm tính</span>
                  <span>{formatCurrency(cartTotal)}</span>
                </div>
                <div className="flex justify-between">
                  <span>Phí vận chuyển</span>
                  <span className="text-primary font-medium">Miễn phí</span>
                </div>
              </div>
              {/* Total */}
              <div className="flex justify-between items-end mb-8">
                <span className="font-label-md text-label-md text-on-surface">Tổng cộng</span>
                <span className="font-headline-md text-headline-sm text-primary">
                  {formatCurrency(cartTotal)}
                </span>
              </div>
              {/* Checkout Button */}
              <button
                type="submit"
                disabled={createOrder.isPending || cartItems.length === 0}
                className="w-full bg-primary hover:bg-on-primary-fixed-variant text-on-primary font-label-md text-label-md py-4 rounded-lg shadow-md hover:shadow-lg transition-all duration-300 mb-4 flex items-center justify-center border-0 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {createOrder.isPending ? 'Đang xử lý...' : 'Đặt hàng'}
                <span className="material-symbols-outlined ml-2" style={{ fontVariationSettings: "'FILL' 1" }}>
                  arrow_forward
                </span>
              </button>
              {/* Security Note */}
              <div className="flex items-center justify-center text-on-surface-variant font-label-sm text-label-sm">
                <span className="material-symbols-outlined mr-2 text-[16px]">lock</span>
                Mã hóa đầu cuối an toàn. Thông tin của bạn được bảo vệ.
              </div>
            </div>
          </div>
        </form>
      </main>
    </div>
  );
};

export default CheckoutPage;
