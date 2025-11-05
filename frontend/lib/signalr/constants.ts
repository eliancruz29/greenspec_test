/**
 * SignalR hub URL from environment or default to localhost
 */
export const HUB_URL =
  process.env.NEXT_PUBLIC_SIGNALR_URL ||
  "https://localhost:5001/hubs/alerts";

/**
 * SignalR event name for receiving alerts
 */
export const ALERT_EVENT_NAME = "ReceiveAlert";

/**
 * Default retry delay in milliseconds
 */
export const DEFAULT_RETRY_DELAY_MS = 5000;

/**
 * Maximum number of retry attempts before giving up
 */
export const MAX_RETRY_ATTEMPTS = 10;

/**
 * Multiplier for exponential backoff retry strategy
 */
export const RETRY_BACKOFF_MULTIPLIER = 1.5;

/**
 * Maximum retry delay cap in milliseconds (1 minute)
 */
export const MAX_RETRY_DELAY_MS = 60000;
