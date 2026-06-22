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
            setError('Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="bg-surface text-on-surface font-body-md antialiased flex flex-col min-h-screen">
            <main className="flex-1 flex items-center justify-center p-sm py-20">
                <div className="w-full max-w-md space-y-xl">
                    <div className="text-center space-y-md">
                        <h1 className="font-headline-md text-headline-md text-primary tracking-tight">FlowerShop</h1>
                        <p className="font-label-sm text-label-sm text-secondary uppercase tracking-[0.4em] font-bold">Secure Portal Access</p>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)} className="bg-surface-container-lowest border border-outline-variant p-lg space-y-lg rounded-xl">
                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Email</label>
                            <input type="email" {...register('username')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                placeholder="Email" />
                            {errors.username && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.username.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Password</label>
                            <input type="password" {...register('password')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                placeholder="Password" />
                            {errors.password && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.password.message}</p>}
                        </div>

                        {error && (
                            <div className="p-md bg-error-container text-error font-label-sm text-label-sm uppercase tracking-widest font-bold text-center">
                                {error}
                            </div>
                        )}

                        <button type="submit" disabled={loading}
                            className="w-full bg-primary text-on-primary py-4 font-label-md text-label-md uppercase tracking-[0.3em] font-bold border border-primary outline-none disabled:opacity-50 rounded-lg btn-luxury btn-primary-luxury">
                            {loading ? 'Authenticating...' : 'Sign In'}
                        </button>

                        <div className="text-center pt-4 border-t border-outline-variant">
                            <p className="font-label-sm text-label-sm text-secondary uppercase tracking-widest">
                                New Member? <Link to="/register" className="text-primary font-bold hover:underline ml-2">Create Account</Link>
                            </p>
                        </div>
                    </form>
                </div>
            </main>
        </div>
    );
};

export default LoginPage;
