import axios from 'axios';
import tokenService from '../services/tokenService';
import { API_BASE_URL } from '../utils/apiUtils';
import { authEvents } from '../utils/eventEmitter';

const axiosClient = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json',
    },
    timeout: 10000,
});

axiosClient.interceptors.request.use(
    (config) => {
        const token = tokenService.getToken();
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

axiosClient.interceptors.response.use(
    (response) => response.data,
    (error) => {
        if (error.response?.status === 401) {
            tokenService.removeToken();
            authEvents.emit('unauthorized');
        }
        console.error('API Error:', error.response || error.message);
        return Promise.reject(error);
    }
);

export default axiosClient;
