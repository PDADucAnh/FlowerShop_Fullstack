const TOKEN_KEY = 'anhcms_token';

const tokenService = {
    getToken: () => localStorage.getItem(TOKEN_KEY),
    setToken: (token: string) => localStorage.setItem(TOKEN_KEY, token),
    removeToken: () => localStorage.removeItem(TOKEN_KEY),
    hasToken: () => !!localStorage.getItem(TOKEN_KEY),
};

export default tokenService;
