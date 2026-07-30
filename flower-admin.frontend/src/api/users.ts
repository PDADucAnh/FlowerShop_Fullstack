import { apiClient } from './client'
import type { User, CreateUserRequest, UpdateUserRequest } from '@/types/user'

export const usersApi = {
  getAll() {
    return apiClient.get<User[]>('/api/Users')
  },
  getById(id: number) {
    return apiClient.get<User>(`/api/Users/${id}`)
  },
  create(data: CreateUserRequest) {
    return apiClient.post<User>('/api/Users', data)
  },
  update(id: number, data: UpdateUserRequest) {
    return apiClient.put(`/api/Users/${id}`, data)
  },
  delete(id: number) {
    return apiClient.delete(`/api/Users/${id}`)
  },
}
