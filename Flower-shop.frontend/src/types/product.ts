export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductName?: string;
  createdDate?: string;
}

export interface ProductInput {
  name: string;
  description?: string;
  price: number;
  discountPrice?: number;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductId?: number;
}
