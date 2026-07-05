import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import authService from '../../services/authService';
import toast from 'react-hot-toast';
import SEO from '../../components/SEO';
import { registerSchema, type RegisterFormData } from '../../schemas/registerSchema';

const RegisterPage: React.FC = () => {
    const [error, setError] = React.useState<string>('');
    const [loading, setLoading] = React.useState<boolean>(false);
    const navigate = useNavigate();

    const { register, handleSubmit, formState: { errors } } = useForm<RegisterFormData>({
        resolver: zodResolver(registerSchema),
    });

    const onSubmit = async (data: RegisterFormData) => {
        setError('');
        setLoading(true);
        try {
            const success = await authService.register({
                fullName: data.fullName,
                email: data.email,
                phone: data.phone,
                address: data.address,
                password: data.password,
            });
            if (success) {
                toast.success('Tạo tài khoản thành công. Vui lòng đăng nhập.');
                navigate('/login');
            } else {
                setError('Đăng ký thất bại. Tên đăng nhập có thể đã tồn tại.');
            }
        } catch {
            setError('Đã xảy ra lỗi trong quá trình đăng ký.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="ambient-bg text-on-background font-body-md antialiased flex flex-col min-h-screen">
            <SEO title="Đăng ký" description="Đăng ký tài khoản mới" />
            <main className="flex-1 flex items-center justify-center p-4 py-12">
                <div className="w-full max-w-md mx-auto">

                    {/* Logo */}
                    <div className="text-center mb-6">
                        <div className="relative inline-flex">
                            <div className="w-20 h-20 rounded-full bg-primary/10 flex items-center justify-center"
                                 style={{ animation: 'float 6s ease-in-out infinite' }}>
                                <div className="w-14 h-14 rounded-full bg-primary flex items-center justify-center"
                                     style={{ boxShadow: '0 8px 40px rgba(171,44,93,0.10)' }}>
                                    <span className="material-symbols-outlined text-on-primary text-[32px]" style={{ fontVariationSettings: "'FILL' 1" }}>local_florist</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Brand */}
                    <div className="text-center mb-8">
                        <h1 className="font-display-xl text-display-xl text-primary tracking-tight leading-none">PDA FLOWER</h1>
                        <p className="font-body-md text-body-md text-on-surface-variant/70 mt-3">Tạo tài khoản</p>
                    </div>

                    {/* Register card */}
                    <form onSubmit={handleSubmit(onSubmit)}
                          className="bg-surface-container-lowest border border-outline-variant/20 rounded-2xl px-8 py-8 space-y-6"
                          style={{ boxShadow: '0 8px 40px rgba(171,44,93,0.10)' }}>

                        {/* Full Name */}
                        <div className="space-y-2">
                            <label htmlFor="fullName" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">badge</span>
                                Họ và tên
                            </label>
                            <input id="fullName" type="text" {...register('fullName')}
                                   className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                    placeholder="Họ và tên của bạn" required />
                            {errors.fullName && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.fullName.message}</p>
                            )}
                        </div>

                        {/* Email */}
                        <div className="space-y-2">
                            <label htmlFor="email" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">email</span>
                                Email
                            </label>
                            <input id="email" type="email" {...register('email')}
                                   className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                    placeholder="email@của bạn" required autoComplete="email" />
                            {errors.email && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.email.message}</p>
                            )}
                        </div>

                        {/* Phone + Address (2-col grid) */}
                        <div className="space-y-2">
                            <label htmlFor="phone" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">phone</span>
                                Số điện thoại
                            </label>
                            <input id="phone" type="tel" {...register('phone')}
                                   className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                    placeholder="Số điện thoại" required />
                            {errors.phone && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.phone.message}</p>
                            )}
                        </div>

                        {/* Address */}
                        <div className="space-y-2">
                            <label htmlFor="address" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">home</span>
                                Địa chỉ giao hàng
                            </label>
                            <textarea id="address" {...register('address')}
                                      className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200 resize-none"
                                      placeholder="Địa chỉ giao hàng" rows={2} required />
                            {errors.address && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.address.message}</p>
                            )}
                        </div>

                        {/* Password + Confirm (2-col grid) */}
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <label htmlFor="password" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                    <span className="material-symbols-outlined text-[16px] text-outline">lock</span>
                                    Mật khẩu
                                </label>
                                <input id="password" type="password" {...register('password')}
                                       className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                        placeholder="Tối thiểu 6 ký tự" required autoComplete="new-password" />
                                {errors.password && (
                                    <p className="font-label-sm text-label-sm text-error mt-1">{errors.password.message}</p>
                                )}
                            </div>
                            <div className="space-y-2">
                                <label htmlFor="confirmPassword" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                    <span className="material-symbols-outlined text-[16px] text-outline">check</span>
                                    Xác nhận mật khẩu
                                </label>
                                <input id="confirmPassword" type="password" {...register('confirmPassword')}
                                       className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                        placeholder="Nhập lại mật khẩu" required autoComplete="new-password" />
                                {errors.confirmPassword && (
                                    <p className="font-label-sm text-label-sm text-error mt-1">{errors.confirmPassword.message}</p>
                                )}
                            </div>
                        </div>

                        {/* Error */}
                        {error && (
                            <div className="p-4 bg-error-container border border-error/20 text-error font-label-sm text-label-sm rounded-xl flex items-center gap-3">
                                <span className="material-symbols-outlined text-[20px]">error</span>
                                {error}
                            </div>
                        )}

                        {/* Submit */}
                        <button type="submit" disabled={loading}
                                className="w-full bg-primary text-on-primary py-3.5 rounded-xl font-label-md text-label-md hover:bg-primary/90 active:scale-[0.98] transition-all duration-200 disabled:opacity-50 cursor-pointer border-0 flex items-center justify-center gap-2"
                                style={{ boxShadow: '0 4px 24px rgba(171,44,93,0.06)' }}>
                            <span>{loading ? 'Đang xử lý...' : 'Đăng ký'}</span>
                            <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
                        </button>

                        {/* Login link */}
                        <div className="text-center pt-4 border-t border-outline-variant/20">
                            <p className="font-label-sm text-label-sm text-on-surface-variant">
                                Đã có tài khoản?{' '}
                                <Link to="/login" className="text-primary font-semibold hover:underline ml-1">Đăng nhập</Link>
                            </p>
                        </div>
                    </form>

                    {/* Footer */}
                    <div className="text-center mt-8">
                        <p className="font-label-sm text-label-sm text-on-surface-variant/40">
                            &copy; {new Date().getFullYear()} PDA FLOWER. Đã đăng ký bản quyền.
                        </p>
                    </div>

                </div>
            </main>
        </div>
    );
};

export default RegisterPage;
