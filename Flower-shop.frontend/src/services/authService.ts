import axiosClient from '../api/axiosClient';

const authService = {
    login: async (username: string, password: string, rememberMe = false) => {
        try {
            const data: any = await axiosClient.post('/Auth/login', { username, password, rememberMe });
            return data;
        } catch (error) {
            console.error("Lỗi đăng nhập:", error);
            throw error;
        }
    },
    register: async (userData: { fullName: string; email: string; phone: string; address: string; password: string }) => {
        try {
            const data: any = await axiosClient.post('/Auth/register', userData);
            return data;
        } catch (error) {
            console.error("Lỗi đăng ký:", error);
            throw error;
        }
    },
    getProfile: async () => {
        try {
            const data: any = await axiosClient.get('/Auth/profile');
            return data;
        } catch (error) {
            console.error("Lỗi lấy hồ sơ:", error);
            throw error;
        }
    },
    updateProfile: async (profileData: { fullName: string; phone: string; address: string }) => {
        try {
            const data: any = await axiosClient.put('/Auth/profile', profileData);
            return data;
        } catch (error) {
            console.error("Lỗi cập nhật hồ sơ:", error);
            throw error;
        }
    },
    changePassword: async (data: { currentPassword: string; newPassword: string }) => {
        try {
            const res: any = await axiosClient.put('/Auth/change-password', data);
            return res;
        } catch (error) {
            console.error("Lỗi đổi mật khẩu:", error);
            throw error;
        }
    },
    forgotPassword: async (email: string) => {
        try {
            const data: any = await axiosClient.post('/Auth/forgot-password', { email });
            return data;
        } catch (error) {
            console.error("Lỗi yêu cầu đặt lại mật khẩu:", error);
            throw error;
        }
    },
    resetPassword: async (data: { token: string; newPassword: string }) => {
        try {
            const res: any = await axiosClient.post('/Auth/reset-password', data);
            return res;
        } catch (error) {
            console.error("Lỗi đặt lại mật khẩu:", error);
            throw error;
        }
    }
};

export default authService;
