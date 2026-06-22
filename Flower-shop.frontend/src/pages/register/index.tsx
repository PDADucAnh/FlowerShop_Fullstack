import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import authService from '../../services/authService';
import toast from 'react-hot-toast';
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
                toast.success('Account created successfully. Please sign in.');
                navigate('/login');
            } else {
                setError('Registration failed. Username may already be taken.');
            }
        } catch {
            setError('An error occurred during registration.');
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
                        <p className="font-label-sm text-label-sm text-secondary uppercase tracking-[0.4em] font-bold">New Account Registration</p>
                    </div>

                    <form onSubmit={handleSubmit(onSubmit)} className="bg-surface-container-lowest border border-outline-variant p-lg space-y-lg rounded-xl">
                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Full Name</label>
                            <input type="text" {...register('fullName')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm font-semibold tracking-widest placeholder:text-outline-variant"
                                placeholder="Full Name" />
                            {errors.fullName && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.fullName.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Email Address</label>
                            <input type="email" {...register('email')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                placeholder="Email" />
                            {errors.email && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.email.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Phone Number</label>
                            <input type="tel" {...register('phone')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                placeholder="Phone Number" />
                            {errors.phone && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.phone.message}</p>}
                        </div>

                        <div className="space-y-sm">
                            <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Shipping Address</label>
                            <textarea {...register('address')}
                                className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant resize-none rounded-lg"
                                placeholder="Shipping Address" rows={3} />
                            {errors.address && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.address.message}</p>}
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-md">
                            <div className="space-y-sm">
                                <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Password</label>
                                <input type="password" {...register('password')}
                                    className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                    placeholder="Password" />
                                {errors.password && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.password.message}</p>}
                            </div>
                            <div className="space-y-sm">
                                <label className="font-label-sm text-label-sm uppercase tracking-widest text-secondary font-bold">Confirm Password</label>
                                <input type="password" {...register('confirmPassword')}
                                    className="w-full bg-surface-container-low border-none focus:ring-1 focus:ring-primary px-md py-4 text-sm tracking-widest placeholder:text-outline-variant"
                                    placeholder="Confirm" />
                                {errors.confirmPassword && <p className="text-error font-label-sm text-label-sm uppercase tracking-widest font-bold mt-1">{errors.confirmPassword.message}</p>}
                            </div>
                        </div>

                        {error && (
                            <div className="p-md bg-error-container text-error font-label-sm text-label-sm uppercase tracking-widest font-bold text-center">
                                {error}
                            </div>
                        )}

                        <button type="submit" disabled={loading}
                            className="w-full bg-primary text-on-primary py-4 font-label-md text-label-md uppercase tracking-[0.3em] font-bold border border-primary outline-none disabled:opacity-50 rounded-lg btn-luxury btn-primary-luxury">
                            {loading ? 'Processing...' : 'Create Account'}
                        </button>

                        <div className="text-center pt-4 border-t border-outline-variant">
                            <p className="font-label-sm text-label-sm text-secondary uppercase tracking-widest">
                                Already have an account? <Link to="/login" className="text-primary font-bold hover:underline ml-2">Sign In</Link>
                            </p>
                        </div>
                    </form>
                </div>
            </main>
        </div>
    );
};

export default RegisterPage;
