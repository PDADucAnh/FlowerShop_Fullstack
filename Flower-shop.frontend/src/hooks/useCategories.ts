import { useQuery } from '@tanstack/react-query';
import categoryProductService from '../services/categoryProductService';
import categoryService from '../services/categoryService';

export const useProductCategories = () =>
  useQuery({ queryKey: ['categories', 'products'], queryFn: () => categoryProductService.getAllCategoryProducts() });

export const useBlogCategories = () =>
  useQuery({ queryKey: ['categories', 'blog'], queryFn: () => categoryService.getBlogCategories() });
