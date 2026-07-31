import { useQuery } from '@tanstack/react-query';
import categoryProductService from '../services/categoryProductService';
import categoryService from '../services/categoryService';

export const useProductCategories = () =>
  useQuery({ queryKey: ['product-categories'], queryFn: () => categoryProductService.getAllProductCategories() });

export const useBlogCategories = () =>
  useQuery({ queryKey: ['post-categories'], queryFn: () => categoryService.getBlogCategories() });
