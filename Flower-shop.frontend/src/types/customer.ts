export interface Customer {
  id: number;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
}

export interface CustomerInput {
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  password?: string;
}
