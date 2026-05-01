import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import axios from 'axios';
import { refresh } from '../api/accountApi';

interface AuthContextType {
    accessToken: string | null;
    isAdmin: boolean;
    isAuthenticated: boolean;
    login: (accessToken: string, refreshToken: string) => void;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

const parseRole = (token: string): boolean => {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        return role === 'Admin' || (Array.isArray(role) && role.includes('Admin'));
    } catch {
        return false;
    }
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [accessToken, setAccessToken] = useState<string | null>(null);
    const [isAdmin, setIsAdmin] = useState(false);

    useEffect(() => {

        const reqInterceptor = axios.interceptors.request.use(config => {
            if (accessToken) {
                config.headers.Authorization = `Bearer ${accessToken}`;
            }
            return config;
        });

        const resInterceptor = axios.interceptors.response.use(
            response => response,
            async error => {
                const storedRefresh = localStorage.getItem('refreshToken');
                if (error.response?.status === 401 && storedRefresh) {
                    try {
                        const result = await refresh(storedRefresh);
                        handleLogin(result.accessToken, result.refreshToken);
                        error.config.headers.Authorization = `Bearer ${result.accessToken}`;
                        return axios(error.config);
                    } catch {
                        handleLogout();
                    }
                }
                return Promise.reject(error);
            }
        );

        return () => {
            axios.interceptors.request.eject(reqInterceptor);
            axios.interceptors.response.eject(resInterceptor);
        };
    }, [accessToken]);

    const handleLogin = (token: string, refreshToken: string) => {
        setAccessToken(token);
        setIsAdmin(parseRole(token));
        localStorage.setItem('refreshToken', refreshToken);
    };

    const handleLogout = () => {
        setAccessToken(null);
        setIsAdmin(false);
        localStorage.removeItem('refreshToken');
    };

    return (
        <AuthContext.Provider value={{
            accessToken,
            isAdmin,
            isAuthenticated: !!accessToken,
            login: handleLogin,
            logout: handleLogout
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
};