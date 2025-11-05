import type * as signalR from "@microsoft/signalr";
import type { AlertDto } from "../api/types/alert.types";

/**
 * Callback function type for alert notifications
 */
export type AlertCallback = (alert: AlertDto) => void;

/**
 * Callback function type for connection state changes
 */
export type ConnectionStateCallback = (
  state: signalR.HubConnectionState
) => void;

/**
 * Callback function type for error handling
 */
export type ErrorCallback = (error: Error) => void;

/**
 * Configuration options for SignalR connection
 */
export interface SignalRConfig {
  /** Retry delay in milliseconds (default: 5000) */
  retryDelayMs?: number;
  /** Maximum number of retry attempts (default: 10) */
  maxRetries?: number;
  /** SignalR log level (default: Information) */
  logLevel?: signalR.LogLevel;
  /** Skip negotiation step (default: true) */
  skipNegotiation?: boolean;
  /** Transport type (default: WebSockets) */
  transport?: signalR.HttpTransportType;
}
