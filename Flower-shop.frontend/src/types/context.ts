import type { Product } from './product';

export interface AuthUser {
  id?: number;
  username: string;
  fullName: string;
  email?: string;
  phone?: string;
  address?: string;
  role: string;
}

export interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (username: string, password: string) => Promise<any>;
  logout: () => void;
  refreshProfile: () => Promise<void>;
  updateProfile: (fullName: string, phone: string, address: string) => Promise<any>;
  changePassword: (currentPassword: string, newPassword: string) => Promise<any>;
}


