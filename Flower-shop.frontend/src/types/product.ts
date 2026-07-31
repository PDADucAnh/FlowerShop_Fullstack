export interface ProductImage {
  id: number
  imageUrl: string
  sortOrder: number
}

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
  productCategoryName?: string;
  productCategoryId?: number;
  viewCount?: number;
  addToCartCount?: number;
  trendingScore?: number;
  trendingBadge?: string;
  originalPrice?: number;
  currentPrice?: number;
  discountPercent?: number;
  discountAmount?: number;
  isFlashSale?: boolean;
  promotionName?: string;
  images?: ProductImage[];
}

export interface ProductInput {
  sku?: string;
  name: string;
  description?: string;
  slug?: string;
  price: number;
  imageUrl?: string;
  stockQuantity: number;
  productCategoryId?: number;
}

export interface ProductFormData {
  sku?: string;
  name: string;
  description: string;
  slug: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;
  productCategoryId: number;
}
