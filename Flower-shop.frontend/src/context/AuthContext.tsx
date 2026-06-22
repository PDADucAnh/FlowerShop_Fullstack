import React, { createContext, useState, useContext, useEffect, useCallback, type ReactNode } from 'react';
import authService from '../services/authService';
import tokenService from '../services/tokenService';
import type { AuthUser, AuthContextType } from '../types/context';

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = tokenService.getToken();
        if (token) {
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                setUser({
                    username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.unique_name,
                    fullName: payload.FullName || '',
                    email: payload.Email || '',
                    phone: payload.Phone || '',
                    address: payload.Address || '',
                    role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role,
                });
            } catch {
                tokenService.removeToken();
            }
        }
        setLoading(false);
    }, []);

    const login = useCallback(async (username: string, password: string) => {
        const response = await authService.login(username, password);
        if (response.token) {
            tokenService.setToken(response.token);
            setUser({
                username: response.username,
                fullName: response.fullName,
                email: response.email,
                phone: response.phone,
                address: response.address,
                role: response.role,
            });
        }
        return response;
    }, []);

    const logout = useCallback(() => {
        tokenService.removeToken();
        setUser(null);
    }, []);

    const refreshProfile = useCallback(async () => {
        try {
            const profile = await authService.getProfile();
            if (profile) {
                setUser((prev) => prev ? { ...prev, ...profile } : profile);
            }
        } catch {
            // silently fail
        }
    }, []);

    const isAuthenticated = !!user;

    const value: AuthContextType = { user, login, logout, refreshProfile, isAuthenticated, loading, token: tokenService.getToken() };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within AuthProvider');
    return context;
};
