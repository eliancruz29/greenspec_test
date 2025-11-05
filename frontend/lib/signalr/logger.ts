/**
 * Logger interface for abstracting logging functionality
 * Allows for dependency injection and testing
 */
export interface Logger {
  /**
   * Log an informational message
   */
  log(message: string, ...args: unknown[]): void;

  /**
   * Log an error message
   */
  error(message: string, ...args: unknown[]): void;

  /**
   * Log a warning message
   */
  warn(message: string, ...args: unknown[]): void;
}

/**
 * Default console-based logger implementation
 * Uses browser console methods for logging
 */
export class ConsoleLogger implements Logger {
  log(message: string, ...args: unknown[]): void {
    console.log(message, ...args);
  }

  error(message: string, ...args: unknown[]): void {
    console.error(message, ...args);
  }

  warn(message: string, ...args: unknown[]): void {
    console.warn(message, ...args);
  }
}

/**
 * Silent logger implementation for testing or production
 * Discards all log messages
 */
export class SilentLogger implements Logger {
  log(): void {
    // No-op
  }

  error(): void {
    // No-op
  }

  warn(): void {
    // No-op
  }
}

/**
 * Default logger instance
 */
export const defaultLogger = new ConsoleLogger();
