import axiosClient from '../api/axiosClient';

const productService = {
    getProductsPaged: async (
        page: number, 
        pageSize: number, 
        minPrice?: number | null, 
        maxPrice?: number | null, 
        productCategoryId?: number | null,
        sortBy?: string | null,
        promotionOnly?: boolean | null
    ) => {
        try {
            const searchParams = new URLSearchParams();
            searchParams.set('page', page.toString());
            searchParams.set('pageSize', pageSize.toString());
            if (minPrice !== undefined && minPrice !== null) searchParams.set('minPrice', minPrice.toString());
            if (maxPrice !== undefined && maxPrice !== null) searchParams.set('maxPrice', maxPrice.toString());
            if (productCategoryId !== undefined && productCategoryId !== null) searchParams.set('productCategoryId', productCategoryId.toString());
            if (sortBy) searchParams.set('sortBy', sortBy);
            if (promotionOnly) searchParams.set('promotionOnly', 'true');
            
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

    getProductsByCategory: async (productCategoryId: number | null) => {
        try {
            const response = await axiosClient.get(`/Products/productcategory/${productCategoryId}`);
            return response.data || response;
        } catch (error) {
            console.error(`API getProductsByCategory error for ID ${productCategoryId}:`, error);
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
    },

    getBestPromotion: async (productId: number) => {
        try {
            const response = await axiosClient.get(`/Promotions/product/${productId}`);
            return response.data || response;
        } catch (error) {
            console.error('API getBestPromotion error:', error);
            return null;
        }
    },

    recalculateCart: async (items: { productId: number; quantity: number; name: string; price: number; promotionPrice?: number }[]) => {
        try {
            const response = await axiosClient.post('/Products/recalculate-cart', { items });
            return response.data || response;
        } catch (error) {
            console.error('API recalculateCart error:', error);
            throw error;
        }
    },
};

export default productService;
