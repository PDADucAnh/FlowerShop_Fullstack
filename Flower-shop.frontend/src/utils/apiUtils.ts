import CONFIG from '../config';

export const API_BASE_URL: string = CONFIG.API_BASE_URL;

export const getImageUrl = (path?: string): string => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${CONFIG.IMAGE_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
