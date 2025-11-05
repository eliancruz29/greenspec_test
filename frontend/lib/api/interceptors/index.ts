import { AxiosInstance } from "axios";
import { authRequestInterceptor, authRequestErrorInterceptor } from "./auth.interceptor";
import { successResponseInterceptor, errorResponseInterceptor } from "./error.interceptor";

/**
 * Sets up all interceptors for the given axios instance
 * This function should be called once when initializing the API client
 */
export const setupInterceptors = (axiosInstance: AxiosInstance): void => {
  // Request interceptors
  axiosInstance.interceptors.request.use(
    authRequestInterceptor,
    authRequestErrorInterceptor
  );

  // Response interceptors
  axiosInstance.interceptors.response.use(
    successResponseInterceptor,
    errorResponseInterceptor
  );
};

export * from "./auth.interceptor";
export * from "./error.interceptor";
