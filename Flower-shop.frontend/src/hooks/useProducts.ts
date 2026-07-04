import { useQuery } from '@tanstack/react-query';
import productService from '../services/productService';
import type { PagedResult } from '../types/pagination';

export const useProducts = () => {
  return useQuery({
    queryKey: ['products'],
    queryFn: () => productService.getAllProducts(),
  });
};

export const useProductsPaged = (
  page: number, 
  pageSize: number, 
  minPrice?: number | null, 
  maxPrice?: number | null, 
  categoryProductId?: number | null
) => {
  return useQuery<PagedResult<any>>({
    queryKey: ['products', 'paged', page, pageSize, minPrice, maxPrice, categoryProductId],
    queryFn: () => productService.getProductsPaged(page, pageSize, minPrice, maxPrice, categoryProductId),
    placeholderData: (prev) => prev,
  });
};

export const useProduct = (id: string | number) => {
  return useQuery({
    queryKey: ['products', id],
    queryFn: () => productService.getProductById(id),
    enabled: !!id,
  });
};

export const useProductsByCategory = (categoryId: number | null) => {
  return useQuery({
    queryKey: ['products', 'category', categoryId],
    queryFn: () => productService.getProductsByCategory(categoryId),
    enabled: categoryId !== null,
  });
};

export const useLatestProducts = (limit: number) => {
  const query = useProducts();
  return {
    ...query,
    data: query.data?.slice(0, limit) ?? [],
  };
};

export const useBestSellingProducts = (limit: number) => {
  return useQuery({
    queryKey: ['products', 'trending', limit],
    queryFn: () => productService.getTrendingProducts(limit),
  });
};

export const useSearchProducts = (query: string) => {
  return useQuery<any[]>({
    queryKey: ['products', 'search', query],
    queryFn: () => productService.searchProducts(query),
    enabled: !!query,
  });
};
