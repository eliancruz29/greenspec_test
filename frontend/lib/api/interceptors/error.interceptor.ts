import { AxiosResponse, AxiosError } from "axios";

/**
 * Response interceptor that handles successful responses
 * Simply returns the response as-is
 */
export const successResponseInterceptor = (response: AxiosResponse): AxiosResponse => {
  return response;
};

/**
 * Response interceptor that handles errors
 * Specifically handles 401 Unauthorized errors by clearing token and redirecting to login
 */
export const errorResponseInterceptor = (error: AxiosError): Promise<AxiosError> => {
  if (error.response?.status === 401) {
    // Clear authentication token
    localStorage.removeItem("token");

    // Redirect to login page
    if (typeof window !== "undefined") {
      window.location.href = "/login";
    }
  }

  return Promise.reject(error);
};
