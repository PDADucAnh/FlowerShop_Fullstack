import axiosClient from '../api/axiosClient';

const authService = {
    login: async (username: string, password: string) => {
        try {
            const response = await axiosClient.post('/Auth/login', { username, password });
            return response.data || response;
        } catch (error) {
            console.error("Lỗi đăng nhập:", error);
            throw error;
        }
    },
    register: async (userData: { fullName: string; email: string; phone: string; address: string; password: string }) => {
        try {
            const response = await axiosClient.post('/Auth/register', userData);
            return response.data || response;
        } catch (error) {
            console.error("Lỗi đăng ký:", error);
            throw error;
        }
    },
    getProfile: async () => {
        try {
            const response = await axiosClient.get('/Auth/profile');
            return response.data || response;
        } catch (error) {
            console.error("Lỗi lấy hồ sơ:", error);
            throw error;
        }
    }
};

export default authService;
