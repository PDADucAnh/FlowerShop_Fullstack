import axios from 'axios';
import tokenService from '../services/tokenService';
import { API_BASE_URL } from '../utils/apiUtils';
import { authEvents } from '../utils/eventEmitter';

const axiosClient = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json; charset=UTF-8',
        'Accept': 'application/json; charset=UTF-8',
    },
    timeout: 60000,
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

function fixMojibake(data: any): any {
    if (typeof data === 'string') {
        try {
            if (/[^\u0000-\u007F]/.test(data)) return data;
            const decoded = decodeURIComponent(escape(data));
            if (decoded !== data && /[^\u0000-\u007F]/.test(decoded)) return decoded;
        } catch {}
        return data;
    }
    if (data && typeof data === 'object') {
        for (const key in data) {
            data[key] = fixMojibake(data[key]);
        }
    }
    return data;
}

axiosClient.interceptors.response.use(
    (response) => {
        if (response.config.responseType === 'blob' || response.config.responseType === 'stream') {
            return response.data;
        }
        return fixMojibake(response.data);
    },
    (error) => {
        if (error.response?.status === 401) {
            tokenService.removeToken();
            authEvents.emit('unauthorized', window.location.pathname);
        }
        console.error('API Error:', error.response || error.message);
        return Promise.reject(error);
    }
);

export default axiosClient;
