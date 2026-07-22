import axiosClient from '../api/axiosClient';

export interface CreateContact {
  name: string;
  email: string;
  phone?: string;
  subject: string;
  message: string;
}

const contactService = {
  submit: async (data: CreateContact) => {
    const response = await axiosClient.post('/contacts', data);
    return response.data || response;
  },
};

export default contactService;
