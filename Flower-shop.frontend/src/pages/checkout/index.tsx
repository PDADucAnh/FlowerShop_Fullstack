import React, { useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useCart } from '../../context/CartContext';
import { useAuth } from '../../context/AuthContext';
import { useCreateOrder } from '../../hooks/useOrders';
import { checkoutSchema, type CheckoutFormData } from '../../schemas/checkoutSchema';

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const CheckoutPage: React.FC = () => {
  const navigate = useNavigate();
  const { cartItems, cartTotal, clearCart } = useCart();
  const { user, refreshProfile } = useAuth();
  const createOrder = useCreateOrder();

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<CheckoutFormData>({
    resolver: zodResolver(checkoutSchema),
  });

  useEffect(() => {
    refreshProfile();
  }, [refreshProfile]);

  useEffect(() => {
    if (user) {
      if (user.fullName) setValue('fullname', user.fullName);
      if (user.email) setValue('email', user.email);
      if (user.phone) setValue('phone', user.phone);
      if (user.address) setValue('address', user.address);
    }
  }, [user, setValue]);

  const onSubmit = async (formData: CheckoutFormData) => {
    if (cartItems.length === 0) return;

    const orderPayload = {
      customerId: user?.id || 0,
      notes: `Delivery to: ${formData.fullname}, Email: ${formData.email}, Contact: ${formData.phone}, Location: ${formData.address}. Narrative: ${formData.notes}`,
      items: cartItems.map(item => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.discountPrice || item.price,
      })),
    };

    try {
      await createOrder.mutateAsync(orderPayload);
      clearCart();
      navigate('/');
    } catch {
      // toast handled by useCreateOrder onError
    }
  };

  if (cartItems.length === 0) {
    return (
      <div className="text-center py-xl px-margin-mobile max-w-container-max mx-auto">
        <div className="size-20 bg-surface-container flex items-center justify-center text-outline mb-4 mx-auto rounded-xl">
          <span className="material-symbols-outlined text-4xl">shopping_bag</span>
        </div>
        <div className="space-y-md">
          <h2 className="font-headline-md text-headline-md text-on-surface uppercase tracking-tighter">Manifest Empty</h2>
          <p className="text-secondary max-w-md mx-auto">You cannot proceed to checkout without items in your collection.</p>
        </div>
        <Link to="/shop" className="bg-primary text-on-primary px-xl py-4 font-label-md text-label-md uppercase tracking-[0.3em] font-bold text-decoration-none inline-block mt-4 rounded-lg btn-luxury btn-primary-luxury">
          Return to Boutique
        </Link>
      </div>
    );
  }

  return (
    <div className="bg-surface text-on-surface font-body-md antialiased">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <header className="mb-stack-lg text-center space-y-md">
            <h3 className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary">Secure Portal</h3>
            <h2 className="font-headline-md text-headline-md text-on-surface uppercase tracking-tighter">Finalize Acquisition</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col lg:flex-row gap-xl">
            <div className="flex-1 space-y-lg">
                <div className="bg-surface-container-lowest border border-outline-variant p-xl space-y-xl rounded-xl">
                    <h5 className="font-headline-sm text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Delivery Credentials</h5>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-lg">
                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Full Name</label>
                            <input type="text" {...register('fullname')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest placeholder:text-outline-variant"
                                placeholder="Recipient Name" />
                            {errors.fullname && <p className="text-error text-[10px] mt-1">{errors.fullname.message}</p>}
                        </div>
                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Phone</label>
                            <input type="text" {...register('phone')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest placeholder:text-outline-variant"
                                placeholder="Contact Number" />
                            {errors.phone && <p className="text-error text-[10px] mt-1">{errors.phone.message}</p>}
                        </div>
                    </div>

                    <div className="space-y-sm">
                        <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Email</label>
                        <input type="email" {...register('email')}
                            className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                            placeholder="Email Address" />
                        {errors.email && <p className="text-error text-[10px] mt-1">{errors.email.message}</p>}
                    </div>

                    <div className="space-y-sm">
                        <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Address</label>
                        <input type="text" {...register('address')}
                            className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest placeholder:text-outline-variant"
                            placeholder="Delivery Address" />
                        {errors.address && <p className="text-error text-[10px] mt-1">{errors.address.message}</p>}
                    </div>

                    <div className="space-y-sm">
                        <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Notes</label>
                        <textarea {...register('notes')}
                            className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-body-md italic leading-relaxed placeholder:text-outline-variant resize-none rounded-lg"
                            rows={4} placeholder="Special delivery instructions..."></textarea>
                    </div>
                </div>

                <div className="bg-surface-container-lowest border border-outline-variant p-xl space-y-lg rounded-xl">
                    <h5 className="font-headline-sm text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Transaction Method</h5>
                    <label className="flex items-start gap-md p-md border border-primary bg-surface-container-low cursor-pointer rounded-lg">
                        <div className="flex items-center h-6">
                            <input type="radio" name="payment" defaultChecked className="size-4 text-primary focus:ring-primary border-primary bg-transparent" />
                        </div>
                        <div className="space-y-1">
                            <span className="font-label-sm text-label-sm uppercase tracking-widest font-bold block">Cash on Delivery (COD)</span>
                            <span className="font-label-sm text-label-sm text-secondary uppercase tracking-widest block">Settle transaction upon receipt of goods.</span>
                        </div>
                    </label>
                </div>
            </div>

            <aside className="w-full lg:w-96 flex-shrink-0">
                <div className="bg-surface-container-low border border-outline-variant p-lg space-y-xl sticky top-32 rounded-xl">
                    <h5 className="font-headline-sm text-headline-sm uppercase tracking-widest border-b border-outline-variant pb-md">Manifest Overview</h5>

                    <div className="space-y-md border-b border-outline-variant pb-md max-h-60 overflow-y-auto no-scrollbar">
                        {cartItems.map(item => (
                            <div className="flex justify-between items-start gap-md" key={item.id}>
                                <div className="flex-1 space-y-1">
                                    <span className="font-label-sm text-label-sm uppercase tracking-widest font-bold block">{item.name}</span>
                                    <span className="font-label-sm text-label-sm text-secondary uppercase tracking-widest block">Qty: {item.quantity}</span>
                                </div>
                                <span className="font-bold text-sm">{formatCurrency((item.discountPrice || item.price) * item.quantity)}</span>
                            </div>
                        ))}
                    </div>

                    <div className="space-y-md">
                        <div className="flex justify-between items-center font-label-sm text-label-sm uppercase tracking-widest">
                            <span className="text-secondary">Subtotal</span>
                            <span className="font-bold">{formatCurrency(cartTotal)}</span>
                        </div>
                        <div className="flex justify-between items-center font-label-sm text-label-sm uppercase tracking-widest">
                            <span className="text-secondary">Delivery Insight</span>
                            <span className="text-primary font-bold uppercase tracking-widest text-[10px]">Complimentary</span>
                        </div>
                    </div>

                    <div className="border-t border-outline-variant pt-lg flex justify-between items-center">
                        <span className="font-label-sm text-label-sm uppercase tracking-[0.2em] font-bold">Total Acquisition</span>
                        <span className="text-2xl font-bold text-on-surface">{formatCurrency(cartTotal)}</span>
                    </div>

                    <button type="submit" disabled={createOrder.isPending || cartItems.length === 0}
                        className="w-full bg-primary text-on-primary py-5 font-label-md text-label-md uppercase tracking-[0.3em] font-bold border border-primary outline-none disabled:opacity-50 disabled:cursor-not-allowed rounded-lg btn-luxury btn-primary-luxury">
                        {createOrder.isPending ? 'Processing...' : 'Confirm Transaction'}
                    </button>

                    <div className="bg-surface-container-lowest border border-outline-variant p-md flex items-start gap-md rounded-lg">
                        <span className="material-symbols-outlined text-secondary">lock</span>
                        <p className="text-[10px] text-secondary uppercase tracking-widest leading-relaxed">Secure end-to-end encryption. Your credentials are protected.</p>
                    </div>
                </div>
            </aside>
        </form>
      </main>
    </div>
  );
};

export default CheckoutPage;
