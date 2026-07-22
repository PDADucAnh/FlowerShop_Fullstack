import axiosClient from '../api/axiosClient';

export interface Page {
  id: number;
  title: string;
  slug: string;
  content: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

const pageService = {
  getBySlug: async (slug: string) => {
    const response = await axiosClient.get<Page>(`/pages/slug/${slug}`);
    return response.data || response;
  },
};

export default pageService;
