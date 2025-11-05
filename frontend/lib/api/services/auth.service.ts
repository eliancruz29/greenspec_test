import { apiClient } from "../client";
import { LoginRequest, LoginResponse } from "../types";

/**
 * Authentication API Service
 * Handles all authentication-related API calls
 */
export class AuthService {
  private readonly basePath = "/auth";

  /**
   * Authenticate user with username and password
   * @param credentials - User login credentials
   * @returns Login response with token and user info
   */
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>(
      `${this.basePath}/login`,
      credentials
    );
    return response.data;
  }
}

// Export a singleton instance
export const authService = new AuthService();
