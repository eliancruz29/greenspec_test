/**
 * SignalR module for real-time alert notifications
 *
 * This module provides a clean, type-safe interface for managing
 * SignalR connections with automatic reconnection and event handling.
 *
 * @module signalr
 */

// Export main class
export { AlertHubConnection } from "./AlertHubConnection";

// Export types
export type {
  AlertCallback,
  ConnectionStateCallback,
  ErrorCallback,
  SignalRConfig,
} from "./types";

// Export logger interfaces and implementations
export type { Logger } from "./logger";
export { ConsoleLogger, SilentLogger, defaultLogger } from "./logger";

// Export constants for advanced usage
export {
  HUB_URL,
  ALERT_EVENT_NAME,
  DEFAULT_RETRY_DELAY_MS,
  MAX_RETRY_ATTEMPTS,
  RETRY_BACKOFF_MULTIPLIER,
  MAX_RETRY_DELAY_MS,
} from "./constants";

// Create and export default singleton instance for backward compatibility
import { AlertHubConnection } from "./AlertHubConnection";

/**
 * Default singleton instance for backward compatibility
 *
 * For better testability and flexibility, consider creating
 * your own instance instead:
 *
 * @example
 * ```typescript
 * import { AlertHubConnection } from '@/lib/signalr';
 *
 * const customHub = new AlertHubConnection({
 *   maxRetries: 5,
 *   retryDelayMs: 3000
 * });
 * ```
 */
export const alertHub = new AlertHubConnection();
