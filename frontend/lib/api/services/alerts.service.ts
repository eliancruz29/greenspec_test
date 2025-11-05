import { apiClient } from "../client";
import { AlertDto, GetAlertsParams, PagedResultDto } from "../types";

/**
 * Alerts API Service
 * Handles all alert-related API calls
 */
export class AlertsService {
  private readonly basePath = "/alerts";

  /**
   * Get paginated list of alerts with optional filters
   * @param params - Query parameters for filtering and pagination
   * @returns Paginated alert results
   */
  async getPagedAlerts(params?: GetAlertsParams): Promise<PagedResultDto<AlertDto>> {
    const response = await apiClient.get<PagedResultDto<AlertDto>>(this.basePath, {
      params,
    });
    return response.data;
  }

  /**
   * Acknowledge an alert by ID
   * @param id - Alert ID to acknowledge
   * @returns Updated alert data
   */
  async acknowledgeAlert(id: number): Promise<AlertDto> {
    const response = await apiClient.post<AlertDto>(`${this.basePath}/${id}/ack`);
    return response.data;
  }
}

// Export a singleton instance
export const alertsService = new AlertsService();
