import type {
  LoginRequest,
  LoginResponse,
  RefreshTokenRequest,
  RefreshTokenResponse,
  User,
} from '@/types/auth'
import type { ApiResponse } from '@/types/api'
import { apiClient } from './client'

export const authApi = {
  login(data: LoginRequest) {
    return apiClient.post<LoginResponse>('/api/auth/login', data)
  },

  refresh(data: RefreshTokenRequest) {
    return apiClient.post<ApiResponse<RefreshTokenResponse>>(
      '/api/auth/refresh',
      data
    )
  },

  logout(refreshToken: string) {
    return apiClient.post('/api/auth/logout', { refreshToken })
  },

  getMe() {
    return apiClient.get<ApiResponse<User>>('/api/auth/me')
  },
}
