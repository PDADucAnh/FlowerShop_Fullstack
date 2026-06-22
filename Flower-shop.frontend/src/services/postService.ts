// Import cấu hình axiosClient dùng chung đã được cấu hình BaseURL ở thư mục api
import axiosClient from '../api/axiosClient';

const postService = {
    /**
     * 1. Lấy danh sách toàn bộ bài viết tin tức từ Backend
     * API Endpoint: GET https://localhost:xxxx/api/Posts (hoặc /api/Blogs tùy cấu hình Backend)
     */
    getAllPosts: async () => {
        try {
            // Thực hiện gọi API GET qua axiosClient
            const response = await axiosClient.get('/Posts');

            // Trả về dữ liệu mảng bài viết (thường nằm trong response.data hoặc trực tiếp response tùy cấu hình client)
            return response.data || response;
        } catch (error) {
            console.error("Lỗi API getAllBlogs:", error);
            throw error; // Đẩy lỗi ra ngoài để Component nhận biết và xử lý UI (như tắt loading, hiện thông báo lỗi)
        }
    },

    /**
     * 2. Lấy thông tin chi tiết của một bài viết theo ID
     * API Endpoint: GET https://localhost:xxxx/api/Posts/{id}
     */
    getPostById: async (id: string | number) => {
        try {
            const response = await axiosClient.get(`/Posts/${id}`);
            return response.data || response;
        } catch (error) {
            console.error(`Lỗi API getBlogById với ID ${id}:`, error);
            throw error;
        }
    },

    /**
     * 3. Lấy danh sách bài viết theo danh mục
     * API Endpoint: GET https://localhost:xxxx/api/Posts/category/{categoryId}
     */
    getPostsByCategory: async (categoryId: number | null) => {
        try {
            const response = await axiosClient.get(`/Posts/category/${categoryId}`);
            return response.data || response;
        } catch (error) {
            console.error(`Lỗi API getPostsByCategory với ID ${categoryId}:`, error);
            throw error;
        }
    }
};

// BẮT BUỘC: Xuất mặc định để file LatestBlog.jsx có thể import trực tiếp không bị lỗi
export default postService;
