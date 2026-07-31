export interface PostCategory {
  id: number;
  name: string;
  description?: string;
}

export interface PostCategoryInput {
  name: string;
  description?: string;
}
