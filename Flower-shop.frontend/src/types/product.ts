export interface Product {
  id: number;
  sku?: string;
  name: string;
  description?: string;
  slug?: string;
  price: number;
  discountPrice?: number;
  promotionPrice?: number;
  promotionPercent?: number;
  promotionType?: string;
  hasFlashSale?: boolean;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductName?: string;
  categoryProductId?: number;
  viewCount?: number;
  addToCartCount?: number;
  trendingScore?: number;
  trendingBadge?: string;
}

export interface ProductInput {
  sku?: string;
  name: string;
  description?: string;
  slug?: string;
  price: number;
  imageUrl?: string;
  stockQuantity: number;
  categoryProductId?: number;
}

export interface ProductFormData {
  sku?: string;
  name: string;
  description: string;
  slug: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;
  categoryProductId: number;
}
