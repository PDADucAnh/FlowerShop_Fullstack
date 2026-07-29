import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react'
import type { User } from '@/types/auth'
import { authApi } from '@/api/auth'

interface AuthContextValue {
  user: User | null
  isLoading: boolean
  isAuthenticated: boolean
  login: (accessToken: string, refreshToken: string, user: User) => void
  logout: () => Promise<void>
  setUser: (user: User) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('accessToken')
    if (!token) {
      setIsLoading(false)
      return
    }
    authApi
      .getMe()
      .then(({ data }) => {
        if (data.data) {
          setUser(data.data)
        }
      })
      .catch(() => {
        localStorage.clear()
      })
      .finally(() => setIsLoading(false))
  }, [])

  const login = useCallback(
    (accessToken: string, refreshToken: string, user: User) => {
      localStorage.setItem('accessToken', accessToken)
      localStorage.setItem('refreshToken', refreshToken)
      setUser(user)
    },
    []
  )

  const logout = useCallback(async () => {
    try {
      const token = localStorage.getItem('refreshToken')
      if (token) {
        await authApi.logout(token)
      }
    } catch {
    } finally {
      localStorage.clear()
      setUser(null)
    }
  }, [])

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        isAuthenticated: !!user,
        login,
        logout,
        setUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return ctx
}
