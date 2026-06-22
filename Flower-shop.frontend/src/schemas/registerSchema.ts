import { z } from 'zod';

export const registerSchema = z.object({
  fullName: z.string().min(1, 'Full name is required').max(100),
  email: z.string().email('Invalid email address'),
  phone: z.string().min(10, 'Minimum 10 digits').max(15, 'Maximum 15 digits').regex(/^\d+$/, 'Phone must contain only digits'),
  address: z.string().min(1, 'Address is required').max(500),
  password: z.string().min(6, 'Minimum 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
});

export type RegisterFormData = z.infer<typeof registerSchema>;
