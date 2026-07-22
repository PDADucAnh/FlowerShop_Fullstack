import axiosClient from '../api/axiosClient';

export interface StoreInfo {
  storeName: string;
  logo: string;
  hotline: string;
  email: string;
  address: string;
  facebook: string;
  zalo: string;
  openHours: string;
}

const settingsService = {
  getStoreInfo: async () => {
    const response = await axiosClient.get<StoreInfo>('/settings/store-info');
    return response.data || response;
  },
};

export default settingsService;
