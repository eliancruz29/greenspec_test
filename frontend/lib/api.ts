import axios from "axios";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "https://localhost:5001/api";

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// API Types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  expiresAt: string;
}

export interface ConfigDto {
  id: number;
  tempMax: number;
  humidityMax: number;
  updatedAt: string;
}

export interface UpdateConfigRequest {
  tempMax: number;
  humidityMax: number;
}

export interface AlertDto {
  id: number;
  type: string;
  value: number;
  threshold: number;
  createdAt: string;
  status: string;
}

// API Functions
export const authApi = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>("/auth/login", credentials);
    return response.data;
  },
};

export const configApi = {
  getConfig: async (): Promise<ConfigDto> => {
    const response = await api.get<ConfigDto>("/config");
    return response.data;
  },
  updateConfig: async (config: UpdateConfigRequest): Promise<ConfigDto> => {
    const response = await api.put<ConfigDto>("/config", config);
    return response.data;
  },
};

export const alertsApi = {
  getAlerts: async (params?: {
    status?: string;
    from?: string;
    to?: string;
  }): Promise<AlertDto[]> => {
    const response = await api.get<AlertDto[]>("/alerts", { params });
    return response.data;
  },
  acknowledgeAlert: async (id: number): Promise<AlertDto> => {
    const response = await api.post<AlertDto>(`/alerts/${id}/ack`);
    return response.data;
  },
};
