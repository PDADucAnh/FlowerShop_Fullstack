import axiosClient from '../api/axiosClient';
import { Advertisement } from '../types/advertisement';

const advertisementService = {
  getActive: async (): Promise<Advertisement[]> => {
    try {
      const response: any = await axiosClient.get('/Advertisements/active');
      return response;
    } catch (error) {
      console.error('API getActiveAdvertisements error:', error);
      throw error;
    }
  },
};

export default advertisementService;
