import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useAuth } from '../../context/AuthContext';
import { loginSchema, type LoginFormData } from '../../schemas/loginSchema';

const LoginPage: React.FC = () => {
    const [error, setError] = React.useState<string>('');
    const [loading, setLoading] = React.useState<boolean>(false);
    const navigate = useNavigate();
    const { login } = useAuth();

    const { register, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = async (data: LoginFormData) => {
        setError('');
        setLoading(true);
        try {
            await login(data.username, data.password);
            navigate('/');
        } catch {
            setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="ambient-bg text-on-background font-body-md antialiased flex flex-col min-h-screen">
            <main className="flex-1 flex items-center justify-center p-4">
                <div className="w-full max-w-sm mx-auto">

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
                        <p className="font-body-md text-body-md text-on-surface-variant/70 mt-3">Đăng nhập khách hàng</p>
                    </div>

                    {/* Login card */}
                    <form onSubmit={handleSubmit(onSubmit)}
                          className="bg-surface-container-lowest border border-outline-variant/20 rounded-2xl px-8 py-8 space-y-6"
                          style={{ boxShadow: '0 8px 40px rgba(171,44,93,0.10)' }}>

                        {/* Email / Username */}
                        <div className="space-y-2">
                            <label htmlFor="username" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">person</span>
                                Email
                            </label>
                            <input id="username" type="email" {...register('username')}
                                   className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                    placeholder="Nhập email của bạn" required autoComplete="email" />
                            {errors.username && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.username.message}</p>
                            )}
                        </div>

                        {/* Password */}
                        <div className="space-y-2">
                            <div className="flex justify-between items-center">
                                <label htmlFor="password" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                    <span className="material-symbols-outlined text-[16px] text-outline">lock</span>
                                    Mật khẩu
                                </label>
                                <Link to="/forgot-password" className="font-label-sm text-label-sm text-primary hover:underline">
                                    Quên mật khẩu?
                                </Link>
                            </div>
                            <input id="password" type="password" {...register('password')}
                                   className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                    placeholder="Nhập mật khẩu" required autoComplete="current-password" />
                            {errors.password && (
                                <p className="font-label-sm text-label-sm text-error mt-1">{errors.password.message}</p>
                            )}
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
                            <span>{loading ? 'Đang đăng nhập...' : 'Đăng nhập'}</span>
                            <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
                        </button>

                        {/* Register link */}
                        <div className="text-center pt-4 border-t border-outline-variant/20">
                            <p className="font-label-sm text-label-sm text-on-surface-variant">
                                Chưa có tài khoản?{' '}
                                <Link to="/register" className="text-primary font-semibold hover:underline ml-1">Đăng ký</Link>
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

export default LoginPage;
