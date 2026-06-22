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
}

export interface CartItem extends Product {
  quantity: number;
}
