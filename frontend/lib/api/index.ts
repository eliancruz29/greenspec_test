import { apiClient } from "./client";
import { setupInterceptors } from "./interceptors";

// Setup interceptors on the API client
setupInterceptors(apiClient);

// Export the configured API client
export { apiClient };

// Export all types
export * from "./types";

// Export all services
export * from "./services";

// For backward compatibility, export the old API structure
import { authService } from "./services/auth.service";
import { configService } from "./services/config.service";
import { alertsService } from "./services/alerts.service";

export const authApi = {
  login: authService.login.bind(authService),
};

export const configApi = {
  getConfig: configService.getConfig.bind(configService),
  updateConfig: configService.updateConfig.bind(configService),
};

export const alertsApi = {
  getPagedAlerts: alertsService.getPagedAlerts.bind(alertsService),
  acknowledgeAlert: alertsService.acknowledgeAlert.bind(alertsService),
};

// Also export the original 'api' axios instance for backward compatibility
export const api = apiClient;
