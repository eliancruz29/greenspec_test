import * as signalR from "@microsoft/signalr";
import type { AlertDto } from "../api/types/alert.types";
import type {
  AlertCallback,
  ConnectionStateCallback,
  ErrorCallback,
  SignalRConfig,
} from "./types";
import type { Logger } from "./logger";
import { defaultLogger } from "./logger";
import {
  HUB_URL,
  ALERT_EVENT_NAME,
  DEFAULT_RETRY_DELAY_MS,
  MAX_RETRY_ATTEMPTS,
  RETRY_BACKOFF_MULTIPLIER,
  MAX_RETRY_DELAY_MS,
} from "./constants";

/**
 * Manages SignalR Hub connection for real-time alert notifications
 *
 * Features:
 * - Automatic reconnection with exponential backoff
 * - Event-based subscription pattern
 * - Configurable retry strategy
 * - Lifecycle management
 * - Dependency-injectable logger
 *
 * @example
 * ```typescript
 * const hub = new AlertHubConnection();
 *
 * // Subscribe to alerts
 * const unsubscribe = hub.onAlert((alert) => {
 *   console.log('Received alert:', alert);
 * });
 *
 * // Start connection
 * await hub.start(token);
 *
 * // Later: cleanup
 * unsubscribe();
 * await hub.stop();
 * ```
 */
export class AlertHubConnection {
  private connection: signalR.HubConnection | null = null;
  private currentToken: string | null = null;
  private retryCount = 0;
  private readonly config: Required<SignalRConfig>;
  private readonly logger: Logger;
  private readonly alertCallbacks: Set<AlertCallback> = new Set();
  private readonly stateChangeCallbacks: Set<ConnectionStateCallback> =
    new Set();
  private readonly errorCallbacks: Set<ErrorCallback> = new Set();

  /**
   * Creates a new AlertHubConnection instance
   *
   * @param config - Optional configuration for retry behavior and transport
   * @param logger - Optional logger implementation (defaults to ConsoleLogger)
   */
  constructor(config: SignalRConfig = {}, logger: Logger = defaultLogger) {
    this.config = {
      retryDelayMs: config.retryDelayMs ?? DEFAULT_RETRY_DELAY_MS,
      maxRetries: config.maxRetries ?? MAX_RETRY_ATTEMPTS,
      logLevel: config.logLevel ?? signalR.LogLevel.Information,
      skipNegotiation: config.skipNegotiation ?? true,
      transport: config.transport ?? signalR.HttpTransportType.WebSockets,
    };
    this.logger = logger;
  }

  /**
   * Starts the SignalR connection with authentication
   *
   * @param token - JWT token for authentication
   * @throws {Error} If token is empty or invalid
   */
  async start(token: string): Promise<void> {
    if (!token) {
      throw new Error("Token is required to start SignalR connection");
    }

    this.currentToken = token;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        skipNegotiation: this.config.skipNegotiation,
        transport: this.config.transport,
        // Security: Use accessTokenFactory instead of URL parameter
        accessTokenFactory: () => this.currentToken || "",
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          // Exponential backoff with max retry limit
          if (retryContext.previousRetryCount >= this.config.maxRetries) {
            this.logger.error(
              `Max retry attempts (${this.config.maxRetries}) reached. Connection failed.`
            );
            return null; // Stop retrying
          }
          const delay =
            this.config.retryDelayMs *
            Math.pow(RETRY_BACKOFF_MULTIPLIER, retryContext.previousRetryCount);
          return Math.min(delay, MAX_RETRY_DELAY_MS);
        },
      })
      .configureLogging(this.config.logLevel)
      .build();

    this.setupEventHandlers();
    this.setupLifecycleHandlers();

    try {
      await this.connection.start();
      this.retryCount = 0;
      this.logger.log("SignalR connected successfully");
      this.notifyStateChange(signalR.HubConnectionState.Connected);
    } catch (err) {
      const error = err instanceof Error ? err : new Error(String(err));
      this.logger.error("SignalR connection error:", error);
      this.notifyError(error);
      await this.handleConnectionError(error);
    }
  }

  /**
   * Stops the SignalR connection and cleans up resources
   * Safe to call multiple times
   */
  async stop(): Promise<void> {
    if (this.connection) {
      try {
        // Remove event handlers before stopping
        this.connection.off(ALERT_EVENT_NAME);
        await this.connection.stop();
        this.logger.log("SignalR disconnected");
        this.notifyStateChange(signalR.HubConnectionState.Disconnected);
      } catch (err) {
        const error = err instanceof Error ? err : new Error(String(err));
        this.logger.error("Error stopping SignalR connection:", error);
        this.notifyError(error);
      } finally {
        this.connection = null;
        this.currentToken = null;
        this.retryCount = 0;
      }
    }
  }

  /**
   * Registers a callback for alert notifications
   *
   * @param callback - Function to call when an alert is received
   * @returns Unsubscribe function to remove the callback
   *
   * @example
   * ```typescript
   * const unsubscribe = hub.onAlert((alert) => {
   *   console.log('New alert:', alert);
   * });
   *
   * // Later, to unsubscribe:
   * unsubscribe();
   * ```
   */
  onAlert(callback: AlertCallback): () => void {
    this.alertCallbacks.add(callback);
    return () => this.alertCallbacks.delete(callback);
  }

  /**
   * Registers a callback for connection state changes
   *
   * @param callback - Function to call when connection state changes
   * @returns Unsubscribe function to remove the callback
   *
   * @example
   * ```typescript
   * const unsubscribe = hub.onStateChange((state) => {
   *   if (state === HubConnectionState.Connected) {
   *     console.log('Connected!');
   *   }
   * });
   * ```
   */
  onStateChange(callback: ConnectionStateCallback): () => void {
    this.stateChangeCallbacks.add(callback);
    return () => this.stateChangeCallbacks.delete(callback);
  }

  /**
   * Registers a callback for connection errors
   *
   * @param callback - Function to call when an error occurs
   * @returns Unsubscribe function to remove the callback
   *
   * @example
   * ```typescript
   * const unsubscribe = hub.onError((error) => {
   *   console.error('Connection error:', error);
   * });
   * ```
   */
  onError(callback: ErrorCallback): () => void {
    this.errorCallbacks.add(callback);
    return () => this.errorCallbacks.delete(callback);
  }

  /**
   * Gets the current connection state
   *
   * @returns Current SignalR connection state
   */
  getConnectionState(): signalR.HubConnectionState {
    return this.connection?.state ?? signalR.HubConnectionState.Disconnected;
  }

  /**
   * Checks if the connection is currently active
   *
   * @returns true if connected, false otherwise
   */
  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  /**
   * Sets up event handlers for incoming SignalR messages
   * @private
   */
  private setupEventHandlers(): void {
    if (!this.connection) return;

    this.connection.on(ALERT_EVENT_NAME, (alert: AlertDto) => {
      this.alertCallbacks.forEach((callback) => {
        try {
          callback(alert);
        } catch (err) {
          const error = err instanceof Error ? err : new Error(String(err));
          this.logger.error("Error in alert callback:", error);
        }
      });
    });
  }

  /**
   * Sets up lifecycle event handlers (reconnecting, reconnected, close)
   * @private
   */
  private setupLifecycleHandlers(): void {
    if (!this.connection) return;

    this.connection.onreconnecting((error) => {
      this.logger.warn("SignalR reconnecting...", error);
      this.notifyStateChange(signalR.HubConnectionState.Reconnecting);
    });

    this.connection.onreconnected((connectionId) => {
      this.logger.log("SignalR reconnected", connectionId);
      this.retryCount = 0;
      this.notifyStateChange(signalR.HubConnectionState.Connected);
    });

    this.connection.onclose((error) => {
      this.logger.warn("SignalR connection closed", error);
      this.notifyStateChange(signalR.HubConnectionState.Disconnected);
      if (error) {
        const err = error instanceof Error ? error : new Error(String(error));
        this.notifyError(err);
      }
    });
  }

  /**
   * Handles connection errors with exponential backoff retry
   * @private
   */
  private async handleConnectionError(error: Error): Promise<void> {
    if (this.retryCount >= this.config.maxRetries) {
      this.logger.error(
        `Max retry attempts (${this.config.maxRetries}) reached. Connection failed permanently.`
      );
      return;
    }

    this.retryCount++;
    const delay =
      this.config.retryDelayMs *
      Math.pow(RETRY_BACKOFF_MULTIPLIER, this.retryCount - 1);
    const cappedDelay = Math.min(delay, MAX_RETRY_DELAY_MS);

    this.logger.log(
      `Retrying connection in ${cappedDelay}ms (attempt ${this.retryCount}/${this.config.maxRetries})`
    );

    setTimeout(async () => {
      if (this.currentToken) {
        try {
          await this.start(this.currentToken);
        } catch (err) {
          // Error will be handled by the start method
        }
      }
    }, cappedDelay);
  }

  /**
   * Notifies all state change callbacks
   * @private
   */
  private notifyStateChange(state: signalR.HubConnectionState): void {
    this.stateChangeCallbacks.forEach((callback) => {
      try {
        callback(state);
      } catch (err) {
        const error = err instanceof Error ? err : new Error(String(err));
        this.logger.error("Error in state change callback:", error);
      }
    });
  }

  /**
   * Notifies all error callbacks
   * @private
   */
  private notifyError(error: Error): void {
    this.errorCallbacks.forEach((callback) => {
      try {
        callback(error);
      } catch (err) {
        const callbackError =
          err instanceof Error ? err : new Error(String(err));
        this.logger.error("Error in error callback:", callbackError);
      }
    });
  }
}
