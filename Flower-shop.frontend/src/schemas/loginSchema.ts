import { z } from 'zod';

export const loginSchema = z.object({
  username: z.string().min(1, 'Email is required').max(100).email('Invalid email format'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  rememberMe: z.boolean().optional(),
});

export type LoginFormData = z.infer<typeof loginSchema>;
