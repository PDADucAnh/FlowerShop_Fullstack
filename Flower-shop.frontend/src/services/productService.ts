import axiosClient from '../api/axiosClient';

const productService = {
    getProductsPaged: async (
        page: number, 
        pageSize: number, 
        minPrice?: number | null, 
        maxPrice?: number | null, 
        categoryProductId?: number | null
    ) => {
        try {
            const searchParams = new URLSearchParams();
            searchParams.set('page', page.toString());
            searchParams.set('pageSize', pageSize.toString());
            if (minPrice !== undefined && minPrice !== null) searchParams.set('minPrice', minPrice.toString());
            if (maxPrice !== undefined && maxPrice !== null) searchParams.set('maxPrice', maxPrice.toString());
            if (categoryProductId !== undefined && categoryProductId !== null) searchParams.set('categoryProductId', categoryProductId.toString());
            
            const response = await axiosClient.get(`/Products/paged?${searchParams.toString()}`);
            return response.data || response;
        } catch (error) {
            console.error('API getProductsPaged error:', error);
            throw error;
        }
    },

    getAllProducts: async () => {
        try {
            const response = await axiosClient.get('/Products');
            return response.data || response;
        } catch (error) {
            console.error('API getAllProducts error:', error);
            throw error;
        }
    },

    getProductById: async (id: string | number) => {
        try {
            const response = await axiosClient.get(`/Products/${id}`);
            return response.data || response;
        } catch (error) {
            console.error(`API getProductById error for ID ${id}:`, error);
            throw error;
        }
    },

    getProductsByCategory: async (categoryProductId: number | null) => {
        try {
            const response = await axiosClient.get(`/Products/categoryproduct/${categoryProductId}`);
            return response.data || response;
        } catch (error) {
            console.error(`API getProductsByCategory error for ID ${categoryProductId}:`, error);
            throw error;
        }
    },

    searchProducts: async (query: string) => {
        try {
            const response = await axiosClient.get(`/Products/search?query=${encodeURIComponent(query)}`);
            return response.data || response;
        } catch (error) {
            console.error('API searchProducts error:', error);
            throw error;
        }
    },

    getTrendingProducts: async (count: number = 10) => {
        try {
            const response = await axiosClient.get(`/Products/trending?count=${count}`);
            return response.data || response;
        } catch (error) {
            console.error('API getTrendingProducts error:', error);
            throw error;
        }
    },

    trackAddToCart: async (productId: number) => {
        try {
            await axiosClient.post(`/Products/${productId}/track-add-to-cart`);
        } catch (error) {
            console.error('API trackAddToCart error:', error);
        }
    }
};

export default productService;
