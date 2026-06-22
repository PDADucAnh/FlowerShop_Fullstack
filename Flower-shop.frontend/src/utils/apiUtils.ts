const getBaseUrl = (): string => {
  const url = process.env.REACT_APP_API_URL;
  if (!url) {
    if (process.env.NODE_ENV === 'production') {
      throw new Error('REACT_APP_API_URL environment variable is required in production');
    }
    return 'https://localhost:7224';
  }
  return url;
};

export const API_BASE_URL: string = getBaseUrl();

export const getImageUrl = (path?: string): string => {
  if (!path) return 'https://via.placeholder.com/400x400?text=No+Image';
  if (path.startsWith('http')) return path;
  return `${API_BASE_URL}${path.startsWith('/') ? '' : '/'}${path}`;
};
