import axios from 'axios';
import { ShortUrlResponse, CreateShortUrlDTO } from '../types';

const BASE = '/api/shorturl';

export const getAll = async (): Promise<ShortUrlResponse[]> => {
    const { data } = await axios.get(BASE);

    if (data && Array.isArray(data['$values'])) {
        return data['$values'];
    }

    return Array.isArray(data) ? data : [];
};

export const getById = async (id: string): Promise<ShortUrlResponse> => {
    const { data } = await axios.get(`${BASE}/${id}`);
    return data;
};

export const create = async (dto: CreateShortUrlDTO): Promise<ShortUrlResponse> => {
    const { data } = await axios.post(BASE, dto);
    return data;
};

export const remove = async (id: string): Promise<void> => {
    await axios.delete(`${BASE}/${id}`);
};

export const removeAll = async (): Promise<void> => {
    await axios.delete(BASE);
};