import { InternalAxiosRequestConfig, AxiosError } from "axios";

/**
 * Request interceptor to add authentication token to requests
 * Retrieves the token from localStorage and adds it to the Authorization header
 */
export const authRequestInterceptor = (
  config: InternalAxiosRequestConfig
): InternalAxiosRequestConfig => {
  const token = localStorage.getItem("token");
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
};

/**
 * Error handler for request interceptor
 */
export const authRequestErrorInterceptor = (error: AxiosError): Promise<AxiosError> => {
  return Promise.reject(error);
};
