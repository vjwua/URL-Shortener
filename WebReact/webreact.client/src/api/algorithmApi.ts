import axios from 'axios';
import { AlgorithmInfoDTO } from '../types';

const BASE = '/api/algorithminfo';

export const getInfo = async (): Promise<AlgorithmInfoDTO> => {
    const { data } = await axios.get(BASE);
    return data;
};

export const updateInfo = async (dto: AlgorithmInfoDTO): Promise<void> => {
    await axios.put(BASE, dto);
};