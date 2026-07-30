export interface User {
  id: number
  username: string
  fullName: string
  email?: string
  phone?: string
  address?: string
  role: string
}

export interface CreateUserRequest {
  username: string
  password: string
  fullName: string
  role: string
}

export interface UpdateUserRequest {
  id: number
  username: string
  password?: string
  fullName: string
  email?: string
  phone?: string
  address?: string
  role: string
}
