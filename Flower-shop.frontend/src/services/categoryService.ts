import axiosClient from '../api/axiosClient';

const categoryService = {
    getProductCategories: () => {
        return axiosClient.get('/CategoriesProducts');
    },
    getBlogCategories: () => {
        return axiosClient.get('/Categories');
    }
};

export default categoryService;
