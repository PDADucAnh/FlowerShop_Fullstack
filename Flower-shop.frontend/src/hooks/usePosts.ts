import { useQuery } from '@tanstack/react-query';
import postService from '../services/postService';

export const usePosts = () =>
  useQuery({ queryKey: ['posts'], queryFn: () => postService.getAllPosts() });

export const usePost = (id: string | number) =>
  useQuery({ queryKey: ['posts', id], queryFn: () => postService.getPostById(id), enabled: !!id });

export const usePostsByCategory = (categoryId: number | null) =>
  useQuery({
    queryKey: ['posts', 'category', categoryId],
    queryFn: () => postService.getPostsByCategory(categoryId),
    enabled: categoryId !== null,
  });
