import { useMutation, useQueryClient } from '@tanstack/react-query';
import orderService from '../services/orderService';
import toast from 'react-hot-toast';

export const useCreateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: any) => orderService.submitOrder(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      toast.success('Transaction complete. Your acquisition has been recorded.');
    },
    onError: () => {
      toast.error('An error occurred during the transaction. Please try again.');
    },
  });
};
