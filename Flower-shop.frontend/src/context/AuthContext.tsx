import React, { createContext, useState, useContext, useEffect, useCallback, useMemo, type ReactNode } from 'react';
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
                const exp = payload.exp;
                if (exp && Date.now() >= exp * 1000) {
                    tokenService.removeToken();
                    setLoading(false);
                    return;
                }
                setUser({
                    id: payload.Id ? Number(payload.Id) : undefined,
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
                id: response.id,
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
        } catch (error: any) {
            console.warn('refreshProfile failed, token may be expired:', error);
            if (error?.response?.status === 401) {
                tokenService.removeToken();
                setUser(null);
            }
        }
    }, []);

    const updateProfile = useCallback(async (fullName: string, phone: string, address: string) => {
        const response = await authService.updateProfile({ fullName, phone, address });
        if (response.token) {
            tokenService.setToken(response.token);
        }
        if (response.user) {
            setUser(response.user);
        }
        return response;
    }, []);

    const changePassword = useCallback(async (currentPassword: string, newPassword: string) => {
        const response = await authService.changePassword({ currentPassword, newPassword });
        return response;
    }, []);

    const isAuthenticated = !!user;

    const value = useMemo<AuthContextType>(() => ({
        user, login, logout, refreshProfile, updateProfile, changePassword, isAuthenticated, loading,
        token: tokenService.getToken(),
    }), [user, login, logout, refreshProfile, updateProfile, changePassword, isAuthenticated, loading]);

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
