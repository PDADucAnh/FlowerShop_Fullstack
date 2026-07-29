import axios from 'axios'
import type { ApiResponse } from '@/types/api'
import type { RefreshTokenResponse } from '@/types/auth'

const API_BASE = import.meta.env.VITE_API_URL || ''

export const apiClient = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
})

let isRefreshing = false
let failedQueue: Array<{
  resolve: (token: string) => void
  reject: (error: unknown) => void
}> = []

function processQueue(error: unknown, token: string | null) {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error)
    } else {
      resolve(token!)
    }
  })
  failedQueue = []
}

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes('/api/auth/')
    ) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({
            resolve: (token: string) => {
              originalRequest.headers.Authorization = `Bearer ${token}`
              resolve(apiClient(originalRequest))
            },
            reject,
          })
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      const accessToken = localStorage.getItem('accessToken')
      const refreshToken = localStorage.getItem('refreshToken')

      if (!accessToken || !refreshToken) {
        isRefreshing = false
        localStorage.clear()
        window.location.href = '/login'
        return Promise.reject(error)
      }

      try {
        const { data } = await axios.post<
          ApiResponse<RefreshTokenResponse>
        >(`${API_BASE}/api/auth/refresh`, { accessToken, refreshToken })

        if (data.data) {
          localStorage.setItem('accessToken', data.data.accessToken)
          localStorage.setItem('refreshToken', data.data.refreshToken)
          processQueue(null, data.data.accessToken)
          originalRequest.headers.Authorization = `Bearer ${data.data.accessToken}`
          return apiClient(originalRequest)
        }

        throw new Error('Refresh failed')
      } catch (refreshError) {
        processQueue(refreshError, null)
        localStorage.clear()
        window.location.href = '/login'
        return Promise.reject(refreshError)
      } finally {
        isRefreshing = false
      }
    }

    return Promise.reject(error)
  }
)
