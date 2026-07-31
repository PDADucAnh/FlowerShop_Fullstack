import axiosClient from '../api/axiosClient';

const categoryService = {
    getProductCategories: () => {
        return axiosClient.get('/ProductCategories');
    },
    getBlogCategories: () => {
        return axiosClient.get('/PostCategories');
    }
};

export default categoryService;
