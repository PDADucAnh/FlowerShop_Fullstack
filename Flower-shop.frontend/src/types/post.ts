export interface Post {
  id: number;
  title: string;
  content: string;
  imageUrl?: string;
  summary?: string;
  createdDate?: string;
  postCategoryName?: string;
  postCategoryId?: number;
  views?: number;
  updatedAt?: string;
}
