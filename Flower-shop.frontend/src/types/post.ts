export interface Post {
  id: number;
  title: string;
  content: string;
  imageUrl?: string;
  summary?: string;
  createdDate?: string;
  categoryName?: string;
  categoryId?: number;
  views?: number;
  updatedAt?: string;
}
