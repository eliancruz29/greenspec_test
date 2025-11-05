/**
 * Type guard utilities for error handling
 */

/**
 * API Error shape from backend
 */
export interface ApiError {
  response: {
    status: number;
    statusText?: string;
    data?: {
      message?: string;
      errors?: Record<string, string[]>;
    };
  };
  message?: string;
}

/**
 * Type guard to check if error is an API error with response
 */
export function isApiError(error: unknown): error is ApiError {
  return (
    typeof error === "object" &&
    error !== null &&
    "response" in error &&
    typeof (error as Record<string, unknown>).response === "object" &&
    (error as Record<string, unknown>).response !== null &&
    "status" in ((error as Record<string, unknown>).response as object)
  );
}

/**
 * Type guard to check if error has a specific status code
 */
export function isApiErrorWithStatus(
  error: unknown,
  status: number
): error is ApiError {
  return isApiError(error) && error.response.status === status;
}

/**
 * Check if error is a 401 Unauthorized
 */
export function isUnauthorizedError(error: unknown): error is ApiError {
  return isApiErrorWithStatus(error, 401);
}

/**
 * Check if error is a 403 Forbidden
 */
export function isForbiddenError(error: unknown): error is ApiError {
  return isApiErrorWithStatus(error, 403);
}

/**
 * Check if error is a 404 Not Found
 */
export function isNotFoundError(error: unknown): error is ApiError {
  return isApiErrorWithStatus(error, 404);
}

/**
 * Check if error is a 422 Validation Error
 */
export function isValidationError(error: unknown): error is ApiError {
  return isApiErrorWithStatus(error, 422);
}

/**
 * Check if error is a 500 Internal Server Error
 */
export function isServerError(error: unknown): error is ApiError {
  return isApiError(error) && error.response.status >= 500;
}

/**
 * Extract error message from unknown error
 */
export function getErrorMessage(error: unknown, fallback: string = "An error occurred"): string {
  if (isApiError(error)) {
    // Try to get message from response data
    if (error.response.data?.message) {
      return error.response.data.message;
    }

    // Try to get status text
    if (error.response.statusText) {
      return error.response.statusText;
    }

    // Fallback to status code
    return `Request failed with status ${error.response.status}`;
  }

  // Check if it's a standard Error object
  if (error instanceof Error) {
    return error.message;
  }

  // Check if it's an object with a message property
  if (
    typeof error === "object" &&
    error !== null &&
    "message" in error &&
    typeof (error as Record<string, unknown>).message === "string"
  ) {
    return (error as Record<string, unknown>).message as string;
  }

  // Convert to string as last resort
  if (typeof error === "string") {
    return error;
  }

  return fallback;
}

/**
 * Extract validation errors from API error
 */
export function getValidationErrors(error: unknown): Record<string, string[]> | null {
  if (isApiError(error) && error.response.data?.errors) {
    return error.response.data.errors;
  }
  return null;
}

/**
 * Type guard for Error instance
 */
export function isError(error: unknown): error is Error {
  return error instanceof Error;
}

/**
 * Type guard to check if error has a message property
 */
export function hasMessage(error: unknown): error is { message: string } {
  return (
    typeof error === "object" &&
    error !== null &&
    "message" in error &&
    typeof (error as Record<string, unknown>).message === "string"
  );
}

/**
 * Safe error formatter for logging
 */
export function formatErrorForLogging(error: unknown): Record<string, unknown> {
  if (isApiError(error)) {
    return {
      type: "ApiError",
      status: error.response.status,
      statusText: error.response.statusText,
      message: getErrorMessage(error),
      data: error.response.data,
    };
  }

  if (isError(error)) {
    return {
      type: "Error",
      name: error.name,
      message: error.message,
      stack: error.stack,
    };
  }

  return {
    type: "Unknown",
    value: String(error),
  };
}
