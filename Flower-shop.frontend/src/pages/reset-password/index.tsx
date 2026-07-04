import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import authService from '../../services/authService';

export default function ResetPassword() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get('token') || '';
    const navigate = useNavigate();

    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    useEffect(() => {
        if (!token) {
            setError('Mã token đặt lại mật khẩu không tìm thấy hoặc không hợp lệ.');
        }
    }, [token]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!token) {
            setError('Không tìm thấy token đặt lại mật khẩu hợp lệ.');
            return;
        }
        if (password !== confirmPassword) {
            setError('Mật khẩu nhập lại không trùng khớp.');
            return;
        }
        if (password.length < 6) {
            setError('Mật khẩu phải dài tối thiểu 6 ký tự.');
            return;
        }

        setLoading(true);
        setMessage('');
        setError('');
        try {
            const res = await authService.resetPassword({ token, newPassword: password });
            setMessage(res.message || 'Mật khẩu của bạn đã được đặt lại thành công. Đang chuyển hướng...');
            setTimeout(() => {
                navigate('/login');
            }, 3000);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại.');
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
                        <p className="font-body-md text-body-md text-on-surface-variant/70 mt-3">Đặt lại mật khẩu</p>
                    </div>

                    {/* Form container */}
                    <form onSubmit={handleSubmit}
                          className="bg-surface-container-lowest border border-outline-variant/20 rounded-2xl px-8 py-8 space-y-6"
                          style={{ boxShadow: '0 8px 40px rgba(171,44,93,0.10)' }}>
                        
                        <p className="text-center text-sm text-on-surface-variant/80 font-body-md leading-relaxed">
                            Vui lòng nhập mật khẩu mới của bạn bên dưới.
                        </p>

                        {/* Password */}
                        <div className="space-y-2">
                            <label htmlFor="password" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">lock</span>
                                Mật khẩu mới
                            </label>
                            <input
                                id="password"
                                name="password"
                                type="password"
                                required
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                placeholder="Nhập mật khẩu mới"
                            />
                        </div>

                        {/* Confirm Password */}
                        <div className="space-y-2">
                            <label htmlFor="confirmPassword" className="font-label-sm text-label-sm text-on-surface-variant flex items-center gap-2">
                                <span className="material-symbols-outlined text-[16px] text-outline">lock_reset</span>
                                Xác nhận mật khẩu mới
                            </label>
                            <input
                                id="confirmPassword"
                                name="confirmPassword"
                                type="password"
                                required
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                className="w-full bg-surface-container-low border border-outline-variant/30 rounded-xl px-4 py-3.5 font-body-md text-body-md text-on-surface placeholder:text-outline/60 focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all duration-200"
                                placeholder="Xác nhận mật khẩu mới"
                            />
                        </div>

                        {/* Message / Success */}
                        {message && (
                            <div className="p-4 bg-green-500/10 border border-green-500/20 text-green-700 font-label-sm text-label-sm rounded-xl flex items-center gap-3">
                                <span className="material-symbols-outlined text-[20px] text-green-600">check_circle</span>
                                <div>{message}</div>
                            </div>
                        )}

                        {/* Error */}
                        {error && (
                            <div className="p-4 bg-error-container border border-error/20 text-error font-label-sm text-label-sm rounded-xl flex items-center gap-3">
                                <span className="material-symbols-outlined text-[20px]">error</span>
                                <div>{error}</div>
                            </div>
                        )}

                        {/* Submit */}
                        <button type="submit" disabled={loading || !token}
                                className="w-full bg-primary text-on-primary py-3.5 rounded-xl font-label-md text-label-md hover:bg-primary/90 active:scale-[0.98] transition-all duration-200 disabled:opacity-50 cursor-pointer border-0 flex items-center justify-center gap-2"
                                style={{ boxShadow: '0 4px 24px rgba(171,44,93,0.06)' }}>
                            <span>{loading ? 'Đang thực hiện...' : 'Đặt lại mật khẩu'}</span>
                            <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
                        </button>
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
}
