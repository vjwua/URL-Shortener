import axios from 'axios';
import { LoginDTO, RegisterDTO, AuthResponse } from '../types';

const BASE = '/api/account';

export const login = async (dto: LoginDTO): Promise<AuthResponse> => {
    const { data } = await axios.post(`${BASE}/login`, dto);
    return data;
};

export const register = async (dto: RegisterDTO): Promise<void> => {
    await axios.post(`${BASE}/register`, dto);
};

export const logout = async (refreshToken: string): Promise<void> => {
    await axios.post(`${BASE}/logout`, { refreshToken });
};

export const refresh = async (refreshToken: string): Promise<AuthResponse> => {
    const { data } = await axios.post(`${BASE}/refresh`, { refreshToken });
    return data;
};