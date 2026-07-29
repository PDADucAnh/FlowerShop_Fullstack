import { apiClient } from './client'
import type { PaginatedResponse } from '@/types/api'
import type { PostDTO, CreatePostDTO, UpdatePostDTO } from '@/types/post'

export interface PostsPagedParams {
  page?: number
  pageSize?: number
  search?: string
}

export const postsApi = {
  getPaged(params: PostsPagedParams = {}) {
    return apiClient.get<PaginatedResponse<PostDTO>>('/api/posts/paged', { params })
  },
  getById(id: number) {
    return apiClient.get<PostDTO>(`/api/posts/${id}`)
  },
  create(dto: CreatePostDTO) {
    return apiClient.post<PostDTO>('/api/posts', dto)
  },
  update(id: number, dto: UpdatePostDTO) {
    return apiClient.put(`/api/posts/${id}`, dto)
  },
  delete(id: number) {
    return apiClient.delete(`/api/posts/${id}`)
  },
}
