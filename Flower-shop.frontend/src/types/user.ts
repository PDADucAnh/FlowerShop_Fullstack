export interface User {
  id: number;
  username: string;
  fullName: string;
  role: string;
}

export interface UserInput {
  username: string;
  password: string;
  fullName: string;
  role: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: string;
  fullName: string;
}
