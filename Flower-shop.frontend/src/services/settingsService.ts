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
  getStoreInfo: () => {
    return axiosClient.get<StoreInfo>('/settings/store-info');
  },
};

export default settingsService;
