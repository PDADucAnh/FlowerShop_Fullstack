import axiosClient from '../api/axiosClient';

const categoryProductService = {
    getAllProductCategories: () => {
        return axiosClient.get('/ProductCategories');
    }
};

export default categoryProductService;
