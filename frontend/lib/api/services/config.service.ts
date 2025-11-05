import { apiClient } from "../client";
import { ConfigDto, UpdateConfigRequest } from "../types";

/**
 * Configuration API Service
 * Handles all configuration-related API calls
 */
export class ConfigService {
  private readonly basePath = "/config";

  /**
   * Get current configuration
   * @returns Current configuration data
   */
  async getConfig(): Promise<ConfigDto> {
    const response = await apiClient.get<ConfigDto>(this.basePath);
    return response.data;
  }

  /**
   * Update configuration
   * @param config - Updated configuration values
   * @returns Updated configuration data
   */
  async updateConfig(config: UpdateConfigRequest): Promise<ConfigDto> {
    const response = await apiClient.put<ConfigDto>(this.basePath, config);
    return response.data;
  }
}

// Export a singleton instance
export const configService = new ConfigService();
