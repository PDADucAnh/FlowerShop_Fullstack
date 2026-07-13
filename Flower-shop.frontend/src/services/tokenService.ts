const TOKEN_KEY = 'pda_flower_token';

const tokenService = {
    getToken: () => sessionStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY),
    setToken: (token: string, rememberMe = false) => {
        if (rememberMe) {
            localStorage.setItem(TOKEN_KEY, token);
        } else {
            sessionStorage.setItem(TOKEN_KEY, token);
        }
    },
    removeToken: () => { sessionStorage.removeItem(TOKEN_KEY); localStorage.removeItem(TOKEN_KEY); },
    hasToken: () => !!(sessionStorage.getItem(TOKEN_KEY) ?? localStorage.getItem(TOKEN_KEY)),
};

export default tokenService;
