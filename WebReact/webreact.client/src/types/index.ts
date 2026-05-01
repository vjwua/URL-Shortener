export interface ShortUrlResponse {
    id: string;
    originalUrl: string;
    shortCode: string;
    createdAt: string;
    createdByUserId: string;
}

export interface CreateShortUrlDTO {
    originalUrl: string;
}

export interface AlgorithmInfoDTO {
    description: string;
}

export interface LoginDTO {
    email: string;
    password: string;
}

export interface RegisterDTO {
    userName: string;
    email: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
}