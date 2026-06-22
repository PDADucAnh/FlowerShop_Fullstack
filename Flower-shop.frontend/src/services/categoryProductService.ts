import axiosClient from '../api/axiosClient';

const categoryProductService = {
    getAllCategoryProducts: () => {
        return axiosClient.get('/CategoriesProducts');
    }
};

export default categoryProductService;
