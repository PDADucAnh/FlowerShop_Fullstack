// Import cấu hình axiosClient dùng chung từ thư mục api
import axiosClient from '../api/axiosClient';

const productService = {
    /**
     * 1. Lấy danh sách toàn bộ sản phẩm thời trang (hoặc theo bộ lọc)
     * API Endpoint: GET https://localhost:xxxx/api/Products
     */
    getAllProducts: async () => {
        try {
            // Thực hiện gọi API GET để lấy danh sách sản phẩm
            const response = await axiosClient.get('/Products');

            // Trả về mảng dữ liệu sản phẩm
            return response.data || response;
        } catch (error) {
            console.error("Lỗi API getAllProducts:", error);
            throw error; // Đẩy lỗi ra ngoài để component ProductGrid bắt được và xử lý giao diện
        }
    },

    /**
     * 2. Lấy thông tin chi tiết của một sản phẩm theo ID
     * API Endpoint: GET https://localhost:xxxx/api/Products/{id}
     */
    getProductById: async (id: string | number) => {
        try {
            const response = await axiosClient.get(`/Products/${id}`);
            return response.data || response;
        } catch (error) {
            console.error(`Lỗi API getProductById với ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * 3. Lấy danh sách sản phẩm theo danh mục sản phẩm
     * API Endpoint: GET https://localhost:xxxx/api/Products/categoryproduct/{categoryProductId}
     */
    getProductsByCategory: async (categoryProductId: number | null) => {
        try {
            const response = await axiosClient.get(`/Products/categoryproduct/${categoryProductId}`);
            return response.data || response;
        } catch (error) {
            console.error(`Lỗi API getProductsByCategory với ID ${categoryProductId}:`, error);
            throw error;
        }
    }
};

// CRITICAL: Xuất mặc định đối tượng này để file ProductGrid.jsx import vào không bị lỗi 'default was not found'
export default productService;
