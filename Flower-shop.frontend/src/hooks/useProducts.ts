import { useQuery } from '@tanstack/react-query';
import productService from '../services/productService';

export const useProducts = () =>
  useQuery({ queryKey: ['products'], queryFn: () => productService.getAllProducts() });

export const useProduct = (id: string | number) =>
  useQuery({ queryKey: ['products', id], queryFn: () => productService.getProductById(id), enabled: !!id });

export const useProductsByCategory = (categoryId: number | null) =>
  useQuery({
    queryKey: ['products', 'category', categoryId],
    queryFn: () => productService.getProductsByCategory(categoryId),
    enabled: categoryId !== null,
  });
