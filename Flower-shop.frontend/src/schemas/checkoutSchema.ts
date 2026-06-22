import { z } from 'zod';

export const checkoutSchema = z.object({
  fullname: z.string().min(1, 'Full name is required'),
  email: z.string().email('Valid email is required'),
  phone: z.string().min(10, 'Valid phone required').max(15),
  address: z.string().min(5, 'Address is required'),
  notes: z.string().optional(),
});

export type CheckoutFormData = z.infer<typeof checkoutSchema>;
