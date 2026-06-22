export interface ApiListResponse<T> {
  $values: T[];
  $id?: string;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
